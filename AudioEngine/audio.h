#ifndef AUDIO_H
#define AUDIO_H

struct Grain {
    int start;
    int length;
};

struct Audio {
	std::vector<float> samples = {};
    std::vector<Grain> grains = {};
    int sampleCount = 0;
    float cursor = 0.0f;
    int currentGrainId = 0;
};

#endif
