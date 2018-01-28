using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AdvancedGIS.DialogForm
{
    public partial class GridParasDialog : Form
    {
        AGIS owner;
        public GridParasDialog(AGIS f)
        {
            InitializeComponent();
            textBox1.Text = "15";
            textBox2.Text = "10";
            radioButton1.Checked = true;
           // checkedListBox1.SelectedIndex = 0;
           // checkedListBox1.SetItemChecked(0,true);
            owner = f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                owner.DisplayGrid(0, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            else
                owner.DisplayGrid(1, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
                owner.DisplayGrid(0, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            else
                owner.DisplayGrid(1, int.Parse(textBox1.Text), int.Parse(textBox2.Text));
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            owner.UnDisplayGrid();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;
        }
    }
}
