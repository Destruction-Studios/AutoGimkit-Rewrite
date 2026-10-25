namespace AutoGimkit
{
    partial class MainScreen
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            currentLatin = new Label();
            tableLookup = new Label();
            startProgramButton = new Button();
            startLabel = new Label();
            stopProgramButton = new Button();
            mistakesLabel = new Label();
            SuspendLayout();
            // 
            // currentLatin
            // 
            currentLatin.AutoSize = true;
            currentLatin.Location = new Point(15, 32);
            currentLatin.Name = "currentLatin";
            currentLatin.Size = new Size(218, 32);
            currentLatin.TabIndex = 0;
            currentLatin.Text = "Current latin: None";
            // 
            // tableLookup
            // 
            tableLookup.AutoSize = true;
            tableLookup.Location = new Point(15, 107);
            tableLookup.Name = "tableLookup";
            tableLookup.Size = new Size(249, 32);
            tableLookup.TabIndex = 1;
            tableLookup.Text = "Current def: Unknown";
            // 
            // startProgramButton
            // 
            startProgramButton.Cursor = Cursors.Hand;
            startProgramButton.Location = new Point(15, 359);
            startProgramButton.Name = "startProgramButton";
            startProgramButton.Size = new Size(503, 46);
            startProgramButton.TabIndex = 2;
            startProgramButton.Text = "Start Program";
            startProgramButton.UseVisualStyleBackColor = true;
            startProgramButton.Click += startProgramButton_ClickAsync;
            // 
            // startLabel
            // 
            startLabel.AutoSize = true;
            startLabel.Location = new Point(15, 311);
            startLabel.Name = "startLabel";
            startLabel.Size = new Size(473, 32);
            startLabel.TabIndex = 3;
            startLabel.Text = "Press start after opening the latin dialogue.";
            // 
            // stopProgramButton
            // 
            stopProgramButton.Enabled = false;
            stopProgramButton.Location = new Point(540, 359);
            stopProgramButton.Name = "stopProgramButton";
            stopProgramButton.Size = new Size(215, 46);
            stopProgramButton.TabIndex = 4;
            stopProgramButton.Text = "Stop Progam";
            stopProgramButton.UseVisualStyleBackColor = true;
            stopProgramButton.Click += button1_Click;
            // 
            // mistakesLabel
            // 
            mistakesLabel.AutoSize = true;
            mistakesLabel.Location = new Point(15, 206);
            mistakesLabel.Name = "mistakesLabel";
            mistakesLabel.Size = new Size(272, 32);
            mistakesLabel.TabIndex = 5;
            mistakesLabel.Text = "Intentional Mistakes: x/x";
            // 
            // MainScreen
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(mistakesLabel);
            Controls.Add(stopProgramButton);
            Controls.Add(startLabel);
            Controls.Add(startProgramButton);
            Controls.Add(tableLookup);
            Controls.Add(currentLatin);
            Name = "MainScreen";
            Size = new Size(770, 420);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label currentLatin;
        private Label tableLookup;
        private Button startProgramButton;
        private Label startLabel;
        private Button stopProgramButton;
        private Label mistakesLabel;
    }
}
