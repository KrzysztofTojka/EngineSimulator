#include "miniaudio.h"
#include "audio_engine.h"
#include "audio.h"

#include <iostream>
#include <Windows.h>

int main() {
    int sampleRate = 44100;
    int cyclesPerGrain = 2;

    AudioEngine audioEngine = AudioEngine(sampleRate);

    std::string file = "assets/3000.wav";
    Audio audio;

    if (!AudioEngine::loadWav(file, audio, sampleRate)) {
        std::cout << "Could not load file " << file << std::endl;
        return -1;
    }

    audio.sampleCount = audio.samples.size();

    std::cout << "Loaded " << file << " (" << audio.samples.size() << " samples)" << std::endl;

    AudioEngine::generateGrains(audio, 1755, cyclesPerGrain, sampleRate, true);

    std::cout << "Generated: " << audio.grains.size() << " grains" << std::endl;

    audioEngine.setAudio(audio);
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
