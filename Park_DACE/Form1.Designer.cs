namespace Park_DACE
{
    partial class ParkDACE
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
            this.btnLaunch = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.listBoxSpots = new System.Windows.Forms.ListBox();
            this.btnSendParkInfoManual = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLaunch
            // 
            this.btnLaunch.Location = new System.Drawing.Point(12, 11);
            this.btnLaunch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.Size = new System.Drawing.Size(165, 38);
            this.btnLaunch.TabIndex = 0;
            this.btnLaunch.Text = "Launch Park DACE";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.btnLaunch_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(12, 53);
            this.btnStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(165, 37);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop Park DACE";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // listBoxSpots
            // 
            this.listBoxSpots.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxSpots.FormattingEnabled = true;
            this.listBoxSpots.ItemHeight = 16;
            this.listBoxSpots.Location = new System.Drawing.Point(192, 11);
            this.listBoxSpots.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.listBoxSpots.Name = "listBoxSpots";
            this.listBoxSpots.Size = new System.Drawing.Size(768, 548);
            this.listBoxSpots.TabIndex = 2;
            // 
            // btnSendParkInfoManual
            // 
            this.btnSendParkInfoManual.Location = new System.Drawing.Point(12, 95);
            this.btnSendParkInfoManual.Name = "btnSendParkInfoManual";
            this.btnSendParkInfoManual.Size = new System.Drawing.Size(164, 61);
            this.btnSendParkInfoManual.TabIndex = 3;
            this.btnSendParkInfoManual.Text = "Send Park Information Mannually";
            this.btnSendParkInfoManual.UseVisualStyleBackColor = true;
            this.btnSendParkInfoManual.Click += new System.EventHandler(this.btnSendParkInfoManual_Click);
            // 
            // ParkDACE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 564);
            this.Controls.Add(this.btnSendParkInfoManual);
            this.Controls.Add(this.listBoxSpots);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnLaunch);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ParkDACE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Park DACE";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLaunch;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ListBox listBoxSpots;
        private System.Windows.Forms.Button btnSendParkInfoManual;
    }
}

