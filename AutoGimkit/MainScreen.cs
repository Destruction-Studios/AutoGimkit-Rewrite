using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoGimkit
{
    public partial class MainScreen : UserControl
    {
        private Form1 hostForm;

        public MainScreen(Form1 hostForm)
        {
            InitializeComponent();
            this.hostForm = hostForm;
        }

        public void SetCurrentLatin(string txt)
        {
            currentLatin.Text = $"Current latin: {txt}";
        }

        public void SetCurrentDef(string txt)
        {
            tableLookup.Text = $"Current def: {txt}";
        }

        public void UpdateMistakesText(int min, int max)
        {
            mistakesLabel.Text = $"Intentional Mistakes: {min}/{max}";
        }

        private async void startProgramButton_ClickAsync(object sender, EventArgs e)
        {
            startProgramButton.Enabled = false;
            stopProgramButton.Enabled = true;

            try
            {
                await Program.StartProgram();
            }
            catch
            {

            }
            finally
            {
                startProgramButton.Enabled = true;
                stopProgramButton.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.StopProgram();
        }
    }
}
