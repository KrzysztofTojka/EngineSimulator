#include "miniaudio.h"
#include "audio_engine.h"
#include "audio.h"

#include <iostream>
#include <Windows.h>

void loadAudio(std::string path, Audio& outAudio, int firstGrainLength, int sampleRate, int cyclesPerGrain, bool debug) {
    if (!AudioEngine::loadWav(path, outAudio, sampleRate)) {
        std::cout << "Could not load file " << path << std::endl;
        return;
    }

    std::cout << "Loaded " << path << " (" << outAudio.samples.size() << " samples)" << std::endl;
    AudioEngine::generateGrains(outAudio, firstGrainLength, cyclesPerGrain, sampleRate, debug);
    std::cout << "Generated: " << outAudio.grains.size() << " grains" << std::endl;
}

int main() {
    int sampleRate = 44100;
    int cyclesPerGrain = 1;

    AudioEngine audioEngine = AudioEngine(sampleRate);

    //Audio audio2500;
    //loadAudio("assets/2500.wav", audio2500, 1057, sampleRate, cyclesPerGrain);

    Audio audio3000;
    loadAudio("assets/3000.wav", audio3000, 877, sampleRate, cyclesPerGrain, false);

    //Audio audio3500;
    //loadAudio("assets/3500.wav", audio3500, 754, sampleRate, cyclesPerGrain);

    Audio audio2000;
    loadAudio("assets/2000.wav", audio2000, 1322, sampleRate, cyclesPerGrain, true);

    Audio audio5000;
    loadAudio("assets/5000.wav", audio5000, 528, sampleRate, cyclesPerGrain, false);

    Audio interpolatedAudio;
    AudioEngine::interpolateAudio(audio2000, audio5000, interpolatedAudio, 0.5f, 0.0f, 0.4f, false);
    std::cout << "Interpolated " << interpolatedAudio.grains.size() << " grains (" << interpolatedAudio.samples.size() << " samples)" << std::endl;

    AudioEngine::saveWav("assets/interpolated.wav", interpolatedAudio, sampleRate);

    audioEngine.setAudio(interpolatedAudio);
    audioEngine.setPlaybackSpeed(1.0f);
    audioEngine.start();

    std::cout << "Playing (speed: " << audioEngine.getPlaybackSpeed() << ")" << std::endl;

    for (int i = 0; i < 500; i++) {
        std::cout << "grainId: " << audioEngine.getActiveAudio()->currentGrainId << ", cursor: " << audioEngine.getActiveAudio()->cursor << "\n";
        Sleep(100);
    }

    std::cin.get();

    return 0;
}