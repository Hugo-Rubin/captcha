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
            this.PixelIntensity.Location = new System.Drawing.Point(194, 98);
            this.PixelIntensity.Name = "PixelIntensity";
            this.PixelIntensity.Size = new System.Drawing.Size(128, 44);
            this.PixelIntensity.TabIndex = 1;
            this.PixelIntensity.Text = "Pixel Intensity";
            this.PixelIntensity.UseVisualStyleBackColor = true;
            this.PixelIntensity.Click += new System.EventHandler(this.PixelIntensity_Click);
            // 
            // TreinamentoRede
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 310);
            this.Controls.Add(this.PixelIntensity);
            this.Name = "TreinamentoRede";
            this.Text = "TreinamentoRede";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button PixelIntensity;
    }
}