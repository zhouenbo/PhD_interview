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
    public partial class PolygonInfo : Form
    {
        public PolygonInfo(int id, int arcNum,double perimeter,double area)
        {
            InitializeComponent();
            label5.Text = id.ToString();
            label6.Text = arcNum.ToString();
            label7.Text = perimeter.ToString("F3");
            label8.Text = area.ToString("F3");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
