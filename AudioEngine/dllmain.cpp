#include <windows.h>
#include "audio_engine.h"

AudioEngine* audioEngine = nullptr;
Audio audio;

extern "C" {
    __declspec(dllexport) void Init(int sampleRate, bool useBuffer, int bufferSize) {
        if (!audioEngine) {
            audioEngine = new AudioEngine(sampleRate, bufferSize);
            audioEngine->setUseBuffer(useBuffer);
        }
    }

    __declspec(dllexport) bool LoadAudio(const char* path, int sampleRate, int firstGrainSize) {
        if (!audioEngine) {
            return false;
        }

        if (AudioEngine::loadWav(path, audio, sampleRate)) {
            AudioEngine::generateGrains(audio, firstGrainSize, sampleRate, false);
            audioEngine->setAudio(audio);
            return true;
        }

        return false;
    }

    __declspec(dllexport) void StartEngine() {
        if (audioEngine) {
            audioEngine->start();
        }
    }

    __declspec(dllexport) void SetPlaybackSpeed(float speed) {
        if (audioEngine) {
            audioEngine->setPlaybackSpeed(speed);
        }
    }

    __declspec(dllexport) void SetVolume(float volume) {
        if (audioEngine) {
            audioEngine->setVolume(volume);

        }
    }

    __declspec(dllexport) void SetRpm(double rpm) {
        if (audioEngine) {
            audioEngine->setRpm((float) rpm);
        }
    }

    __declspec(dllexport) void SetLoad(double load) {
        if (audioEngine) {
            audioEngine->setLoad((float)load);
        }
    }
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    return TRUE;
}