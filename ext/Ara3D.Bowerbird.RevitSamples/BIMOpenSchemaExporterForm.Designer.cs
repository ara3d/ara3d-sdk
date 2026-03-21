namespace Ara3D.BIMOpenSchema.Revit2025
{
    partial class BIMOpenSchemaExporterForm
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
        /// Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BIMOpenSchemaExporterForm));
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            groupBoxLog = new System.Windows.Forms.GroupBox();
            richTextBoxLog = new System.Windows.Forms.RichTextBox();
            groupBoxExport = new System.Windows.Forms.GroupBox();
            buttonRunExport = new System.Windows.Forms.Button();
            labelExportInstructions = new System.Windows.Forms.Label();
            groupBoxSettings = new System.Windows.Forms.GroupBox();
            buttonAdvancedSettings = new System.Windows.Forms.Button();
            checkBoxIncludeGeometry = new System.Windows.Forms.CheckBox();
            checkBoxIncludeLinks = new System.Windows.Forms.CheckBox();
            buttonBrowse = new System.Windows.Forms.Button();
            textBoxOutputFolder = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label1 = new System.Windows.Forms.Label();
            splitContainer2 = new System.Windows.Forms.SplitContainer();
            groupBoxAbout = new System.Windows.Forms.GroupBox();
            buttonMoreInfo = new System.Windows.Forms.Button();
            labelAbout = new System.Windows.Forms.Label();
            groupBoxPostExport = new System.Windows.Forms.GroupBox();
            tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            buttonLaunch = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            groupBoxLog.SuspendLayout();
            groupBoxExport.SuspendLayout();
            groupBoxSettings.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            groupBoxAbout.SuspendLayout();
            groupBoxPostExport.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // folderBrowserDialog1
            // 
            folderBrowserDialog1.Description = "Choose the folder where the BIM Open Schema export will be created.";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new System.Drawing.Size(768, 546);
            splitContainer1.SplitterDistance = 489;
            splitContainer1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(groupBoxLog, 0, 3);
            tableLayoutPanel1.Controls.Add(groupBoxExport, 0, 2);
            tableLayoutPanel1.Controls.Add(groupBoxSettings, 0, 1);
            tableLayoutPanel1.Controls.Add(groupBox1, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.ImeMode = System.Windows.Forms.ImeMode.AlphaFull;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 176F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 163F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            tableLayoutPanel1.Size = new System.Drawing.Size(489, 546);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBoxLog
            // 
            groupBoxLog.Controls.Add(richTextBoxLog);
            groupBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBoxLog.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBoxLog.Location = new System.Drawing.Point(3, 427);
            groupBoxLog.Name = "groupBoxLog";
            groupBoxLog.Padding = new System.Windows.Forms.Padding(8);
            groupBoxLog.Size = new System.Drawing.Size(483, 116);
            groupBoxLog.TabIndex = 5;
            groupBoxLog.TabStop = false;
            groupBoxLog.Text = "Log";
            // 
            // richTextBoxLog
            // 
            richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            richTextBoxLog.Font = new System.Drawing.Font("Segoe UI", 9F);
            richTextBoxLog.Location = new System.Drawing.Point(8, 30);
            richTextBoxLog.Name = "richTextBoxLog";
            richTextBoxLog.Size = new System.Drawing.Size(467, 78);
            richTextBoxLog.TabIndex = 2;
            richTextBoxLog.Text = "";
            // 
            // groupBoxExport
            // 
            groupBoxExport.Controls.Add(buttonRunExport);
            groupBoxExport.Controls.Add(labelExportInstructions);
            groupBoxExport.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBoxExport.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBoxExport.Location = new System.Drawing.Point(3, 264);
            groupBoxExport.Name = "groupBoxExport";
            groupBoxExport.Size = new System.Drawing.Size(483, 157);
            groupBoxExport.TabIndex = 3;
            groupBoxExport.TabStop = false;
            groupBoxExport.Text = "Export";
            // 
            // buttonRunExport
            // 
            buttonRunExport.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            buttonRunExport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonRunExport.AutoEllipsis = true;
            buttonRunExport.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            buttonRunExport.Location = new System.Drawing.Point(3, 106);
            buttonRunExport.Name = "buttonRunExport";
            buttonRunExport.Size = new System.Drawing.Size(472, 41);
            buttonRunExport.TabIndex = 5;
            buttonRunExport.Text = "Run Export";
            buttonRunExport.UseVisualStyleBackColor = true;
            buttonRunExport.Click += buttonRunExport_Click;
            // 
            // labelExportInstructions
            // 
            labelExportInstructions.Dock = System.Windows.Forms.DockStyle.Fill;
            labelExportInstructions.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            labelExportInstructions.Location = new System.Drawing.Point(3, 25);
            labelExportInstructions.Name = "labelExportInstructions";
            labelExportInstructions.Padding = new System.Windows.Forms.Padding(5);
            labelExportInstructions.Size = new System.Drawing.Size(477, 129);
            labelExportInstructions.TabIndex = 0;
            labelExportInstructions.Text = resources.GetString("labelExportInstructions.Text");
            // 
            // groupBoxSettings
            // 
            groupBoxSettings.Controls.Add(buttonAdvancedSettings);
            groupBoxSettings.Controls.Add(checkBoxIncludeGeometry);
            groupBoxSettings.Controls.Add(checkBoxIncludeLinks);
            groupBoxSettings.Controls.Add(buttonBrowse);
            groupBoxSettings.Controls.Add(textBoxOutputFolder);
            groupBoxSettings.Controls.Add(label2);
            groupBoxSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBoxSettings.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBoxSettings.Location = new System.Drawing.Point(3, 88);
            groupBoxSettings.Name = "groupBoxSettings";
            groupBoxSettings.Size = new System.Drawing.Size(483, 170);
            groupBoxSettings.TabIndex = 2;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "Settings";
            // 
            // buttonAdvancedSettings
            // 
            buttonAdvancedSettings.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            buttonAdvancedSettings.AutoEllipsis = true;
            buttonAdvancedSettings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            buttonAdvancedSettings.Location = new System.Drawing.Point(9, 136);
            buttonAdvancedSettings.Name = "buttonAdvancedSettings";
            buttonAdvancedSettings.Size = new System.Drawing.Size(155, 23);
            buttonAdvancedSettings.TabIndex = 5;
            buttonAdvancedSettings.Text = "Advanced Settings ...";
            buttonAdvancedSettings.UseVisualStyleBackColor = true;
            buttonAdvancedSettings.Click += buttonAdvancedSettings_Click;
            // 
            // checkBoxIncludeGeometry
            // 
            checkBoxIncludeGeometry.AutoSize = true;
            checkBoxIncludeGeometry.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            checkBoxIncludeGeometry.Location = new System.Drawing.Point(9, 111);
            checkBoxIncludeGeometry.Name = "checkBoxIncludeGeometry";
            checkBoxIncludeGeometry.Size = new System.Drawing.Size(137, 19);
            checkBoxIncludeGeometry.TabIndex = 4;
            checkBoxIncludeGeometry.Text = "Include 3D Geometry";
            checkBoxIncludeGeometry.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncludeLinks
            // 
            checkBoxIncludeLinks.AutoSize = true;
            checkBoxIncludeLinks.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            checkBoxIncludeLinks.Location = new System.Drawing.Point(9, 87);
            checkBoxIncludeLinks.Name = "checkBoxIncludeLinks";
            checkBoxIncludeLinks.Size = new System.Drawing.Size(95, 19);
            checkBoxIncludeLinks.TabIndex = 3;
            checkBoxIncludeLinks.Text = "Include Links";
            checkBoxIncludeLinks.UseVisualStyleBackColor = true;
            // 
            // buttonBrowse
            // 
            buttonBrowse.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            buttonBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonBrowse.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            buttonBrowse.Location = new System.Drawing.Point(402, 87);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new System.Drawing.Size(75, 29);
            buttonBrowse.TabIndex = 2;
            buttonBrowse.Text = "Browse ...";
            buttonBrowse.UseVisualStyleBackColor = true;
            buttonBrowse.Click += buttonBrowse_Click;
            // 
            // textBoxOutputFolder
            // 
            textBoxOutputFolder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxOutputFolder.Font = new System.Drawing.Font("Segoe UI", 9F);
            textBoxOutputFolder.Location = new System.Drawing.Point(6, 52);
            textBoxOutputFolder.Name = "textBoxOutputFolder";
            textBoxOutputFolder.Size = new System.Drawing.Size(469, 23);
            textBoxOutputFolder.TabIndex = 1;
            // 
            // label2
            // 
            label2.Dock = System.Windows.Forms.DockStyle.Fill;
            label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(3, 25);
            label2.Name = "label2";
            label2.Padding = new System.Windows.Forms.Padding(5);
            label2.Size = new System.Drawing.Size(477, 142);
            label2.TabIndex = 0;
            label2.Text = "Output Folder Location";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBox1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(483, 79);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = ".BOS Exporter";
            // 
            // label1
            // 
            label1.Dock = System.Windows.Forms.DockStyle.Fill;
            label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(3, 25);
            label1.Name = "label1";
            label1.Padding = new System.Windows.Forms.Padding(5);
            label1.Size = new System.Drawing.Size(477, 51);
            label1.TabIndex = 0;
            label1.Text = "Export Revit models to a high-performance .BOS package\r\nfor fast viewing, querying, analysis, and transformation.";
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            splitContainer2.Location = new System.Drawing.Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(groupBoxAbout);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(groupBoxPostExport);
            splitContainer2.Size = new System.Drawing.Size(275, 546);
            splitContainer2.SplitterDistance = 431;
            splitContainer2.TabIndex = 0;
            // 
            // groupBoxAbout
            // 
            groupBoxAbout.Controls.Add(buttonMoreInfo);
            groupBoxAbout.Controls.Add(labelAbout);
            groupBoxAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBoxAbout.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBoxAbout.Location = new System.Drawing.Point(0, 0);
            groupBoxAbout.Name = "groupBoxAbout";
            groupBoxAbout.Size = new System.Drawing.Size(275, 431);
            groupBoxAbout.TabIndex = 7;
            groupBoxAbout.TabStop = false;
            groupBoxAbout.Text = "About BIM Open Schema";
            // 
            // buttonMoreInfo
            // 
            buttonMoreInfo.DialogResult = System.Windows.Forms.DialogResult.Ignore;
            buttonMoreInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            buttonMoreInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            buttonMoreInfo.Location = new System.Drawing.Point(3, 403);
            buttonMoreInfo.Name = "buttonMoreInfo";
            buttonMoreInfo.Size = new System.Drawing.Size(269, 25);
            buttonMoreInfo.TabIndex = 2;
            buttonMoreInfo.Text = "More info";
            buttonMoreInfo.UseVisualStyleBackColor = true;
            buttonMoreInfo.Click += buttonMoreInfo_Click;
            // 
            // labelAbout
            // 
            labelAbout.Dock = System.Windows.Forms.DockStyle.Fill;
            labelAbout.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            labelAbout.Location = new System.Drawing.Point(3, 25);
            labelAbout.Name = "labelAbout";
            labelAbout.Padding = new System.Windows.Forms.Padding(5);
            labelAbout.Size = new System.Drawing.Size(269, 403);
            labelAbout.TabIndex = 0;
            labelAbout.Text = resources.GetString("labelAbout.Text");
            // 
            // groupBoxPostExport
            // 
            groupBoxPostExport.Controls.Add(tableLayoutPanel2);
            groupBoxPostExport.Dock = System.Windows.Forms.DockStyle.Fill;
            groupBoxPostExport.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            groupBoxPostExport.Location = new System.Drawing.Point(0, 0);
            groupBoxPostExport.Name = "groupBoxPostExport";
            groupBoxPostExport.Size = new System.Drawing.Size(275, 111);
            groupBoxPostExport.TabIndex = 9;
            groupBoxPostExport.TabStop = false;
            groupBoxPostExport.Text = "Post Export";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 1;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(buttonLaunch, 0, 1);
            tableLayoutPanel2.Controls.Add(button1, 0, 0);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(3, 25);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new System.Drawing.Size(269, 83);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // buttonLaunch
            // 
            buttonLaunch.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            buttonLaunch.AutoEllipsis = true;
            buttonLaunch.Dock = System.Windows.Forms.DockStyle.Fill;
            buttonLaunch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            buttonLaunch.Location = new System.Drawing.Point(3, 44);
            buttonLaunch.Name = "buttonLaunch";
            buttonLaunch.Size = new System.Drawing.Size(263, 36);
            buttonLaunch.TabIndex = 9;
            buttonLaunch.Text = "Launch Ara 3D Studio";
            buttonLaunch.UseVisualStyleBackColor = true;
            buttonLaunch.Click += buttonLaunch_Click_1;
            // 
            // button1
            // 
            button1.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuBar;
            button1.AutoEllipsis = true;
            button1.Dock = System.Windows.Forms.DockStyle.Fill;
            button1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            button1.Location = new System.Drawing.Point(3, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(263, 35);
            button1.TabIndex = 8;
            button1.Text = "Open in file explorer";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // BIMOpenSchemaExporterForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(768, 546);
            Controls.Add(splitContainer1);
            MinimumSize = new System.Drawing.Size(298, 293);
            Name = "BIMOpenSchemaExporterForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "BIM Open Schema Exporter";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            groupBoxLog.ResumeLayout(false);
            groupBoxExport.ResumeLayout(false);
            groupBoxSettings.ResumeLayout(false);
            groupBoxSettings.PerformLayout();
            groupBox1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            groupBoxAbout.ResumeLayout(false);
            groupBoxPostExport.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBoxExport;
        private System.Windows.Forms.Button buttonRunExport;
        private System.Windows.Forms.Label labelExportInstructions;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.Button buttonAdvancedSettings;
        private System.Windows.Forms.CheckBox checkBoxIncludeGeometry;
        private System.Windows.Forms.CheckBox checkBoxIncludeLinks;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxOutputFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBoxAbout;
        private System.Windows.Forms.Button buttonMoreInfo;
        private System.Windows.Forms.Label labelAbout;
        private System.Windows.Forms.GroupBox groupBoxPostExport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button buttonLaunch;
        private System.Windows.Forms.Button button1;
    }
}