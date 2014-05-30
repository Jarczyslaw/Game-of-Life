using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class FormSettings : CustomForm
    {
        public int Cnt { get; set; }

        public class GridEntry
        {
            public int Nr { get; set; }
        }

        public FormSettings()
        {
            Cnt = 0;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //textBox4.Text = (GC.GetTotalMemory(true) / 1024f).ToString("0.000") + "KB";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Storage.Engine.Interval = Convert.ToInt64(1000 / numericUpDown1.Value);
        }

        public void ThreadLogText(string s)
        {
            Storage.SettingsForm.AddLogText(s);
        }

        public void InitForm()
        {
            FormCollection fc = Application.OpenForms;
            int ct = 0;
            foreach (Form f in fc)
            {
                if (f is FormSettings)
                {
                    ct++;
                }
            }
            if (ct == 0)
            {
                InitializeComponent();
                dataGridView1.AutoGenerateColumns = false;
            }
            else
            {
                Close();
            }
        }

        public void UpdateGameInfo(float ips)
        {
            if (ips != -1)
            {
                textBox1.Text = ips.ToString("0.00");
            }
            textBox3.Text = Storage.Game.Iterations.Count.ToString();
            if (Storage.Game.GameState == Game.GameStates.Run)
            {
                textBox2.BackColor = Color.Tomato;
                textBox2.Text = "RUN";
            }
            else
            {
                textBox2.BackColor = Color.YellowGreen;
                textBox2.Text = "PAUSED";
            }
            List<GridEntry> g = new List<GridEntry>();
            for (int i = 0; i < Storage.Game.Iterations.Count; i++)
            {
                GridEntry e = new GridEntry();
                e.Nr = i + 1;
                g.Add(e);
            }
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = g;
        }

 
        public void AddLogText(string s)
        {
            Cnt++;
            richTextBox1.Text = richTextBox1.Text + Cnt + ". " + DateTime.Now.ToString("[hh:mm:ss.fff] ") + s + Environment.NewLine;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
            }
        }
    }
}
