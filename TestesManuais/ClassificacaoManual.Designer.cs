namespace TestesManuais
{
    partial class ClassificacaoManual
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClassificacaoManual));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panelMover = new System.Windows.Forms.Panel();
            this.txtNovaPasta = new System.Windows.Forms.TextBox();
            this.chkNaoExiste = new System.Windows.Forms.CheckBox();
            this.btnMover = new System.Windows.Forms.Button();
            this.cbMoverPara = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panelMover.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(624, 649);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.TabStop = false;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            this.tabControl1.Deselected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Deselected);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "checked.png");
            // 
            // panelMover
            // 
            this.panelMover.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMover.BackColor = System.Drawing.SystemColors.Control;
            this.panelMover.Controls.Add(this.txtNovaPasta);
            this.panelMover.Controls.Add(this.chkNaoExiste);
            this.panelMover.Controls.Add(this.btnMover);
            this.panelMover.Controls.Add(this.cbMoverPara);
            this.panelMover.Controls.Add(this.label1);
            this.panelMover.Enabled = false;
            this.panelMover.Location = new System.Drawing.Point(642, 12);
            this.panelMover.Name = "panelMover";
            this.panelMover.Size = new System.Drawing.Size(191, 266);
            this.panelMover.TabIndex = 4;
            // 
            // txtNovaPasta
            // 
            this.txtNovaPasta.Enabled = false;
            this.txtNovaPasta.Location = new System.Drawing.Point(10, 86);
            this.txtNovaPasta.Name = "txtNovaPasta";
            this.txtNovaPasta.Size = new System.Drawing.Size(168, 20);
            this.txtNovaPasta.TabIndex = 8;
            this.txtNovaPasta.TabStop = false;
            this.txtNovaPasta.TextChanged += new System.EventHandler(this.txtNovaPasta_TextChanged);
            // 
            // chkNaoExiste
            // 
            this.chkNaoExiste.AutoSize = true;
            this.chkNaoExiste.Location = new System.Drawing.Point(10, 63);
            this.chkNaoExiste.Name = "chkNaoExiste";
            this.chkNaoExiste.Size = new System.Drawing.Size(142, 17);
            this.chkNaoExiste.TabIndex = 7;
            this.chkNaoExiste.TabStop = false;
            this.chkNaoExiste.Text = "A pasta ainda não existe";
            this.chkNaoExiste.UseVisualStyleBackColor = true;
            this.chkNaoExiste.CheckedChanged += new System.EventHandler(this.chkNaoExiste_CheckedChanged);
            // 
            // btnMover
            // 
            this.btnMover.Enabled = false;
            this.btnMover.Location = new System.Drawing.Point(7, 122);
            this.btnMover.Name = "btnMover";
            this.btnMover.Size = new System.Drawing.Size(171, 50);
            this.btnMover.TabIndex = 6;
            this.btnMover.TabStop = false;
            this.btnMover.Text = "Mover seleção";
            this.btnMover.UseVisualStyleBackColor = true;
            this.btnMover.Click += new System.EventHandler(this.btnMover_Click);
            // 
            // cbMoverPara
            // 
            this.cbMoverPara.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMoverPara.FormattingEnabled = true;
            this.cbMoverPara.Location = new System.Drawing.Point(10, 36);
            this.cbMoverPara.Name = "cbMoverPara";
            this.cbMoverPara.Size = new System.Drawing.Size(168, 21);
            this.cbMoverPara.Sorted = true;
            this.cbMoverPara.TabIndex = 5;
            this.cbMoverPara.TabStop = false;
            this.cbMoverPara.SelectedIndexChanged += new System.EventHandler(this.cbMoverPara_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Mover imagens selecionadas para:";
            // 
            // ClassificacaoManual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 673);
            this.Controls.Add(this.panelMover);
            this.Controls.Add(this.tabControl1);
            this.Name = "ClassificacaoManual";
            this.Text = "ClassificacaoManual";
            this.Load += new System.EventHandler(this.ClassificacaoManual_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ClassificacaoManual_KeyUp);
            this.panelMover.ResumeLayout(false);
            this.panelMover.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Panel panelMover;
        private System.Windows.Forms.TextBox txtNovaPasta;
        private System.Windows.Forms.CheckBox chkNaoExiste;
        private System.Windows.Forms.Button btnMover;
        private System.Windows.Forms.ComboBox cbMoverPara;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}