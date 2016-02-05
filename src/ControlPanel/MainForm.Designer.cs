/*
 * Created by SharpDevelop.
 * User: Paul
 * Date: 12/19/2015
 * Time: 2:01 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace TE.Apps.Staging
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label lblSourceDirectory;
		private System.Windows.Forms.TextBox txtSourceDirectory;
		private System.Windows.Forms.Button btnSourceBrowse;
		private System.Windows.Forms.Button btnDestinationBrowse;
		private System.Windows.Forms.TextBox txtDestinationDirectory;
		private System.Windows.Forms.Label lblDestinationDirectory;
		private System.Windows.Forms.Label label1;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnApply = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblSourceDirectory = new System.Windows.Forms.Label();
			this.txtSourceDirectory = new System.Windows.Forms.TextBox();
			this.btnSourceBrowse = new System.Windows.Forms.Button();
			this.btnDestinationBrowse = new System.Windows.Forms.Button();
			this.txtDestinationDirectory = new System.Windows.Forms.TextBox();
			this.lblDestinationDirectory = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(440, 256);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnApply
			// 
			this.btnApply.Enabled = false;
			this.btnApply.Location = new System.Drawing.Point(359, 256);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(75, 23);
			this.btnApply.TabIndex = 1;
			this.btnApply.Text = "Apply";
			this.btnApply.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(278, 256);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// lblSourceDirectory
			// 
			this.lblSourceDirectory.Location = new System.Drawing.Point(12, 40);
			this.lblSourceDirectory.Name = "lblSourceDirectory";
			this.lblSourceDirectory.Size = new System.Drawing.Size(116, 23);
			this.lblSourceDirectory.TabIndex = 3;
			this.lblSourceDirectory.Text = "Source Directory:";
			// 
			// txtSourceDirectory
			// 
			this.txtSourceDirectory.Location = new System.Drawing.Point(134, 41);
			this.txtSourceDirectory.Name = "txtSourceDirectory";
			this.txtSourceDirectory.Size = new System.Drawing.Size(353, 20);
			this.txtSourceDirectory.TabIndex = 4;
			// 
			// btnSourceBrowse
			// 
			this.btnSourceBrowse.Location = new System.Drawing.Point(493, 40);
			this.btnSourceBrowse.Name = "btnSourceBrowse";
			this.btnSourceBrowse.Size = new System.Drawing.Size(26, 23);
			this.btnSourceBrowse.TabIndex = 5;
			this.btnSourceBrowse.Text = "...";
			this.btnSourceBrowse.UseVisualStyleBackColor = true;
			// 
			// btnDestinationBrowse
			// 
			this.btnDestinationBrowse.Location = new System.Drawing.Point(493, 68);
			this.btnDestinationBrowse.Name = "btnDestinationBrowse";
			this.btnDestinationBrowse.Size = new System.Drawing.Size(26, 23);
			this.btnDestinationBrowse.TabIndex = 8;
			this.btnDestinationBrowse.Text = "...";
			this.btnDestinationBrowse.UseVisualStyleBackColor = true;
			// 
			// txtDestinationDirectory
			// 
			this.txtDestinationDirectory.Location = new System.Drawing.Point(134, 69);
			this.txtDestinationDirectory.Name = "txtDestinationDirectory";
			this.txtDestinationDirectory.Size = new System.Drawing.Size(353, 20);
			this.txtDestinationDirectory.TabIndex = 7;
			// 
			// lblDestinationDirectory
			// 
			this.lblDestinationDirectory.Location = new System.Drawing.Point(12, 68);
			this.lblDestinationDirectory.Name = "lblDestinationDirectory";
			this.lblDestinationDirectory.Size = new System.Drawing.Size(116, 23);
			this.lblDestinationDirectory.TabIndex = 6;
			this.lblDestinationDirectory.Text = "Destination Directory:";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 137);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 9;
			this.label1.Text = "label1";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(527, 291);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnDestinationBrowse);
			this.Controls.Add(this.txtDestinationDirectory);
			this.Controls.Add(this.lblDestinationDirectory);
			this.Controls.Add(this.btnSourceBrowse);
			this.Controls.Add(this.txtSourceDirectory);
			this.Controls.Add(this.lblSourceDirectory);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Backup Staging Settings";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
