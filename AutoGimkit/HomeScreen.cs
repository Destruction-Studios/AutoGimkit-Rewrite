namespace AutoGimkit
{
    public partial class HomeScreen : UserControl
    {
        private Form1 hostForm;
        private string speedMultiText = "Speed Multi: 1=normal 2=twice as slow";

        private int myMin = 0;
        private int myMax = 0;

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

        public void UpdateMinAndMax(int min, int max)
        {
            if (CheckMinAndMax(min, max))
            {
                Program.dataSaving?.SetIncorrectMin(myMin);
                Program.dataSaving?.SetIncorrectMax(myMax);
            }
            UpdateMinAndMaxLabels();
        }

        private bool CheckMinAndMax(int newMin, int newMax)
        {
            newMin = Math.Max(0, newMin);

            if (newMin > newMax)
                newMin = 0;
            if (newMax < newMin)
                newMax = 0;

            bool didMinChange = myMin != newMin;
            bool didMaxChange = myMax != newMax;

            myMin = newMin;
            myMax = newMax;

            return didMinChange || didMaxChange;
        }

        private void UpdateMinAndMaxLabels()
        {
            minTextBox.Text = myMin.ToString();
            maxTextBox.Text = myMax.ToString();
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

        private void minTextBox_TextChanged(object sender, EventArgs e)
        {
            int min;
            bool success = int.TryParse(minTextBox.Text, out min);
            if (!success)
                min = 0;


            UpdateMinAndMax(min, myMax);
        }

        private void maxTextBox_TextChanged(object sender, EventArgs e)
        {
            int max;
            bool success = int.TryParse(maxTextBox.Text, out max);
            if (!success)
                max = myMin;


            UpdateMinAndMax(myMin, max);
        }
    }
}
