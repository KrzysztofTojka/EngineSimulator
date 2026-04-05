#ifndef AUDIO_BUFFER_H
#define AUDIO_BUFFER_H

#include <vector>
#include <atomic>
#include <cassert>

class AudioBuffer {
private:
	int size;
	std::vector<float> samples;
	alignas(64) std::atomic<int> writeIndex;
	alignas(64) std::atomic<int> readIndex;

public:
	AudioBuffer(int size) {
		assert(size > 0 && (size & (size - 1)) == 0 && "Size must be power of 2");

		this->size = size;
		this->writeIndex = 0;
		this->readIndex = 0;
		samples.resize(size);
	}

	int getFreeSpace() {
		return size - getSampleCount() - 1; // n - 1 rule
	}

	int getSampleCount() {
		int write = writeIndex.load(std::memory_order_acquire);
		int read = readIndex.load(std::memory_order_relaxed);

		return write >= read ? write - read : size - (read - write);
	}

	bool read(float& outSample) {
		int read = readIndex.load(std::memory_order_relaxed);
		int write = writeIndex.load(std::memory_order_acquire);
		if (read == write) {
			return false; // empty
		}

		outSample = samples[read];

		read = (read + 1) & (size - 1); // modulo size
		readIndex.store(read, std::memory_order_release); // ready to write
		return true;
	}

	bool write(float sample) {
		int write = writeIndex.load(std::memory_order_relaxed);
		int read = readIndex.load(std::memory_order_acquire);

		if (((write + 1) & (size - 1)) == read) {
			return false; // full
		}

		samples[write] = sample;

		write = (write + 1) & (size - 1); // modulo size
		writeIndex.store(write, std::memory_order_release); // ready to read
		return true;
	}

	bool writeSilence(int length) {
		int write = writeIndex.load(std::memory_order_relaxed);
		int read = readIndex.load(std::memory_order_acquire);

		for (int i = 0; i < length; i++) {
			if (((write + 1) & (size - 1)) == read) {
				writeIndex.store(write, std::memory_order_release);
				return false; // full
			}

			samples[write] = 0.0f;
			write = (write + 1) & (size - 1); // modulo size
		}

		writeIndex.store(write, std::memory_order_release);
		return true;
	}
};

#endif
