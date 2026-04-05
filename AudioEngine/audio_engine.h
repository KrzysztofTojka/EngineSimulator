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
#include "audio_buffer.h"


class ENGINE_API AudioEngine {
private:
    AudioBuffer buffer;
    Audio* activeAudio;
    ma_device_config config;
    ma_device device;
    int sampleRate;
    bool useBuffer;

    float playbackSpeed;
    float volume;

    static void audioCallback(ma_device* pDevice, void* pOutput, const void* pInput, ma_uint32 frameCount);
    void processAudioStatic(float* pOutput, ma_uint32 frameCount);
    void processAudioBuffer(float* pOutput, ma_uint32 frameCount);

public:
    AudioEngine(int sampleRate, int bufferSize);

    void setAudio(Audio& audio);
    void start();
    void stop();

    void playGrain(Audio* audio, int grainId);
    void playGrain(Audio* audio, int grainId, float cursorOffset);
    void playRandomGrain(Audio* audio, int minId, int maxId);
    void playRandomGrain(Audio* audio);

    static bool loadWav(const std::string& filePath, Audio& output, int sampleRate);
    static bool saveWav(const std::string& filePath, const Audio& input, int sampleRate);
    
    static void generateGrains(Audio& audio, int firstGrainSize, int cyclesPerGrain, int sampleRate, bool debug);
    static int findNextGrain(const std::vector<float>& samples, int firstSample, int prevSize, int direction, int sampleRate, int totalSamples);
    static void interpolateGrains(const Audio& audio1, const Audio& audio2, const Grain& grain1, const Grain& grain2, std::vector<float>& outSamples, Grain& newGrain, float proportion, float phase1, float phase2, bool debug);
    static void interpolateGrains(const Audio& audio1, const Audio& audio2, const Grain& grain1, const Grain& grain2, std::vector<float>& outSamples, Grain& newGrain, float proportion, bool debug);
    static void interpolateAudio(const Audio& audio1, const Audio& audio2, Audio& outAudio, float proportion, float phase1, float phase2, bool debug);
    static void interpolateAudio(const Audio& audio1, const Audio& audio2, Audio& outAudio, float proportion, bool debug);

    void interpolateToBuffer(const Audio& audio1, const Audio& audio2, const Grain& grain1, const Grain& grain2, float proportion);

    AudioBuffer& getBuffer();
    Audio* getActiveAudio();
    double getBufferLengthMs();
    void playFromBuffer(bool useBuffer);
    bool isUsingBuffer();
    float getPlaybackSpeed();
    void setPlaybackSpeed(float playbackSpeed);
    float getVolume();
    void setVolume(float volume);
};

#endif
