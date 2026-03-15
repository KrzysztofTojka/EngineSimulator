using System;
using EngineSimulator;
using NAudio.Extras;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

public class EngineSoundPlayer {
    private IWavePlayer outputDevice;
    private SmoothResampler resampler;
    private VolumeSampleProvider volumeControl;
    private float baseRpm;

    public EngineSoundPlayer(string filePath, float baseRpm) {
        this.baseRpm = baseRpm;
        var reader = new AudioFileReader(filePath);
        var loop = new LoopStream(reader);

        resampler = new SmoothResampler(loop.ToSampleProvider());

        volumeControl = new VolumeSampleProvider(resampler);
        volumeControl.Volume = 0.5f;

        outputDevice = new WaveOutEvent() { DesiredLatency = 100 };
        outputDevice.Init(volumeControl);
    }

    public void SetVolume(float volume) {
        volumeControl.Volume = MathHelper.Clamp(volume, 0.0f, 1.0f);
    }

    public void SetRPM(float currentRpm) {
        //resampler.Pitch = currentRpm / baseRpm;
        resampler.Pitch = (float)(1.0 * Math.Pow(currentRpm / baseRpm, 0.6));
    }

    public void Play() => outputDevice.Play();
}