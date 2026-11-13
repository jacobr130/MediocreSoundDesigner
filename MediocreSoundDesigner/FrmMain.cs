using MediocreSoundDesigner.Properties;
using System.CodeDom;

namespace MediocreSoundDesigner;

public partial class FrmMain : Form
{
    // colors
    private static readonly Color[] SAMPLES_DRAW_COLORS =
  {
        Color.LightCyan, Color.LightGreen, Color.LightPink, Color.LightYellow, Color.LightCoral,
        Color.LightSalmon
    };
    // fields
    private TrackManager? trackManager;
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
        // undo
        ToolStripMenuItem menuEditUndo = new("Undo") { ShortcutKeys = Keys.Control | Keys.Z  };
        menuEditUndo.Click += (s, e) => UndoSelectedTrack();
        menuEdit.DropDownItems.Insert(0, menuEditUndo);
        DoubleBuffered = true;
        trackManager = new(panTracks);
        panTracks.ClientSizeChanged += (_, _) =>
        {
            if (IsHandleCreated)
                BeginInvoke((Action)RealignTrackRows);
        };

        // reverb
        ToolStripMenuItem menuEditReverb = new("Reverb");
        menuEditReverb.Click += (s, e) =>
        {
            Track track = trackManager?.SelectedTrackRow?.track;
            if (track is null) return;
            track.Reverb();
            track.PanDrawSamples?.Invalidate();
        };
        menuEdit.DropDownItems.Add(menuEditReverb);

        // ff and sd
        ToolStripMenuItem menuEditFast = new("Fast Forward (2x)");
        menuEditFast.Click += (s, e) => ApplySpeed(2f);
        menuEdit.DropDownItems.Add(menuEditFast);
        ToolStripMenuItem menuEditSlow = new("Slow Down (0.5x)");
        menuEditSlow.Click += (s, e) => ApplySpeed(0.5f);
        menuEdit.DropDownItems.Add(menuEditSlow);


        btnPlay.BackgroundImage = Resources.play;
        trackManager.DrawTimescale = trkDrawTimescale.Value;
        panTracks.AutoScroll = true;
        panTracks.HorizontalScroll.Enabled = false;

        // playhead width is set here
        lblPlayhead.Width = 2;

        // resizing stuff
        trkDrawTimescale.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        panTracks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
        btnPlay.Anchor = AnchorStyles.Bottom;
        btnStop.Anchor = AnchorStyles.Bottom;
        btnPlay.BringToFront();
        btnStop.BringToFront();

        WindowState = FormWindowState.Maximized;

        Rectangle area = Screen.FromControl(this).WorkingArea;
        Size = new Size(Math.Min(Width, area.Width), Math.Min(Height, area.Height));
    }

    // GUI controls
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (trackManager?.SelectedTrackRow is not null)
        {
            Track selectedTrack = trackManager.SelectedTrackRow.track;
            if (keyData == Keys.Left)
            {
                selectedTrack.ShiftSampleOver(-5);
                selectedTrack.PanDrawSamples.Invalidate();
                return true;
            }
            else if (keyData == Keys.Right)
            {
                selectedTrack.ShiftSampleOver(+5);
                selectedTrack.PanDrawSamples.Invalidate();
                return true;
            }
            else if (keyData == Keys.Delete)
            {
                if (trackManager.SelectedTrackRow is not null)
                {
                    if (MessageBox.Show("This will delete the selected track. This is NOT undoable!", "Are You Sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        CtrlTrackRow trackRow = trackManager.SelectedTrackRow;
                        panTracks.Controls.Remove(trackRow);
                        trackRow.track.PanDrawSamples.Dispose();
                        trackRow.track.Dispose();
                        trackManager.RemTrack(trackRow);
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

    private void btnStop_Click(object sender, EventArgs e)
    {
        trackManager.StopAll();
    }

    private void menuFileOpen_Click(object sender, EventArgs e)
    {
        ofdLoadFile.DefaultExt = "wav";
        if (ofdLoadFile.ShowDialog() == DialogResult.OK)
        {
            Track newTrack = new(ofdLoadFile.FileName);
            CtrlTrackRow trackRow = CreateTrackRow(newTrack);
            trackManager.AddTrack(trackRow);
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
        trackManager.SelectedTrackRow?.track.Flip();
    }
    private void menuEditReverse_Click(object sender, EventArgs e)
    {
        trackManager.SelectedTrackRow?.track.Reverse();
    }
    private void menuEditVolume_Click(object sender, EventArgs e)
    {
        if (trackManager.SelectedTrackRow is not null)
        {
            FrmEditVolume frm = new();
            frm.SetSound(trackManager.SelectedTrackRow.track);
            frm.ShowDialog();
        }
    }
    private void menuEditEliminateSilence_Click(object sender, EventArgs e)
    {
        if (trackManager.SelectedTrackRow is not null)
        {
            FrmEditEliminateSilence frm = new();
            frm.SetSound(trackManager.SelectedTrackRow.track);
            frm.ShowDialog();
        }
    }

    private void menuGenRecord_Click(object sender, EventArgs e)
    {
        Track newTrack = new(new float[0]);
        CtrlTrackRow trackRow = CreateTrackRow(newTrack);
        trackManager.AddTrack(trackRow);
        RealignTrackRows();
        newTrack.PanDrawSamples.Invalidate();

        FrmGenRecord frm = new();
        frm.SetSound(trackManager.SelectedTrackRow.track);
        if (frm.ShowDialog() == DialogResult.Cancel)
        {
            trackManager.RemTrack(trackRow);
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
        CtrlTrackRow trackRow = CreateTrackRow(newTrack);
        trackManager.AddTrack(trackRow);
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
        CtrlTrackRow trackRow = CreateTrackRow(newTrack);
        trackManager.AddTrack(trackRow);
        RealignTrackRows();
        newTrack.PanDrawSamples.Invalidate();
    }

    private void tmrUpdatePlaybackState_Tick(object sender, EventArgs e)
    {
        if (trackManager.TrackRows.Count > 0)
        {
            btnPlay.BackgroundImage = trackManager.State == NAudio.Wave.PlaybackState.Playing ? Resources.pause : Resources.play;
            btnPlay.Visible = true;
            btnStop.BackgroundImage = Resources.stop;
            btnStop.Visible = true; 
            lblPlayhead.Left = trackManager.GetPlayheadPos();
            lblPlayhead.Visible = true;
        }
        else
        {
            btnPlay.BackgroundImage = null;
            btnStop.BackgroundImage = null;
            btnPlay.Visible = false;
            btnStop.Visible = false;
            lblPlayhead.Visible = false;
        }
    }

    private void trkDrawTimescale_Scroll(object sender, EventArgs e)
    {
        trackManager.DrawTimescale = trkDrawTimescale.Value;
        trackManager.RedrawAll();
    }

    // Helper methods

    // undo method
    private void UndoSelectedTrack()
    {
        Track track = trackManager?.SelectedTrackRow?.track;

        if (track is null || !track.CanUndo) return;
        track.Undo();
        track.PanDrawSamples?.Invalidate();
    }

    // ff and sd
    private void ApplySpeed(float scale)
    {
        Track track = trackManager?.SelectedTrackRow?.track;
        if (track is null) return;
        track.AlterTimeScale(scale);
        track.PanDrawSamples?.Invalidate();
    }
    private CtrlTrackRow CreateTrackRow(Track track)
    {
        numTracks++;
        // added this to keep track of how many tracks
        track.TrackNumber = numTracks;

        var trackRow = new CtrlTrackRow(track);
        trackRow.Width = panTracks.ClientSize.Width;
        trackRow.BorderStyle = BorderStyle.FixedSingle;
        trackRow.LblTrackNum.Text = $"Track {numTracks}";

        trackRow.pnlSampleView.Click += (sender, e) =>
        {
            trackManager.SelectTrackRow(trackRow);
        };

        trackRow.pnlSampleView.DoubleClick += (sender, e) =>
        {
            track.ToggleDisabled();
            track.PanDrawSamples.Invalidate();
        };

        trackRow.pnlSampleView.Paint += (sender, e) =>
        {
            // color stuff
            Color color = SAMPLES_DRAW_COLORS[track.TrackNumber % SAMPLES_DRAW_COLORS.Length];
            trackRow.pnlSampleView.BackColor = color;
            // end of color stuff
            track.DrawSamples(e.Graphics, trackManager.DrawTimescale);
        };

        trackRow.btnMuteTrack.Click += (sender, e) =>
        {
            if (track.IsDisabled)
            {
                track.ToggleDisabled();
                track.PanDrawSamples.Invalidate();
                trackRow.btnMuteTrack.BackColor = SystemColors.ControlLightLight;
            }
            else
            {
                track.ToggleDisabled();
                track.PanDrawSamples.Invalidate();
                trackRow.btnMuteTrack.BackColor = Color.LightSlateGray;
            }
        };

        trackRow.btnSoloTrack.Click += (sender, e) =>
        {   
            if (!trackRow.track.IsSolo)
            {
                trackManager.SoloTrack(track);
                foreach (CtrlTrackRow row in trackManager.TrackRows)
                {   
                    row.track.PanDrawSamples.Invalidate();

                    // make sure the buttons are colored correctly
                    if (row.track != track) row.btnSoloTrack.BackColor = SystemColors.ControlLightLight;
                    else row.btnSoloTrack.BackColor = Color.DarkOrange;

                    if (row.track.IsDisabled)
                    {
                        row.btnMuteTrack.BackColor = Color.LightSlateGray;
                    }
                    else row.btnMuteTrack.BackColor = SystemColors.ControlLightLight;
                }

                // no reason to mute a solo'd track 
                trackRow.btnMuteTrack.BackColor = SystemColors.InactiveCaption;
                trackRow.btnMuteTrack.Enabled = false;
            }
            else
            {
                trackManager.UnSoloTrack(track);
                trackRow.btnMuteTrack.Enabled = true;
                foreach (CtrlTrackRow row in trackManager.TrackRows)
                {
                    row.track.PanDrawSamples.Invalidate();
                    row.btnMuteTrack.BackColor = SystemColors.ControlLightLight;
                    row.btnSoloTrack.BackColor = SystemColors.ControlLightLight;
                }
            }
        };

        panTracks.Controls.Add(trackRow);
        track.LinkPanels(trackRow.pnlSampleView, panTracks);
        trackManager.SelectTrackRow(trackRow);

        return trackRow;
    }

    private void RealignTrackRows()
    {
        if (trackManager is null) return; 

        int top = 0;
        foreach (CtrlTrackRow trackRow in trackManager.TrackRows)
        {
            Track track = trackRow.track;
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
        lblPlayhead.Height = top;
    }
}
