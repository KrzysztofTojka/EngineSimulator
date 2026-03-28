#include "soloud.h"
#include "soloud_wav.h"

#include <iostream>
#include <cmath>
#include <algorithm>

const double MAX_RPM = 6000;

static double getSpeed(double rpm, double baseRpm) {
    //return rpm / baseRpm;
    return std::pow(rpm / baseRpm, 0.6);
}

static double getVolume(double rpm, double baseRpm, double hearRadius) {
    double t = std::clamp(std::abs(rpm - baseRpm) / hearRadius, 0.0, 1.0);
    return std::cos(t * M_PI * 0.5);
    //return 1.0 - (std::pow(t, 2));
}

int main() {
    SoLoud::Soloud soloud;
    
    soloud.init();
    std::cout << "backend: " << soloud.getBackendString() << std::endl;
    
    SoLoud::Wav soundIdle;
    soundIdle.load("assets/idle.wav");
    soundIdle.setLooping(true);
    int handleIdle = soloud.play(soundIdle, 1.0);

    SoLoud::Wav sound3000;
    sound3000.load("assets/3000.wav");
    sound3000.setLooping(true);
    int handle3000 = soloud.play(sound3000, 0.0);

    Sleep(3000);

    std::cout << "START" << std::endl;

    double nextLogRpm = 750.0;

    for (double rpm = 750.0; rpm <= MAX_RPM; rpm += (1 * 600.0 * std::lerp(0.8, 1.5, std::pow(rpm / MAX_RPM, 1.5)) / (1000.0 / 10.0))) {
        soloud.setRelativePlaySpeed(handleIdle, getSpeed(rpm, 700));
        soloud.setVolume(handleIdle, getVolume(rpm, 700, 1000));

        soloud.setRelativePlaySpeed(handle3000, getSpeed(rpm, 3000));
        soloud.setVolume(handle3000, std::lerp(0.0, 1.0, std::clamp((rpm - 750) / (1100 - 750), 0.0, 1.0)));

        if (rpm >= nextLogRpm) {
            std::cout << "RPM " << (int)rpm << "\n";
            nextLogRpm += 100.0;
        }
        
        Sleep(10);
    }

    for (double rpm = MAX_RPM; rpm >= 750.0; rpm -= (1 * 1200.0 * std::lerp(0.4, 1.5, std::pow(rpm / MAX_RPM, 1.5)) / (1000.0 / 10.0))) {
        soloud.setRelativePlaySpeed(handleIdle, getSpeed(rpm, 700));
        soloud.setVolume(handleIdle, getVolume(rpm, 700, 1000));

        soloud.setRelativePlaySpeed(handle3000, getSpeed(rpm, 3000));
        soloud.setVolume(handle3000, std::lerp(0.0, 1.0, std::clamp((rpm - 750) / (1100 - 750), 0.0, 1.0)));

        if (rpm <= nextLogRpm) {
            std::cout << "RPM " << (int)rpm << "\n";
            nextLogRpm -= 100.0;
        }

        Sleep(10);
    }

    std::cout << "STOP" << std::endl;

    Sleep(3000);

    soloud.deinit();
    return 0;
}