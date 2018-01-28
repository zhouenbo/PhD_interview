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
    public partial class DenseGridParas : Form
    {
        AGIS owner;
        public DenseGridParas(AGIS f)
        {
            InitializeComponent();
            textBox1.Text = "2";
            textBox2.Text = "2";
            owner = f;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            owner.DisplayGrid(owner.methodC,owner.xPaceC*int.Parse(textBox1.Text),owner.yPaceC*int.Parse(textBox2.Text));
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
