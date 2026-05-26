#ifndef SOUND_EFFECTS_H
#define SOUND_EFFECTS_H

#include "math_helper.h"
#include <vector>

inline void applySaturation(float& sample, float saturation) {
    sample = std::copysign(std::pow(std::abs(sample), 1.0f / (saturation + 1.0f)), sample);
}

inline void applyBassBoost(float& sample, float& state, float gain) {
    float low = state + 0.1f * (sample - state);
    state = low;
    sample = sample + (low * (gain - 1.0f));
}

inline void applyResonance(float& sample, float& phase, float targetFreq, float power, float engineRpm) {
    float engineFreq = (engineRpm * 4) / 120.0f;
    float finalFreq = engineFreq + targetFreq;

    phase += (2.0f * M_PI * finalFreq) / 44100.0f;
    if (phase > 2.0f * M_PI) {
        phase -= 2.0f * M_PI;
    }

    sample += std::sin(phase) * power;
}

inline void applyLowPass(float& sample, float& state, float cutoff) {
    state = state + cutoff * (sample - state);
    sample = state;
}

#endif
