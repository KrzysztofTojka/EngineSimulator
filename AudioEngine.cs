using EngineSimulator;
using System;
using System.Runtime.InteropServices;

public class AudioEngine {
    private const string DllPath = "AudioEngine/AudioEngine.dll";

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void Init(int sampleRate, bool useBuffer, int bufferSize);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool LoadAudio(string path, int sampleRate, int firstGrainSize);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void StartEngine();

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetPlaybackSpeed(float speed);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetVolume(float volume);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SetRpm(double rpm);

    static double prevVolume = 0.0;

    public static void Update(double rpm, double load, double dt) {
        double rpmVolume = Math.Min(1.0, Math.Pow(rpm / 1200.0, 1.5));
        double loadVolume = 0.4 + 0.6 * load;
        double targetVolume = rpmVolume * loadVolume;
        double finalVolume = MathHelper.Lerp(prevVolume, targetVolume, dt / 250.0);
        prevVolume = finalVolume;

        AudioEngine.SetVolume((float)finalVolume);
        AudioEngine.SetRpm(rpm);
    }


}