#ifndef AUDIO_H
#define AUDIO_H

#include "math_helper.h"

struct Grain {
    int start;
    int length;
};

struct Audio {
	std::vector<float> samples = {};
    std::vector<Grain> grains = {};
    float cursor = 0.0f;
    int currentGrainId = 0;
};

inline static Grain& getRandomGrain(Audio& audio) {
    return audio.grains[randomInt(0, audio.grains.size() - 1)];
}

#endif
