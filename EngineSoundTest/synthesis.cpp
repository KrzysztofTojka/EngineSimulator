#define MINIAUDIO_IMPLEMENTATION
#include "miniaudio.h"

#include <iostream>
#include <vector>

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
    std::vector<float> samples;

    if (!loadWav(file, samples)) {
        std::cout << "Could not load file " << file << std::endl;
        return -1;
    }

    std::cout << "Loaded " << file << " (" << samples.size() << " samples)" << std::endl;
}
