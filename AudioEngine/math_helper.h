#ifndef MATH_HELPER_H
#define MATH_HELPER_H

#include <random>
#include <chrono>

static std::random_device random;
static std::mt19937 randGen(random());

inline static double roundTo(double value, int decimalPlaces) {
    double multiplier = std::pow(10.0, decimalPlaces);
    return std::round(value * multiplier) / multiplier;
}

inline static int randomInt(int min, int max) {
    std::uniform_int_distribution<> dis(min, max);
    return dis(randGen);
}

inline static long now() {
    return std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::high_resolution_clock::now().time_since_epoch()).count();
}

inline static double rpmToMs(double rpm) {
    return 1000.0 * (1.0 / (rpm / 60.0));
}

inline static double rpmToSamples(double rpm, int sampleRate) {
    return (1.0 / (rpm / 60.0)) * sampleRate;
}

#endif
