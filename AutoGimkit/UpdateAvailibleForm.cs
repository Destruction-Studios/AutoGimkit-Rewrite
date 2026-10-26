using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace AutoGimkit
{

    public partial class UpdateAvailibleForm : Form
    {
        private const string UPDATE_URL = "https://github.com/Destruction-Studios/AutoGimkit-Rewrite/releases/latest";

        public UpdateAvailibleForm(string currentVersion, string latestVersion)
        {
            TopMost = true;
            MaximizeBox = false;
            MinimizeBox = false;
            InitializeComponent();

            versionLabel.Text = $"Current: {currentVersion}, latest: {latestVersion}";
        }

        private void ignoreButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void getButton_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = UPDATE_URL
            });
            Close();
        }
    }
}
