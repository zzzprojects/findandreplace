namespace FindAndReplace.App
{
	partial class AboutBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.lblProductName = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.lnkCompany = new System.Windows.Forms.LinkLabel();
			this.lnkProduct = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// lblProductName
			// 
			this.lblProductName.AutoSize = true;
			this.lblProductName.Location = new System.Drawing.Point(22, 17);
			this.lblProductName.Name = "lblProductName";
			this.lblProductName.Size = new System.Drawing.Size(75, 13);
			this.lblProductName.TabIndex = 0;
			this.lblProductName.Text = "Product Name";
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.Location = new System.Drawing.Point(22, 40);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(42, 13);
			this.lblVersion.TabIndex = 1;
			this.lblVersion.Text = "Version";
			// 
			// lblCopyright
			// 
			this.lblCopyright.AutoSize = true;
			this.lblCopyright.Location = new System.Drawing.Point(22, 96);
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Size = new System.Drawing.Size(51, 13);
			this.lblCopyright.TabIndex = 2;
			this.lblCopyright.Text = "Copyright";
			// 
			// lnkCompany
			// 
			this.lnkCompany.AutoSize = true;
			this.lnkCompany.Location = new System.Drawing.Point(22, 119);
			this.lnkCompany.Name = "lnkCompany";
			this.lnkCompany.Size = new System.Drawing.Size(51, 13);
			this.lnkCompany.TabIndex = 3;
			this.lnkCompany.TabStop = true;
			this.lnkCompany.Text = "Company";
			this.lnkCompany.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCompany_LinkClicked);
			// 
			// lnkProduct
			// 
			this.lnkProduct.AutoSize = true;
			this.lnkProduct.Location = new System.Drawing.Point(22, 63);
			this.lnkProduct.Name = "lnkProduct";
			this.lnkProduct.Size = new System.Drawing.Size(67, 13);
			this.lnkProduct.TabIndex = 4;
			this.lnkProduct.TabStop = true;
			this.lnkProduct.Text = "Product Link";
			this.lnkProduct.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkProduct_LinkClicked);
			// 
			// AboutBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(217, 153);
			this.Controls.Add(this.lnkProduct);
			this.Controls.Add(this.lnkCompany);
			this.Controls.Add(this.lblCopyright);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lblProductName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AboutBox";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblProductName;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.LinkLabel lnkCompany;
		private System.Windows.Forms.LinkLabel lnkProduct;

	}
}
