namespace LogAnalyzer.UI.WinForms {
    partial class AnalysisResultsForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.resultsTextbox = new System.Windows.Forms.RichTextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.analyzersList = new System.Windows.Forms.CheckedListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.logFilesList = new System.Windows.Forms.CheckedListBox();
            this.wordWrapCheckbox = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.formCaptionTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.setFormCaptionButton = new System.Windows.Forms.Button();
            this.logFileListContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openContainingFolderCommand = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.logFileListContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // resultsTextbox
            // 
            this.resultsTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsTextbox.Location = new System.Drawing.Point(0, 0);
            this.resultsTextbox.Name = "resultsTextbox";
            this.resultsTextbox.ReadOnly = true;
            this.resultsTextbox.Size = new System.Drawing.Size(776, 233);
            this.resultsTextbox.TabIndex = 0;
            this.resultsTextbox.Text = "";
            this.resultsTextbox.WordWrap = false;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(686, 413);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(102, 30);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // analyzersList
            // 
            this.analyzersList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analyzersList.FormattingEnabled = true;
            this.analyzersList.Location = new System.Drawing.Point(3, 3);
            this.analyzersList.Name = "analyzersList";
            this.analyzersList.Size = new System.Drawing.Size(762, 126);
            this.analyzersList.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.resultsTextbox);
            this.splitContainer1.Size = new System.Drawing.Size(776, 395);
            this.splitContainer1.SplitterDistance = 158;
            this.splitContainer1.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 158);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.analyzersList);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(768, 132);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Analyzers";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.logFilesList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(768, 132);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Log files";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // logFilesList
            // 
            this.logFilesList.ContextMenuStrip = this.logFileListContextMenu;
            this.logFilesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logFilesList.FormattingEnabled = true;
            this.logFilesList.Location = new System.Drawing.Point(3, 3);
            this.logFilesList.Name = "logFilesList";
            this.logFilesList.Size = new System.Drawing.Size(762, 126);
            this.logFilesList.TabIndex = 0;
            // 
            // wordWrapCheckbox
            // 
            this.wordWrapCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.wordWrapCheckbox.AutoSize = true;
            this.wordWrapCheckbox.Location = new System.Drawing.Point(12, 420);
            this.wordWrapCheckbox.Name = "wordWrapCheckbox";
            this.wordWrapCheckbox.Size = new System.Drawing.Size(81, 17);
            this.wordWrapCheckbox.TabIndex = 4;
            this.wordWrapCheckbox.Text = "Word Wrap";
            this.wordWrapCheckbox.UseVisualStyleBackColor = true;
            this.wordWrapCheckbox.CheckedChanged += new System.EventHandler(this.wordWrapCheckbox_CheckedChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.setFormCaptionButton);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.formCaptionTextbox);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(768, 132);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Options";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // formCaptionTextbox
            // 
            this.formCaptionTextbox.Location = new System.Drawing.Point(6, 29);
            this.formCaptionTextbox.Name = "formCaptionTextbox";
            this.formCaptionTextbox.Size = new System.Drawing.Size(264, 20);
            this.formCaptionTextbox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Form caption";
            // 
            // setFormCaptionButton
            // 
            this.setFormCaptionButton.Location = new System.Drawing.Point(276, 27);
            this.setFormCaptionButton.Name = "setFormCaptionButton";
            this.setFormCaptionButton.Size = new System.Drawing.Size(75, 23);
            this.setFormCaptionButton.TabIndex = 2;
            this.setFormCaptionButton.Text = "Set";
            this.setFormCaptionButton.UseVisualStyleBackColor = true;
            this.setFormCaptionButton.Click += new System.EventHandler(this.setFormCaptionButton_Click);
            // 
            // logFileListContextMenu
            // 
            this.logFileListContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openContainingFolderCommand});
            this.logFileListContextMenu.Name = "logFileListContextMenu";
            this.logFileListContextMenu.Size = new System.Drawing.Size(198, 26);
            this.logFileListContextMenu.Opened += new System.EventHandler(this.logFileListContextMenu_Opened);
            // 
            // openContainingFolderCommand
            // 
            this.openContainingFolderCommand.Name = "openContainingFolderCommand";
            this.openContainingFolderCommand.Size = new System.Drawing.Size(197, 22);
            this.openContainingFolderCommand.Text = "Open containing folder";
            this.openContainingFolderCommand.Click += new System.EventHandler(this.openContainingFolderCommand_Click);
            // 
            // AnalysisResultsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 449);
            this.Controls.Add(this.wordWrapCheckbox);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.closeButton);
            this.Name = "AnalysisResultsForm";
            this.Text = "Analysis Results";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.logFileListContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox resultsTextbox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.CheckedListBox analyzersList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox wordWrapCheckbox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckedListBox logFilesList;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button setFormCaptionButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox formCaptionTextbox;
        private System.Windows.Forms.ContextMenuStrip logFileListContextMenu;
        private System.Windows.Forms.ToolStripMenuItem openContainingFolderCommand;
    }
}