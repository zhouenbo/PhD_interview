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
    public partial class IsolineParasDialog : Form
    {
         AGIS owner;
         bool flag;
        public IsolineParasDialog(AGIS f,bool fl)
        {
            InitializeComponent();
            owner = f;
            textBox1.Text = "0";
            textBox2.Text = "100";
            flag = fl;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (flag)
            {
                owner.generateIsoLineGrid(double.Parse(textBox1.Text), double.Parse(textBox2.Text),checkBox1.Checked);
                owner.gridIsoline = true;
            }
            else
                owner.generateIsoLineTIN(double.Parse(textBox1.Text), double.Parse(textBox2.Text),checkBox1.Checked);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        

    }
}
