using EngineSimulator;
using System;
using System.Runtime.InteropServices;

public class AudioEngine {
    private const string DllPath = "AudioEngine/AudioEngine.dll";

    [DllImport(DllPath)]
    public static extern void Init(int sampleRate, bool useBuffer, int bufferSize);

    [DllImport(DllPath)]
    public static extern void StartEngine();

    [DllImport(DllPath)]
    public static extern void SetPlaybackSpeed(float speed);

    [DllImport(DllPath)]
    public static extern void SetVolume(float volume);

    [DllImport(DllPath)]
    public static extern void SetRpm(double rpm);

    [DllImport(DllPath)]
    public static extern void SetLoad(double load);

    static double prevVolume = 0.0;

    public static void Update(double rpm, double load, double dt) {
        double rpmVolume = Math.Min(1.0, Math.Pow(rpm / 800.0, 2.0));
        double loadVolume = 0.8 + 0.2 * load;
        double targetVolume = rpmVolume * loadVolume;
        double finalVolume = MathHelper.Lerp(prevVolume, targetVolume, dt / 250.0);
        AudioEngine.SetVolume((float)finalVolume);
        prevVolume = finalVolume;

        AudioEngine.SetLoad(load);
        AudioEngine.SetRpm(rpm);
    }
}