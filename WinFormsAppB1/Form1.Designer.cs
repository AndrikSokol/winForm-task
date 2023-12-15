namespace WinFormsAppB1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonGenerateFiles = new Button();
            labelStatus = new Label();
            buttonConcatFiles = new Button();
            textBoxFilter = new TextBox();
            labelFilter = new Label();
            labelConcatFiles = new Label();
            buttonAddInExcel = new Button();
            progressBarExcel = new ProgressBar();
            labelLoader = new Label();
            labelCountOfAddedInExcel = new Label();
            labelRemainingRows = new Label();
            labelCountOfAddedInExcelValue = new Label();
            labelRemainingRowsValue = new Label();
            buttonImportDataInSQL = new Button();
            label2 = new Label();
            textBoxCountRowsForAddInExcel = new TextBox();
            buttonCalculateSumOfInt = new Button();
            labelSumInt = new Label();
            labelAvgFloat = new Label();
            buttonCalculateAvgOfFloat = new Button();
            panel1 = new Panel();
            label1 = new Label();
            buttonLoadOSV = new Button();
            dataGridViewFilesName = new DataGridView();
            ColumnFileName = new DataGridViewTextBoxColumn();
            labelOSVLoad = new Label();
            dataGridViewOSV = new DataGridView();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewFilesName).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOSV).BeginInit();
            SuspendLayout();
            // 
            // buttonGenerateFiles
            // 
            buttonGenerateFiles.FlatStyle = FlatStyle.System;
            buttonGenerateFiles.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            buttonGenerateFiles.Location = new Point(12, 20);
            buttonGenerateFiles.Name = "buttonGenerateFiles";
            buttonGenerateFiles.Size = new Size(111, 36);
            buttonGenerateFiles.TabIndex = 0;
            buttonGenerateFiles.Text = "Generate 100 files";
            buttonGenerateFiles.UseVisualStyleBackColor = true;
            buttonGenerateFiles.Click += buttonGenerateFiles_Click;
            // 
            // labelStatus
            // 
            labelStatus.AutoSize = true;
            labelStatus.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelStatus.Location = new Point(144, 28);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(70, 20);
            labelStatus.TabIndex = 1;
            labelStatus.Text = "labelStatus";
            // 
            // buttonConcatFiles
            // 
            buttonConcatFiles.FlatStyle = FlatStyle.System;
            buttonConcatFiles.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            buttonConcatFiles.Location = new Point(188, 86);
            buttonConcatFiles.Name = "buttonConcatFiles";
            buttonConcatFiles.Size = new Size(111, 36);
            buttonConcatFiles.TabIndex = 2;
            buttonConcatFiles.Text = "Concat 100 files";
            buttonConcatFiles.UseVisualStyleBackColor = true;
            buttonConcatFiles.Click += buttonConcatFiles_Click;
            // 
            // textBoxFilter
            // 
            textBoxFilter.Location = new Point(54, 94);
            textBoxFilter.Name = "textBoxFilter";
            textBoxFilter.Size = new Size(111, 23);
            textBoxFilter.TabIndex = 3;
            // 
            // labelFilter
            // 
            labelFilter.AutoSize = true;
            labelFilter.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelFilter.Location = new Point(12, 97);
            labelFilter.Name = "labelFilter";
            labelFilter.Size = new Size(40, 20);
            labelFilter.TabIndex = 4;
            labelFilter.Text = "Filter ";
            // 
            // labelConcatFiles
            // 
            labelConcatFiles.AutoSize = true;
            labelConcatFiles.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelConcatFiles.Location = new Point(38, 125);
            labelConcatFiles.Name = "labelConcatFiles";
            labelConcatFiles.Size = new Size(155, 20);
            labelConcatFiles.TabIndex = 5;
            labelConcatFiles.Text = "Message after concatFiles";
            // 
            // buttonAddInExcel
            // 
            buttonAddInExcel.FlatStyle = FlatStyle.System;
            buttonAddInExcel.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            buttonAddInExcel.Location = new Point(8, 41);
            buttonAddInExcel.Name = "buttonAddInExcel";
            buttonAddInExcel.Size = new Size(111, 36);
            buttonAddInExcel.TabIndex = 6;
            buttonAddInExcel.Text = "Add in Excel";
            buttonAddInExcel.UseVisualStyleBackColor = true;
            buttonAddInExcel.Click += buttonAddInExcel_Click;
            // 
            // progressBarExcel
            // 
            progressBarExcel.Location = new Point(68, 143);
            progressBarExcel.Name = "progressBarExcel";
            progressBarExcel.Size = new Size(215, 23);
            progressBarExcel.TabIndex = 7;
            // 
            // labelLoader
            // 
            labelLoader.AutoSize = true;
            labelLoader.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelLoader.Location = new Point(8, 143);
            labelLoader.Name = "labelLoader";
            labelLoader.Size = new Size(54, 20);
            labelLoader.TabIndex = 8;
            labelLoader.Text = "Loading";
            // 
            // labelCountOfAddedInExcel
            // 
            labelCountOfAddedInExcel.AutoSize = true;
            labelCountOfAddedInExcel.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelCountOfAddedInExcel.Location = new Point(8, 89);
            labelCountOfAddedInExcel.Name = "labelCountOfAddedInExcel";
            labelCountOfAddedInExcel.Size = new Size(93, 20);
            labelCountOfAddedInExcel.TabIndex = 9;
            labelCountOfAddedInExcel.Text = "Count of added";
            // 
            // labelRemainingRows
            // 
            labelRemainingRows.AutoSize = true;
            labelRemainingRows.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelRemainingRows.Location = new Point(8, 112);
            labelRemainingRows.Name = "labelRemainingRows";
            labelRemainingRows.Size = new Size(179, 20);
            labelRemainingRows.TabIndex = 10;
            labelRemainingRows.Text = "The remaining number of rows";
            // 
            // labelCountOfAddedInExcelValue
            // 
            labelCountOfAddedInExcelValue.AutoSize = true;
            labelCountOfAddedInExcelValue.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelCountOfAddedInExcelValue.Location = new Point(208, 89);
            labelCountOfAddedInExcelValue.Name = "labelCountOfAddedInExcelValue";
            labelCountOfAddedInExcelValue.Size = new Size(16, 20);
            labelCountOfAddedInExcelValue.TabIndex = 11;
            labelCountOfAddedInExcelValue.Text = "0";
            // 
            // labelRemainingRowsValue
            // 
            labelRemainingRowsValue.AutoSize = true;
            labelRemainingRowsValue.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelRemainingRowsValue.Location = new Point(208, 112);
            labelRemainingRowsValue.Name = "labelRemainingRowsValue";
            labelRemainingRowsValue.Size = new Size(16, 20);
            labelRemainingRowsValue.TabIndex = 12;
            labelRemainingRowsValue.Text = "0";
            // 
            // buttonImportDataInSQL
            // 
            buttonImportDataInSQL.FlatStyle = FlatStyle.System;
            buttonImportDataInSQL.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            buttonImportDataInSQL.Location = new Point(125, 41);
            buttonImportDataInSQL.Name = "buttonImportDataInSQL";
            buttonImportDataInSQL.Size = new Size(171, 35);
            buttonImportDataInSQL.TabIndex = 13;
            buttonImportDataInSQL.Text = "Import Data in SQLSERVER";
            buttonImportDataInSQL.UseVisualStyleBackColor = true;
            buttonImportDataInSQL.Click += buttonImportDataInSQL_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(8, 15);
            label2.Name = "label2";
            label2.Size = new Size(108, 20);
            label2.TabIndex = 15;
            label2.Text = "Count rows to add";
            // 
            // textBoxCountRowsForAddInExcel
            // 
            textBoxCountRowsForAddInExcel.Location = new Point(121, 12);
            textBoxCountRowsForAddInExcel.Name = "textBoxCountRowsForAddInExcel";
            textBoxCountRowsForAddInExcel.Size = new Size(100, 23);
            textBoxCountRowsForAddInExcel.TabIndex = 16;
            // 
            // buttonCalculateSumOfInt
            // 
            buttonCalculateSumOfInt.FlatStyle = FlatStyle.System;
            buttonCalculateSumOfInt.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            buttonCalculateSumOfInt.Location = new Point(33, 207);
            buttonCalculateSumOfInt.Name = "buttonCalculateSumOfInt";
            buttonCalculateSumOfInt.Size = new Size(112, 36);
            buttonCalculateSumOfInt.TabIndex = 17;
            buttonCalculateSumOfInt.Text = "Get Sum of Int";
            buttonCalculateSumOfInt.UseVisualStyleBackColor = true;
            buttonCalculateSumOfInt.Click += buttonCalculateSumOfInt_Click;
            // 
            // labelSumInt
            // 
            labelSumInt.AutoSize = true;
            labelSumInt.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelSumInt.Location = new Point(161, 215);
            labelSumInt.Name = "labelSumInt";
            labelSumInt.Size = new Size(75, 20);
            labelSumInt.TabIndex = 18;
            labelSumInt.Text = "labelSumInt";
            // 
            // labelAvgFloat
            // 
            labelAvgFloat.AutoSize = true;
            labelAvgFloat.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelAvgFloat.Location = new Point(161, 274);
            labelAvgFloat.Name = "labelAvgFloat";
            labelAvgFloat.Size = new Size(83, 20);
            labelAvgFloat.TabIndex = 19;
            labelAvgFloat.Text = "labelAvgFloat";
            // 
            // buttonCalculateAvgOfFloat
            // 
            buttonCalculateAvgOfFloat.FlatStyle = FlatStyle.System;
            buttonCalculateAvgOfFloat.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            buttonCalculateAvgOfFloat.Location = new Point(34, 266);
            buttonCalculateAvgOfFloat.Name = "buttonCalculateAvgOfFloat";
            buttonCalculateAvgOfFloat.Size = new Size(112, 36);
            buttonCalculateAvgOfFloat.TabIndex = 20;
            buttonCalculateAvgOfFloat.Text = "Get Avg Of Float";
            buttonCalculateAvgOfFloat.UseVisualStyleBackColor = true;
            buttonCalculateAvgOfFloat.Click += buttonCalculateAvgOfFloat_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(label1);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(buttonCalculateAvgOfFloat);
            panel1.Controls.Add(labelAvgFloat);
            panel1.Controls.Add(textBoxCountRowsForAddInExcel);
            panel1.Controls.Add(labelSumInt);
            panel1.Controls.Add(buttonAddInExcel);
            panel1.Controls.Add(buttonCalculateSumOfInt);
            panel1.Controls.Add(buttonImportDataInSQL);
            panel1.Controls.Add(labelRemainingRows);
            panel1.Controls.Add(labelLoader);
            panel1.Controls.Add(progressBarExcel);
            panel1.Controls.Add(labelRemainingRowsValue);
            panel1.Controls.Add(labelCountOfAddedInExcel);
            panel1.Controls.Add(labelCountOfAddedInExcelValue);
            panel1.Location = new Point(549, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(303, 318);
            panel1.TabIndex = 21;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Arial Narrow", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(121, 184);
            label1.Name = "label1";
            label1.Size = new Size(92, 20);
            label1.TabIndex = 21;
            label1.Text = "SQL Operation";
            // 
            // buttonLoadOSV
            // 
            buttonLoadOSV.Font = new Font("Times New Roman", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            buttonLoadOSV.Location = new Point(12, 148);
            buttonLoadOSV.Name = "buttonLoadOSV";
            buttonLoadOSV.Size = new Size(111, 36);
            buttonLoadOSV.TabIndex = 22;
            buttonLoadOSV.Text = "Load OSV";
            buttonLoadOSV.UseVisualStyleBackColor = true;
            buttonLoadOSV.Click += buttonLoadOSV_Click;
            // 
            // dataGridViewFilesName
            // 
            dataGridViewFilesName.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewFilesName.Columns.AddRange(new DataGridViewColumn[] { ColumnFileName });
            dataGridViewFilesName.Location = new Point(12, 190);
            dataGridViewFilesName.Name = "dataGridViewFilesName";
            dataGridViewFilesName.RowTemplate.Height = 25;
            dataGridViewFilesName.Size = new Size(531, 140);
            dataGridViewFilesName.TabIndex = 23;
            dataGridViewFilesName.CellMouseClick += dataGridViewFilesName_CellMouseClick;
            // 
            // ColumnFileName
            // 
            ColumnFileName.HeaderText = "File";
            ColumnFileName.Name = "ColumnFileName";
            ColumnFileName.ReadOnly = true;
            ColumnFileName.Width = 480;
            // 
            // labelOSVLoad
            // 
            labelOSVLoad.AutoSize = true;
            labelOSVLoad.Font = new Font("Times New Roman", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            labelOSVLoad.Location = new Point(144, 158);
            labelOSVLoad.Name = "labelOSVLoad";
            labelOSVLoad.Size = new Size(106, 17);
            labelOSVLoad.TabIndex = 24;
            labelOSVLoad.Text = "Load Osv Status";
            // 
            // dataGridViewOSV
            // 
            dataGridViewOSV.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewOSV.Location = new Point(10, 338);
            dataGridViewOSV.Name = "dataGridViewOSV";
            dataGridViewOSV.RowTemplate.Height = 25;
            dataGridViewOSV.Size = new Size(842, 185);
            dataGridViewOSV.TabIndex = 25;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.MenuBar;
            ClientSize = new Size(864, 535);
            Controls.Add(dataGridViewOSV);
            Controls.Add(labelOSVLoad);
            Controls.Add(dataGridViewFilesName);
            Controls.Add(buttonLoadOSV);
            Controls.Add(panel1);
            Controls.Add(labelConcatFiles);
            Controls.Add(labelFilter);
            Controls.Add(textBoxFilter);
            Controls.Add(buttonConcatFiles);
            Controls.Add(labelStatus);
            Controls.Add(buttonGenerateFiles);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Form1";
            Text = "B1 Task";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewFilesName).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridViewOSV).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonGenerateFiles;
        private Label labelStatus;
        private Button buttonConcatFiles;
        private TextBox textBoxFilter;
        private Label labelFilter;
        private Label labelConcatFiles;
        private Button buttonAddInExcel;
        private ProgressBar progressBarExcel;
        private Label labelLoader;
        private Label labelCountOfAddedInExcel;
        private Label labelRemainingRows;
        private Label labelCountOfAddedInExcelValue;
        private Label labelRemainingRowsValue;
        private Button buttonImportDataInSQL;
        private Label label2;
        private TextBox textBoxCountRowsForAddInExcel;
        private Button buttonCalculateSumOfInt;
        private Label labelSumInt;
        private Label labelAvgFloat;
        private Button buttonCalculateAvgOfFloat;
        private Panel panel1;
        private Label label1;
        private Button buttonLoadOSV;
        private DataGridView dataGridViewFilesName;
        private Label labelOSVLoad;
        private DataGridView dataGridViewOSV;
        private DataGridViewTextBoxColumn ColumnFileName;
    }
}