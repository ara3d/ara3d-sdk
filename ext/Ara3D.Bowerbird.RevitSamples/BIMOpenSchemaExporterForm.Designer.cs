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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            exportDirTextBox = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            chooseFolderButton = new System.Windows.Forms.Button();
            buttonExport = new System.Windows.Forms.Button();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            checkBoxIncludeLinks = new System.Windows.Forms.CheckBox();
            checkBoxMeshGeometry = new System.Windows.Forms.CheckBox();
            buttonLanchAra3D = new System.Windows.Forms.Button();
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            comboBoxLod = new System.Windows.Forms.ComboBox();
            buttonSettings = new System.Windows.Forms.Button();
            buttonHelp = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // exportDirTextBox
            // 
            exportDirTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            exportDirTextBox.Location = new System.Drawing.Point(11, 33);
            exportDirTextBox.Margin = new System.Windows.Forms.Padding(2);
            exportDirTextBox.Name = "exportDirTextBox";
            exportDirTextBox.Size = new System.Drawing.Size(494, 23);
            exportDirTextBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 10F);
            label1.Location = new System.Drawing.Point(8, 9);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(108, 19);
            label1.TabIndex = 2;
            label1.Text = "Export Directory";
            // 
            // chooseFolderButton
            // 
            chooseFolderButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            chooseFolderButton.Location = new System.Drawing.Point(509, 33);
            chooseFolderButton.Margin = new System.Windows.Forms.Padding(2);
            chooseFolderButton.Name = "chooseFolderButton";
            chooseFolderButton.Size = new System.Drawing.Size(150, 28);
            chooseFolderButton.TabIndex = 3;
            chooseFolderButton.Text = "Choose folder ...";
            chooseFolderButton.UseVisualStyleBackColor = true;
            chooseFolderButton.Click += chooseFolderButton_Click;
            // 
            // buttonExport
            // 
            buttonExport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonExport.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            buttonExport.Location = new System.Drawing.Point(23, 131);
            buttonExport.Name = "buttonExport";
            buttonExport.Size = new System.Drawing.Size(610, 33);
            buttonExport.TabIndex = 8;
            buttonExport.Text = "Run Export";
            buttonExport.UseVisualStyleBackColor = true;
            buttonExport.Click += buttonExport_Click;
            // 
            // checkBoxIncludeLinks
            // 
            checkBoxIncludeLinks.AutoSize = true;
            checkBoxIncludeLinks.Checked = true;
            checkBoxIncludeLinks.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxIncludeLinks.Location = new System.Drawing.Point(23, 63);
            checkBoxIncludeLinks.Margin = new System.Windows.Forms.Padding(2);
            checkBoxIncludeLinks.Name = "checkBoxIncludeLinks";
            checkBoxIncludeLinks.Size = new System.Drawing.Size(163, 19);
            checkBoxIncludeLinks.TabIndex = 9;
            checkBoxIncludeLinks.Text = "Include linked documents";
            checkBoxIncludeLinks.UseVisualStyleBackColor = true;
            // 
            // checkBoxMeshGeometry
            // 
            checkBoxMeshGeometry.AutoSize = true;
            checkBoxMeshGeometry.Checked = true;
            checkBoxMeshGeometry.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxMeshGeometry.Location = new System.Drawing.Point(23, 84);
            checkBoxMeshGeometry.Margin = new System.Windows.Forms.Padding(2);
            checkBoxMeshGeometry.Name = "checkBoxMeshGeometry";
            checkBoxMeshGeometry.Size = new System.Drawing.Size(119, 19);
            checkBoxMeshGeometry.TabIndex = 11;
            checkBoxMeshGeometry.Text = "Include geometry";
            checkBoxMeshGeometry.UseVisualStyleBackColor = true;
            // 
            // buttonLanchAra3D
            // 
            buttonLanchAra3D.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            buttonLanchAra3D.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            buttonLanchAra3D.Location = new System.Drawing.Point(23, 489);
            buttonLanchAra3D.Name = "buttonLanchAra3D";
            buttonLanchAra3D.Size = new System.Drawing.Size(610, 26);
            buttonLanchAra3D.TabIndex = 12;
            buttonLanchAra3D.Text = "Launch Ara 3D Studio ...";
            buttonLanchAra3D.UseVisualStyleBackColor = true;
            buttonLanchAra3D.Click += buttonLaunchAra3D_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richTextBox1.Location = new System.Drawing.Point(8, 185);
            richTextBox1.Margin = new System.Windows.Forms.Padding(2);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(655, 283);
            richTextBox1.TabIndex = 13;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            // 
            // comboBoxLod
            // 
            comboBoxLod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBoxLod.FormattingEnabled = true;
            comboBoxLod.Items.AddRange(new object[] { "Coarse Detail", "Medium Detail", "Fine Detail" });
            comboBoxLod.Location = new System.Drawing.Point(151, 84);
            comboBoxLod.Margin = new System.Windows.Forms.Padding(2);
            comboBoxLod.Name = "comboBoxLod";
            comboBoxLod.Size = new System.Drawing.Size(133, 23);
            comboBoxLod.TabIndex = 16;
            // 
            // buttonSettings
            // 
            buttonSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonSettings.Location = new System.Drawing.Point(355, 66);
            buttonSettings.Margin = new System.Windows.Forms.Padding(2);
            buttonSettings.Name = "buttonSettings";
            buttonSettings.Size = new System.Drawing.Size(150, 28);
            buttonSettings.TabIndex = 17;
            buttonSettings.Text = "Edit Settings ...";
            buttonSettings.UseVisualStyleBackColor = true;
            // 
            // buttonHelp
            // 
            buttonHelp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonHelp.Location = new System.Drawing.Point(509, 65);
            buttonHelp.Margin = new System.Windows.Forms.Padding(2);
            buttonHelp.Name = "buttonHelp";
            buttonHelp.Size = new System.Drawing.Size(150, 28);
            buttonHelp.TabIndex = 18;
            buttonHelp.Text = "Help ...";
            buttonHelp.UseVisualStyleBackColor = true;
            buttonHelp.Click += buttonHelp_Click;
            // 
            // BIMOpenSchemaExporterForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(676, 527);
            Controls.Add(buttonHelp);
            Controls.Add(buttonSettings);
            Controls.Add(comboBoxLod);
            Controls.Add(richTextBox1);
            Controls.Add(buttonLanchAra3D);
            Controls.Add(checkBoxMeshGeometry);
            Controls.Add(checkBoxIncludeLinks);
            Controls.Add(buttonExport);
            Controls.Add(chooseFolderButton);
            Controls.Add(label1);
            Controls.Add(exportDirTextBox);
            Margin = new System.Windows.Forms.Padding(2);
            MinimumSize = new System.Drawing.Size(346, 204);
            Name = "BIMOpenSchemaExporterForm";
            Text = "BIM Open Schema Exporter";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox exportDirTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button chooseFolderButton;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox checkBoxIncludeLinks;
        private System.Windows.Forms.CheckBox checkBoxMeshGeometry;
        private System.Windows.Forms.Button buttonLanchAra3D;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox comboBoxLod;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.Button buttonHelp;
    }
}