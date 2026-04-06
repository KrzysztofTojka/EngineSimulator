#define NOMINMAX

#include "miniaudio.h"
#include "audio_engine.h"
#include "audio.h"
#include "sample_map.h"

#include <Windows.h>
#include <iostream>
#include <chrono>

void loadAudio(std::string path, Audio& outAudio, int rpm, int sampleRate, bool debug) {
    if (!AudioEngine::loadWav(path, outAudio, sampleRate)) {
        std::cout << "Could not load file " << path << std::endl;
        return;
    }

    std::cout << "Loaded " << path << " (" << outAudio.samples.size() << " samples)" << std::endl;

    int size = AudioEngine::findGrainSize(outAudio, rpm, 0, sampleRate);

    AudioEngine::generateGrains(outAudio, size, sampleRate, debug);
    std::cout << "Generated: " << outAudio.grains.size() << " grains" << std::endl;
}

void staticTest() {
    int sampleRate = 44100;
    int bufferSize = 16384; // 2^14, around 0.37s

    AudioEngine audioEngine = AudioEngine(sampleRate, bufferSize);

    //Audio audio2500;
    //loadAudio("assets/samples/2500.wav", audio2500, 1057, sampleRate, cyclesPerGrain);

    Audio audio3000;
    loadAudio("assets/samples/3000.wav", audio3000, 3000, sampleRate, false);

    //Audio audio3500;
    //loadAudio("assets/samples/3500.wav", audio3500, 754, sampleRate);

    Audio audio2000;
    loadAudio("assets/samples/3500.wav", audio2000, 3500, sampleRate, true);

    Audio audio5000;
    loadAudio("assets/samples/4000.wav", audio5000, 4000, sampleRate, false);

    Audio interpolatedAudio;
    AudioEngine::interpolateAudio(audio2000, audio5000, interpolatedAudio, 0.5f, false);
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
}

void liveTest() {
    int sampleRate = 44100;
    int bufferSize = 16384; // 2^14, around 0.37s

    AudioEngine audioEngine = AudioEngine(sampleRate, bufferSize);
    audioEngine.setUseBuffer(true);
    audioEngine.getBuffer().writeSilence(2000);
    audioEngine.start();

    double loopTime = 3300;

    float minRpm = 1000;
    float maxRpm = 6000;

    float rpm = minRpm;
    float lastGrainRpm = rpm;
    bool accel = true;

    long startTime = now();

    int grainId = 0;

    while (true) {
        double rpmDelta = (maxRpm - minRpm) / (loopTime / 5);

        if (accel) {
            rpm += rpmDelta;
            if (rpm > maxRpm) {
                accel = false;
            }
        }
        else {
            rpm -= rpmDelta;
            if (rpm < minRpm) {
                accel = true;
            }
        }

        audioEngine.setRpm(rpm);

        Sleep(5);
    }
}

int main() {
    //staticTest();
    liveTest();

    std::cin.get();

    return 0;
}