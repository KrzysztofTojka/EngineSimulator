#define MINIAUDIO_IMPLEMENTATION
#define NOMINMAX

#include "audio_engine.h"
#include "math_helper.h"
#include "sample_map.h"

#include <Windows.h>
#include <timeapi.h> // Windows timer moment :)
#pragma comment(lib, "winmm.lib")

#include <iostream>
#include <algorithm>


AudioEngine::AudioEngine(int sampleRate, int bufferSize) : buffer(AudioBuffer(bufferSize)), activeAudio(nullptr), useBuffer(false), playbackSpeed(1.0f), volume(1.0f), sampleRate(sampleRate) {
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

    // -1, 40
    for (int i = firstSample + prevSize; i - (firstSample + prevSize) < 40; i++) {
        if (samples[i] > 0.0f) {
            endEstimated = i;
            break;
        }
    }

    if (endEstimated == -1) {
        return -1;
    }

    // 5, -30
    for (int i = endEstimated + 5; i > endEstimated - 30; i--) {
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

void AudioEngine::interpolateGrains(const Audio& audio1, const Audio& audio2, const Grain& grain1, const Grain& grain2, std::vector<float>& outSamples, Grain& newGrain, float proportion, float phase1, float phase2, bool debug) {
    newGrain.start = outSamples.size();
    newGrain.length = (int)((1.0 - proportion) * grain1.length + proportion * grain2.length);

    for (int j = 0; j < newGrain.length; j++) {
        float relativePos1 = (float)j / (float)newGrain.length + phase1;
        if (relativePos1 > 1.0f) {
            relativePos1 -= 1.0f;
        }
        float relativePos2 = (float)j / (float)newGrain.length + phase2;
        if (relativePos2 > 1.0f) {
            relativePos2 -= 1.0f;
        }

        float pos1 = grain1.start + (relativePos1 * grain1.length);
        float pos2 = grain2.start + (relativePos2 * grain2.length);

        float sample1 = std::lerp(audio1.samples[(int)pos1], audio1.samples[(int)pos1 + 1], pos1 - (int)pos1);
        float sample2 = std::lerp(audio2.samples[(int)pos2], audio2.samples[(int)pos2 + 1], pos2 - (int)pos2);

        //float resultSample = (1.0f - proportion) * sample1 + proportion * sample2;
        //float resultSample = std::sqrt(1.0f - proportion) * sample1 + std::sqrt(proportion) * sample2;
        float resultSample = pow(1.0f - proportion, 0.85f) * sample1 + pow(proportion, 0.85f) * sample2;
        outSamples.push_back(resultSample);
    }
}

void AudioEngine::interpolateGrains(const Audio& audio1, const Audio& audio2, const Grain& grain1, const Grain& grain2, std::vector<float>& outSamples, Grain& newGrain, float proportion, bool debug) {
    interpolateGrains(audio1, audio2, grain1, grain2, outSamples, newGrain, proportion, 0.0f, 0.0f, debug);
}

void AudioEngine::interpolateAudio(const Audio& audio1, const Audio& audio2, Audio& outAudio, float proportion, float phase1, float phase2, bool debug) {
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

        interpolateGrains(audio1, audio2, audio1.grains[i], audio2.grains[i], outAudio.samples, newGrain, proportion, phase1, phase2, debug);

        if (debug) std::cout << i << ": " << newGrain.length << "\n";

        outAudio.grains.push_back(newGrain);
    }
}

void AudioEngine::interpolateAudio(const Audio& audio1, const Audio& audio2, Audio& outAudio, float proportion, bool debug) {
    interpolateAudio(audio1, audio2, outAudio, proportion, 0.0f, 0.0f, debug);
}

void AudioEngine::interpolateToBuffer(const Audio& audio1, const Audio& audio2, const Grain& grain1, const Grain& grain2, float proportion) {
    std::vector<float> newSamples;
    Grain newGrain;

    interpolateGrains(audio1, audio2, grain1, grain2, newSamples, newGrain, proportion, false);

    for (float sample : newSamples) {
        while (!buffer.write(sample)) {
            Sleep(1);
        }
    }
}

void AudioEngine::runGenerator() {
    SampleMap sampleMap = SampleMap(sampleRate);
    sampleMap.loadSamples("assets/samples");

    setUseBuffer(true);
    getBuffer().writeSilence(1000);

    // TODO change it
    float minRpm = 1000;
    float maxRpm = 6000;

    rpm = std::clamp(rpm, minRpm, maxRpm);

    double lastGrainRpm = rpm;

    int grainId = 0;

    while (isRunning) {
        double avgRpm = (rpm + lastGrainRpm) / 2.0;
        double avgCycleLength = rpmToMs(avgRpm);

        if (getBufferLengthMs() < std::min(100.0, avgCycleLength * 4)) {
            SamplePair samplePair = sampleMap.getClosestSamples(avgRpm);

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
            float proportion = 1.0f - (avgCycleLength - cycleLengthLower) / (cycleLengthUpper - cycleLengthLower);
            proportion = std::clamp(proportion, 0.0f, 1.0f);

            interpolateToBuffer(*samplePair.lowerAudio, *samplePair.upperAudio, grain1, grain2, proportion);

            lastGrainRpm = rpm;

            //std::cout << now() - startTime << "ms - RPM: " << audioEngine.getRpm() << ", prop:" << roundTo(proportion, 4) << ", avgLen: " << avgCycleLength << ", buf: " << audioEngine.getBufferLengthMs() << ", low: " << samplePair.lowerRpm << "\n";
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
