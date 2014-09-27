namespace BaixarImagensCaptcha
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
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.label4 = new System.Windows.Forms.Label();
            this.txtQtde = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCopiasRF3 = new System.Windows.Forms.TextBox();
            this.btnRF3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnBaixarImagens = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtURLImagem = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUrlSite = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblRecPosition = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFolder = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(189, 9);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(637, 569);
            this.webBrowser1.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Quantidade de imagens:";
            // 
            // txtQtde
            // 
            this.txtQtde.Location = new System.Drawing.Point(130, 9);
            this.txtQtde.Name = "txtQtde";
            this.txtQtde.Size = new System.Drawing.Size(46, 20);
            this.txtQtde.TabIndex = 0;
            this.txtQtde.Text = "10";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtCopiasRF3);
            this.groupBox1.Controls.Add(this.btnRF3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.btnBaixarImagens);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtURLImagem);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtUrlSite);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(10, 79);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 495);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tipo de download";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 275);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Copias por captcha:";
            // 
            // txtCopiasRF3
            // 
            this.txtCopiasRF3.Location = new System.Drawing.Point(18, 291);
            this.txtCopiasRF3.Name = "txtCopiasRF3";
            this.txtCopiasRF3.Size = new System.Drawing.Size(125, 20);
            this.txtCopiasRF3.TabIndex = 19;
            this.txtCopiasRF3.Text = "10";
            // 
            // btnRF3
            // 
            this.btnRF3.Location = new System.Drawing.Point(18, 315);
            this.btnRF3.Name = "btnRF3";
            this.btnRF3.Size = new System.Drawing.Size(125, 69);
            this.btnRF3.TabIndex = 18;
            this.btnRF3.Text = "Baixar RF3";
            this.btnRF3.UseVisualStyleBackColor = true;
            this.btnRF3.Click += new System.EventHandler(this.btnRF3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(21, 146);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(125, 35);
            this.button2.TabIndex = 15;
            this.button2.Text = "Exibir Retângulo";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnBaixarImagens
            // 
            this.btnBaixarImagens.Location = new System.Drawing.Point(21, 69);
            this.btnBaixarImagens.Name = "btnBaixarImagens";
            this.btnBaixarImagens.Size = new System.Drawing.Size(122, 23);
            this.btnBaixarImagens.TabIndex = 1;
            this.btnBaixarImagens.Text = "Baixar Imagens";
            this.btnBaixarImagens.UseVisualStyleBackColor = true;
            this.btnBaixarImagens.Click += new System.EventHandler(this.btnBaixarImagens_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "URL Imagem:";
            // 
            // txtURLImagem
            // 
            this.txtURLImagem.Location = new System.Drawing.Point(3, 46);
            this.txtURLImagem.Name = "txtURLImagem";
            this.txtURLImagem.Size = new System.Drawing.Size(163, 20);
            this.txtURLImagem.TabIndex = 0;
            this.txtURLImagem.Text = "http://pfeserv1.fazenda.sp.gov.br/sintegrapfe/consultaSintegraServlet";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "URL Site:";
            // 
            // txtUrlSite
            // 
            this.txtUrlSite.Location = new System.Drawing.Point(3, 121);
            this.txtUrlSite.Name = "txtUrlSite";
            this.txtUrlSite.Size = new System.Drawing.Size(163, 20);
            this.txtUrlSite.TabIndex = 2;
            this.txtUrlSite.Text = "https://www2.fazenda.mg.gov.br/sol/ctrl/SOL/RELAT/CONSULTA_707?ACAO=VISUALIZAR";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(21, 188);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 69);
            this.button1.TabIndex = 3;
            this.button1.Text = "Iniciar Captura Por PrintScreen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 584);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(826, 23);
            this.progressBar1.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.lblRecPosition);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Cursor = System.Windows.Forms.Cursors.NoMove2D;
            this.panel1.Location = new System.Drawing.Point(10, 478);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 12;
            this.panel1.Visible = false;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // lblRecPosition
            // 
            this.lblRecPosition.AutoSize = true;
            this.lblRecPosition.Location = new System.Drawing.Point(4, 6);
            this.lblRecPosition.Name = "lblRecPosition";
            this.lblRecPosition.Size = new System.Drawing.Size(35, 13);
            this.lblRecPosition.TabIndex = 1;
            this.lblRecPosition.Text = "label1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.pictureBox1.Location = new System.Drawing.Point(188, 88);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 11);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Salvar imagens em:";
            // 
            // txtFolder
            // 
            this.txtFolder.Location = new System.Drawing.Point(8, 47);
            this.txtFolder.Margin = new System.Windows.Forms.Padding(2);
            this.txtFolder.Name = "txtFolder";
            this.txtFolder.Size = new System.Drawing.Size(170, 20);
            this.txtFolder.TabIndex = 14;
            this.txtFolder.Text = "C:\\OCR\\Testes\\RF3\\";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 607);
            this.Controls.Add(this.txtFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtQtde);
            this.Controls.Add(this.webBrowser1);
            this.Name = "Form1";
            this.Text = "Baixar imagens captcha";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtQtde;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnBaixarImagens;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtURLImagem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtUrlSite;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblRecPosition;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFolder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCopiasRF3;
        private System.Windows.Forms.Button btnRF3;
    }
}

