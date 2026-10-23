namespace MediocreSoundDesigner {
  partial class FrmEditVolume {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      nudVolume = new NumericUpDown();
      label1 = new Label();
      btnApply = new Button();
      btnCancel = new Button();
      ((System.ComponentModel.ISupportInitialize)nudVolume).BeginInit();
      SuspendLayout();
      // 
      // nudVolume
      // 
      nudVolume.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
      nudVolume.Increment = new decimal(new int[] { 2, 0, 0, 0 });
      nudVolume.Location = new Point(203, 54);
      nudVolume.Margin = new Padding(7, 7, 7, 7);
      nudVolume.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
      nudVolume.Name = "nudVolume";
      nudVolume.Size = new Size(98, 39);
      nudVolume.TabIndex = 0;
      nudVolume.TextAlign = HorizontalAlignment.Center;
      nudVolume.Value = new decimal(new int[] { 100, 0, 0, 0 });
      // 
      // label1
      // 
      label1.AutoSize = true;
      label1.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
      label1.ForeColor = SystemColors.ControlLightLight;
      label1.Location = new Point(68, 56);
      label1.Margin = new Padding(7, 0, 7, 0);
      label1.Name = "label1";
      label1.Size = new Size(121, 37);
      label1.TabIndex = 1;
      label1.Text = "Volume:";
      // 
      // btnApply
      // 
      btnApply.BackColor = Color.Black;
      btnApply.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
      btnApply.ForeColor = Color.Lime;
      btnApply.Location = new Point(234, 162);
      btnApply.Margin = new Padding(7, 7, 7, 7);
      btnApply.Name = "btnApply";
      btnApply.Size = new Size(125, 47);
      btnApply.TabIndex = 2;
      btnApply.Text = "Apply";
      btnApply.UseVisualStyleBackColor = false;
      btnApply.Click += btnApply_Click;
      // 
      // btnCancel
      // 
      btnCancel.AutoSize = true;
      btnCancel.BackColor = Color.Black;
      btnCancel.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
      btnCancel.ForeColor = Color.Red;
      btnCancel.Location = new Point(37, 162);
      btnCancel.Margin = new Padding(7, 7, 7, 7);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new Size(144, 47);
      btnCancel.TabIndex = 3;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = false;
      btnCancel.Click += btnCancel_Click;
      // 
      // FrmEditVolume
      // 
      AutoScaleDimensions = new SizeF(16F, 37F);
      AutoScaleMode = AutoScaleMode.Font;
      BackColor = Color.Black;
      ClientSize = new Size(381, 231);
      Controls.Add(btnCancel);
      Controls.Add(btnApply);
      Controls.Add(label1);
      Controls.Add(nudVolume);
      Margin = new Padding(16, 17, 16, 17);
      Name = "FrmEditVolume";
      Text = "FrmEditVolume";
      ((System.ComponentModel.ISupportInitialize)nudVolume).EndInit();
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion

    private NumericUpDown nudVolume;
    private Label label1;
    private Button btnApply;
    private Button btnCancel;
  }
}