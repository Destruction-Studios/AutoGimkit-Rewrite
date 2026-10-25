namespace AutoGimkit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panelContainer = new Panel();
            SuspendLayout();
            // 
            // panelContainer
            // 
            panelContainer.Location = new Point(12, 10);
            panelContainer.Name = "panelContainer";
            panelContainer.Size = new Size(770, 420);
            panelContainer.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panelContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "AutoGimkit";
            ResumeLayout(false);
        }

        #endregion

        private Panel panelContainer;
    }
}
