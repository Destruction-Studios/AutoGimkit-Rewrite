namespace AutoGimkit
{
    public partial class HomeScreen : UserControl
    {
        private Form1 hostForm;
        private string speedMultiText = "Speed Multi: 1=normal 2=twice as slow";

        public HomeScreen(Form1 hostForm)
        {
            InitializeComponent();
            this.hostForm = hostForm;
        }

        public void SetGimkitIdText(string text)
        {
            gimkitIdTextbox.Text = text;
        }

        public void SetSpeedMultiplierValue(float value)
        {
            speedMultiplayerTrack.Value = (int)value * 10;
            UpdateSpeedText(value);
        }

        public void UpdateMaxMistakes(int max)
        {
            maxTextBox.Text = max.ToString();
        }

        private void UpdateSpeedText(float currentValue)
        {
            speedMultiLabel.Text = $"{speedMultiText} ({currentValue})";
        }

        public float CalculateCurrentSpeedMulti() => speedMultiplayerTrack.Value / 10f;

        private async void button1_Click(object sender, EventArgs e)
        {
            DataSaving? ds = Program.dataSaving;

            hostForm.ShowMain();

            await Program.OpenChrome();
        }

        private void gimkitIdTextbox_TextChanged(object sender, EventArgs e)
        {
            Program.dataSaving?.SetGimkitId(gimkitIdTextbox.Text);
        }

        private void speedMultiplayerTrack_Scroll(object sender, EventArgs e)
        {
            float currentValue = CalculateCurrentSpeedMulti();

            Program.dataSaving?.SetSpeedMultiplier(currentValue);

            UpdateSpeedText(currentValue);
        }

        private void maxTextBox_TextChanged(object sender, EventArgs e)
        {
            int max;
            bool success = int.TryParse(maxTextBox.Text, out max);
            if (!success)
                max = 0;

            max = Math.Max(0, Math.Min(max, 100));

            Program.dataSaving?.SetIncorrectMax(max);

            maxTextBox.Text = max.ToString();
        }
    }
}
