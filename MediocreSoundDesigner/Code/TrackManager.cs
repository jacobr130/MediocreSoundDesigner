using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.IO.Pipelines;

namespace MediocreSoundDesigner; 

public class TrackManager {
  private Track pseudoCombinedTrack;
  private WaveOut outputDevice;
  private WaveFormat playbackWaveFormat;
  private Panel panContainer;
  private bool paused = false;

  public List<CtrlTrackRow> TrackRows { get; private set; }
    public CtrlTrackRow SelectedTrackRow { get; private set; }
  public int DrawTimescale { get; set; }
  public PlaybackState State => outputDevice?.PlaybackState ?? PlaybackState.Stopped;

  public TrackManager(Panel panContainer) {
    TrackRows = new();
    SelectedTrackRow = null;
    pseudoCombinedTrack = null;
        this.panContainer = panContainer;
  }

    public void AddTrack(CtrlTrackRow trackRow) {
		TrackRows.Add(trackRow);
	}

    public void RemTrack(CtrlTrackRow trackRow) {
        TrackRows.Remove(trackRow);
    }

    public void SelectTrackRow(CtrlTrackRow trackRow) {
        SelectedTrackRow = trackRow;
        TrackRows.ForEach(row => {
          row.track.PanDrawSamples.BorderStyle = BorderStyle.None;
          row.track.PanDrawSamples.BackColor = Color.DimGray;
    });
        trackRow.track.PanDrawSamples.BorderStyle = BorderStyle.Fixed3D;
        trackRow.track.PanDrawSamples.BackColor = Color.Gray;
  }

  public void RedrawAll() {
    TrackRows.ForEach(trackRow => trackRow.track.PanDrawSamples.Invalidate());
  }

  public void SaveToFile(string filename) {
    CombineTracks();
    pseudoCombinedTrack.SaveToFile(filename);
  }

    public void PlayAll()
    {
        if (paused)
        {
            paused = false;
            outputDevice?.Play();
            return;
        }

        var inputs = TrackRows
            .Select(row => row.track.GetSampleProvider())
            .ToList();
        if (inputs.Count == 0) return;

        var mixer = new MixingSampleProvider(inputs[0].WaveFormat)
        {
            ReadFully = true
        };
        foreach (var input in inputs) mixer.AddMixerInput(input);
        playbackWaveFormat = mixer.WaveFormat;

        outputDevice?.Dispose();
        outputDevice = new WaveOut();
        outputDevice.Init(new SampleToWaveProvider(mixer));
        outputDevice.Play();
    }

    public void PauseAll() 
    {
        paused = true;
        outputDevice?.Pause(); 
    }

    public void StopAll()
    {
        paused = false;
        outputDevice?.Stop();
    }

    private void CombineTracks() {
    pseudoCombinedTrack?.Dispose();
    List<float> combinedSamples = new();
    foreach (CtrlTrackRow trackRow in TrackRows) {
      Track track = trackRow.track;
      float[] trackSamples = (float[])track.GetAdjustedSamples().Clone();
      for (int i = 0; i < trackSamples.Length; i++) {
        if (combinedSamples.Count <= i) {
          combinedSamples.Add(0);
        }
        if (!track.IsDisabled) {
          combinedSamples[i] += trackSamples[i];
        }
      }
    }
    pseudoCombinedTrack = new(combinedSamples.ToArray());
    pseudoCombinedTrack.LinkPanels(null, panContainer);
  }
  public int GetPlayheadPos() {
    if (outputDevice is null || playbackWaveFormat is null || DrawTimescale <= 0)
    {
      return 0;
    }

    int bytesPerSample = playbackWaveFormat.BitsPerSample / 8;
    if (bytesPerSample <= 0)
    {
      return 0;
    }

    // GetPosition / bytesPerSample = # of samples played
    long samplesPlayed = outputDevice.GetPosition() / bytesPerSample;
    // samplesPlayed / DrawTimescale = pos on screen of current location
    return (int)(samplesPlayed / DrawTimescale);
  }
    
  public void SoloTrack(Track track)
  {
        track.IsSolo = true;
        if (track.IsDisabled) track.ToggleDisabled();
        foreach (CtrlTrackRow trackRow in TrackRows)
        {
            Track t = trackRow.track;
            if (t != track)
            {
                if (!t.IsDisabled) t.ToggleDisabled();
                t.IsSolo = false;
            }
        }
  }

  public void UnSoloTrack(Track track)
  {     
        track.IsSolo = false;
        foreach (CtrlTrackRow trackRow in TrackRows)
        {   
            trackRow.btnMuteTrack.Enabled = true;

            Track t = trackRow.track;
            if (t != track)
            {
                if (t.IsDisabled) t.ToggleDisabled();
            }
        }
    }

    public void Dispose()
    {
        outputDevice?.Dispose();
    }
}
