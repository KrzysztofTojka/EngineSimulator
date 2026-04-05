#define NOMINMAX

#include "miniaudio.h"
#include "audio_engine.h"
#include "audio.h"

#include <iostream>
#include <Windows.h>
#include <chrono>

void loadAudio(std::string path, Audio& outAudio, int firstGrainLength, int sampleRate, int cyclesPerGrain, bool debug) {
    if (!AudioEngine::loadWav(path, outAudio, sampleRate)) {
        std::cout << "Could not load file " << path << std::endl;
        return;
    }

    std::cout << "Loaded " << path << " (" << outAudio.samples.size() << " samples)" << std::endl;
    AudioEngine::generateGrains(outAudio, firstGrainLength, cyclesPerGrain, sampleRate, debug);
    std::cout << "Generated: " << outAudio.grains.size() << " grains" << std::endl;
}

void staticTest() {
    int sampleRate = 44100;
    int bufferSize = 16384; // 2^14, around 0.37s
    int cyclesPerGrain = 1;

    AudioEngine audioEngine = AudioEngine(sampleRate, bufferSize);

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

long now() {
    return std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::high_resolution_clock::now().time_since_epoch()).count();
}

double rpmToMs(double rpm) {
    return 1000.0 * (1.0 / (rpm / 60.0));
}

void liveTest() {
    int sampleRate = 44100;
    int bufferSize = 16384; // 2^14, around 0.37s
    int cyclesPerGrain = 1;

    Audio audio2000;
    loadAudio("assets/2000.wav", audio2000, 1322, sampleRate, cyclesPerGrain, false);

    Audio audio5000;
    loadAudio("assets/5000.wav", audio5000, 528, sampleRate, cyclesPerGrain, false);

    AudioEngine audioEngine = AudioEngine(sampleRate, bufferSize);
    audioEngine.playFromBuffer(true);
    //audioEngine.getBuffer().writeSilence(6000);
    audioEngine.start();

    double loopTime = 4000;

    double minRpm = 2000;
    double maxRpm = 5000;

    double maxCycleLength = rpmToMs(minRpm);
    double minCycleLength = rpmToMs(maxRpm);

    double rpm = minRpm;
    double lastGrainRpm = rpm;
    bool accel = true;

    long startTime = now();

    while (true) {
        double avgRpm = (rpm + lastGrainRpm) / 2.0;
        double avgCycleLength = rpmToMs(avgRpm);

        if (audioEngine.getBufferLengthMs() < std::min(120.0, avgCycleLength * 4)) {
            Grain& grain1 = getRandomGrain(audio2000);
            Grain& grain2 = getRandomGrain(audio5000);
            float proportion = 1.0f - (avgCycleLength - minCycleLength) / (maxCycleLength - minCycleLength);
            proportion = std::clamp(proportion, 0.0f, 1.0f);
            
            audioEngine.interpolateToBuffer(audio2000, audio5000, grain1, grain2, proportion);
            
            lastGrainRpm = rpm;

            std::cout << now() - startTime << "ms - RPM: " << rpm << ", prop:" << roundTo(proportion, 4) << ", avgLen: " << avgCycleLength << ", buf: " << audioEngine.getBufferLengthMs() << "\n";
        }

        double rpmDelta = (maxRpm - minRpm) / (loopTime / 5);

        if (accel) {
            rpm += rpmDelta;
            if (rpm > maxRpm) {
                accel = false;
            }
        } else {
            rpm -= rpmDelta;
            if (rpm < minRpm) {
                accel = true;
            }
        }

        Sleep(5);
    }
}

int main() {
    //staticTest();
    liveTest();

    std::cin.get();

    return 0;
}