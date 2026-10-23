namespace MediocreSoundDesigner {
  partial class FrmGenRecord {
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
      btnApply = new Button();
      btnCancel = new Button();
      btnRecord = new Button();
      SuspendLayout();
      // 
      // btnApply
      // 
      btnApply.BackColor = Color.Black;
      btnApply.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
      btnApply.ForeColor = Color.Lime;
      btnApply.Location = new Point(234, 162);
      btnApply.Margin = new Padding(7);
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
      btnCancel.Margin = new Padding(7);
      btnCancel.Name = "btnCancel";
      btnCancel.Size = new Size(144, 47);
      btnCancel.TabIndex = 3;
      btnCancel.Text = "Cancel";
      btnCancel.UseVisualStyleBackColor = false;
      btnCancel.Click += btnCancel_Click;
      // 
      // btnRecord
      // 
      btnRecord.AutoSize = true;
      btnRecord.BackColor = Color.Black;
      btnRecord.BackgroundImage = Properties.Resources.record;
      btnRecord.BackgroundImageLayout = ImageLayout.Stretch;
      btnRecord.FlatStyle = FlatStyle.Popup;
      btnRecord.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
      btnRecord.ForeColor = Color.Red;
      btnRecord.Location = new Point(129, 29);
      btnRecord.Margin = new Padding(7);
      btnRecord.Name = "btnRecord";
      btnRecord.Padding = new Padding(5);
      btnRecord.Size = new Size(125, 110);
      btnRecord.TabIndex = 4;
      btnRecord.UseVisualStyleBackColor = false;
      btnRecord.Click += btnRecord_Click;
      // 
      // FrmGenRecord
      // 
      AutoScaleDimensions = new SizeF(16F, 37F);
      AutoScaleMode = AutoScaleMode.Font;
      BackColor = Color.Black;
      ClientSize = new Size(381, 231);
      Controls.Add(btnRecord);
      Controls.Add(btnCancel);
      Controls.Add(btnApply);
      Margin = new Padding(16, 17, 16, 17);
      Name = "FrmGenRecord";
      Text = "Record Audio";
      ResumeLayout(false);
      PerformLayout();
    }

    #endregion
    private Button btnApply;
    private Button btnCancel;
    private Button btnRecord;
  }
}