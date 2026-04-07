#ifndef AUDIO_H
#define AUDIO_H

#define NOMINMAX

#include "math_helper.h"
#include <cmath>

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

inline Grain& getRandomGrain(Audio& audio) {
    return audio.grains[randomInt(0, audio.grains.size() - 1)];
}

inline void getRandomGrains(const Audio& audio1, const Audio& audio2, Grain& grain1, Grain& grain2) {
    int rand = randomInt(0, (std::min)(audio1.grains.size(), audio2.grains.size()) - 1);
    grain1 = audio1.grains[rand];
    grain2 = audio2.grains[rand];
}

#endif
