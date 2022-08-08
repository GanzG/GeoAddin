namespace GeoAddin.Openings_Windows
{
    partial class OpeningWin_ElementSelection
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
            this.close_bt = new System.Windows.Forms.Button();
            this.result_DGV = new System.Windows.Forms.DataGridView();
            this.CatGroup = new System.Windows.Forms.GroupBox();
            this.cat_9_ComBox = new System.Windows.Forms.ComboBox();
            this.cat_2_ComBox = new System.Windows.Forms.ComboBox();
            this.cat_3_ComBox = new System.Windows.Forms.ComboBox();
            this.cat_8_ComBox = new System.Windows.Forms.ComboBox();
            this.cat_4_ComBox = new System.Windows.Forms.ComboBox();
            this.cat_5_ComBox = new System.Windows.Forms.ComboBox();
            this.cat_7_ComBox = new System.Windows.Forms.ComboBox();
            this.cat_6_ComBox = new System.Windows.Forms.ComboBox();
            this.ParamGroup = new System.Windows.Forms.GroupBox();
            this.relLabel = new System.Windows.Forms.Label();
            this.relationship_ComBox = new System.Windows.Forms.ComboBox();
            this.ruleValue_5_tb = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ruleValue_1_tb = new System.Windows.Forms.TextBox();
            this.rule_5_ComBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ruleValue_4_tb = new System.Windows.Forms.TextBox();
            this.rule_1_ComBox = new System.Windows.Forms.ComboBox();
            this.param_1_ComBox = new System.Windows.Forms.ComboBox();
            this.rule_4_ComBox = new System.Windows.Forms.ComboBox();
            this.param_2_ComBox = new System.Windows.Forms.ComboBox();
            this.rule_2_ComBox = new System.Windows.Forms.ComboBox();
            this.ruleValue_3_tb = new System.Windows.Forms.TextBox();
            this.param_5_ComBox = new System.Windows.Forms.ComboBox();
            this.param_4_ComBox = new System.Windows.Forms.ComboBox();
            this.ruleValue_2_tb = new System.Windows.Forms.TextBox();
            this.rule_3_ComBox = new System.Windows.Forms.ComboBox();
            this.param_3_ComBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Результат = new System.Windows.Forms.Label();
            this.result_bt = new System.Windows.Forms.Button();
            this.addToResult_bt = new System.Windows.Forms.Button();
            this.clearAll_bt = new System.Windows.Forms.Button();
            this.saveDGV_bt = new System.Windows.Forms.Button();
            this.loadDGV_bt = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.action_bt = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.delete_bt = new System.Windows.Forms.Button();
            this.add_bt = new System.Windows.Forms.Button();
            this.mainGroup = new System.Windows.Forms.GroupBox();
            this.progress_pb = new System.Windows.Forms.ProgressBar();
            this.action_ComBox = new System.Windows.Forms.ComboBox();
            this.rule_rb = new System.Windows.Forms.RadioButton();
            this.cat_rb = new System.Windows.Forms.RadioButton();
            this.getPath_sfd = new System.Windows.Forms.SaveFileDialog();
            this.openFile_ofd = new System.Windows.Forms.OpenFileDialog();
            this.timeLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.result_DGV)).BeginInit();
            this.CatGroup.SuspendLayout();
            this.ParamGroup.SuspendLayout();
            this.mainGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // close_bt
            // 
            this.close_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.close_bt.Location = new System.Drawing.Point(478, 335);
            this.close_bt.Name = "close_bt";
            this.close_bt.Size = new System.Drawing.Size(109, 42);
            this.close_bt.TabIndex = 0;
            this.close_bt.Text = "Отмена";
            this.close_bt.UseVisualStyleBackColor = true;
            this.close_bt.Click += new System.EventHandler(this.close_bt_Click);
            // 
            // result_DGV
            // 
            this.result_DGV.AllowUserToAddRows = false;
            this.result_DGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.result_DGV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.result_DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.result_DGV.Location = new System.Drawing.Point(10, 48);
            this.result_DGV.Name = "result_DGV";
            this.result_DGV.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.result_DGV.RowHeadersVisible = false;
            this.result_DGV.Size = new System.Drawing.Size(585, 165);
            this.result_DGV.TabIndex = 1;
            // 
            // CatGroup
            // 
            this.CatGroup.Controls.Add(this.cat_9_ComBox);
            this.CatGroup.Controls.Add(this.cat_2_ComBox);
            this.CatGroup.Controls.Add(this.cat_3_ComBox);
            this.CatGroup.Controls.Add(this.cat_8_ComBox);
            this.CatGroup.Controls.Add(this.cat_4_ComBox);
            this.CatGroup.Controls.Add(this.cat_5_ComBox);
            this.CatGroup.Controls.Add(this.cat_7_ComBox);
            this.CatGroup.Controls.Add(this.cat_6_ComBox);
            this.CatGroup.Location = new System.Drawing.Point(16, 41);
            this.CatGroup.Name = "CatGroup";
            this.CatGroup.Size = new System.Drawing.Size(171, 101);
            this.CatGroup.TabIndex = 3;
            this.CatGroup.TabStop = false;
            // 
            // cat_9_ComBox
            // 
            this.cat_9_ComBox.FormattingEnabled = true;
            this.cat_9_ComBox.Location = new System.Drawing.Point(6, 235);
            this.cat_9_ComBox.Name = "cat_9_ComBox";
            this.cat_9_ComBox.Size = new System.Drawing.Size(157, 21);
            this.cat_9_ComBox.TabIndex = 10;
            // 
            // cat_2_ComBox
            // 
            this.cat_2_ComBox.FormattingEnabled = true;
            this.cat_2_ComBox.Location = new System.Drawing.Point(6, 46);
            this.cat_2_ComBox.Name = "cat_2_ComBox";
            this.cat_2_ComBox.Size = new System.Drawing.Size(157, 21);
            this.cat_2_ComBox.TabIndex = 3;
            // 
            // cat_3_ComBox
            // 
            this.cat_3_ComBox.FormattingEnabled = true;
            this.cat_3_ComBox.Location = new System.Drawing.Point(195, 46);
            this.cat_3_ComBox.Name = "cat_3_ComBox";
            this.cat_3_ComBox.Size = new System.Drawing.Size(157, 21);
            this.cat_3_ComBox.TabIndex = 4;
            // 
            // cat_8_ComBox
            // 
            this.cat_8_ComBox.FormattingEnabled = true;
            this.cat_8_ComBox.Location = new System.Drawing.Point(195, 181);
            this.cat_8_ComBox.Name = "cat_8_ComBox";
            this.cat_8_ComBox.Size = new System.Drawing.Size(157, 21);
            this.cat_8_ComBox.TabIndex = 9;
            // 
            // cat_4_ComBox
            // 
            this.cat_4_ComBox.FormattingEnabled = true;
            this.cat_4_ComBox.Location = new System.Drawing.Point(195, 73);
            this.cat_4_ComBox.Name = "cat_4_ComBox";
            this.cat_4_ComBox.Size = new System.Drawing.Size(157, 21);
            this.cat_4_ComBox.TabIndex = 5;
            // 
            // cat_5_ComBox
            // 
            this.cat_5_ComBox.FormattingEnabled = true;
            this.cat_5_ComBox.Location = new System.Drawing.Point(195, 100);
            this.cat_5_ComBox.Name = "cat_5_ComBox";
            this.cat_5_ComBox.Size = new System.Drawing.Size(157, 21);
            this.cat_5_ComBox.TabIndex = 6;
            // 
            // cat_7_ComBox
            // 
            this.cat_7_ComBox.FormattingEnabled = true;
            this.cat_7_ComBox.Location = new System.Drawing.Point(195, 154);
            this.cat_7_ComBox.Name = "cat_7_ComBox";
            this.cat_7_ComBox.Size = new System.Drawing.Size(157, 21);
            this.cat_7_ComBox.TabIndex = 8;
            // 
            // cat_6_ComBox
            // 
            this.cat_6_ComBox.FormattingEnabled = true;
            this.cat_6_ComBox.Location = new System.Drawing.Point(195, 127);
            this.cat_6_ComBox.Name = "cat_6_ComBox";
            this.cat_6_ComBox.Size = new System.Drawing.Size(157, 21);
            this.cat_6_ComBox.TabIndex = 7;
            // 
            // ParamGroup
            // 
            this.ParamGroup.Controls.Add(this.relLabel);
            this.ParamGroup.Controls.Add(this.relationship_ComBox);
            this.ParamGroup.Controls.Add(this.ruleValue_5_tb);
            this.ParamGroup.Controls.Add(this.label5);
            this.ParamGroup.Controls.Add(this.ruleValue_1_tb);
            this.ParamGroup.Controls.Add(this.rule_5_ComBox);
            this.ParamGroup.Controls.Add(this.label4);
            this.ParamGroup.Controls.Add(this.label3);
            this.ParamGroup.Controls.Add(this.ruleValue_4_tb);
            this.ParamGroup.Controls.Add(this.rule_1_ComBox);
            this.ParamGroup.Controls.Add(this.param_1_ComBox);
            this.ParamGroup.Controls.Add(this.rule_4_ComBox);
            this.ParamGroup.Controls.Add(this.param_2_ComBox);
            this.ParamGroup.Controls.Add(this.rule_2_ComBox);
            this.ParamGroup.Controls.Add(this.ruleValue_3_tb);
            this.ParamGroup.Controls.Add(this.param_5_ComBox);
            this.ParamGroup.Controls.Add(this.param_4_ComBox);
            this.ParamGroup.Controls.Add(this.ruleValue_2_tb);
            this.ParamGroup.Controls.Add(this.rule_3_ComBox);
            this.ParamGroup.Controls.Add(this.param_3_ComBox);
            this.ParamGroup.Location = new System.Drawing.Point(211, 41);
            this.ParamGroup.Name = "ParamGroup";
            this.ParamGroup.Size = new System.Drawing.Size(396, 133);
            this.ParamGroup.TabIndex = 4;
            this.ParamGroup.TabStop = false;
            // 
            // relLabel
            // 
            this.relLabel.AutoSize = true;
            this.relLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.relLabel.Location = new System.Drawing.Point(89, 101);
            this.relLabel.Name = "relLabel";
            this.relLabel.Size = new System.Drawing.Size(204, 17);
            this.relLabel.TabIndex = 47;
            this.relLabel.Text = "Отношение между условиями";
            // 
            // relationship_ComBox
            // 
            this.relationship_ComBox.FormattingEnabled = true;
            this.relationship_ComBox.Location = new System.Drawing.Point(301, 100);
            this.relationship_ComBox.Name = "relationship_ComBox";
            this.relationship_ComBox.Size = new System.Drawing.Size(88, 21);
            this.relationship_ComBox.TabIndex = 45;
            // 
            // ruleValue_5_tb
            // 
            this.ruleValue_5_tb.Location = new System.Drawing.Point(713, 130);
            this.ruleValue_5_tb.Name = "ruleValue_5_tb";
            this.ruleValue_5_tb.Size = new System.Drawing.Size(90, 20);
            this.ruleValue_5_tb.TabIndex = 44;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(308, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 17);
            this.label5.TabIndex = 36;
            this.label5.Text = "Значение";
            // 
            // ruleValue_1_tb
            // 
            this.ruleValue_1_tb.Location = new System.Drawing.Point(301, 49);
            this.ruleValue_1_tb.Name = "ruleValue_1_tb";
            this.ruleValue_1_tb.Size = new System.Drawing.Size(90, 20);
            this.ruleValue_1_tb.TabIndex = 35;
            // 
            // rule_5_ComBox
            // 
            this.rule_5_ComBox.FormattingEnabled = true;
            this.rule_5_ComBox.Location = new System.Drawing.Point(602, 129);
            this.rule_5_ComBox.Name = "rule_5_ComBox";
            this.rule_5_ComBox.Size = new System.Drawing.Size(90, 21);
            this.rule_5_ComBox.TabIndex = 43;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(203, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 17);
            this.label4.TabIndex = 34;
            this.label4.Text = "Условие";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(60, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Параметр";
            // 
            // ruleValue_4_tb
            // 
            this.ruleValue_4_tb.Location = new System.Drawing.Point(713, 94);
            this.ruleValue_4_tb.Name = "ruleValue_4_tb";
            this.ruleValue_4_tb.Size = new System.Drawing.Size(90, 20);
            this.ruleValue_4_tb.TabIndex = 42;
            // 
            // rule_1_ComBox
            // 
            this.rule_1_ComBox.FormattingEnabled = true;
            this.rule_1_ComBox.Location = new System.Drawing.Point(190, 48);
            this.rule_1_ComBox.Name = "rule_1_ComBox";
            this.rule_1_ComBox.Size = new System.Drawing.Size(90, 21);
            this.rule_1_ComBox.TabIndex = 33;
            // 
            // param_1_ComBox
            // 
            this.param_1_ComBox.FormattingEnabled = true;
            this.param_1_ComBox.Location = new System.Drawing.Point(6, 48);
            this.param_1_ComBox.Name = "param_1_ComBox";
            this.param_1_ComBox.Size = new System.Drawing.Size(163, 21);
            this.param_1_ComBox.TabIndex = 19;
            // 
            // rule_4_ComBox
            // 
            this.rule_4_ComBox.FormattingEnabled = true;
            this.rule_4_ComBox.Location = new System.Drawing.Point(602, 93);
            this.rule_4_ComBox.Name = "rule_4_ComBox";
            this.rule_4_ComBox.Size = new System.Drawing.Size(90, 21);
            this.rule_4_ComBox.TabIndex = 41;
            // 
            // param_2_ComBox
            // 
            this.param_2_ComBox.FormattingEnabled = true;
            this.param_2_ComBox.Location = new System.Drawing.Point(430, 20);
            this.param_2_ComBox.Name = "param_2_ComBox";
            this.param_2_ComBox.Size = new System.Drawing.Size(151, 21);
            this.param_2_ComBox.TabIndex = 20;
            // 
            // rule_2_ComBox
            // 
            this.rule_2_ComBox.FormattingEnabled = true;
            this.rule_2_ComBox.Location = new System.Drawing.Point(602, 20);
            this.rule_2_ComBox.Name = "rule_2_ComBox";
            this.rule_2_ComBox.Size = new System.Drawing.Size(90, 21);
            this.rule_2_ComBox.TabIndex = 37;
            // 
            // ruleValue_3_tb
            // 
            this.ruleValue_3_tb.Location = new System.Drawing.Point(713, 59);
            this.ruleValue_3_tb.Name = "ruleValue_3_tb";
            this.ruleValue_3_tb.Size = new System.Drawing.Size(90, 20);
            this.ruleValue_3_tb.TabIndex = 40;
            // 
            // param_5_ComBox
            // 
            this.param_5_ComBox.FormattingEnabled = true;
            this.param_5_ComBox.Location = new System.Drawing.Point(430, 129);
            this.param_5_ComBox.Name = "param_5_ComBox";
            this.param_5_ComBox.Size = new System.Drawing.Size(151, 21);
            this.param_5_ComBox.TabIndex = 27;
            // 
            // param_4_ComBox
            // 
            this.param_4_ComBox.FormattingEnabled = true;
            this.param_4_ComBox.Location = new System.Drawing.Point(430, 92);
            this.param_4_ComBox.Name = "param_4_ComBox";
            this.param_4_ComBox.Size = new System.Drawing.Size(151, 21);
            this.param_4_ComBox.TabIndex = 22;
            // 
            // ruleValue_2_tb
            // 
            this.ruleValue_2_tb.Location = new System.Drawing.Point(713, 21);
            this.ruleValue_2_tb.Name = "ruleValue_2_tb";
            this.ruleValue_2_tb.Size = new System.Drawing.Size(90, 20);
            this.ruleValue_2_tb.TabIndex = 38;
            // 
            // rule_3_ComBox
            // 
            this.rule_3_ComBox.FormattingEnabled = true;
            this.rule_3_ComBox.Location = new System.Drawing.Point(602, 58);
            this.rule_3_ComBox.Name = "rule_3_ComBox";
            this.rule_3_ComBox.Size = new System.Drawing.Size(90, 21);
            this.rule_3_ComBox.TabIndex = 39;
            // 
            // param_3_ComBox
            // 
            this.param_3_ComBox.FormattingEnabled = true;
            this.param_3_ComBox.Location = new System.Drawing.Point(430, 58);
            this.param_3_ComBox.Name = "param_3_ComBox";
            this.param_3_ComBox.Size = new System.Drawing.Size(151, 21);
            this.param_3_ComBox.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(18, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 5;
            this.label1.Text = "Категория";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(207, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(174, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Параметр и значения";
            // 
            // Результат
            // 
            this.Результат.AutoSize = true;
            this.Результат.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Результат.Location = new System.Drawing.Point(6, 16);
            this.Результат.Name = "Результат";
            this.Результат.Size = new System.Drawing.Size(157, 20);
            this.Результат.TabIndex = 7;
            this.Результат.Text = "Результат выборки";
            // 
            // result_bt
            // 
            this.result_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.result_bt.Location = new System.Drawing.Point(10, 249);
            this.result_bt.Name = "result_bt";
            this.result_bt.Size = new System.Drawing.Size(114, 42);
            this.result_bt.TabIndex = 8;
            this.result_bt.Text = "Сформировать выборку";
            this.result_bt.UseVisualStyleBackColor = true;
            this.result_bt.Click += new System.EventHandler(this.result_bt_Click);
            // 
            // addToResult_bt
            // 
            this.addToResult_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.addToResult_bt.Location = new System.Drawing.Point(130, 249);
            this.addToResult_bt.Name = "addToResult_bt";
            this.addToResult_bt.Size = new System.Drawing.Size(114, 42);
            this.addToResult_bt.TabIndex = 9;
            this.addToResult_bt.Text = "Дополнить выборку";
            this.addToResult_bt.UseVisualStyleBackColor = true;
            this.addToResult_bt.Click += new System.EventHandler(this.addToResult_bt_Click);
            // 
            // clearAll_bt
            // 
            this.clearAll_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.clearAll_bt.Location = new System.Drawing.Point(10, 297);
            this.clearAll_bt.Name = "clearAll_bt";
            this.clearAll_bt.Size = new System.Drawing.Size(114, 42);
            this.clearAll_bt.TabIndex = 10;
            this.clearAll_bt.Text = "Очистить фильтр";
            this.clearAll_bt.UseVisualStyleBackColor = true;
            this.clearAll_bt.Click += new System.EventHandler(this.clearAll_bt_Click);
            // 
            // saveDGV_bt
            // 
            this.saveDGV_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.saveDGV_bt.Location = new System.Drawing.Point(10, 345);
            this.saveDGV_bt.Name = "saveDGV_bt";
            this.saveDGV_bt.Size = new System.Drawing.Size(114, 42);
            this.saveDGV_bt.TabIndex = 11;
            this.saveDGV_bt.Text = "Сохранить выборку";
            this.saveDGV_bt.UseVisualStyleBackColor = true;
            this.saveDGV_bt.Click += new System.EventHandler(this.saveDGV_bt_Click);
            // 
            // loadDGV_bt
            // 
            this.loadDGV_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loadDGV_bt.Location = new System.Drawing.Point(130, 345);
            this.loadDGV_bt.Name = "loadDGV_bt";
            this.loadDGV_bt.Size = new System.Drawing.Size(114, 42);
            this.loadDGV_bt.TabIndex = 12;
            this.loadDGV_bt.Text = "Загрузить выборку";
            this.loadDGV_bt.UseVisualStyleBackColor = true;
            this.loadDGV_bt.Click += new System.EventHandler(this.loadDGV_bt_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(6, 226);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(193, 20);
            this.label7.TabIndex = 13;
            this.label7.Text = "Формирование выборки";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(353, 216);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(215, 20);
            this.label8.TabIndex = 16;
            this.label8.Text = "Действия над элементами";
            // 
            // action_bt
            // 
            this.action_bt.Enabled = false;
            this.action_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.action_bt.Location = new System.Drawing.Point(357, 244);
            this.action_bt.Name = "action_bt";
            this.action_bt.Size = new System.Drawing.Size(114, 33);
            this.action_bt.TabIndex = 15;
            this.action_bt.Text = "Выполнить:";
            this.action_bt.UseVisualStyleBackColor = true;
            this.action_bt.Click += new System.EventHandler(this.action_bt_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button3.Location = new System.Drawing.Point(130, 297);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(114, 42);
            this.button3.TabIndex = 17;
            this.button3.Text = "Шаблон поиска";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // delete_bt
            // 
            this.delete_bt.BackColor = System.Drawing.Color.Transparent;
            this.delete_bt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.delete_bt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.delete_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.delete_bt.Location = new System.Drawing.Point(258, 361);
            this.delete_bt.Name = "delete_bt";
            this.delete_bt.Size = new System.Drawing.Size(82, 26);
            this.delete_bt.TabIndex = 19;
            this.delete_bt.Text = "Удалить";
            this.delete_bt.UseVisualStyleBackColor = false;
            this.delete_bt.Click += new System.EventHandler(this.delete_bt_Click);
            // 
            // add_bt
            // 
            this.add_bt.BackColor = System.Drawing.Color.Transparent;
            this.add_bt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.add_bt.Cursor = System.Windows.Forms.Cursors.Hand;
            this.add_bt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.add_bt.Location = new System.Drawing.Point(258, 330);
            this.add_bt.Name = "add_bt";
            this.add_bt.Size = new System.Drawing.Size(82, 26);
            this.add_bt.TabIndex = 18;
            this.add_bt.Text = "Добавить";
            this.add_bt.UseVisualStyleBackColor = false;
            this.add_bt.Click += new System.EventHandler(this.add_bt_Click);
            // 
            // mainGroup
            // 
            this.mainGroup.Controls.Add(this.progress_pb);
            this.mainGroup.Controls.Add(this.action_ComBox);
            this.mainGroup.Controls.Add(this.rule_rb);
            this.mainGroup.Controls.Add(this.cat_rb);
            this.mainGroup.Controls.Add(this.delete_bt);
            this.mainGroup.Controls.Add(this.add_bt);
            this.mainGroup.Controls.Add(this.Результат);
            this.mainGroup.Controls.Add(this.button3);
            this.mainGroup.Controls.Add(this.close_bt);
            this.mainGroup.Controls.Add(this.label8);
            this.mainGroup.Controls.Add(this.result_DGV);
            this.mainGroup.Controls.Add(this.action_bt);
            this.mainGroup.Controls.Add(this.result_bt);
            this.mainGroup.Controls.Add(this.addToResult_bt);
            this.mainGroup.Controls.Add(this.label7);
            this.mainGroup.Controls.Add(this.clearAll_bt);
            this.mainGroup.Controls.Add(this.loadDGV_bt);
            this.mainGroup.Controls.Add(this.saveDGV_bt);
            this.mainGroup.Location = new System.Drawing.Point(16, 195);
            this.mainGroup.Name = "mainGroup";
            this.mainGroup.Size = new System.Drawing.Size(593, 397);
            this.mainGroup.TabIndex = 18;
            this.mainGroup.TabStop = false;
            // 
            // progress_pb
            // 
            this.progress_pb.Location = new System.Drawing.Point(486, 16);
            this.progress_pb.Name = "progress_pb";
            this.progress_pb.Size = new System.Drawing.Size(100, 23);
            this.progress_pb.TabIndex = 49;
            this.progress_pb.Tag = "";
            // 
            // action_ComBox
            // 
            this.action_ComBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.action_ComBox.FormattingEnabled = true;
            this.action_ComBox.Location = new System.Drawing.Point(480, 251);
            this.action_ComBox.Name = "action_ComBox";
            this.action_ComBox.Size = new System.Drawing.Size(107, 21);
            this.action_ComBox.TabIndex = 48;
            // 
            // rule_rb
            // 
            this.rule_rb.AutoSize = true;
            this.rule_rb.Location = new System.Drawing.Point(357, 358);
            this.rule_rb.Name = "rule_rb";
            this.rule_rb.Size = new System.Drawing.Size(69, 17);
            this.rule_rb.TabIndex = 21;
            this.rule_rb.TabStop = true;
            this.rule_rb.Text = "Правило";
            this.rule_rb.UseVisualStyleBackColor = true;
            // 
            // cat_rb
            // 
            this.cat_rb.AutoSize = true;
            this.cat_rb.Location = new System.Drawing.Point(357, 335);
            this.cat_rb.Name = "cat_rb";
            this.cat_rb.Size = new System.Drawing.Size(80, 17);
            this.cat_rb.TabIndex = 20;
            this.cat_rb.TabStop = true;
            this.cat_rb.Text = "Категорию";
            this.cat_rb.UseVisualStyleBackColor = true;
            // 
            // getPath_sfd
            // 
            this.getPath_sfd.Filter = "Таблица Excel | *.xlsx";
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(13, 595);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(0, 13);
            this.timeLabel.TabIndex = 50;
            // 
            // OpeningWin_ElementSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 609);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.mainGroup);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ParamGroup);
            this.Controls.Add(this.CatGroup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "OpeningWin_ElementSelection";
            this.Text = "Селектор элементов";
            this.Load += new System.EventHandler(this.OpeningWin_ElementSelection_Load);
            this.TextChanged += new System.EventHandler(this.selectControl);
            ((System.ComponentModel.ISupportInitialize)(this.result_DGV)).EndInit();
            this.CatGroup.ResumeLayout(false);
            this.ParamGroup.ResumeLayout(false);
            this.ParamGroup.PerformLayout();
            this.mainGroup.ResumeLayout(false);
            this.mainGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button close_bt;
        private System.Windows.Forms.DataGridView result_DGV;
        private System.Windows.Forms.GroupBox CatGroup;
        private System.Windows.Forms.GroupBox ParamGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cat_9_ComBox;
        private System.Windows.Forms.ComboBox cat_8_ComBox;
        private System.Windows.Forms.ComboBox cat_7_ComBox;
        private System.Windows.Forms.ComboBox cat_6_ComBox;
        private System.Windows.Forms.ComboBox cat_5_ComBox;
        private System.Windows.Forms.ComboBox cat_4_ComBox;
        private System.Windows.Forms.ComboBox cat_3_ComBox;
        private System.Windows.Forms.ComboBox cat_2_ComBox;
        private System.Windows.Forms.Label relLabel;
        private System.Windows.Forms.ComboBox relationship_ComBox;
        private System.Windows.Forms.TextBox ruleValue_5_tb;
        private System.Windows.Forms.ComboBox rule_5_ComBox;
        private System.Windows.Forms.TextBox ruleValue_4_tb;
        private System.Windows.Forms.ComboBox rule_4_ComBox;
        private System.Windows.Forms.TextBox ruleValue_3_tb;
        private System.Windows.Forms.ComboBox rule_3_ComBox;
        private System.Windows.Forms.TextBox ruleValue_2_tb;
        private System.Windows.Forms.ComboBox rule_2_ComBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ruleValue_1_tb;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox rule_1_ComBox;
        private System.Windows.Forms.ComboBox param_5_ComBox;
        private System.Windows.Forms.ComboBox param_1_ComBox;
        private System.Windows.Forms.ComboBox param_2_ComBox;
        private System.Windows.Forms.ComboBox param_3_ComBox;
        private System.Windows.Forms.ComboBox param_4_ComBox;
        private System.Windows.Forms.Label Результат;
        private System.Windows.Forms.Button result_bt;
        private System.Windows.Forms.Button addToResult_bt;
        private System.Windows.Forms.Button clearAll_bt;
        private System.Windows.Forms.Button saveDGV_bt;
        private System.Windows.Forms.Button loadDGV_bt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button action_bt;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button delete_bt;
        private System.Windows.Forms.Button add_bt;
        private System.Windows.Forms.GroupBox mainGroup;
        private System.Windows.Forms.RadioButton rule_rb;
        private System.Windows.Forms.RadioButton cat_rb;
        private System.Windows.Forms.ComboBox action_ComBox;
        private System.Windows.Forms.ProgressBar progress_pb;
        private System.Windows.Forms.SaveFileDialog getPath_sfd;
        private System.Windows.Forms.OpenFileDialog openFile_ofd;
        private System.Windows.Forms.Label timeLabel;
    }
}