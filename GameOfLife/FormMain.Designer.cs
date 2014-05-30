namespace GameOfLife
{
    partial class FormMain
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
            this.zoomBox1 = new GameOfLife.ZoomBox();
            this.SuspendLayout();
            // 
            // zoomBox1
            // 
            this.zoomBox1.Location = new System.Drawing.Point(12, 12);
            this.zoomBox1.MaxZoom = 0F;
            this.zoomBox1.MinZoom = 0F;
            this.zoomBox1.Name = "zoomBox1";
            this.zoomBox1.Size = new System.Drawing.Size(378, 286);
            this.zoomBox1.Source = null;
            this.zoomBox1.TabIndex = 0;
            this.zoomBox1.Zoom = 0F;
            this.zoomBox1.Zoomed = null;
            this.zoomBox1.ZoomStep = 0F;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 493);
            this.Controls.Add(this.zoomBox1);
            this.Name = "FormMain";
            this.Text = "Game of Life";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private ZoomBox zoomBox1;

    }
}

