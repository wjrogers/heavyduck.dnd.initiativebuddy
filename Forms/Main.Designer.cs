namespace HeavyDuck.Dnd.InitiativeBuddy.Forms
{
    partial class Main
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
            this.toolstrip = new System.Windows.Forms.ToolStrip();
            this.browser = new System.Windows.Forms.WebBrowser();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.encounter_tabs = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // toolstrip
            // 
            this.toolstrip.Location = new System.Drawing.Point(0, 0);
            this.toolstrip.Name = "toolstrip";
            this.toolstrip.Size = new System.Drawing.Size(934, 25);
            this.toolstrip.TabIndex = 0;
            // 
            // browser
            // 
            this.browser.AllowNavigation = false;
            this.browser.AllowWebBrowserDrop = false;
            this.browser.Dock = System.Windows.Forms.DockStyle.Right;
            this.browser.IsWebBrowserContextMenuEnabled = false;
            this.browser.Location = new System.Drawing.Point(594, 25);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.ScriptErrorsSuppressed = true;
            this.browser.Size = new System.Drawing.Size(340, 439);
            this.browser.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(591, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 439);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // encounter_tabs
            // 
            this.encounter_tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.encounter_tabs.Location = new System.Drawing.Point(0, 25);
            this.encounter_tabs.Name = "encounter_tabs";
            this.encounter_tabs.SelectedIndex = 0;
            this.encounter_tabs.Size = new System.Drawing.Size(591, 439);
            this.encounter_tabs.TabIndex = 3;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 464);
            this.Controls.Add(this.encounter_tabs);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.browser);
            this.Controls.Add(this.toolstrip);
            this.Name = "Main";
            this.Text = "Initiative Buddy";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolstrip;
        private System.Windows.Forms.WebBrowser browser;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.TabControl encounter_tabs;
    }
}