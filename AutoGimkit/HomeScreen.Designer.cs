namespace AutoGimkit
{
    partial class HomeScreen
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
            startButton = new Button();
            gimkitIdTextbox = new TextBox();
            gimkitIdLabel = new Label();
            speedMultiLabel = new Label();
            speedMultiplayerTrack = new TrackBar();
            minMaxLabel = new Label();
            maxTextBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)speedMultiplayerTrack).BeginInit();
            SuspendLayout();
            // 
            // startButton
            // 
            startButton.Cursor = Cursors.Hand;
            startButton.Location = new Point(27, 349);
            startButton.Name = "startButton";
            startButton.Size = new Size(219, 46);
            startButton.TabIndex = 0;
            startButton.Text = "Open Chrome";
            startButton.UseVisualStyleBackColor = true;
            startButton.Click += button1_Click;
            // 
            // gimkitIdTextbox
            // 
            gimkitIdTextbox.Location = new Point(27, 61);
            gimkitIdTextbox.Name = "gimkitIdTextbox";
            gimkitIdTextbox.Size = new Size(721, 39);
            gimkitIdTextbox.TabIndex = 1;
            gimkitIdTextbox.TextChanged += gimkitIdTextbox_TextChanged;
            // 
            // gimkitIdLabel
            // 
            gimkitIdLabel.AutoSize = true;
            gimkitIdLabel.Location = new Point(28, 16);
            gimkitIdLabel.Name = "gimkitIdLabel";
            gimkitIdLabel.Size = new Size(512, 32);
            gimkitIdLabel.TabIndex = 2;
            gimkitIdLabel.Text = "Gimkit ID: part of the url after gimkit.com/join/";
            // 
            // speedMultiLabel
            // 
            speedMultiLabel.AutoSize = true;
            speedMultiLabel.Location = new Point(28, 116);
            speedMultiLabel.Name = "speedMultiLabel";
            speedMultiLabel.Size = new Size(671, 32);
            speedMultiLabel.TabIndex = 3;
            speedMultiLabel.Text = "Negative Speed Multi: 1=normal 2=twice as slow (current val)";
            // 
            // speedMultiplayerTrack
            // 
            speedMultiplayerTrack.LargeChange = 1;
            speedMultiplayerTrack.Location = new Point(28, 166);
            speedMultiplayerTrack.Maximum = 100;
            speedMultiplayerTrack.Minimum = 10;
            speedMultiplayerTrack.Name = "speedMultiplayerTrack";
            speedMultiplayerTrack.Size = new Size(720, 90);
            speedMultiplayerTrack.TabIndex = 4;
            speedMultiplayerTrack.Value = 10;
            speedMultiplayerTrack.Scroll += speedMultiplayerTrack_Scroll;
            // 
            // minMaxLabel
            // 
            minMaxLabel.AutoSize = true;
            minMaxLabel.Location = new Point(27, 224);
            minMaxLabel.Name = "minMaxLabel";
            minMaxLabel.Size = new Size(643, 32);
            minMaxLabel.TabIndex = 5;
            minMaxLabel.Text = "The max answers the program will automatically get wrong";
            // 
            // maxTextBox
            // 
            maxTextBox.Location = new Point(28, 276);
            maxTextBox.Name = "maxTextBox";
            maxTextBox.Size = new Size(200, 39);
            maxTextBox.TabIndex = 7;
            maxTextBox.TextChanged += maxTextBox_TextChanged;
            // 
            // HomeScreen
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(maxTextBox);
            Controls.Add(minMaxLabel);
            Controls.Add(speedMultiplayerTrack);
            Controls.Add(speedMultiLabel);
            Controls.Add(gimkitIdLabel);
            Controls.Add(gimkitIdTextbox);
            Controls.Add(startButton);
            Name = "HomeScreen";
            Size = new Size(770, 420);
            ((System.ComponentModel.ISupportInitialize)speedMultiplayerTrack).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button startButton;
        private TextBox gimkitIdTextbox;
        private Label gimkitIdLabel;
        private Label speedMultiLabel;
        private TrackBar speedMultiplayerTrack;
        private Label minMaxLabel;
        private TextBox maxTextBox;
    }
}
