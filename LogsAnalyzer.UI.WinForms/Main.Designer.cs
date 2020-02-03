namespace LogAnalyzer.UI.WinForms {
    partial class Main {
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
            this.analyzersList = new System.Windows.Forms.CheckedListBox();
            this.logFilesList = new System.Windows.Forms.ListBox();
            this.logFilesListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addLogFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeLogFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllLogsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultsTextbox = new System.Windows.Forms.TextBox();
            this.analyzeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.wordWrapResults = new System.Windows.Forms.CheckBox();
            this.logFilesListMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // analyzersList
            // 
            this.analyzersList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzersList.FormattingEnabled = true;
            this.analyzersList.Location = new System.Drawing.Point(18, 174);
            this.analyzersList.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.analyzersList.Name = "analyzersList";
            this.analyzersList.Size = new System.Drawing.Size(517, 94);
            this.analyzersList.TabIndex = 0;
            // 
            // logFilesList
            // 
            this.logFilesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logFilesList.ContextMenuStrip = this.logFilesListMenu;
            this.logFilesList.FormattingEnabled = true;
            this.logFilesList.Location = new System.Drawing.Point(18, 31);
            this.logFilesList.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.logFilesList.Name = "logFilesList";
            this.logFilesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.logFilesList.Size = new System.Drawing.Size(517, 108);
            this.logFilesList.TabIndex = 0;
            // 
            // logFilesListMenu
            // 
            this.logFilesListMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.logFilesListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLogFileMenuItem,
            this.removeLogFileMenuItem,
            this.removeAllLogsMenuItem});
            this.logFilesListMenu.Name = "logFilesListMenu";
            this.logFilesListMenu.Size = new System.Drawing.Size(135, 70);
            this.logFilesListMenu.Opened += new System.EventHandler(this.logFilesListMenu_Opened);
            // 
            // addLogFileMenuItem
            // 
            this.addLogFileMenuItem.Name = "addLogFileMenuItem";
            this.addLogFileMenuItem.Size = new System.Drawing.Size(134, 22);
            this.addLogFileMenuItem.Text = "Add...";
            this.addLogFileMenuItem.Click += new System.EventHandler(this.addLogFileMenuItem_Click);
            // 
            // removeLogFileMenuItem
            // 
            this.removeLogFileMenuItem.Name = "removeLogFileMenuItem";
            this.removeLogFileMenuItem.Size = new System.Drawing.Size(134, 22);
            this.removeLogFileMenuItem.Text = "Remove";
            this.removeLogFileMenuItem.Click += new System.EventHandler(this.removeLogFileMenuItem_Click);
            // 
            // removeAllLogsMenuItem
            // 
            this.removeAllLogsMenuItem.Name = "removeAllLogsMenuItem";
            this.removeAllLogsMenuItem.Size = new System.Drawing.Size(134, 22);
            this.removeAllLogsMenuItem.Text = "Remove All";
            this.removeAllLogsMenuItem.Click += new System.EventHandler(this.removeAllLogsMenuItem_Click);
            // 
            // resultsTextbox
            // 
            this.resultsTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsTextbox.Location = new System.Drawing.Point(18, 312);
            this.resultsTextbox.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.resultsTextbox.Multiline = true;
            this.resultsTextbox.Name = "resultsTextbox";
            this.resultsTextbox.ReadOnly = true;
            this.resultsTextbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.resultsTextbox.Size = new System.Drawing.Size(517, 92);
            this.resultsTextbox.TabIndex = 3;
            // 
            // analyzeButton
            // 
            this.analyzeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzeButton.Location = new System.Drawing.Point(418, 414);
            this.analyzeButton.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.analyzeButton.Name = "analyzeButton";
            this.analyzeButton.Size = new System.Drawing.Size(114, 34);
            this.analyzeButton.TabIndex = 4;
            this.analyzeButton.Text = "Analyze Logs";
            this.analyzeButton.UseVisualStyleBackColor = true;
            this.analyzeButton.Click += new System.EventHandler(this.analyzeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Log files";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 155);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Log analyzers";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 291);
            this.label3.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Analysis";
            // 
            // wordWrapResults
            // 
            this.wordWrapResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.wordWrapResults.AutoSize = true;
            this.wordWrapResults.Checked = true;
            this.wordWrapResults.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wordWrapResults.Location = new System.Drawing.Point(18, 412);
            this.wordWrapResults.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.wordWrapResults.Name = "wordWrapResults";
            this.wordWrapResults.Size = new System.Drawing.Size(84, 17);
            this.wordWrapResults.TabIndex = 8;
            this.wordWrapResults.Text = "Word wrap?";
            this.wordWrapResults.UseVisualStyleBackColor = true;
            this.wordWrapResults.CheckedChanged += new System.EventHandler(this.wordWrapResults_CheckedChanged);
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(555, 456);
            this.Controls.Add(this.wordWrapResults);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.analyzeButton);
            this.Controls.Add(this.resultsTextbox);
            this.Controls.Add(this.analyzersList);
            this.Controls.Add(this.logFilesList);
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.Name = "Main";
            this.Text = "Logs Analyzer";
            this.logFilesListMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox analyzersList;
        private System.Windows.Forms.ListBox logFilesList;
        private System.Windows.Forms.ContextMenuStrip logFilesListMenu;
        private System.Windows.Forms.ToolStripMenuItem addLogFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeLogFileMenuItem;
        private System.Windows.Forms.TextBox resultsTextbox;
        private System.Windows.Forms.Button analyzeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem removeAllLogsMenuItem;
        private System.Windows.Forms.CheckBox wordWrapResults;
    }
}

