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
    int cursor = 0;
    int currentGrainId = 0;
};

#endif
