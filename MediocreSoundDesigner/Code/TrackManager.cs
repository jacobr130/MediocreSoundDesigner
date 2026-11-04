using NAudio.Wave;

namespace MediocreSoundDesigner; 

public class TrackManager {
  private Track pseudoCombinedTrack;
  private Panel panContainer;

  public List<CtrlTrackRow> TrackRows { get; private set; }
    public CtrlTrackRow SelectedTrackRow { get; private set; }
  public int DrawTimescale { get; set; }
  public PlaybackState State => pseudoCombinedTrack?.State ?? PlaybackState.Stopped;

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

	public void PlayAll() {
    CombineTracks();
    pseudoCombinedTrack.Play();
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
  public void PauseAll() {
    pseudoCombinedTrack?.Pause();
  }
  public int GetPlayheadPos() {
    return (pseudoCombinedTrack?.GetPlayheadPos() ?? 0) / DrawTimescale;
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
            Track t = trackRow.track;
            if (t != track)
            {
                if (t.IsDisabled) t.ToggleDisabled();
            }
        }
    }
}
