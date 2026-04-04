#ifndef AUDIO_ENGINE_H
#define AUDIO_ENGINE_H

#include <vector>
#include <string>
#include "miniaudio.h"
#include "audio.h"


class AudioEngine {
private:
    Audio* activeAudio;
    ma_device_config config;
    ma_device device;
    int sampleRate;

    static void audioCallback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount);
    void processAudio(float* pOutput, ma_uint32 frameCount);

public:
    AudioEngine(int sampleRate);

    void setAudio(Audio& audio);
    void start();
    void stop();

    void playGrain(Audio* audio, int grainId);
    void playRandomGrain(Audio* audio, int minId, int maxId);
    void playRandomGrain(Audio* audio);

    static bool loadWav(const std::string& filePath, Audio& out, int sampleRate);
    static void generateGrains(Audio& audio, int firstGrainSize, int cyclesPerGrain, int sampleRate, bool debug);
    static int findNextGrain(const std::vector<float>& samples, int firstSample, int prevSize, int direction, int sampleRate, int totalSamples);

    Audio* getActiveAudio();
};

#endif
