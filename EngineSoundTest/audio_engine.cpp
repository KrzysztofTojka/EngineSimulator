#define MINIAUDIO_IMPLEMENTATION

#include "audio_engine.h"
#include "math_helper.h"

#include <iostream>
#include <algorithm>


AudioEngine::AudioEngine(int sampleRate) {
    config = ma_device_config_init(ma_device_type_playback);
    config.playback.format = ma_format_f32;
    config.playback.channels = 2;
    this->sampleRate = sampleRate;
    config.sampleRate = sampleRate;
    config.dataCallback = AudioEngine::audioCallback;
    config.pUserData = this;

    activeAudio = nullptr;

    ma_device_init(NULL, &config, &device);
}

void AudioEngine::audioCallback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount) {
    AudioEngine* audioEngine = (AudioEngine*)pDevice->pUserData;

    audioEngine->processAudio((float*)pOutput, frameCount);
}

void AudioEngine::processAudio(float* pOutput, ma_uint32 frameCount) {
    Audio* audio = activeAudio;

    for (ma_uint32 i = 0; i < frameCount; i++) {
        float sample = audio->samples[audio->cursor];

        pOutput[i * 2] = sample; // left
        pOutput[i * 2 + 1] = sample; // right

        audio->cursor++;

        if (activeAudio->grains.size() == 0 && audio->cursor >= audio->sampleCount) {
            audio->cursor = 0;
            continue;
        }

        Grain* currentGrain = &audio->grains[audio->currentGrainId];

        if (audio->cursor >= currentGrain->start + currentGrain->length) {
            if (audio->currentGrainId + 1 >= audio->grains.size()) {
                playGrain(audio, 0);
                continue;
            }

            playGrain(audio, audio->currentGrainId + 1);
        }
    }
}

void AudioEngine::setAudio(Audio& audio) {
    audio.currentGrainId = 0;
    audio.cursor = 0;
    activeAudio = &audio;
}

void AudioEngine::start() {
    ma_device_start(&device);
}

void AudioEngine::stop() {
    ma_device_stop(&device);
}

void AudioEngine::playGrain(Audio* audio, int grainId) {
    grainId = std::clamp(grainId, 0, (int)audio->grains.size() - 1);
    audio->currentGrainId = grainId;
    audio->cursor = audio->grains[grainId].start;
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

int AudioEngine::findNextGrain(const std::vector<float>& samples, int firstSample, int prevSize, int direction, int sampleRate, int totalSamples) {
    int endEstimated = -1;

    for (int i = firstSample + prevSize; i - (firstSample + prevSize) < 10; i++) {
        if (samples[i] > 0.0f) {
            endEstimated = i;
            break;
        }
    }

    if (endEstimated == -1) {
        return -1;
    }

    for (int i = endEstimated + 5; i > endEstimated - 10; i--) {
        if (std::signbit(samples[i]) != std::signbit(samples[i - 1])) {
            int lastSample = i;
            int size = lastSample - firstSample;
            return lastSample;
        }
    }

    return -1;
}

void AudioEngine::generateGrains(Audio& audio, int firstGrainSize, int cyclesPerGrain, int sampleRate, bool debug) {
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

        double lengthSeconds = (double)grainSize / sampleRate;
        double cycleLength = lengthSeconds / cyclesPerGrain;
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

Audio* AudioEngine::getActiveAudio() {
    return activeAudio;
}