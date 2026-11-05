using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace MediocreSoundDesigner
{
    internal class TrackSampleProvider : ISampleProvider
    {
        public WaveFormat WaveFormat { get; }
        private readonly Track track;
        private int pos = 0;

        public TrackSampleProvider(Track track)
        {
            this.track = track;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(track.SampleRate, track.Channels);
        }

        public int Read(Span<float> buf)
        {   
            int count = Math.Min(buf.Length, Math.Max(0, track.Samples.Length - pos));

            if (track.IsDisabled)
            {
                buf[..count].Clear();
            }
            else
            {
                float volume = track.Volume;
                for (int i = 0; i < count; i++)
                {
                    buf[i] = track.Samples[pos + i] * volume;
                }
            }

            pos += count;

            return count; 
        }

    }
}
