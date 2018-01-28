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
    public partial class TopologyForm : Form
    {
        AGIS owner;
        public TopologyForm(AGIS o)
        {
            InitializeComponent();
            owner = o;
            showTable();
        }

        void showTable()
        {
            if(owner!=null)
            {
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("结点", typeof(int));
                dt1.Columns.Add("弧段", typeof(String));
                dt1.Columns.Add("坐标", typeof(String));
                for(int i=0;i<owner.mp.Count;i++)
                {
                    if (owner.mp[i].lineNum > 1)
                    {
                        DataRow dr = dt1.NewRow();
                        dr[0] = owner.mp[i].ID;
                        StringBuilder sb = new StringBuilder();
                        sb.Append(owner.convert(owner.mp[i].relLine[0]));
                        for (int j = 1; j < owner.mp[i].lineNum; j++)
                        {
                            sb.Append(","+owner.convert(owner.mp[i].relLine[j]));
                        }
                        dr[1] = sb.ToString();
                        dr[2] = "(" + owner.mp[i].x + "," + owner.mp[i].y + ")";
                        dt1.Rows.Add(dr);
                    }
                }
                dataGridView1.DataSource = dt1;

                DataTable dt2 = new DataTable();
                dt2.Columns.Add("中间点", typeof(int));
                dt2.Columns.Add("弧段", typeof(String));
                dt2.Columns.Add("坐标", typeof(String));
                for (int i = 0; i < owner.mp.Count; i++)
                {
                    if (owner.mp[i].lineNum == 1)
                    {
                        DataRow dr = dt2.NewRow();
                        dr[0] = owner.mp[i].ID;
                        dr[1] = owner.mp[i].relLine[0];
                        dr[2] = "(" + owner.mp[i].x + "," + owner.mp[i].y + ")";
                        dt2.Rows.Add(dr);
                    }
                }
                dataGridView2.DataSource = dt2;

                DataTable dt3 = new DataTable();
                dt3.Columns.Add("弧段", typeof(int));
                dt3.Columns.Add("始结点", typeof(int));
                dt3.Columns.Add("末结点", typeof(int));
                dt3.Columns.Add("左多边形", typeof(int));
                dt3.Columns.Add("右多边形", typeof(int));
                for (int i = 0; i < owner.ma.Count; i++)
                {
                    DataRow dr = dt3.NewRow();
                    dr[0] = owner.ma[i].ID;
                    dr[1] = owner.ma[i].startPoint;
                    dr[2] = owner.ma[i].endPoint;
                    dr[3] = owner.ma[i].leftPoly;
                    dr[4] = owner.ma[i].rightPoly;
                    dt3.Rows.Add(dr);
                }
                dataGridView3.DataSource = dt3;

                DataTable dt4 = new DataTable();
                dt4.Columns.Add("多边形", typeof(int));
                dt4.Columns.Add("弧段", typeof(String));
                dt4.Columns.Add("邻接多边形", typeof(String));
                for (int i = 0; i < owner.mpol.Count; i++)
                {
                    DataRow dr = dt4.NewRow();
                    dr[0] = owner.mpol[i].ID;
                    StringBuilder sb=new StringBuilder();
                    sb.Append(owner.mpol[i].arcList[0]);
                    for (int j = 1; j < owner.mpol[i].arcNum;j++)
                    {
                        sb.Append("," + owner.mpol[i].arcList[j]);
                    }
                    dr[1] = sb.ToString();
                    sb.Remove(0,sb.Length);
                    sb.Append(owner.mpol[i].adjPoly[0]);
                    for (int j = 1; j < owner.mpol[i].adjPolyNum; j++)
                    {
                        sb.Append("," + owner.mpol[i].adjPoly[j]);
                    }
                    dr[2] = sb.ToString();
                    dt4.Rows.Add(dr);
                }
                dataGridView4.DataSource = dt4;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "保存";
            saveFileDialog1.Filter = "文本文件|*.txt";
            saveFileDialog1.FileName = "拓扑关系表";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);

                    sw.WriteLine("结点拓扑表");
                    sw.WriteLine("结点 弧段 坐标");
                    for (int i = 0; i < owner.mp.Count; i++)
                    {
                        if (owner.mp[i].lineNum > 1)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(owner.mp[i].ID+" ");
                            sb.Append(owner.convert(owner.mp[i].relLine[0]));
                            for (int j = 1; j < owner.mp[i].lineNum; j++)
                            {
                                sb.Append("," + owner.convert(owner.mp[i].relLine[j]));
                            }
                            sb.Append(" (" + owner.mp[i].x + "," + owner.mp[i].y + ")");
                            sw.WriteLine(sb.ToString());
                        }
                    }

                    sw.WriteLine("\n中间点拓扑表");
                    sw.WriteLine("中间点 弧段 坐标");
                    for (int i = 0; i < owner.mp.Count; i++)
                    {
                        if (owner.mp[i].lineNum == 1)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(owner.mp[i].ID + " ");
                            sb.Append(owner.mp[i].relLine[0]+" ");
                            sb.Append("(" + owner.mp[i].x + "," + owner.mp[i].y + ")");
                            sw.WriteLine(sb.ToString());
                        }
                    }

                    sw.WriteLine("\n弧段拓扑表");
                    sw.WriteLine("弧段 始结点 末结点 左多边形 右多边形");
                    for (int i = 0; i < owner.ma.Count; i++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(owner.ma[i].ID+" ");
                        sb.Append(owner.ma[i].startPoint+" ");
                        sb.Append(owner.ma[i].endPoint+" ");
                        sb.Append(owner.ma[i].leftPoly+" ");
                        sb.Append(owner.ma[i].rightPoly+" ");
                        sw.WriteLine(sb.ToString());
                    }

                    sw.WriteLine("\n多边形拓扑表");
                    sw.WriteLine("多边形 弧段 邻接多边形");
                    for (int i = 0; i < owner.mpol.Count; i++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(owner.mpol[i].ID+" ");
                        sb.Append(owner.mpol[i].arcList[0]);
                        for (int j = 1; j < owner.mpol[i].arcNum; j++)
                        {
                            sb.Append("," + owner.mpol[i].arcList[j]);
                        }
                        sb.Append(" ");
                        sb.Append(owner.mpol[i].adjPoly[0]);
                        for (int j = 1; j < owner.mpol[i].adjPolyNum; j++)
                        {
                            sb.Append("," + owner.mpol[i].adjPoly[j]);
                        }
                        sw.WriteLine(sb.ToString());
                    }
                    sw.Close();
                    fs.Close();                    
                }
                else
                {
                    MessageBox.Show("请输入文件名", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
