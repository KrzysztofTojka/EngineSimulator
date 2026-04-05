#define MINIAUDIO_IMPLEMENTATION
#define NOMINMAX

#include "audio_engine.h"
#include "math_helper.h"

#include <iostream>
#include <algorithm>


AudioEngine::AudioEngine(int sampleRate) {
    this->config = ma_device_config_init(ma_device_type_playback);
    config.playback.format = ma_format_f32;
    config.playback.channels = 2;
    config.sampleRate = sampleRate;
    config.dataCallback = AudioEngine::audioCallback;
    config.pUserData = this;

    this->activeAudio = nullptr;
    this->playbackSpeed = 1.0f;
    this->volume = 1.0f;
    this->sampleRate = sampleRate;

    ma_device_init(NULL, &config, &device);
}

void AudioEngine::audioCallback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount) {
    AudioEngine* audioEngine = (AudioEngine*)pDevice->pUserData;

    audioEngine->processAudio((float*)pOutput, frameCount);
}

void AudioEngine::processAudio(float* pOutput, ma_uint32 frameCount) {
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

void AudioEngine::setAudio(Audio& audio) {
    audio.currentGrainId = 0;
    audio.cursor = 0.0f;
    activeAudio = &audio;
}

void AudioEngine::start() {
    ma_device_start(&device);
}

void AudioEngine::stop() {
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
        float resultSample = std::sqrt(1.0f - proportion) * sample1 + std::sqrt(proportion) * sample2;
        outSamples.push_back(resultSample);
    }
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

Audio* AudioEngine::getActiveAudio() {
    return activeAudio;
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
