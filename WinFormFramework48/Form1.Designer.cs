
namespace WinFormFramework48
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TargetTextBox = new System.Windows.Forms.TextBox();
			this.StartButton = new System.Windows.Forms.Button();
			this.DrawingPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// TargetTextBox
			// 
			this.TargetTextBox.Location = new System.Drawing.Point(12, 12);
			this.TargetTextBox.Name = "TargetTextBox";
			this.TargetTextBox.Size = new System.Drawing.Size(333, 20);
			this.TargetTextBox.TabIndex = 0;
			// 
			// StartButton
			// 
			this.StartButton.Location = new System.Drawing.Point(13, 412);
			this.StartButton.Name = "StartButton";
			this.StartButton.Size = new System.Drawing.Size(75, 23);
			this.StartButton.TabIndex = 1;
			this.StartButton.Text = "button1";
			this.StartButton.UseVisualStyleBackColor = true;
			this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
			// 
			// DrawingPanel
			// 
			this.DrawingPanel.Location = new System.Drawing.Point(12, 38);
			this.DrawingPanel.Name = "DrawingPanel";
			this.DrawingPanel.Size = new System.Drawing.Size(612, 335);
			this.DrawingPanel.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.DrawingPanel);
			this.Controls.Add(this.StartButton);
			this.Controls.Add(this.TargetTextBox);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TargetTextBox;
		private System.Windows.Forms.Button StartButton;
		private System.Windows.Forms.Panel DrawingPanel;
	}
}

