using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGimkit
{
    public sealed class LoadingForm : Form
    {
        public LoadingForm(string message)
        {
            Text = "Please wait";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = false;          // no close button
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(340, 110);

            var label = new Label
            {
                Text = message,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 10),
                Size = new Size(320, 50)
            };

            var bar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Location = new Point(20, 70),
                Size = new Size(300, 22)
            };

            Controls.Add(label);
            Controls.Add(bar);
        }
    }
}