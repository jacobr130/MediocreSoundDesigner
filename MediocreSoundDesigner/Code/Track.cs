using NAudio.Wave;

namespace MediocreSoundDesigner;

public enum SampleRegion {
  BEGINNING_ONLY,
  END_ONLY,
  BEGINNING_AND_END,
  ANYWHERE,
}

public class Track : IDisposable {
  // Fields
  // added this for undo button
  private readonly Stack<(float[] Samples, float Volume)> undoStack = new();
  private string file;
  private TimeSpan duration;
  private float offset;
  private float timeScale;
  public int SampleRate;
  public float[] Samples;
  private float pitch;
  private int bits;
  public int Channels;
  private AudioFileReader fileReader;
  private WaveOut outputDevice;
  private Panel panContainer;
  private int drawTimescale;

    // consts

    public int TrackNumber { get; set; }
  private static readonly Color SAMPLES_DRAW_COLOR_DISABLED = Color.LightGray;

  // properties
  public int DrawOffset { get; set; }
  public bool IsDisabled { get; private set; }
    public bool IsSolo { get; set; } = false;
  public Panel PanDrawSamples { get; private set; }
  public float Volume { get; private set; }
  private string tempFilePath => Path.Combine(Path.GetTempPath(), "MediocreSoundDesigner", "temp.wav");
  public PlaybackState State => outputDevice?.PlaybackState ?? PlaybackState.Stopped;

  // ctor
  public Track(string file) {
    fileReader = new AudioFileReader(file);
    int sampleCount = (int)(fileReader.Length / 4);
    this.Samples = new float[sampleCount];
    int samplesRead = ((ISampleProvider)fileReader).Read(Samples);
    if (samplesRead < sampleCount) {
      Array.Resize(ref Samples, samplesRead);
    }

    this.file = file;
    this.duration = fileReader.TotalTime;
    this.offset = 0;
    this.timeScale = 1.0f;
    this.Volume = 1.0f;
    this.pitch = 1.0f;
    this.bits = fileReader.WaveFormat.BitsPerSample;
    this.Channels = fileReader.WaveFormat.Channels;
    this.SampleRate = fileReader.WaveFormat.SampleRate;
    IsDisabled = false;
  }
  public Track(float[] samples) {
    this.Samples = samples;

    this.file = "";
    this.duration = TimeSpan.Zero;
    this.offset = 0;
    this.timeScale = 1.0f;
    this.Volume = 1.0f;
    this.pitch = 1.0f;
    this.bits = 32;
    this.Channels = 1;
    this.SampleRate = 44100;
    IsDisabled = false;
  }
  public void LinkPanels(Panel panDrawSamples, Panel panContainer) {
    this.PanDrawSamples = panDrawSamples;
    this.panContainer = panContainer;
  }

  // edits
  // added for undo stuff
  public bool CanUndo => undoStack.Count > 0;
  private void SaveUndoState() { undoStack.Push((CloneSamples(), Volume)); }
  public void Undo()
    {
        if (!CanUndo) return;
        var state = undoStack.Pop();
        Samples = state.Samples;
        Volume = state.Volume;
    }
  public float[] CloneSamples() {
    return (float[])Samples.Clone();
  }
  public void ReplaceSamples(float[] samples) {
    this.Samples = samples;
  }
  // end of undo stuff

  // added for reverb
  public void Reverb(float decay, float mix) 
  {
    SaveUndoState();
    int rate = SampleRate > 0 ? SampleRate : 44100;
    int ch = Math.Max(1, Channels);
    int frames = Samples.Length / ch;
    int outFrames = frames + (int)(rate * 1.5f);
    float[] result = new float[outFrames * ch];

    for (int c = 0; c < ch; c++) 
    {
      float[] dry = new float[outFrames];
      for (int f = 0; f < frames; f++) dry[f] = Samples[f * ch + c];

      float[] wet = new float[outFrames];
      foreach (float ms in new[] { 29.7f, 37.1f, 41.1f, 43.7f })
      {
        int d = (int)(rate * ms / 1000f);
        float[] comb = new float[outFrames];
        for (int n = 0; n < outFrames; n++) 
        {
          float feedback = n >= d ? comb[n - d] * decay : 0f;
          comb[n] = dry[n] + feedback;
          wet[n] += comb[n] * 0.25f;
        }
      }

      wet = AllPass(wet, (int)(rate * 0.005f), 0.7f);
      wet = AllPass(wet, (int)(rate * 0.0017f), 0.7f);

      for (int n = 0; n < outFrames; n++) 
      {
        result[n * ch + c] = dry[n] * (1f - mix) + wet[n] * mix;
      }
    }

    float peak = 0f;
    foreach (float s in result) peak = Math.Max(peak, Math.Abs(s));
    if (peak > 1f)
    {
      for (int i = 0; i < result.Length; i++) result[i] /= peak;
    }
    Samples = result;
    PanDrawSamples.Invalidate();
    }

  private static float[] AllPass(float[] x, int delay, float g)
  {
    float[] y = new float[x.Length];
    for (int n = 0; n < x.Length; n++)
    {
      float xd = n >= delay ? x[n - delay] : 0f;
      float yd = n >= delay ? y[n - delay] : 0f;
      y[n] = -g * x[n] + xd + g *yd;
    }
    return y;
  }

  // end of reverb stuff
  public void Reverse() {
    SaveUndoState();
    for (int i = 0; i < Samples.Length / 2; i++) {
      var temp = Samples[i];
      Samples[i] = Samples[Samples.Length - 1 - i];
      Samples[Samples.Length - 1 - i] = temp;
      PanDrawSamples.Invalidate();
    }
  }
  public void Flip() {
    SaveUndoState();
    for (int i = 0; i < Samples.Length; i++) {
      Samples[i] *= -1;
      PanDrawSamples.Invalidate();
    }
  }
  public void Trim() {
  }
  public void AlterTimeScale(float scale) {
  }
  public void SetVolume(float newVolume) {
    if (newVolume == Volume) return;
    SaveUndoState();
    Volume = newVolume;
    PanDrawSamples.Invalidate();
  }
  public void EliminateSilence(SampleRegion region, float minDuration, float thres) {
    SaveUndoState();
    float[] newSamples = CloneSamples();
    if (region is SampleRegion.BEGINNING_ONLY or SampleRegion.BEGINNING_AND_END) {
      int silentDurationInNumSamples = 0;
      for (int i = 0; i < newSamples.Length; i++) {
        if (Math.Abs(newSamples[i]) <= thres) {
          silentDurationInNumSamples++;
        }
        else {
          break;
        }
      }
      if ((float)silentDurationInNumSamples / SampleRate >= minDuration) {
        newSamples = newSamples[silentDurationInNumSamples..];
      }
    }
    if (region is SampleRegion.END_ONLY or SampleRegion.BEGINNING_AND_END) {
      int silentDurationInNumSamples = 0;
      for (int i = newSamples.Length - 1; i >= 0; i--) {
        if (Math.Abs(newSamples[i]) <= thres) {
          silentDurationInNumSamples++;
        }
        else {
          break;
        }
      }
      if ((float)silentDurationInNumSamples / SampleRate >= minDuration) {
        newSamples = newSamples[0..^silentDurationInNumSamples];
      }
    }
    if (region is SampleRegion.ANYWHERE) {
      int silentDurationInNumSamples = 0;
      int silentRegionStart = 0;
      int i;
      for (i = 0; i < newSamples.Length; i++) {
        if (Math.Abs(newSamples[i]) <= thres) {
          silentDurationInNumSamples++;
        }
        else {
          if ((float)silentDurationInNumSamples / SampleRate >= minDuration) {
            newSamples = [..newSamples[0..silentRegionStart], ..newSamples[i..]];
            i = 0;
          }
          silentRegionStart = i + 1;
          silentDurationInNumSamples = 0;
        }
      }
      if ((float)silentDurationInNumSamples / SampleRate >= minDuration) {
        newSamples = [.. newSamples[0..silentRegionStart], .. newSamples[i..]];
      }
    }
    Samples = newSamples;
    PanDrawSamples.Invalidate();
  }

    // returns an object that supplies the track's samples to the mixer
    public ISampleProvider GetSampleProvider()
    {
        return new TrackSampleProvider(this);
    }

  public void ToggleDisabled() {
    IsDisabled = !IsDisabled;
  }

    // file i/o
  public void SaveToFile(string path) {
    Directory.CreateDirectory(Path.GetDirectoryName(path));
    using var writer = new WaveFileWriter(path, fileReader?.WaveFormat ?? new());
    writer.WriteSamples(GetAdjustedSamples(), 0, Samples.Length);
  }
  public float[] GetAdjustedSamples() {
    float[] adjSamples = new float[Samples.Length];
    for (int s = 0; s < Samples.Length; s++) {
      adjSamples[s] = Samples[s] * Volume;
    }
    float[] deadSpace = new float[DrawOffset * drawTimescale];
    for (int i = 0; i < DrawOffset; i++) {
      deadSpace[i] = 0.0f;
    }

    return [.. deadSpace, .. adjSamples];
  }

  // render
  public void ShiftSampleOver(int amount) {
    DrawOffset += amount;
    DrawOffset = Math.Max(0, DrawOffset);
  }
  public void DrawSamples(Graphics g, int drawTimescale) {
    if (PanDrawSamples is null) {
      return;
    }

    this.drawTimescale = drawTimescale;
    float heightAdj = PanDrawSamples.Height / 2;
    if (Samples != null && Samples.Length > 0) {
      int x = DrawOffset;
      Pen pen = new(IsDisabled ? SAMPLES_DRAW_COLOR_DISABLED : Color.Black);
      List<Point> points = new();
      for (int s = 0; s < Samples.Length; s += drawTimescale) {
        var sample = (int)(Samples[s] * heightAdj * Volume);
        points.Add(new(x, PanDrawSamples.Height / 2));
        points.Add(new(x, PanDrawSamples.Height / 2 - sample));
        x++;
      }
      g.DrawLines(pen, points.ToArray());
      pen.Dispose();
    }
  }

  // dispose
  public void Dispose() {
    outputDevice?.Dispose();
    fileReader?.Dispose();
  }
}
