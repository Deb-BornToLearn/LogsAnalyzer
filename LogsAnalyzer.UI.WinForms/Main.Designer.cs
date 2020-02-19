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
            this.logFilesListMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addLogFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLogsCollectionFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.removeLogFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAllLogsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSelectedCollectionFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAllToCollectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analyzeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.analyzersList = new System.Windows.Forms.TreeView();
            this.logFilesList = new System.Windows.Forms.TreeView();
            this.logFilesListMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // logFilesListMenu
            // 
            this.logFilesListMenu.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.logFilesListMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLogFileMenuItem,
            this.addFolderMenuItem,
            this.addLogsCollectionFileMenuItem,
            this.toolStripMenuItem2,
            this.removeLogFileMenuItem,
            this.removeAllLogsMenuItem,
            this.toolStripMenuItem1,
            this.saveSelectedCollectionFileMenuItem,
            this.saveAllToCollectionMenuItem});
            this.logFilesListMenu.Name = "logFilesListMenu";
            this.logFilesListMenu.Size = new System.Drawing.Size(259, 170);
            this.logFilesListMenu.Opened += new System.EventHandler(this.logFilesListMenu_Opened);
            // 
            // addLogFileMenuItem
            // 
            this.addLogFileMenuItem.Name = "addLogFileMenuItem";
            this.addLogFileMenuItem.Size = new System.Drawing.Size(258, 22);
            this.addLogFileMenuItem.Text = "Add file(s)...";
            this.addLogFileMenuItem.Click += new System.EventHandler(this.addLogFileMenuItem_Click);
            // 
            // addFolderMenuItem
            // 
            this.addFolderMenuItem.Name = "addFolderMenuItem";
            this.addFolderMenuItem.Size = new System.Drawing.Size(258, 22);
            this.addFolderMenuItem.Text = "Add folder...";
            this.addFolderMenuItem.Click += new System.EventHandler(this.addFolderMenuItem_Click);
            // 
            // addLogsCollectionFileMenuItem
            // 
            this.addLogsCollectionFileMenuItem.Name = "addLogsCollectionFileMenuItem";
            this.addLogsCollectionFileMenuItem.Size = new System.Drawing.Size(258, 22);
            this.addLogsCollectionFileMenuItem.Text = "Add logs collection file...";
            this.addLogsCollectionFileMenuItem.Click += new System.EventHandler(this.addLogsCollectionFileMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(255, 6);
            // 
            // removeLogFileMenuItem
            // 
            this.removeLogFileMenuItem.Name = "removeLogFileMenuItem";
            this.removeLogFileMenuItem.Size = new System.Drawing.Size(258, 22);
            this.removeLogFileMenuItem.Text = "Remove checked items";
            this.removeLogFileMenuItem.Click += new System.EventHandler(this.removeLogFileMenuItem_Click);
            // 
            // removeAllLogsMenuItem
            // 
            this.removeAllLogsMenuItem.Name = "removeAllLogsMenuItem";
            this.removeAllLogsMenuItem.Size = new System.Drawing.Size(258, 22);
            this.removeAllLogsMenuItem.Text = "Remove all";
            this.removeAllLogsMenuItem.Click += new System.EventHandler(this.removeAllLogsMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(255, 6);
            // 
            // saveSelectedCollectionFileMenuItem
            // 
            this.saveSelectedCollectionFileMenuItem.Name = "saveSelectedCollectionFileMenuItem";
            this.saveSelectedCollectionFileMenuItem.Size = new System.Drawing.Size(258, 22);
            this.saveSelectedCollectionFileMenuItem.Text = "Save selected to log file collection..";
            this.saveSelectedCollectionFileMenuItem.Click += new System.EventHandler(this.saveSelectedCollectionFileMenuItem_Click);
            // 
            // saveAllToCollectionMenuItem
            // 
            this.saveAllToCollectionMenuItem.Name = "saveAllToCollectionMenuItem";
            this.saveAllToCollectionMenuItem.Size = new System.Drawing.Size(258, 22);
            this.saveAllToCollectionMenuItem.Text = "Save all to log file collection...";
            this.saveAllToCollectionMenuItem.Click += new System.EventHandler(this.saveAllToCollectionMenuItem_Click);
            // 
            // analyzeButton
            // 
            this.analyzeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzeButton.Location = new System.Drawing.Point(456, 329);
            this.analyzeButton.Margin = new System.Windows.Forms.Padding(1);
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
            this.label2.Location = new System.Drawing.Point(16, 169);
            this.label2.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Log analyzers";
            // 
            // analyzersList
            // 
            this.analyzersList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.analyzersList.CheckBoxes = true;
            this.analyzersList.Location = new System.Drawing.Point(18, 185);
            this.analyzersList.Name = "analyzersList";
            this.analyzersList.Size = new System.Drawing.Size(552, 138);
            this.analyzersList.TabIndex = 7;
            this.analyzersList.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.analyzersList_AfterCheck);
            // 
            // logFilesList
            // 
            this.logFilesList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logFilesList.CheckBoxes = true;
            this.logFilesList.ContextMenuStrip = this.logFilesListMenu;
            this.logFilesList.Location = new System.Drawing.Point(19, 29);
            this.logFilesList.Name = "logFilesList";
            this.logFilesList.Size = new System.Drawing.Size(551, 129);
            this.logFilesList.TabIndex = 8;
            this.logFilesList.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.logFilesList_AfterCheck);
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(590, 369);
            this.Controls.Add(this.logFilesList);
            this.Controls.Add(this.analyzersList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.analyzeButton);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "Main";
            this.Text = "Logs Analyzer";
            this.logFilesListMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip logFilesListMenu;
        private System.Windows.Forms.ToolStripMenuItem addLogFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeLogFileMenuItem;
        private System.Windows.Forms.Button analyzeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem removeAllLogsMenuItem;
        private System.Windows.Forms.TreeView analyzersList;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveSelectedCollectionFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAllToCollectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addFolderMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.TreeView logFilesList;
        private System.Windows.Forms.ToolStripMenuItem addLogsCollectionFileMenuItem;
    }
}

