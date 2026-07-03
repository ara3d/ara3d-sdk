using System;
using System.Windows.Forms;

namespace Ara3D.Bowerbird.RevitSamples
{
    public partial class BosBackgroundExporterForm : Form
    {
        public BosBackgroundExporterForm()
        {
            InitializeComponent();
        }

        public string NowString()
            => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");

        public void SetIdle()
            => SetIdle(NowString());

        public void SetIdle(string txt)
            => textBoxIdle.BeginInvoke(() => textBoxIdle.Text = txt);

        public void SetHeartBeat()
            => SetHeartBeat(NowString());

        public void SetHeartBeat(string txt)
            => textBoxHeartbeat.BeginInvoke(() => textBoxHeartbeat.Text = txt);

        public void SetId(string txt)
            => textBoxId.BeginInvoke(() => textBoxId.Text = txt);

        public void SetQueueSize(int n)
            => textBoxQueue.BeginInvoke(() => textBoxQueue.Text = n.ToString());

        public void SetLastProcess()
            => SetLastProcess(NowString());

        public void SetLastProcess(string text)
            => textBoxProcess.BeginInvoke(() => textBoxProcess.Text = text);

        private void button1_Click(object sender, EventArgs e)
        {

        }

        public event EventHandler OnReset
        {
            add => button1.Click += value;
            remove => button1.Click -= value;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxId_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
