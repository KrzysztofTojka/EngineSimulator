#ifndef MATH_HELPER_H
#define MATH_HELPER_H

#include <random>

static std::random_device random;
static std::mt19937 randGen(random());

double roundTo(double value, int decimalPlaces) {
    double multiplier = std::pow(10.0, decimalPlaces);
    return std::round(value * multiplier) / multiplier;
}

int randomInt(int min, int max) {
    std::uniform_int_distribution<> dis(min, max);
    return dis(randGen);
}

#endif
