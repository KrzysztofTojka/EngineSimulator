using EngineSimulator;
using System;
using System.Runtime.InteropServices;

public class AudioEngine {
    private const string DllPath = "AudioEngine/AudioEngine.dll";

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Init(int sampleRate);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool LoadAudio(string path, int sampleRate, int firstGrainSize);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void StartEngine();

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetPlaybackSpeed(float speed);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetVolume(float volume);

    static double prevVolume = 0.0;

    public static void Update(double rpm, double load, double dt) {
        float speed = (float)Math.Pow(rpm / 3000, 0.75);
        AudioEngine.SetPlaybackSpeed(Math.Max(0.2f, speed));

        double baseVolume = Math.Min(Math.Pow(rpm / 4000, 0.6), 1.0);
        double loadVolume = 0.25 + 0.75 * load;
        double targetVolume = baseVolume * loadVolume;
        double finalVolume = MathHelper.Lerp(prevVolume, targetVolume, dt / 100.0);

        AudioEngine.SetVolume((float) finalVolume);
        prevVolume = finalVolume;
    }
}