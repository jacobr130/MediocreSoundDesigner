using MediocreSoundDesigner.Properties;

namespace MediocreSoundDesigner;

public partial class FrmMain : Form
{
    // fields
    private TrackManager trackManager;
    private int numTracks = 0;

    // ctor
    public FrmMain()
    {
        InitializeComponent();
        Setup();
    }

    // setup
    private void Setup()
    {
        DoubleBuffered = true;
        trackManager = new(panTracks);
        panTracks.ClientSizeChanged += (_, _) =>
        {
            if (IsHandleCreated)
                BeginInvoke((Action)RealignTrackRows);
        };

        btnPlay.BackgroundImage = Resources.play;
        trackManager.DrawTimescale = trkDrawTimescale.Value;
        panTracks.AutoScroll = true;
        panTracks.HorizontalScroll.Enabled = false;
        RealignTrackRows();
    }

    // GUI controls
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (trackManager.SelectedTrack is not null)
        {
            if (keyData == Keys.Left)
            {
                trackManager.SelectedTrack.ShiftSampleOver(-5);
                trackManager.SelectedTrack.PanDrawSamples.Invalidate();
                return true;
            }
            else if (keyData == Keys.Right)
            {
                trackManager.SelectedTrack.ShiftSampleOver(+5);
                trackManager.SelectedTrack.PanDrawSamples.Invalidate();
                return true;
            }
            else if (keyData == Keys.Delete)
            {
                if (trackManager.SelectedTrack is not null)
                {
                    if (MessageBox.Show("This will delete the selected track. This is NOT undoable!", "Are You Sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        panTracks.Controls.Remove(trackManager.SelectedTrack.PanDrawSamples);
                        trackManager.SelectedTrack.PanDrawSamples.Dispose();
                        trackManager.SelectedTrack.Dispose();
                        trackManager.RemTrack(trackManager.SelectedTrack);
                        RealignTrackRows();
                        return true;
                    }
                }
            }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void FrmMain_SizeChanged(object sender, EventArgs e)
    {
        RealignTrackRows();
    }

    private void btnPlay_Click(object sender, EventArgs e)
    {
        if (trackManager.State is NAudio.Wave.PlaybackState.Stopped or NAudio.Wave.PlaybackState.Paused)
        {
            trackManager.PlayAll();
            btnPlay.BackgroundImage = Resources.pause;
        }
        else
        {
            trackManager.PauseAll();
            btnPlay.BackgroundImage = Resources.play;
        }
    }

    private void menuFileOpen_Click(object sender, EventArgs e)
    {
        ofdLoadFile.DefaultExt = "wav";
        if (ofdLoadFile.ShowDialog() == DialogResult.OK)
        {
            Track newTrack = new(ofdLoadFile.FileName);
            CreateTrackRow(newTrack);
            trackManager.AddTrack(newTrack);
            RealignTrackRows();
            newTrack.PanDrawSamples.Invalidate();
        }
    }
    private void menuFileSave_Click(object sender, EventArgs e)
    {
        sfdSaveFile.DefaultExt = "wav";
        if (sfdSaveFile.ShowDialog() == DialogResult.OK)
        {
            trackManager.SaveToFile(sfdSaveFile.FileName);
        }
    }

    private void menuEditFlip_Click(object sender, EventArgs e)
    {
        trackManager.SelectedTrack?.Flip();
    }
    private void menuEditReverse_Click(object sender, EventArgs e)
    {
        trackManager.SelectedTrack?.Reverse();
    }
    private void menuEditVolume_Click(object sender, EventArgs e)
    {
        if (trackManager.SelectedTrack is not null)
        {
            FrmEditVolume frm = new();
            frm.SetSound(trackManager.SelectedTrack);
            frm.ShowDialog();
        }
    }
    private void menuEditEliminateSilence_Click(object sender, EventArgs e)
    {
        if (trackManager.SelectedTrack is not null)
        {
            FrmEditEliminateSilence frm = new();
            frm.SetSound(trackManager.SelectedTrack);
            frm.ShowDialog();
        }
    }

    private void menuGenRecord_Click(object sender, EventArgs e)
    {
        Track newTrack = new(new float[0]);
        CreateTrackRow(newTrack);
        trackManager.AddTrack(newTrack);
        RealignTrackRows();
        newTrack.PanDrawSamples.Invalidate();

        FrmGenRecord frm = new();
        frm.SetSound(trackManager.SelectedTrack);
        if (frm.ShowDialog() == DialogResult.Cancel)
        {
            trackManager.RemTrack(newTrack);
        }
    }
    private void menuGenNoise_Click(object sender, EventArgs e)
    {
        const int NUM_SAMPLES = 100_000;
        float[] samples = new float[NUM_SAMPLES];
        Random rand = new();
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            samples[i] = rand.NextSingle() * 2.0f - 1.0f;
        }
        Track newTrack = new(samples);
        CreateTrackRow(newTrack);
        trackManager.AddTrack(newTrack);
        RealignTrackRows();
        newTrack.PanDrawSamples.Invalidate();
    }
    private void menuGenGradientNoise_Click(object sender, EventArgs e)
    {
        const int NUM_SAMPLES = 250_000;
        float[] samples = new float[NUM_SAMPLES];
        Random rand = new();
        float cur = 0;
        float inc = 0.005f;
        int dir = +1;
        for (int i = 0; i < NUM_SAMPLES; i++)
        {
            cur += (inc * dir);
            if (rand.NextSingle() < 0.5f)
            {
                dir *= -1;
            }
            if (cur > 0.9f)
            {
                dir = -1;
            }
            if (cur < -0.9f)
            {
                dir = +1;
            }
            samples[i] = cur;
        }
        Track newTrack = new(samples);
        CreateTrackRow(newTrack);
        trackManager.AddTrack(newTrack);
        RealignTrackRows();
        newTrack.PanDrawSamples.Invalidate();
    }

    private void tmrUpdatePlaybackState_Tick(object sender, EventArgs e)
    {
        if (trackManager.Tracks.Count > 0)
        {
            btnPlay.BackgroundImage = trackManager.State == NAudio.Wave.PlaybackState.Playing ? Resources.pause : Resources.play;
            btnPlay.Visible = true;
            lblPlayhead.Left = trackManager.GetPlayheadPos();
            lblPlayhead.Visible = true;
        }
        else
        {
            btnPlay.BackgroundImage = null;
            btnPlay.Visible = false;
            lblPlayhead.Visible = false;
        }
    }

    private void trkDrawTimescale_Scroll(object sender, EventArgs e)
    {
        trackManager.DrawTimescale = trkDrawTimescale.Value;
        trackManager.RedrawAll();
    }

    // Helper methods
    private CtrlTrackRow CreateTrackRow(Track track)
    {
        var trackRow = new CtrlTrackRow();
        trackRow.Width = panTracks.ClientSize.Width;
        trackRow.BorderStyle = BorderStyle.FixedSingle;
        numTracks++;
        trackRow.LblTrackNum.Text = $"Track {numTracks}";

        trackRow.pnlSampleView.Click += (sender, e) =>
        {
            trackManager.SelectTrack(track);
        };

        trackRow.pnlSampleView.DoubleClick += (sender, e) =>
        {
            track.ToggleDisabled();
            track.PanDrawSamples.Invalidate();
        };

        trackRow.pnlSampleView.Paint += (sender, e) =>
        {
            track.DrawSamples(e.Graphics, trackManager.DrawTimescale);
        };

        panTracks.Controls.Add(trackRow);
        track.LinkPanels(trackRow.pnlSampleView, panTracks);
        trackManager.SelectTrack(track);

        return trackRow;
    }

    private void RealignTrackRows()
    {
        int top = 0;
        foreach (Track track in trackManager.Tracks)
        {
            if (track.PanDrawSamples.Parent is not CtrlTrackRow row)
                continue;

            row.Left = 0;
            row.Top = top;
            row.Width = panTracks.ClientSize.Width;

            int rowHeight = row.Height;
            row.pnlTrackOptions.Left = row.ClientSize.Width - row.pnlTrackOptions.Width;
            row.pnlTrackOptions.Top = 0;
            row.pnlTrackOptions.Height = rowHeight;

            track.PanDrawSamples.Left = 0;
            track.PanDrawSamples.Top = 0;
            track.PanDrawSamples.Width = row.ClientSize.Width - row.pnlTrackOptions.Width;
            track.PanDrawSamples.Height = rowHeight;
            track.PanDrawSamples.Invalidate();

            top += rowHeight;
        }
    }
}
