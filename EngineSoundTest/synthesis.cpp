#define MINIAUDIO_IMPLEMENTATION
#include "miniaudio.h"

#include <iostream>
#include <vector>

struct Grain {
    int start;
    int length;
};

struct Audio {
    std::vector<float> samples = {};
    std::vector<Grain> grains = {};
    int sampleCount = 0;
    int cursor = 0;
};

double roundTo(double value, int decimalPlaces) {
    double multiplier = std::pow(10.0, decimalPlaces);
    return std::round(value * multiplier) / multiplier;
}

// Miniaudio callback
void data_callback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount) {
    float* outputF32 = (float*) pOutput;
    Audio* audio = (Audio*) pDevice->pUserData;

    for (ma_uint32 i = 0; i < frameCount; i++) {
        float sample = audio->samples[audio->cursor];

        outputF32[i * 2] = sample; // left
        outputF32[i * 2 + 1] = sample; // right

        audio->cursor++;

        if (audio->cursor >= audio->sampleCount) {
            audio->cursor = 0;
        }
    }
}

bool loadWav(const std::string& filePath, std::vector<float>& out, int sampleRate) {
    ma_decoder decoder;
    ma_decoder_config config = ma_decoder_config_init(ma_format_f32, 1, sampleRate);

    ma_result result = ma_decoder_init_file(filePath.c_str(), &config, &decoder);
    if (result != MA_SUCCESS) {
        return false;
    }

    ma_uint64 frameCount;
    ma_decoder_get_length_in_pcm_frames(&decoder, &frameCount);

    out.resize(frameCount);

    ma_uint64 framesRead;
    result = ma_decoder_read_pcm_frames(&decoder, out.data(), frameCount, &framesRead);

    ma_decoder_uninit(&decoder);
    return (result == MA_SUCCESS);
}

int findNextGrain(std::vector<float>& samples, int firstSample, int prevSize, int direction, int sampleRate, int totalSamples) {
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

void generateGrains(Audio& audio, int firstGrainSize, int cyclesPerGrain, int sampleRate, bool debug) {
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

int main() {
    int sampleRate = 44100;
    int cyclesPerGrain = 2;

    std::string file = "assets/3000.wav";
    Audio audio;

    if (!loadWav(file, audio.samples, sampleRate)) {
        std::cout << "Could not load file " << file << std::endl;
        return -1;
    }

    audio.sampleCount = audio.samples.size();

    std::cout << "Loaded " << file << " (" << audio.samples.size() << " samples)" << std::endl;

    generateGrains(audio, 1755, cyclesPerGrain, sampleRate, true);

    std::cout << "Generated: " << audio.grains.size() << " grains" << std::endl;

    ma_device_config config = ma_device_config_init(ma_device_type_playback);
    config.playback.format = ma_format_f32;
    config.playback.channels = 2;
    config.sampleRate = 44100;
    config.dataCallback = data_callback;
    config.pUserData = &audio;

    ma_device device;
    ma_device_init(NULL, &config, &device);
    ma_device_start(&device);

    std::cin.get();

    return 0;
}
