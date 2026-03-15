using NAudio.Wave;
using System;

public class SmoothResampler : ISampleProvider {
    private readonly ISampleProvider source;
    private readonly float[] sourceBuffer;
    private float sourcePos;
    private int sourceRead;

    public WaveFormat WaveFormat => source.WaveFormat;
    public float Pitch { get; set; } = 1.0f;

    public SmoothResampler(ISampleProvider source) {
        this.source = source;
        this.sourceBuffer = new float[WaveFormat.SampleRate * WaveFormat.Channels];
    }

    public int Read(float[] buffer, int offset, int count) {
        int samplesRead = 0;
        int channels = WaveFormat.Channels;

        while (samplesRead < count) {
            if (sourcePos >= sourceRead / channels) {
                sourceRead = source.Read(sourceBuffer, 0, sourceBuffer.Length);
                sourcePos = 0;
                if (sourceRead == 0) return samplesRead;
            }

            int intPos = (int)sourcePos;
            float fraction = sourcePos - intPos;

            for (int i = 0; i < channels; i++) {
                float s1 = sourceBuffer[intPos * channels + i];
                float s2 = (intPos + 1) * channels + i < sourceRead ? sourceBuffer[(intPos + 1) * channels + i] : s1;

                buffer[offset + samplesRead++] = s1 + fraction * (s2 - s1);
            }

            sourcePos += Pitch;
        }
        return samplesRead;
    }
}