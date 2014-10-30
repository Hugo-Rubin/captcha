namespace ClienteSintegra
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
            this.txtResposta = new System.Windows.Forms.TextBox();
            this.pbImagem = new System.Windows.Forms.PictureBox();
            this.btnReconhecer = new System.Windows.Forms.Button();
            this.cbEstado = new System.Windows.Forms.ComboBox();
            this.lblTempo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbImagem)).BeginInit();
            this.SuspendLayout();
            // 
            // txtResposta
            // 
            this.txtResposta.Location = new System.Drawing.Point(123, 52);
            this.txtResposta.Name = "txtResposta";
            this.txtResposta.ReadOnly = true;
            this.txtResposta.Size = new System.Drawing.Size(89, 20);
            this.txtResposta.TabIndex = 2;
            // 
            // pbImagem
            // 
            this.pbImagem.Location = new System.Drawing.Point(12, 89);
            this.pbImagem.Name = "pbImagem";
            this.pbImagem.Size = new System.Drawing.Size(200, 90);
            this.pbImagem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbImagem.TabIndex = 4;
            this.pbImagem.TabStop = false;
            // 
            // btnReconhecer
            // 
            this.btnReconhecer.Location = new System.Drawing.Point(12, 39);
            this.btnReconhecer.Name = "btnReconhecer";
            this.btnReconhecer.Size = new System.Drawing.Size(105, 44);
            this.btnReconhecer.TabIndex = 1;
            this.btnReconhecer.Text = "&Reconhecer";
            this.btnReconhecer.UseVisualStyleBackColor = true;
            this.btnReconhecer.Click += new System.EventHandler(this.btnReconhecer_Click);
            // 
            // cbEstado
            // 
            this.cbEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEstado.FormattingEnabled = true;
            this.cbEstado.Items.AddRange(new object[] {
            "NFE - Nota Fiscal Eletrônica",
            "RF - Receita Federal",
            "RF3 - Receita Federal",
            "SI - Siscarga",
            "CRJ - Consignações / Rio de Janeiro",
            "CA - Consignações Aeronáutica",
            "CM - Consignação Marinha",
            "SP - São Paulo",
            "RJ - Rio de Janeiro",
            "MG - Minas Gerais",
            "AM - Amazonas",
            "AC - Acre",
            "AL - Alagoas",
            "AP - Amapá",
            "BA - Bahia ",
            "CE - Ceará",
            "DF - Distrito Federal ",
            "ES - Espírito Santo",
            "GO - Goiás",
            "MA - Maranhão",
            "MT - Mato Grosso",
            "MS - Mato Grosso do Sul",
            "PA - Pará",
            "PB - Paraíba",
            "PR - Paraná",
            "PE - Pernambuco",
            "PI - Piauí",
            "RN - Rio Grande do Norte",
            "RS - Rio Grande do Sul",
            "RO - Rondônia",
            "RR - Roraima",
            "SC - Santa Catarina",
            "SE - Sergipe",
            "TO - Tocantins"});
            this.cbEstado.Location = new System.Drawing.Point(12, 12);
            this.cbEstado.Name = "cbEstado";
            this.cbEstado.Size = new System.Drawing.Size(200, 21);
            this.cbEstado.TabIndex = 0;
            // 
            // lblTempo
            // 
            this.lblTempo.AutoSize = true;
            this.lblTempo.Location = new System.Drawing.Point(9, 186);
            this.lblTempo.Name = "lblTempo";
            this.lblTempo.Size = new System.Drawing.Size(0, 13);
            this.lblTempo.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 208);
            this.Controls.Add(this.lblTempo);
            this.Controls.Add(this.cbEstado);
            this.Controls.Add(this.txtResposta);
            this.Controls.Add(this.pbImagem);
            this.Controls.Add(this.btnReconhecer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " OCR";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbImagem)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtResposta;
        private System.Windows.Forms.PictureBox pbImagem;
        private System.Windows.Forms.Button btnReconhecer;
        private System.Windows.Forms.ComboBox cbEstado;
        private System.Windows.Forms.Label lblTempo;
    }
}

