#define MINIAUDIO_IMPLEMENTATION
#define NOMINMAX

#include "audio_engine.h"
#include "math_helper.h"
#include "sample_map.h"
#include "sound_effects.h"

#include <Windows.h>
#include <timeapi.h> // Windows timer moment :)
#pragma comment(lib, "winmm.lib")

#include <iostream>
#include <algorithm>


AudioEngine::AudioEngine(int sampleRate, int bufferSize) : buffer(AudioBuffer(bufferSize)), activeAudio(nullptr), useBuffer(false), playbackSpeed(1.0f), volume(1.0f), rpm(0.0f), load(-1.0f), sampleRate(sampleRate) {
    this->config = ma_device_config_init(ma_device_type_playback);
    config.playback.format = ma_format_f32;
    config.playback.channels = 2;
    config.sampleRate = sampleRate;
    config.dataCallback = AudioEngine::audioCallback;
    config.pUserData = this;

    ma_device_init(NULL, &config, &device);
}

void AudioEngine::audioCallback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount) {
    AudioEngine* audioEngine = (AudioEngine*)pDevice->pUserData;

    if (audioEngine->isUsingBuffer()) {
        audioEngine->processAudioBuffer((float*)pOutput, frameCount);
    } else {
        audioEngine->processAudioStatic((float*)pOutput, frameCount);
    }    
}

void AudioEngine::processAudioStatic(float* pOutput, ma_uint32 frameCount) {
    if (activeAudio == nullptr || activeAudio->samples.empty()) {
        for (ma_uint32 i = 0; i < frameCount * 2; i++) {
            pOutput[i] = 0.0f;
        }

        return;
    }

    Audio* audio = activeAudio;

    for (ma_uint32 i = 0; i < frameCount; i++) {
        float sample1 = audio->samples[(int)audio->cursor];
        float sample2 = audio->samples[(int)audio->cursor + 1];
        float fraction = audio->cursor - (int)audio->cursor;

        float sample = sample1 + fraction * (sample2 - sample1);

        sample *= volume;

        pOutput[i * 2] = sample; // left
        pOutput[i * 2 + 1] = sample; // right

        audio->cursor += playbackSpeed;

        if (audio->grains.size() == 0 && audio->cursor >= audio->samples.size()) {
            audio->cursor = 0.0f;
            continue;
        }

        Grain* currentGrain = &audio->grains[audio->currentGrainId];

        if (audio->cursor >= currentGrain->start + currentGrain->length) {
            float overflow = audio->cursor - (currentGrain->start + currentGrain->length);

            if (audio->currentGrainId + 1 >= audio->grains.size()) {
                playGrain(audio, 0, overflow);
                continue;
            }

            playGrain(audio, audio->currentGrainId + 1, overflow);
        }
    }
}

void AudioEngine::processAudioBuffer(float* pOutput, ma_uint32 frameCount) {
    for (ma_uint32 i = 0; i < frameCount; i++) {
        float sample;
        if (!buffer.read(sample)) {
            sample = 0.0f;
        }

        sample *= volume;

        pOutput[i * 2] = sample; // left
        pOutput[i * 2 + 1] = sample; // right
    }
}

void AudioEngine::setAudio(Audio& audio) {
    audio.currentGrainId = 0;
    audio.cursor = 0.0f;
    activeAudio = &audio;
}

void AudioEngine::start() {
    if (useBuffer && !isRunning) {
        isRunning = true;
        timeBeginPeriod(1);
        generatorThread = std::thread(&AudioEngine::runGenerator, this);
    }
    ma_device_start(&device);
}

void AudioEngine::stop() {
    isRunning = false;
    if (generatorThread.joinable()) {
        generatorThread.join();
    }
    timeEndPeriod(1);
    ma_device_stop(&device);
}

void AudioEngine::playGrain(Audio* audio, int grainId) {
    playGrain(audio, grainId, 0.0f);
}

void AudioEngine::playGrain(Audio* audio, int grainId, float cursorOffset) {
    grainId = std::clamp(grainId, 0, (int)audio->grains.size() - 1);
    audio->currentGrainId = grainId;
    audio->cursor = audio->grains[grainId].start + cursorOffset;
}

void AudioEngine::playRandomGrain(Audio* audio, int minId, int maxId) {
    int grainId;

    do {
        grainId = randomInt(minId, maxId);
    } while (grainId == audio->currentGrainId);

    playGrain(audio, grainId);
}

void AudioEngine::playRandomGrain(Audio* audio) {
    playRandomGrain(audio, 0, audio->grains.size() - 1);
}

bool AudioEngine::loadWav(const std::string& filePath, Audio& out, int sampleRate) {
    ma_decoder decoder;
    ma_decoder_config config = ma_decoder_config_init(ma_format_f32, 1, sampleRate);

    ma_result result = ma_decoder_init_file(filePath.c_str(), &config, &decoder);
    if (result != MA_SUCCESS) {
        return false;
    }

    ma_uint64 frameCount;
    ma_decoder_get_length_in_pcm_frames(&decoder, &frameCount);

    out.samples.resize(frameCount);

    ma_uint64 framesRead;
    result = ma_decoder_read_pcm_frames(&decoder, out.samples.data(), frameCount, &framesRead);

    ma_decoder_uninit(&decoder);
    return (result == MA_SUCCESS);
}

bool AudioEngine::saveWav(const std::string& filePath, const Audio& input, int sampleRate) {
    ma_encoder_config config = ma_encoder_config_init(ma_encoding_format_wav, ma_format_f32, 1, sampleRate);

    ma_encoder encoder;
    ma_result result = ma_encoder_init_file(filePath.c_str(), &config, &encoder);
    if (result != MA_SUCCESS) {
        return false;
    }

    ma_uint64 framesSaved;
    result = ma_encoder_write_pcm_frames(&encoder, input.samples.data(), input.samples.size(), &framesSaved);

    ma_encoder_uninit(&encoder);

    return (result == MA_SUCCESS);
}

int AudioEngine::findNextGrain(const std::vector<float>& samples, int firstSample, int prevSize, int direction, int sampleRate, int totalSamples) {
    int endEstimated = -1;

    // -1, 3.5%
    for (int i = firstSample + prevSize; i - (firstSample + prevSize) < prevSize * 0.035; i++) {
        if (samples[i] > 0.0f) {
            endEstimated = i;
            break;
        }
    }

    if (endEstimated == -1) {
        return -1;
    }

    // 5, -2.5%
    int rangeEnd = endEstimated - 0.975 * (endEstimated - firstSample);
    for (int i = endEstimated + 5; i > rangeEnd; i--) {
        if (std::signbit(samples[i]) != std::signbit(samples[i - 1])) {
            int lastSample = i;
            int size = lastSample - firstSample;
            return lastSample;
        }
    }

    return -1;
}

int AudioEngine::findGrainSize(Audio& audio, double referenceRpm, int start, int sampleRate) {
    int referenceSize = (int)((1.0 / (referenceRpm / 60.0)) * sampleRate);
    int rangeMin = (int)(referenceSize * 0.9);
    int rangeMax = (int)(referenceSize * 1.1);

    float maxCorrelation = -1.0f;
    int bestSize = 0;

    for (int size = start + rangeMin; size < start + rangeMax; size++) {
        //if (std::signbit(audio.samples[size]) == std::signbit(audio.samples[size + 1])) {
        if (!(audio.samples[size] < 0.0f && audio.samples[size + 1] > 0.0f)) {
            continue;
        }

        int windowSize = std::min(referenceSize, (int) audio.samples.size() - start - referenceSize);

        float correlation = 0.0f;
        float energy = 0.00001f; // div by zero lol

        for (int j = 0; j < windowSize; j++) {
            correlation += audio.samples[j] * audio.samples[j + size];
            energy += std::pow(audio.samples[j + size], 2);
        }

        correlation = correlation / std::sqrt(energy);

        if (correlation > maxCorrelation) {
            maxCorrelation = correlation;
            bestSize = size;
        }
    }

    return bestSize;
}

void AudioEngine::generateGrains(Audio& audio, int firstGrainSize, int sampleRate, bool debug) {
    int grainStart = 0;
    int grainSize = firstGrainSize;
    int direction = 1;

    int totalSamples = audio.samples.size();

    audio.grains.clear();

    double prevRpm;

    for (int i = 0; grainStart + grainSize < totalSamples; i++) {
        int grainEnd = findNextGrain(audio.samples, grainStart, grainSize, direction, sampleRate, totalSamples);

        if (grainEnd == -1) {
            if (debug) std::cout << "Error at " << grainStart << " (prevSize: " << grainSize << ")\n";
            break;
        }

        double cycleLength = (double)grainSize / sampleRate;
        double rpm = (1.0 / cycleLength) * 60.0;

        double sizeDelta = grainSize - (grainEnd - grainStart);
        grainSize = grainEnd - grainStart;

        Grain grain;
        grain.start = grainStart;
        grain.length = grainSize;

        audio.grains.push_back(grain);

        if (debug && (i % 1 == 0 || prevRpm - rpm > 10.0)) {
            std::cout << i << ": " << grainStart << " - " << grainEnd << " (len: " << grainSize << ", delta: " << sizeDelta << ", " << roundTo(rpm, 2) << " RPM)\n";
        }

        grainStart = grainEnd;
        prevRpm = rpm;
    }
}

float bassState = 0.0f;
float resonancePhase = 0.0f;

void AudioEngine::interpolateGrains(const Audio& audio1, const Audio& audio2, const Grain& grain1, const Grain& grain2, std::vector<float>& outSamples, Grain& newGrain, float proportionStart, float proportionEnd, float rpm, float load, bool debug) {
    float proportionAvg = (proportionStart + proportionEnd) / 2.0;
    
    newGrain.start = outSamples.size();
    newGrain.length = (int)((1.0 - proportionAvg) * grain1.length + proportionAvg * grain2.length);

    for (int j = 0; j < newGrain.length; j++) {
        float relativePos = (float)j / (float)newGrain.length;

        float pos1 = grain1.start + (relativePos * grain1.length);
        pos1 = std::min(pos1, (float) audio1.samples.size() - 2);
        float pos2 = grain2.start + (relativePos * grain2.length);
        pos2 = std::min(pos2, (float)audio2.samples.size() - 2);

        float sample1 = std::lerp(audio1.samples[(int)pos1], audio1.samples[(int)pos1 + 1], pos1 - (int)pos1);
        float sample2 = std::lerp(audio2.samples[(int)pos2], audio2.samples[(int)pos2 + 1], pos2 - (int)pos2);

        float localProportion = std::lerp(proportionStart, proportionEnd, relativePos);

        //float resultSample = (1.0f - proportion) * sample1 + proportion * sample2;
        //float resultSample = std::sqrt(1.0f - proportion) * sample1 + std::sqrt(proportion) * sample2;
        float resultSample = pow(1.0f - localProportion, 0.85f) * sample1 + pow(localProportion, 0.85f) * sample2;

        if (load != -1.0f && rpm != -1.0f) {      
            //applyResonance(resultSample, resonancePhase, 20.0f, 0.1f * std::pow(load, 1.25), rpm);

            applySaturation(resultSample, std::lerp(0.0f, 0.2f, load));
            applyBassBoost(resultSample, bassState, std::lerp(1.0f, 1.8f, load));
        }

        outSamples.push_back(resultSample);
    }
}

void AudioEngine::interpolateAudio(const Audio& audio1, const Audio& audio2, Audio& outAudio, float proportion, bool debug) {
    int minSize = std::min(audio1.grains.size(), audio2.grains.size());

    for (int i = 0; i < minSize; i++) {
        // Modulation test
        if (i < minSize / 2.0) {
            proportion = (float)i / (minSize / 2.0);
        }
        else {
            proportion = (float)(minSize - i) / (minSize / 2.0);
        }

        Grain newGrain;

        interpolateGrains(audio1, audio2, audio1.grains[i], audio2.grains[i], outAudio.samples, newGrain, proportion, proportion, -1.0f, -1.0f, debug);

        if (debug) std::cout << i << ": " << newGrain.length << "\n";

        outAudio.grains.push_back(newGrain);
    }
}

float AudioEngine::calculateProportion(float cycleLength, float cycleLengthLower, float cycleLengthUpper) {
    if (std::abs(cycleLengthUpper - cycleLengthLower) < 0.0001f) {
        return 0.0f;
    }
    return std::clamp(1.0f - (cycleLength - cycleLengthLower) / (cycleLengthUpper - cycleLengthLower), 0.0f, 1.0f);
}

void AudioEngine::interpolateToBuffer(const Audio& audio1, const Audio& audio2, const Grain& grain1, const Grain& grain2, float proportionStart, float proportionEnd, float load) {
    std::vector<float> newSamples;
    Grain newGrain;

    interpolateGrains(audio1, audio2, grain1, grain2, newSamples, newGrain, proportionStart, proportionEnd, rpm, load, false);

    for (float sample : newSamples) {
        while (!buffer.write(sample)) {
            Sleep(1);
        }
    }
}

void AudioEngine::runGenerator() {
    SampleMap sampleMap = SampleMap(sampleRate);
    sampleMap.loadSamples("assets/samples", false);

    setUseBuffer(true);
    getBuffer().writeSilence(1000);

    // TODO change it
    float minRpm = 670;
    float maxRpm = 6000;

    float currentRpm = rpm;
    double lastGrainRpm = currentRpm;

    int grainId = 0;

    while (isRunning) {
        currentRpm = rpm;

        double avgRpm = (currentRpm + lastGrainRpm) / 2.0;
        double avgCycleLength = (rpmToMs(currentRpm) + rpmToMs(lastGrainRpm)) / 2.0;

        if (getBufferLengthMs() < std::min(100.0, avgCycleLength * 4)) {
            SamplePair samplePair = sampleMap.getClosestSamples(avgRpm);

            if (!samplePair.lowerAudio || !samplePair.upperAudio || samplePair.lowerAudio->grains.empty() || samplePair.upperAudio->grains.empty()) {
                Sleep(5);
                continue;
            }

            if (grainId >= std::min(samplePair.lowerAudio->grains.size(), samplePair.upperAudio->grains.size())) {
                grainId = 0;
            }

            // TODO better grain selection algorithm
            //Grain& grain1 = getRandomGrain(*samplePair.lowerAudio);
            Grain& grain1 = samplePair.lowerAudio->grains[grainId];
            //Grain& grain2 = getRandomGrain(*samplePair.upperAudio);
            Grain& grain2 = samplePair.upperAudio->grains[grainId];

            grainId++;

            double cycleLengthLower = rpmToMs(samplePair.upperRpm);
            double cycleLengthUpper = rpmToMs(samplePair.lowerRpm);
            //float proportion = calculateProportion(avgCycleLength, cycleLengthLower, cycleLengthUpper);
            //float proportion = 1.0f - (avgCycleLength - cycleLengthLower) / (cycleLengthUpper - cycleLengthLower);

            // TODO handle cross-pair proportions
            float proportionStart = calculateProportion(rpmToMs(lastGrainRpm), cycleLengthLower, cycleLengthUpper);
            float proportionEnd = calculateProportion(rpmToMs(currentRpm), cycleLengthLower, cycleLengthUpper);

            interpolateToBuffer(*samplePair.lowerAudio, *samplePair.upperAudio, grain1, grain2, proportionStart, proportionEnd, load);

            lastGrainRpm = currentRpm;

            //std::cout << "RPM: " << currentRpm << ", load: " << load << ", avgLen: " << avgCycleLength << ", buf: " << getBufferLengthMs() << ", low: " << samplePair.lowerRpm << "\n";
        }

        Sleep(5);
    }
}

AudioBuffer& AudioEngine::getBuffer() {
    return buffer;
}

Audio* AudioEngine::getActiveAudio() {
    return activeAudio;
}

double AudioEngine::getBufferLengthMs() {
    return 1000.0 * ((double) buffer.getSampleCount() / sampleRate);
}

void AudioEngine::setUseBuffer(bool useBuffer) {
    this->useBuffer = useBuffer;
}

bool AudioEngine::isUsingBuffer() {
    return useBuffer;
}

float AudioEngine::getPlaybackSpeed() {
    return playbackSpeed;
}

void AudioEngine::setPlaybackSpeed(float playbackSpeed) {
    this->playbackSpeed = playbackSpeed;
}

float AudioEngine::getVolume() {
    return volume;
}

void AudioEngine::setVolume(float volume) {
    volume = std::clamp(volume, 0.0f, 1.0f);
    this->volume = volume;
}

float AudioEngine::getRpm() {
    return rpm;
}

void AudioEngine::setRpm(float rpm) {
    this->rpm = rpm;
}

float AudioEngine::getLoad() {
    return load;
}

void AudioEngine::setLoad(float load) {
    load = std::clamp(load, 0.0f, 1.0f);
    this->load = load;
}
