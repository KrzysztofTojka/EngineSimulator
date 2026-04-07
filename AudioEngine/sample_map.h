#ifndef SAMPLE_MAP_H
#define SAMPLE_MAP_H

#include <map>
#include <filesystem>
#include <regex>
#include <iostream>
#include "audio.h"
#include "audio_engine.h"

namespace fs = std::filesystem;

struct SamplePair {
	int lowerRpm;
	int upperRpm;
	Audio* lowerAudio;
	Audio* upperAudio;
};

class SampleMap {
private:
	std::map<int, Audio> samples;
	int sampleRate;
	bool empty;

public:
	SampleMap(int sampleRate) : sampleRate(sampleRate), empty(true) {

	}

	SampleMap(int sampleRate, std::vector<int> rpms) : sampleRate(sampleRate), empty(false) {
		for (int rpm : rpms) {
			samples[rpm] = Audio();
		}
	}

	void loadSamples(std::string dirPath, bool debug) {
		fs::path dir = fs::path(dirPath);
		for (auto& entry : fs::directory_iterator(dir)) {
			if (!entry.is_regular_file()) {
				continue;
			}

			std::string fileName = entry.path().filename().string();
			std::string extension = entry.path().extension().string();

			if (extension != ".wav") {
				continue;
			}

			std::smatch result;
			std::regex pattern = std::regex("^(\\d+)\.wav$");

			if (std::regex_search(fileName, result, pattern)) {
				int rpm = std::stoi(result[1]);

				if (!empty && !samples.contains(rpm)) {
					continue;
				}

				Audio& audio = samples[rpm];
				std::string samplePath = dirPath + "/" + fileName;

				AudioEngine::loadWav(samplePath, audio, sampleRate);
				if (debug) std::cout << "Loaded " << fileName << " - " << audio.samples.size() << std::endl;

				int grainSize = AudioEngine::findGrainSize(audio, rpm, 0, sampleRate);
				if (debug) std::cout << "Grain size: " << grainSize << std::endl;

				AudioEngine::generateGrains(audio, grainSize, sampleRate, debug);
				if (debug) std::cout << "Generated: " << audio.grains.size() << " grains" << std::endl;
				if (debug) std::cout << "---------------------------------------------------" << std::endl;
			}
		}

		empty = false;
	}

	SamplePair getClosestSamples(double rpm) {
		int minRpm = samples.begin()->first;

		if (rpm <= minRpm) {
			return { minRpm, minRpm, &samples[minRpm], &samples[minRpm] };
		}

		int maxRpm = samples.rbegin()->first;

		if (rpm >= maxRpm) {
			return { maxRpm, maxRpm, &samples[maxRpm], &samples[maxRpm] };
		}

		auto itUpper = samples.lower_bound((int)rpm);

		if (itUpper == samples.begin()) {
			return { minRpm, minRpm, &itUpper->second, &itUpper->second };
		}

		auto itLower = std::prev(itUpper);

		return { itLower->first, itUpper->first, &(itLower->second), &(itUpper->second) };
	}

};

#endif
