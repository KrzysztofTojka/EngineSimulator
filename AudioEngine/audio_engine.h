#ifndef AUDIO_ENGINE_H
#define AUDIO_ENGINE_H

#ifdef BUILD_DLL
#define ENGINE_API __declspec(dllexport)
#else
#define ENGINE_API
#endif

#include <vector>
#include <string>
#include "miniaudio.h"
#include "audio.h"


class ENGINE_API AudioEngine {
private:
    Audio* activeAudio;
    ma_device_config config;
    ma_device device;
    int sampleRate;

    float playbackSpeed;
    float volume;

    static void audioCallback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount);
    void processAudio(float* pOutput, ma_uint32 frameCount);

public:
    AudioEngine(int sampleRate);

    void setAudio(Audio& audio);
    void start();
    void stop();

    void playGrain(Audio* audio, int grainId);
    void playGrain(Audio* audio, int grainId, float cursorOffset);
    void playRandomGrain(Audio* audio, int minId, int maxId);
    void playRandomGrain(Audio* audio);

    static bool loadWav(const std::string& filePath, Audio& out, int sampleRate);
    static void generateGrains(Audio& audio, int firstGrainSize, int cyclesPerGrain, int sampleRate, bool debug);
    static int findNextGrain(const std::vector<float>& samples, int firstSample, int prevSize, int direction, int sampleRate, int totalSamples);

    Audio* getActiveAudio();
    float getPlaybackSpeed();
    void setPlaybackSpeed(float playbackSpeed);
    float getVolume();
    void setVolume(float volume);
};

#endif
