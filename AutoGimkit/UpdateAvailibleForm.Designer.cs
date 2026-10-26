namespace AutoGimkit
{
    partial class UpdateAvailibleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateAvailibleForm));
            label1 = new Label();
            getButton = new Button();
            ignoreButton = new Button();
            versionLabel = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(302, 32);
            label1.TabIndex = 0;
            label1.Text = "There is an update avaible!";
            // 
            // getButton
            // 
            getButton.Location = new Point(12, 132);
            getButton.Name = "getButton";
            getButton.Size = new Size(398, 46);
            getButton.TabIndex = 1;
            getButton.Text = "Get Update";
            getButton.UseVisualStyleBackColor = true;
            getButton.Click += getButton_Click;
            // 
            // ignoreButton
            // 
            ignoreButton.Enabled = false;
            ignoreButton.Location = new Point(295, 99);
            ignoreButton.Name = "ignoreButton";
            ignoreButton.Size = new Size(173, 46);
            ignoreButton.TabIndex = 2;
            ignoreButton.Text = "Ignore";
            ignoreButton.UseVisualStyleBackColor = true;
            ignoreButton.Visible = false;
            ignoreButton.Click += ignoreButton_Click;
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.Location = new Point(12, 72);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(78, 32);
            versionLabel.TabIndex = 3;
            versionLabel.Text = "label2";
            // 
            // UpdateAvailibleForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(422, 190);
            Controls.Add(versionLabel);
            Controls.Add(ignoreButton);
            Controls.Add(getButton);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "UpdateAvailibleForm";
            Text = "Update Availbile";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button getButton;
        private Button ignoreButton;
        private Label versionLabel;
    }
}