namespace TestesManuais
{
    partial class BatchTestesForm
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
            this.comboEstado = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPasta = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRemocaoDeFundo = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbExcluirPasta = new System.Windows.Forms.CheckBox();
            this.btnChecarErros = new System.Windows.Forms.Button();
            this.btnSeparacao = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.lblUltimaExecucao = new System.Windows.Forms.Label();
            this.comboPadroes = new System.Windows.Forms.ComboBox();
            this.btnLog = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.workingLog = new System.Windows.Forms.ListBox();
            this.lblResposta = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.log = new System.Windows.Forms.ListBox();
            this.lblTempo = new System.Windows.Forms.Label();
            this.lblRestantes = new System.Windows.Forms.Label();
            this.lblProcessados = new System.Windows.Forms.Label();
            this.lblEncontrados = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboWebService = new System.Windows.Forms.ComboBox();
            this.chkUsarWebService = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbPausa = new System.Windows.Forms.TextBox();
            this.cbPausa = new System.Windows.Forms.CheckBox();
            this.chkExibirImagens = new System.Windows.Forms.CheckBox();
            this.btnReconhecimento = new System.Windows.Forms.Button();
            this.btnClassificacao = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.intTestsBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboEstado
            // 
            this.comboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEstado.FormattingEnabled = true;
            this.comboEstado.Items.AddRange(new object[] {
            "NFE - Nota Fiscal Eletrônica",
            "RF - Receita Federal",
            "RF3 - Receita Federal 3",
            "SI - Siscarga",
            "TRTSP - Tribunal Regional do Trabalho de São Paulo",
            "TJPE -  ",
            "CAM - Sistema de Consignações da Aeronáutica/Marinha",
            "CA - Sistema de Consignações da Aeronáutica",
            "CM - Sistema de Consignações da Marinha",
            "CRJ - Sistema de Consignações RJ - Padrão Cinza",
            "CRJa - Sistema de Consignações RJ - Padrão Azul",
            "CRJv - Sistema de Consignações RJ - Padrão Verde",
            "SP - São Paulo",
            "RJ - Rio de Janeiro",
            "MG - Minas Gerais",
            "AM - Amazonas",
            "AC - Acre",
            "AL - Alagoas",
            "AP - Amapá",
            "BA - Bahia",
            "CE - Ceará",
            "DF - Distrito Federal",
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
            "TO - Tocantins",
            "CCT - CCT_CaptchaA"});
            this.comboEstado.Location = new System.Drawing.Point(119, 20);
            this.comboEstado.Name = "comboEstado";
            this.comboEstado.Size = new System.Drawing.Size(564, 21);
            this.comboEstado.TabIndex = 0;
            this.comboEstado.SelectedIndexChanged += new System.EventHandler(this.comboEstado_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Selecione o estado:";
            // 
            // txtPasta
            // 
            this.txtPasta.Location = new System.Drawing.Point(119, 47);
            this.txtPasta.Name = "txtPasta";
            this.txtPasta.ReadOnly = true;
            this.txtPasta.Size = new System.Drawing.Size(564, 20);
            this.txtPasta.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Pasta base:";
            // 
            // btnRemocaoDeFundo
            // 
            this.btnRemocaoDeFundo.Location = new System.Drawing.Point(44, 97);
            this.btnRemocaoDeFundo.Name = "btnRemocaoDeFundo";
            this.btnRemocaoDeFundo.Size = new System.Drawing.Size(96, 52);
            this.btnRemocaoDeFundo.TabIndex = 4;
            this.btnRemocaoDeFundo.Text = "Validar Remoção de Fundo";
            this.btnRemocaoDeFundo.UseVisualStyleBackColor = true;
            this.btnRemocaoDeFundo.Click += new System.EventHandler(this.btnRemocaoDeFundo_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbExcluirPasta);
            this.groupBox1.Controls.Add(this.btnChecarErros);
            this.groupBox1.Controls.Add(this.btnSeparacao);
            this.groupBox1.Location = new System.Drawing.Point(146, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(107, 150);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            // 
            // cbExcluirPasta
            // 
            this.cbExcluirPasta.AutoSize = true;
            this.cbExcluirPasta.Location = new System.Drawing.Point(12, 111);
            this.cbExcluirPasta.Name = "cbExcluirPasta";
            this.cbExcluirPasta.Size = new System.Drawing.Size(86, 17);
            this.cbExcluirPasta.TabIndex = 11;
            this.cbExcluirPasta.Text = "Excluir pasta";
            this.cbExcluirPasta.UseVisualStyleBackColor = true;
            // 
            // btnChecarErros
            // 
            this.btnChecarErros.Location = new System.Drawing.Point(16, 78);
            this.btnChecarErros.Name = "btnChecarErros";
            this.btnChecarErros.Size = new System.Drawing.Size(75, 23);
            this.btnChecarErros.TabIndex = 10;
            this.btnChecarErros.Text = "Checar Erros";
            this.btnChecarErros.UseVisualStyleBackColor = true;
            this.btnChecarErros.Click += new System.EventHandler(this.btnChecarErros_Click);
            // 
            // btnSeparacao
            // 
            this.btnSeparacao.Location = new System.Drawing.Point(16, 19);
            this.btnSeparacao.Name = "btnSeparacao";
            this.btnSeparacao.Size = new System.Drawing.Size(75, 52);
            this.btnSeparacao.TabIndex = 9;
            this.btnSeparacao.Text = "Validar Separação";
            this.btnSeparacao.UseVisualStyleBackColor = true;
            this.btnSeparacao.Click += new System.EventHandler(this.btnSeparacao_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.lblUltimaExecucao);
            this.groupBox2.Controls.Add(this.comboPadroes);
            this.groupBox2.Controls.Add(this.btnLog);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.workingLog);
            this.groupBox2.Controls.Add(this.lblResposta);
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this.log);
            this.groupBox2.Controls.Add(this.lblTempo);
            this.groupBox2.Controls.Add(this.lblRestantes);
            this.groupBox2.Controls.Add(this.lblProcessados);
            this.groupBox2.Controls.Add(this.lblEncontrados);
            this.groupBox2.Location = new System.Drawing.Point(12, 330);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(672, 181);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(146, 141);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 27;
            this.button6.Text = "button6";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // lblUltimaExecucao
            // 
            this.lblUltimaExecucao.AutoSize = true;
            this.lblUltimaExecucao.Location = new System.Drawing.Point(32, 112);
            this.lblUltimaExecucao.Name = "lblUltimaExecucao";
            this.lblUltimaExecucao.Size = new System.Drawing.Size(89, 13);
            this.lblUltimaExecucao.TabIndex = 12;
            this.lblUltimaExecucao.Text = "Ultima execucao:";
            // 
            // comboPadroes
            // 
            this.comboPadroes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPadroes.FormattingEnabled = true;
            this.comboPadroes.Items.AddRange(new object[] {
            "Todos",
            "Bandeira",
            "Distorcida",
            "Segmentada, Bandeira",
            "Segmentada, Distorcida"});
            this.comboPadroes.Location = new System.Drawing.Point(423, 19);
            this.comboPadroes.Name = "comboPadroes";
            this.comboPadroes.Size = new System.Drawing.Size(243, 21);
            this.comboPadroes.TabIndex = 11;
            this.comboPadroes.SelectedIndexChanged += new System.EventHandler(this.comboPadroes_SelectedIndexChanged);
            // 
            // btnLog
            // 
            this.btnLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLog.Location = new System.Drawing.Point(546, 145);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(119, 26);
            this.btnLog.TabIndex = 10;
            this.btnLog.Text = "0";
            this.btnLog.UseVisualStyleBackColor = true;
            this.btnLog.Visible = false;
            this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(544, 151);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Última Execução";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(423, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Working";
            // 
            // workingLog
            // 
            this.workingLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workingLog.BackColor = System.Drawing.SystemColors.MenuBar;
            this.workingLog.FormattingEnabled = true;
            this.workingLog.Location = new System.Drawing.Point(423, 47);
            this.workingLog.Name = "workingLog";
            this.workingLog.Size = new System.Drawing.Size(118, 95);
            this.workingLog.TabIndex = 7;
            this.workingLog.SelectedIndexChanged += new System.EventHandler(this.workingLog_SelectedIndexChanged);
            // 
            // lblResposta
            // 
            this.lblResposta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResposta.Location = new System.Drawing.Point(213, 112);
            this.lblResposta.Name = "lblResposta";
            this.lblResposta.Size = new System.Drawing.Size(204, 20);
            this.lblResposta.TabIndex = 6;
            this.lblResposta.Text = "SHM3";
            this.lblResposta.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(217, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(200, 90);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // log
            // 
            this.log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.log.BackColor = System.Drawing.SystemColors.MenuBar;
            this.log.FormattingEnabled = true;
            this.log.Location = new System.Drawing.Point(547, 47);
            this.log.Name = "log";
            this.log.Size = new System.Drawing.Size(118, 95);
            this.log.TabIndex = 4;
            this.log.SelectedIndexChanged += new System.EventHandler(this.log_SelectedIndexChanged);
            // 
            // lblTempo
            // 
            this.lblTempo.AutoSize = true;
            this.lblTempo.Location = new System.Drawing.Point(31, 91);
            this.lblTempo.Name = "lblTempo";
            this.lblTempo.Size = new System.Drawing.Size(90, 13);
            this.lblTempo.TabIndex = 3;
            this.lblTempo.Text = "Tempo decorrido:";
            // 
            // lblRestantes
            // 
            this.lblRestantes.AutoSize = true;
            this.lblRestantes.Location = new System.Drawing.Point(25, 68);
            this.lblRestantes.Name = "lblRestantes";
            this.lblRestantes.Size = new System.Drawing.Size(97, 13);
            this.lblRestantes.TabIndex = 2;
            this.lblRestantes.Text = "Arquivos restantes:";
            // 
            // lblProcessados
            // 
            this.lblProcessados.AutoSize = true;
            this.lblProcessados.Location = new System.Drawing.Point(9, 44);
            this.lblProcessados.Name = "lblProcessados";
            this.lblProcessados.Size = new System.Drawing.Size(114, 13);
            this.lblProcessados.TabIndex = 1;
            this.lblProcessados.Text = "Arquivos processados:";
            // 
            // lblEncontrados
            // 
            this.lblEncontrados.AutoSize = true;
            this.lblEncontrados.Location = new System.Drawing.Point(9, 21);
            this.lblEncontrados.Name = "lblEncontrados";
            this.lblEncontrados.Size = new System.Drawing.Size(113, 13);
            this.lblEncontrados.TabIndex = 0;
            this.lblEncontrados.Text = "Arquivos encontrados:";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(2, 517);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(692, 23);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(276, 207);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 49);
            this.button1.TabIndex = 20;
            this.button1.Text = "Validar Classificação";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.tbPausa);
            this.groupBox3.Controls.Add(this.cbPausa);
            this.groupBox3.Controls.Add(this.chkExibirImagens);
            this.groupBox3.Controls.Add(this.btnReconhecimento);
            this.groupBox3.Location = new System.Drawing.Point(390, 78);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(235, 231);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.comboWebService);
            this.groupBox4.Controls.Add(this.chkUsarWebService);
            this.groupBox4.Location = new System.Drawing.Point(11, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(214, 76);
            this.groupBox4.TabIndex = 19;
            this.groupBox4.TabStop = false;
            // 
            // comboWebService
            // 
            this.comboWebService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboWebService.Enabled = false;
            this.comboWebService.FormattingEnabled = true;
            this.comboWebService.Items.AddRange(new object[] {
            "Local",
            "Remoto"});
            this.comboWebService.Location = new System.Drawing.Point(37, 42);
            this.comboWebService.Name = "comboWebService";
            this.comboWebService.Size = new System.Drawing.Size(121, 21);
            this.comboWebService.TabIndex = 19;
            // 
            // chkUsarWebService
            // 
            this.chkUsarWebService.AutoSize = true;
            this.chkUsarWebService.Location = new System.Drawing.Point(10, 19);
            this.chkUsarWebService.Name = "chkUsarWebService";
            this.chkUsarWebService.Size = new System.Drawing.Size(200, 17);
            this.chkUsarWebService.TabIndex = 18;
            this.chkUsarWebService.Text = "Usar WebService nos testes de rede";
            this.chkUsarWebService.UseVisualStyleBackColor = true;
            this.chkUsarWebService.CheckedChanged += new System.EventHandler(this.chkUsarWebService_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(128, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "ms";
            // 
            // tbPausa
            // 
            this.tbPausa.Location = new System.Drawing.Point(22, 201);
            this.tbPausa.Name = "tbPausa";
            this.tbPausa.Size = new System.Drawing.Size(100, 20);
            this.tbPausa.TabIndex = 16;
            this.tbPausa.Text = "1000";
            // 
            // cbPausa
            // 
            this.cbPausa.AutoSize = true;
            this.cbPausa.Location = new System.Drawing.Point(29, 178);
            this.cbPausa.Name = "cbPausa";
            this.cbPausa.Size = new System.Drawing.Size(125, 17);
            this.cbPausa.TabIndex = 15;
            this.cbPausa.Text = "Pausa entre imagens";
            this.cbPausa.UseVisualStyleBackColor = true;
            // 
            // chkExibirImagens
            // 
            this.chkExibirImagens.AutoSize = true;
            this.chkExibirImagens.Checked = true;
            this.chkExibirImagens.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExibirImagens.Location = new System.Drawing.Point(29, 155);
            this.chkExibirImagens.Name = "chkExibirImagens";
            this.chkExibirImagens.Size = new System.Drawing.Size(93, 17);
            this.chkExibirImagens.TabIndex = 14;
            this.chkExibirImagens.Text = "Exibir imagens";
            this.chkExibirImagens.UseVisualStyleBackColor = true;
            // 
            // btnReconhecimento
            // 
            this.btnReconhecimento.Location = new System.Drawing.Point(22, 93);
            this.btnReconhecimento.Name = "btnReconhecimento";
            this.btnReconhecimento.Size = new System.Drawing.Size(132, 53);
            this.btnReconhecimento.TabIndex = 13;
            this.btnReconhecimento.Text = "Validar Reconhecimento";
            this.btnReconhecimento.UseVisualStyleBackColor = true;
            this.btnReconhecimento.Click += new System.EventHandler(this.btnReconhecimento_Click);
            // 
            // btnClassificacao
            // 
            this.btnClassificacao.Location = new System.Drawing.Point(276, 96);
            this.btnClassificacao.Name = "btnClassificacao";
            this.btnClassificacao.Size = new System.Drawing.Size(92, 52);
            this.btnClassificacao.TabIndex = 18;
            this.btnClassificacao.Text = "Classificação Manual Por Caractere";
            this.btnClassificacao.UseVisualStyleBackColor = true;
            this.btnClassificacao.Click += new System.EventHandler(this.btnClassificacao_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(276, 262);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 48);
            this.button2.TabIndex = 21;
            this.button2.Text = "Eliminar Imagens Idênticas";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(10, 17);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(96, 54);
            this.button3.TabIndex = 22;
            this.button3.Text = "Clonar Classificadas";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(112, 17);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(95, 54);
            this.button4.TabIndex = 23;
            this.button4.Text = "Temp";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(276, 154);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(92, 48);
            this.button5.TabIndex = 24;
            this.button5.Text = "Classificação Manual Por Imagem";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button3);
            this.groupBox5.Controls.Add(this.button4);
            this.groupBox5.Location = new System.Drawing.Point(34, 236);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(219, 81);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            // 
            // intTestsBtn
            // 
            this.intTestsBtn.Location = new System.Drawing.Point(44, 171);
            this.intTestsBtn.Name = "intTestsBtn";
            this.intTestsBtn.Size = new System.Drawing.Size(96, 52);
            this.intTestsBtn.TabIndex = 26;
            this.intTestsBtn.Text = "Testes";
            this.intTestsBtn.UseVisualStyleBackColor = true;
            this.intTestsBtn.Click += new System.EventHandler(this.intTestsBtn_Click);
            // 
            // BatchTestesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 541);
            this.Controls.Add(this.intTestsBtn);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnClassificacao);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnRemocaoDeFundo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtPasta);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboEstado);
            this.Name = "BatchTestesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lote de Testes";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboEstado;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPasta;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnRemocaoDeFundo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnChecarErros;
        private System.Windows.Forms.Button btnSeparacao;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.Label lblRestantes;
        private System.Windows.Forms.Label lblProcessados;
        private System.Windows.Forms.Label lblEncontrados;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox log;
        private System.Windows.Forms.Label lblResposta;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox cbExcluirPasta;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboWebService;
        private System.Windows.Forms.CheckBox chkUsarWebService;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbPausa;
        private System.Windows.Forms.CheckBox cbPausa;
        private System.Windows.Forms.CheckBox chkExibirImagens;
        private System.Windows.Forms.Button btnReconhecimento;
        private System.Windows.Forms.Button btnClassificacao;
        private System.Windows.Forms.ListBox workingLog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.ComboBox comboPadroes;
        private System.Windows.Forms.Label lblUltimaExecucao;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button intTestsBtn;
        private System.Windows.Forms.Button button6;
    }
}