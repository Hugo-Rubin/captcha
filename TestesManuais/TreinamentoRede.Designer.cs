namespace TestesManuais
{
    partial class TreinamentoRede
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
            this.PixelIntensity = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PixelIntensity
            // 
            this.PixelIntensity.Location = new System.Drawing.Point(146, 80);
            this.PixelIntensity.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PixelIntensity.Name = "PixelIntensity";
            this.PixelIntensity.Size = new System.Drawing.Size(96, 36);
            this.PixelIntensity.TabIndex = 1;
            this.PixelIntensity.Text = "Pixel Intensity";
            this.PixelIntensity.UseVisualStyleBackColor = true;
            this.PixelIntensity.Click += new System.EventHandler(this.PixelIntensity_Click);
            // 
            // TreinamentoRede
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 252);
            this.Controls.Add(this.PixelIntensity);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "TreinamentoRede";
            this.Text = "TreinamentoRede";
            this.Load += new System.EventHandler(this.TreinamentoRede_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button PixelIntensity;
    }
}