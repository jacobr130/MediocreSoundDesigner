namespace MediocreSoundDesigner
{
    partial class CtrlTrackRow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlSampleView = new Panel();
            pnlTrackOptions = new Panel();
            btnMuteTrack = new Button();
            LblTrackNum = new Label();
            btnSoloTrack = new Button();
            pnlTrackOptions.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSampleView
            // 
            pnlSampleView.AutoScroll = true;
            pnlSampleView.BackColor = Color.DimGray;
            pnlSampleView.Location = new Point(0, 0);
            pnlSampleView.Name = "pnlSampleView";
            pnlSampleView.Size = new Size(796, 150);
            pnlSampleView.TabIndex = 0;
            // 
            // pnlTrackOptions
            // 
            pnlTrackOptions.Anchor = AnchorStyles.Right;
            pnlTrackOptions.BackColor = SystemColors.GradientActiveCaption;
            pnlTrackOptions.Controls.Add(btnSoloTrack);
            pnlTrackOptions.Controls.Add(btnMuteTrack);
            pnlTrackOptions.Controls.Add(LblTrackNum);
            pnlTrackOptions.Location = new Point(795, 0);
            pnlTrackOptions.Name = "pnlTrackOptions";
            pnlTrackOptions.Size = new Size(160, 150);
            pnlTrackOptions.TabIndex = 1;
            pnlTrackOptions.Paint += pnlTrackOptions_Paint;
            // 
            // btnMuteTrack
            // 
            btnMuteTrack.BackColor = SystemColors.ControlLightLight;
            btnMuteTrack.FlatAppearance.BorderSize = 0;
            btnMuteTrack.FlatStyle = FlatStyle.Flat;
            btnMuteTrack.Location = new Point(7, 42);
            btnMuteTrack.Name = "btnMuteTrack";
            btnMuteTrack.Size = new Size(75, 23);
            btnMuteTrack.TabIndex = 0;
            btnMuteTrack.Text = "Mute";
            btnMuteTrack.UseVisualStyleBackColor = false;
            // 
            // LblTrackNum
            // 
            LblTrackNum.AutoSize = true;
            LblTrackNum.Location = new Point(48, 9);
            LblTrackNum.Name = "LblTrackNum";
            LblTrackNum.Size = new Size(65, 15);
            LblTrackNum.TabIndex = 2;
            LblTrackNum.Text = "Track Num";
            // 
            // btnSoloTrack
            // 
            btnSoloTrack.BackColor = SystemColors.ControlLightLight;
            btnSoloTrack.FlatAppearance.BorderSize = 0;
            btnSoloTrack.FlatStyle = FlatStyle.Flat;
            btnSoloTrack.Location = new Point(7, 71);
            btnSoloTrack.Name = "btnSoloTrack";
            btnSoloTrack.Size = new Size(75, 23);
            btnSoloTrack.TabIndex = 3;
            btnSoloTrack.Text = "Solo";
            btnSoloTrack.UseVisualStyleBackColor = false;
            // 
            // CtrlTrackRow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlTrackOptions);
            Controls.Add(pnlSampleView);
            Name = "CtrlTrackRow";
            Size = new Size(955, 150);
            pnlTrackOptions.ResumeLayout(false);
            pnlTrackOptions.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        public Panel pnlSampleView;
        public Panel pnlTrackOptions;
        public Label LblTrackNum;
        public Button btnMuteTrack;
        public Button btnSoloTrack;
    }
}
