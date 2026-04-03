#define MINIAUDIO_IMPLEMENTATION
#include "miniaudio.h"

#include <iostream>
#include <vector>

struct Audio {
    std::vector<float> samples = {};
    int sampleCount = 0;
    int cursor = 0;
};

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

bool loadWav(const std::string& filePath, std::vector<float>& out) {
    ma_decoder decoder;
    ma_decoder_config config = ma_decoder_config_init(ma_format_f32, 1, 44100);

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

int main() {
    std::string file = "assets/3000.wav";
    Audio audio;

    if (!loadWav(file, audio.samples)) {
        std::cout << "Could not load file " << file << std::endl;
        return -1;
    }

    audio.sampleCount = audio.samples.size();

    std::cout << "Loaded " << file << " (" << audio.samples.size() << " samples)" << std::endl;

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
}
