using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using AdvancedGIS.dataStructure;

namespace AdvancedGIS
{
    public partial class AGIS : Form
    {
        private List<Dt> data;
        private double scale;
        private double maxx, maxy, minx, miny;
        private const double margin=7;

        public int xPaceC=15, yPaceC=10;
        public int methodC=0;

        public bool gridFlag=false;
        public bool gridFlag1 = false;
        public bool TINFlag = false;

        public List<Arc> triArc = new List<Arc>();
        public List<int> TINConvex = new List<int>();
        public List<Triangle> TINRes = new List<Triangle>();

        public bool gridIsoline = false;
        public bool topo = false;

        public List<List<PointF>> resGrid = new List<List<PointF>>();

        public List<MyPoint> mp = new List<MyPoint>();
        public List<MyArc> ma = new List<MyArc>();
        public List<MyPolygon> mpol = new List<MyPolygon>();

        public AGIS()
        {
            InitializeComponent();
            data=new List<Dt>();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            openFileDialog.Filter = "文本文件|*.txt|文本文件|*.dat|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                StreamReader reader = new StreamReader(fName, Encoding.Default);
                //循环读取所有行
                while (!reader.EndOfStream)
                {
                    //将每行数据，用空格分割成2段
                    string[] dataRead = reader.ReadLine().Split(',');
                    Dt tmp = new Dt();
                    tmp.setIndex(int.Parse(dataRead[0]));
                    tmp.setName(dataRead[1]);
                    tmp.setX(double.Parse(dataRead[2]));
                    tmp.setY(double.Parse(dataRead[3]));
                    tmp.setZ(double.Parse(dataRead[4]));
                    data.Add(tmp);
                }
                reader.Dispose();
                CalculateExtent();
                CalculateScale();
                DrawOriginData();
            }
            else
            {
             //   MessageBox.Show();
            }
        }
        
        private void CalculateExtent()
        {
            maxx = double.MinValue;
            minx = double.MaxValue;
            maxy = double.MinValue;
            miny = double.MaxValue;
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].getX() > maxx)
                    maxx = data[i].getX();
                if (data[i].getX() < minx)
                    minx = data[i].getX();
                if (data[i].getY() > maxy)
                    maxy = data[i].getY();
                if (data[i].getY() < miny)
                    miny = data[i].getY();
            }
        }
        private void CalculateScale()
        {
            if (data.Count <= 0)
                return;
            if (1.0 * (maxy - miny) / (maxx - minx) > 1.0 * (pictureBox1.Height - 2*margin) / (pictureBox1.Width - 2*margin))
                scale = (maxy - miny) / (pictureBox1.Height - 2 * margin);
            else
                scale = (maxx - minx) / (pictureBox1.Width - 2 * margin);
        }

        private Dt CoordinateConvert(Dt tmp)
        {
            Dt s = new Dt();
            s.setX((tmp.getX()-minx)/scale+margin);
            s.setY(pictureBox1.Height-(tmp.getY()-miny)/scale-margin);
            s.setIndex(tmp.getIndex());
            s.setName(tmp.getName());
            s.setZ(tmp.getZ());
            return s;
        }

        public void DrawOriginData()
        {
            //Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
           // Graphics g = Graphics.FromImage(bmp);
            Graphics g = pictureBox1.CreateGraphics();
            //Pen myPen = new Pen(Color.Black);
            SolidBrush myBrush = new SolidBrush(Color.Blue);
            for (int i = 0; i < data.Count; i++)
            {
                Dt tmp = CoordinateConvert(data[i]);
                g.FillEllipse(myBrush, new RectangleF((float)tmp.getX() - 2, (float)tmp.getY() - 2, 5, 5));
            }
            //myBrush.Color = Color.Red;
            //g.FillEllipse(myBrush, new RectangleF((float)394.05897356497195 - 2, 430-2, 5, 5));
            
            myBrush.Dispose();
            g.Dispose();
           // pictureBox1.Image = bmp;
        }

        private void 生成格网ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdvancedGIS.DialogForm.GridParasDialog gpd = new DialogForm.GridParasDialog(this);
            gpd.Show(this);
        }
    
        public void DisplayGrid(int med,int xPace,int yPace)
        {
            if(data.Count==0)
            {
                MessageBox.Show("未导入点数据！");
                return;
            }

            xPaceC = xPace;
            yPaceC = yPace;
            methodC = med;
            pictureBox1.Refresh();
            DrawOriginData();

            Graphics g = pictureBox1.CreateGraphics();
            Pen myPen = new Pen(Color.Black);
            double xinterval = (pictureBox1.Width - 2 * margin) / xPace;
            double yinterval = (pictureBox1.Height - 2 * margin) / yPace;
            for (int i = 0; i < xPace+1;i++ )
            {
                g.DrawLine(myPen, new Point((int)(margin + i * xinterval), (int)margin), new Point((int)(margin + i * xinterval), (int)(pictureBox1.Height - margin)));
            }

            for(int i=0;i<yPace+1;i++)
            {
                g.DrawLine(myPen, new Point((int)margin, (int)(margin + i * yinterval)), new Point((int)(pictureBox1.Width - margin), (int)(margin + i * yinterval)));
            }

            g.Dispose();
            myPen.Dispose();
            gridFlag = true;
            gridFlag1 = true;
        }

        public void UnDisplayGrid()
        {
            gridFlag = false;
            pictureBox1.Refresh();
            DrawOriginData();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawOriginData();
        }

        private void tIN模型ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdvancedGIS.DialogForm.DenseGridParas dgp = new DialogForm.DenseGridParas(this);
            dgp.Show(this);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if(gridFlag == true)
            {
                pictureBox1.Refresh();
                DisplayGrid(methodC,xPaceC,yPaceC);

                double xinterval = (pictureBox1.Width - 2 * margin) / xPaceC;
                double yinterval = (pictureBox1.Height - 2 * margin) / yPaceC;

                int xIndex;
                if ((e.X - margin) / xinterval - (int)((e.X - margin) / xinterval) >= 0.5)
                    xIndex = (int)((e.X - margin) / xinterval) + 1;
                else
                    xIndex = (int)((e.X - margin) / xinterval);

                int yIndex;
                if ((pictureBox1.Height - e.Y - margin) / yinterval - (int)((pictureBox1.Height - e.Y - margin) / yinterval) >= 0.5)
                    yIndex = (int)((pictureBox1.Height - e.Y - margin) / yinterval) + 1;
                else
                    yIndex = (int)((pictureBox1.Height - e.Y - margin) / yinterval);

                if (Math.Abs(margin + xIndex * xinterval - e.X) <= 5 && Math.Abs(pictureBox1.Height - margin - yIndex * yinterval - e.Y) <= 5)
                {
                    Graphics g = pictureBox1.CreateGraphics();
                    Pen myPen = new Pen(Color.Green, 3);
                    SolidBrush myBrush = new SolidBrush(Color.Red);
                    g.DrawLine(myPen, new Point((int)margin, (int)(pictureBox1.Height - margin - yIndex * yinterval)), new Point((int)(pictureBox1.Width - margin), (int)(pictureBox1.Height - margin - yIndex * yinterval)));
                    g.DrawLine(myPen, new Point((int)(margin + xIndex * xinterval), (int)margin), new Point((int)(margin + xIndex * xinterval), (int)(pictureBox1.Height - margin)));
                    g.FillEllipse(myBrush, new RectangleF((float)(margin + xIndex * xinterval - 2), (float)(pictureBox1.Height - margin - yIndex * yinterval - 2), 5, 5));
                    g.Dispose();
                    myPen.Dispose();
                    myBrush.Dispose();
                    if (methodC == 0)
                        MessageBox.Show("Z值：" + GetAttributes0(margin + xIndex * xinterval, pictureBox1.Height - margin - yIndex * yinterval).ToString("F2"));
                    else
                        MessageBox.Show("Z值：" + GetAttributes1(margin + xIndex * xinterval, pictureBox1.Height - margin - yIndex * yinterval).ToString("F2"));
                }
            }

            if(topo==true&&gridIsoline==true)
            {
                showTopo();
                MyPoint tmp = new MyPoint((e.X-margin)*scale+minx,(pictureBox1.Height-e.Y-margin)*scale+miny);
                for(int i=0;i<mpol.Count;i++)
                {
                     if(isPointInPolygon(tmp,i))//重画，查询属性
                     {
                         Graphics g = pictureBox1.CreateGraphics();
                         SolidBrush myBrush = new SolidBrush(Color.White);
                         //myBrush.Color = Color.FromArgb(125,Color.Black);

                         int pointNumPoly = 0;
                         for (int j = 0; j < mpol[i].arcList.Count; j++)
                         {
                             pointNumPoly += ma[convert(mpol[i].arcList[j])].midPointNum + 1;
                         }
                         Point[] pArray = new Point[pointNumPoly];
                         pointNumPoly = 0;
                         for (int j = 0; j < mpol[i].arcList.Count; j++)
                         {
                             if (mpol[i].arcList[j] >= 0)
                             {
                                 pArray[pointNumPoly] = new Point((int)((mp[ma[convert(mpol[i].arcList[j])].startPoint].x - minx) / scale + margin), (int)(pictureBox1.Height - (mp[ma[convert(mpol[i].arcList[j])].startPoint].y - miny) / scale - margin));
                                 pointNumPoly++;
                                 for (int k = 0; k < ma[convert(mpol[i].arcList[j])].midPointNum; k++)
                                 {
                                     pArray[pointNumPoly] = new Point((int)((mp[ma[convert(mpol[i].arcList[j])].midPoint[k]].x - minx) / scale + margin), (int)(pictureBox1.Height - (mp[ma[convert(mpol[i].arcList[j])].midPoint[k]].y - miny) / scale - margin));
                                     pointNumPoly++;
                                 }
                             }
                             else
                             {
                                 pArray[pointNumPoly] = new Point((int)((mp[ma[convert(mpol[i].arcList[j])].endPoint].x - minx) / scale + margin), (int)(pictureBox1.Height - (mp[ma[convert(mpol[i].arcList[j])].endPoint].y - miny) / scale - margin));
                                 pointNumPoly++;
                                 for (int k = ma[convert(mpol[i].arcList[j])].midPointNum - 1; k >= 0; k--)
                                 {
                                     pArray[pointNumPoly] = new Point((int)((mp[ma[convert(mpol[i].arcList[j])].midPoint[k]].x - minx) / scale + margin), (int)(pictureBox1.Height - (mp[ma[convert(mpol[i].arcList[j])].midPoint[k]].y - miny) / scale - margin));
                                     pointNumPoly++;
                                 }
                             }
                         }
                         g.FillPolygon(myBrush, pArray);
                         AdvancedGIS.DialogForm.PolygonInfo PI = new DialogForm.PolygonInfo(mpol[i].ID,mpol[i].arcNum,mpol[i].GetPerimeter(mp,ma),mpol[i].GetArea(mp,ma));
                         PI.ShowDialog();
                         break;
                     }
                }
            }
        }

        public bool isPointInPolygon(MyPoint p,int PolyIndex)
        {
            int num = 0;
            for (int i = 0; i < mpol[PolyIndex].arcNum; i++)
            {
                int arcIndex = convert(mpol[PolyIndex].arcList[i]);
                MyPoint p1, p2;
                if (mpol[PolyIndex].arcList[i] >= 0)
                {
                    p1 = mp[ma[arcIndex].startPoint];
                    for (int j = 0; j < ma[arcIndex].midPointNum; j++)
                    {
                        p2 = mp[ma[arcIndex].midPoint[j]];
                        double tmp = (p1.y - p.y) * (p2.y - p.y);
                        if (tmp < 0)
                        {
                            double tt = p.x - (p.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) - p1.x;
                            if (tt < 0)
                            {
                                num++;
                            }
                            else if (tt == 0)
                            {
                                return false;//在线上，不在多边形内
                            }
                        }
                        //else if (tmp == 0)//射线必经过线段端点
                        //{
                        //    if (Math.Abs(p1.y - p2.y) > 0.1)//只经过一个端点的情况
                        //    {
                        //        if (Math.Abs(p1.y - p.y) < 0.1)
                        //        {
                        //            int st1 = (i - 1) % p.PointCount;
                        //            int st2 = (i + 1) % p.PointCount;
                        //            if ((p.pointArray[st1].Y - m.Y) * (p.pointArray[st2].Y - m.Y) < 0)
                        //            {
                        //                num++;
                        //            }
                        //        }
                        //    }
                        //    else//经过两个端点的情况
                        //    {
                        //        if (p.pointArray[(i - 1) % p.PointCount].Y != m.Y)
                        //        {
                        //            for (int j = 0; j < p.PointCount - 3; j++)
                        //            {
                        //                if (p.pointArray[(i + 2 + j) % p.PointCount].Y != m.Y)
                        //                {
                        //                    if ((p.pointArray[(i - 1) % p.PointCount].Y - m.Y) * (p.pointArray[(i + 2 + j) % p.PointCount].Y - m.Y) < 0)
                        //                        num++;

                        //                    break;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        p1 = p2;
                    }
                    if (ma[arcIndex].midPointNum != 0)
                        p1 = mp[ma[arcIndex].midPoint[ma[arcIndex].midPointNum - 1]];
                    p2 = mp[ma[arcIndex].endPoint];
                    double tmpt = (p1.y - p.y) * (p2.y - p.y);
                    if (tmpt < 0)
                    {
                        double tt = p.x - (p.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) - p1.x;
                        if (tt < 0)
                        {
                            num++;
                        }
                        else if (tt == 0)
                        {
                            return false;//在线上，不在多边形内
                        }
                    }
                }
                else
                {
                    p1 = mp[ma[arcIndex].endPoint];
                    for (int j = ma[arcIndex].midPointNum - 1; j >= 0; j--)
                    {
                        p2 = mp[ma[arcIndex].midPoint[j]];
                        double tmp = (p1.y - p.y) * (p2.y - p.y);
                        if (tmp < 0)
                        {
                            double tt = p.x - (p.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) - p1.x;
                            if (tt < 0)
                            {
                                num++;
                            }
                            else if (tt == 0)
                            {
                                return false;//在线上，不在多边形内
                            }
                        }
                        p1 = p2;
                    }
                    if(ma[arcIndex].midPointNum!=0)
                        p1 = mp[ma[arcIndex].midPoint[0]];
                    p2 = mp[ma[arcIndex].startPoint];
                    double tmpt = (p1.y - p.y) * (p2.y - p.y);
                    if (tmpt < 0)
                    {
                        double tt = p.x - (p.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) - p1.x;
                        if (tt < 0)
                        {
                            num++;
                        }
                        else if (tt == 0)
                        {
                            return false;//在线上，不在多边形内
                        }
                    }
                }
            }
            if (num % 2 == 0)
                return false;
            else
                return true;
        }

        private double calAlpha(MyPoint p,MyPoint p1)
        {
            double cos1, sin1,res;
            cos1 = cos(p, p1, new MyPoint(p.x+1, p.y));
            sin1 = sin(p, p1, new MyPoint(p.x+1, p.y));
            if (sin1 >= 0)
                res=Math.Acos(cos1) / Math.PI * 180;
            else
                res=(Math.Acos(-cos1) / Math.PI * 180 + 180);
            return res;
        }

        /// <summary>
        /// 按方位加权平均法插值
        /// </summary>
        /// <param name="x0">插值点屏幕横坐标</param>
        /// <param name="y0">插值点屏幕纵坐标</param>
        /// <returns>插值点属性预测值</returns>
        private double GetAttributes1(double x0,double y0)
        {
            double x = (x0 - margin) * scale + minx;//插值点位置横坐标
            double y = (pictureBox1.Height - y0 - margin) * scale + miny;//插值点位置纵坐标
            double preValue = 0, wSum = 0;
            List<int> index = new List<int>();
            List<double> minDis = new List<double>();
            for (int i = 0; i < 8;i++ )//对八个方位，分别找离插值点最近的点
            {
                int tt1 = -1;
                double tt2 = double.MaxValue;
                for(int j=0;j<data.Count;j++)
                {
                    double alpha=calAlpha(new MyPoint(x,y),new MyPoint(data[j].getX(),data[j].getY()));
                    if(alpha>=i*45&&alpha<(i+1)*45)
                    {
                        if(Math.Sqrt((x-data[j].getY())*(x-data[j].getY())+(y-data[j].getY())*(y-data[j].getY()))<tt2)
                        {
                            tt1 = j;
                            tt2 = Math.Sqrt((x - data[j].getY()) * (x - data[j].getY()) + (y - data[j].getY()) * (y - data[j].getY()));
                        }
                    }
                }
                if(tt1>=0)
                {
                    index.Add(tt1);
                    minDis.Add(tt2);
                }
            }
            for (int i = 0; i < index.Count; i++)//计算插值结果
            {
                double wTemp = 1;
                for (int j = 0; j < minDis.Count; j++)
                {
                    if (j != i)
                        wTemp *= minDis[j] * minDis[j];
                }
                wSum += wTemp;
                preValue += data[index[i]].getZ() * wTemp;
            }
            preValue = preValue / wSum;
            return preValue;
        }
    
        /// <summary>
        /// 距离平方倒数法插值
        /// </summary>
        /// <param name="x">插值点屏幕横坐标</param>
        /// <param name="y">插值点屏幕纵坐标</param>
        /// <returns>插值点预测的属性值</returns>
        private double GetAttributes0(double x,double y)
        {
            double wSum = 0,sum=0;
            double xo = (x - margin) * scale + minx;//待插值位置横坐标
            double yo = (pictureBox1.Height-y - margin) * scale + miny;//待插值位置纵坐标
            for (int i = 0; i < data.Count;i++ )
            {
                double reciprocal = 1 / Math.Sqrt((data[i].getX() - xo) * (data[i].getX() - xo) + (data[i].getY() - yo) * (data[i].getY() - yo));
                wSum += reciprocal;
                sum += data[i].getZ() * reciprocal;
            }
            double preVal = sum / wSum;
            return preVal;
        }

        private void 生成TINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data.Count == 0)
            {
                MessageBox.Show("未导入点数据！");
                return;
            }
            TINFlag = true;
            pictureBox1.Refresh();
            DrawOriginData();

            List<int> convex=AdvancedGIS.algorithm.TINGenerator.generateConvex(data);
            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush myBrush = new SolidBrush(Color.Red);
            Pen myPen = new Pen(Color.Gray);
            //for (int i = 0; i < convex.Count; i++)
            //{
            //    Dt tmp = CoordinateConvert(data[convex[i]]);
            //    g.FillEllipse(myBrush, new RectangleF((float)tmp.getX() - 2, (float)tmp.getY() - 2, 5, 5));
            //    g.DrawLine(myPen, new PointF((float)tmp.getX(), (float)tmp.getY()), new PointF((float)CoordinateConvert(data[convex[(i + 1) % convex.Count]]).getX(), (float)CoordinateConvert(data[convex[(i + 1) % convex.Count]]).getY()));
            //}

            AdvancedGIS.algorithm.TINGenerator.generateTIN(data,convex,triArc,TINConvex,TINRes);
            foreach(Triangle t in TINRes)
            {
                Dt tmp1 = CoordinateConvert(data[t.ID1]);
                Dt tmp2 = CoordinateConvert(data[t.ID2]);
                Dt tmp3 = CoordinateConvert(data[t.ID3]);
                g.DrawLine(myPen, new PointF((float)tmp1.getX(), (float)tmp1.getY()), new PointF((float)tmp2.getX(), (float)tmp2.getY()));
                g.DrawLine(myPen, new PointF((float)tmp1.getX(), (float)tmp1.getY()), new PointF((float)tmp3.getX(), (float)tmp3.getY()));
                g.DrawLine(myPen, new PointF((float)tmp3.getX(), (float)tmp3.getY()), new PointF((float)tmp2.getX(), (float)tmp2.getY()));

            }

            myPen.Dispose();
            myBrush.Dispose();
            g.Dispose();
        }

        private void 生成等值线ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(gridFlag1==false)
            {
                MessageBox.Show("还未生成格网！");
                return;
            }
            gridFlag = false;
            AdvancedGIS.DialogForm.IsolineParasDialog ipd = new DialogForm.IsolineParasDialog(this,true);
            ipd.Show(this);
        }
        /// <summary>
        /// 网格自动生成等值线
        /// </summary>
        /// <param name="val">等值线的初始值</param>
        /// <param name="IsoInterval">等值线间隔</param>
        /// <param name="isSmooth">等值线是否光滑</param>
        public void generateIsoLineGrid(double val,double IsoInterval,bool isSmooth)
        {
            resGrid.Clear();
            //计算H矩阵
            double[,] H = new double[xPaceC + 1, yPaceC + 1];
            double xinterval = (pictureBox1.Width - 2 * margin) / xPaceC;
            double yinterval = (pictureBox1.Height - 2 * margin) / yPaceC;
            double HMax = double.MinValue, HMin = double.MaxValue;
            for (int i = 0; i < xPaceC + 1; i++)
            {
                for (int j = 0; j < yPaceC + 1; j++)
                {
                    if (methodC == 0)
                        H[i, j] = GetAttributes0(margin + xinterval * i, pictureBox1.Height - (margin + yinterval * j));
                    else
                        H[i, j] = GetAttributes1(margin + xinterval * i, pictureBox1.Height - (margin + yinterval * j));
                    if (HMax < H[i, j])
                        HMax = H[i, j];
                    if (HMin > H[i, j])
                        HMin = H[i, j];
                }
            }
            //找到位于插值区域内的等值线最小值
            if(val>HMax)
            {
                if ((int)((val - HMax) / IsoInterval) == (int)((val - HMin) / IsoInterval) && (int)((val - HMax) / IsoInterval) != (val - HMax) / IsoInterval)
                    return;
                else
                {
                    while(val>HMin)
                    {
                        val = val - IsoInterval;
                    }
                    val = val + IsoInterval;
                }
            }
            else if(val<HMin)
            {
                if ((int)((HMin - val) / IsoInterval) == (int)((HMax - val) / IsoInterval) && (int)((HMin - val) / IsoInterval) != (HMin - val) / IsoInterval)
                    return;
                else
                {
                    while (val < HMin)
                        val = val + IsoInterval;
                }
            }
            else
            {
                while(val>HMin)
                    val = val -  IsoInterval;
                val = val + IsoInterval;
            }

            for(double HZ=val;HZ<=HMax;HZ=HZ+IsoInterval)//对位于插值区域内的每个值进行插值
            {
                //对等值线的值HZ，计算HH
                double[,] HH = new double[xPaceC, yPaceC+1];
                for (int i = 0; i < xPaceC; i++)
                {
                    for (int j = 0; j < yPaceC+1; j++)
                    {
                        HH[i, j] = (HZ - H[i, j]) / (H[i+1, j] - H[i, j]);
                        if (HH[i, j] < 0 || HH[i, j] > 1)
                            HH[i, j] = 2;
                        if (H[i, j] == 0)
                            H[i, j] = 0.0001;
                        if (H[i, j] == 1)
                            H[i, j] = 0.9999;
                    }
                }
                //对等值线的值HZ，计算SS
                double[,] SS = new double[xPaceC+1, yPaceC];
                for (int i = 0; i < xPaceC+1; i++)
                {
                    for (int j = 0; j < yPaceC; j++)
                    {
                        SS[i, j] = (HZ - H[i, j]) / (H[i, j+1] - H[i, j]);
                        if (SS[i, j] < 0 || SS[i, j] > 1)
                            SS[i, j] = 2;
                        if (SS[i, j] == 0)
                            SS[i, j] = 0.0001;
                        if (SS[i, j] == 1)
                            SS[i, j] = 0.9999;
                    }
                }
                for (int i = 0; i < xPaceC; i++)//底边界找线头
                {
                    if (HH[i, 0] >= 0 && HH[i, 0] <= 1)
                    {
                        PointF A = new PointF((float)(i + 0.1), -1);
                        PointF B = new PointF((float)(i + HH[i, 0]), 0);
                        resGrid.Add(GetOneIsoline(A, B, H, SS, HH));
                    }
                }
                for (int i = 0; i < yPaceC; i++)//左边界找线头
                {
                    if(SS[0,i]>=0&&SS[0,i]<=1)
                    {
                        PointF A = new PointF(-1,(float)(i + 0.1));
                        PointF B = new PointF(0, (float)(i + SS[0, i]));
                        resGrid.Add(GetOneIsoline(A, B, H, SS, HH));
                    }
                }
                for (int i = 0; i < xPaceC; i++)//顶边界找线头
                {
                    if (HH[i, yPaceC] >= 0 && HH[i, yPaceC] <= 1)
                    {
                        PointF A = new PointF((float)(i + 0.1), yPaceC + 1);
                        PointF B = new PointF((float)(i + HH[i, yPaceC]), yPaceC);
                        resGrid.Add(GetOneIsoline(A, B, H, SS, HH));
                    }
                }
                for (int i = 0; i < yPaceC; i++)//右边界找线头
                {
                    if(SS[xPaceC,i]>=0&&SS[xPaceC,i]<=1)
                    {
                        PointF A = new PointF(xPaceC+1, (float)(i + 0.1));
                        PointF B = new PointF(xPaceC, (float)(i + SS[xPaceC, i]));
                        resGrid.Add(GetOneIsoline(A, B, H, SS, HH));
                    }
                }
                //找闭合等值线，纵边上找线头
                for(int i=1;i<xPaceC;i++)
                {
                    for(int j=0;j<yPaceC;j++)
                    {
                        if(SS[i,j]>=0&&SS[i,j]<=1)
                        {
                            PointF A = new PointF(i-1, (float)(j + 0.1));
                            PointF B = new PointF(i, (float)(j + SS[i,j]));
                            resGrid.Add(GetOneIsoline(A, B, H, SS, HH));
                        }
                    }
                }
            }
            //绘制等值线
            Graphics g = pictureBox1.CreateGraphics();
            Pen myPen=new Pen(Color.Green);
            foreach(List<PointF> lp in resGrid)
            {
                PointF[] temp = new PointF[lp.Count];
                for (int i = 0; i < lp.Count; i++)
                {
                    PointF tmp2 = new PointF((float)((lp[i].X - minx) / scale + margin), (float)(pictureBox1.Height - margin - (lp[i].Y - miny) / scale));
                    temp[i] = tmp2;
                }
                if (isSmooth)//画光滑等值线
                    g.DrawCurve(myPen, temp);
                else//画不光滑等值线
                    g.DrawLines(myPen, temp);
            }
            myPen.Dispose();
            g.Dispose();
            return;
        }
        /// <summary>
        /// 根据初始线头在格网中追踪一条等值线
        /// </summary>
        /// <param name="AA">初始假想点，确定初始追踪方向</param>
        /// <param name="BB">线头</param>
        /// <param name="H">H矩阵</param>
        /// <param name="SS">SS矩阵</param>
        /// <param name="HH">HH矩阵</param>
        /// <returns>一条等值线</returns>
        public List<PointF> GetOneIsoline(PointF AA,PointF BB,double[,]H,double[,] SS,double[,]HH)
        {
            double xinterval = (pictureBox1.Width - 2 * margin) / xPaceC;
            double yinterval = (pictureBox1.Height - 2 * margin) / yPaceC;
            List<PointF> isol = new List<PointF>();
            PointF A = AA;
            PointF B = BB;
            isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
            //开始追踪
            while (((int)A.Y < (int)B.Y && B.Y < yPaceC) || ((int)A.X < (int)B.X && B.X < xPaceC) || ((int)A.Y >= (int)B.Y && (int)A.X >= (int)B.X && (int)B.X < B.X && B.Y > 0) || ((int)A.Y >= (int)B.Y && (int)A.X >= (int)B.X && (int)B.Y < B.Y && B.X > 0))//追踪没有达到格网边界
            {
                if ((int)A.Y < (int)B.Y)//自下向上追踪
                {
                    if (SS[(int)B.X, (int)B.Y] >= 0 && SS[(int)B.X, (int)B.Y] <= 1)//左边有点
                    {
                        if (SS[(int)B.X + 1, (int)B.Y] >= 0 && SS[(int)B.X + 1, (int)B.Y] <= 1)//三边都有点
                        {
                            double disL = Math.Sqrt(((B.X - (int)B.X) * xinterval) * ((B.X - (int)B.X) * xinterval) + yinterval * yinterval * SS[(int)B.X, (int)B.Y] * SS[(int)B.X, (int)B.Y]);
                            double disR = Math.Sqrt(((1 + (int)B.X - B.X) * xinterval) * ((1 + (int)B.X - B.X) * xinterval) + SS[(int)B.X + 1, (int)B.Y] * yinterval * SS[(int)B.X + 1, (int)B.Y] * yinterval);
                            if (disL > disR)//判断距离左右的距离
                            {
                                PointF tt = new PointF((int)B.X + 1, (float)(B.Y + SS[(int)B.X + 1, (int)B.Y]));
                                A = B;
                                B = tt;
                                isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));                               
                                HH[(int)A.X, (int)A.Y] = 2;
                                continue;
                            }
                            else
                            {
                                PointF tt = new PointF((int)B.X, (float)(B.Y + SS[(int)B.X, (int)B.Y]));
                                A = B;
                                B = tt;
                                isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                                HH[(int)A.X, (int)A.Y] = 2;
                                continue;
                            }                            
                        }
                        else//只有左边有点
                        {
                            PointF tt = new PointF((int)B.X, (float)(B.Y + SS[(int)B.X, (int)B.Y]));
                            A = B;
                            B = tt;
                            isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                            HH[(int)A.X, (int)A.Y] = 2;
                            continue;
                        }
                    }
                    if (SS[(int)B.X + 1, (int)B.Y] >= 0 && SS[(int)B.X + 1, (int)B.Y] <= 1)//只有右边有点
                    {
                        PointF tt = new PointF((int)B.X + 1, (float)(B.Y + SS[(int)B.X + 1, (int)B.Y]));
                        A = B;
                        B = tt;
                        isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                        HH[(int)A.X, (int)A.Y] = 2;
                        continue;
                    }
                    if (HH[(int)B.X, (int)B.Y + 1] >= 0 && HH[(int)B.X, (int)B.Y + 1] <= 1)//只有右边有点
                    {
                        PointF tt = new PointF((int)B.X + (float)HH[(int)B.X, (int)B.Y + 1], (int)B.Y + 1);
                        A = B;
                        B = tt;
                        isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                        HH[(int)A.X, (int)A.Y] = 2;
                        continue;
                    }
                }
                if ((int)A.X < (int)B.X)//从左向右追踪
                {
                    if (HH[(int)B.X, (int)B.Y + 1] >= 0 && HH[(int)B.X, (int)B.Y + 1]<=1)//上边有点
                    {
                        if(HH[(int)B.X,(int)B.Y]>=0&&HH[(int)B.X,(int)B.Y]<=1)//三边都有点
                        {
                            double disd = Math.Sqrt(((B.Y - (int)B.Y) * yinterval) * ((B.Y - (int)B.Y) * yinterval) + xinterval * xinterval * HH[(int)B.X,(int)B.Y] * HH[(int)B.X,(int)B.Y]);
                            double disu = Math.Sqrt(((1 + (int)B.Y - B.Y) * yinterval) * ((1 + (int)B.Y - B.Y) * yinterval) + HH[(int)B.X, (int)B.Y + 1] * xinterval * HH[(int)B.X, (int)B.Y + 1] * xinterval);
                            if (disd > disu)//判断上下边的距离
                            {
                                PointF tt = new PointF((float)(B.X + HH[(int)B.X, (int)B.Y + 1]), (int)B.Y + 1);
                                A = B;
                                B = tt;
                                isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                                SS[(int)A.X, (int)A.Y] = 2;
                                continue;
                            }
                            else
                            {
                                PointF tt = new PointF((float)(B.X + HH[(int)B.X, (int)B.Y]), (int)B.Y);
                                A = B;
                                B = tt;
                                isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                                SS[(int)A.X, (int)A.Y] = 2;
                                continue;
                            }
                        }
                        else//只有上边有点
                        {
                            PointF tt = new PointF((float)(B.X + HH[(int)B.X, (int)B.Y + 1]), (int)B.Y + 1);
                            A = B;
                            B = tt;
                            isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                            SS[(int)A.X, (int)A.Y] = 2;
                            continue;
                        }
                    }
                    if(HH[(int)B.X,(int)B.Y]>=0&&HH[(int)B.X,(int)B.Y]<=1)//只有下边有点
                    {
                        PointF tt = new PointF((float)(B.X + HH[(int)B.X, (int)B.Y]), (int)B.Y);
                        A = B;
                        B = tt;
                        isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                        SS[(int)A.X, (int)A.Y] = 2;
                        continue;
                    }
                    if(SS[(int)B.X+1,(int)B.Y]>=0&&SS[(int)B.X+1,(int)B.Y]<=1)//只有右边有点
                    {
                        PointF tt = new PointF((float)(B.X + 1), (int)B.Y + (float)SS[(int)B.X + 1, (int)B.Y]);
                        A = B;
                        B = tt;
                        isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                        SS[(int)A.X, (int)A.Y] = 2;
                        continue;
                    }
                    
                }
                if ((int)B.X < B.X)//自上向下追踪
                {
                    if (SS[(int)B.X, (int)B.Y - 1] >= 0 && SS[(int)B.X, (int)B.Y - 1]<=1)//左边有点
                    {
                        if(SS[(int)B.X+1,(int)B.Y-1]>=0&&SS[(int)B.X+1,(int)B.Y-1]<=1)//三边都有点
                        {
                            double disL = Math.Sqrt(((B.X - (int)B.X) * xinterval) * ((B.X - (int)B.X) * xinterval) + yinterval * yinterval * (1 - SS[(int)B.X, (int)B.Y - 1]) * (1 - SS[(int)B.X, (int)B.Y - 1]));
                            double disR = Math.Sqrt(((1 + (int)B.X - B.X) * xinterval) * ((1 + (int)B.X - B.X) * xinterval) + (1 - SS[(int)B.X + 1, (int)B.Y - 1]) * yinterval * (1 - SS[(int)B.X + 1, (int)B.Y - 1]) * yinterval);
                            if (disL > disR)//判断连左边还是右边
                            {
                                PointF tt = new PointF((int)B.X + 1, (float)(B.Y-1 + SS[(int)B.X + 1, (int)B.Y-1]));
                                A = B;
                                B = tt;
                                isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                                HH[(int)A.X, (int)A.Y] = 2;
                                continue;
                            }
                            else
                            {
                                PointF tt = new PointF((int)B.X, (float)(B.Y-1 + SS[(int)B.X, (int)B.Y-1]));
                                A = B;
                                B = tt;
                                isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                                HH[(int)A.X, (int)A.Y] = 2;
                                continue;
                            }
                        }
                        else//只有左边有点
                        {
                            PointF tt = new PointF((int)B.X, (float)(B.Y - 1 + SS[(int)B.X, (int)B.Y - 1]));
                            A = B;
                            B = tt;
                            isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                            HH[(int)A.X, (int)A.Y] = 2;
                            continue;
                        }
                    }
                    if (SS[(int)B.X + 1, (int)B.Y - 1] >= 0 && SS[(int)B.X + 1, (int)B.Y - 1] <= 1)//只有右边有点
                    {
                        PointF tt = new PointF((int)B.X + 1, (float)(B.Y - 1 + SS[(int)B.X + 1, (int)B.Y - 1]));
                        A = B;
                        B = tt;
                        isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                        HH[(int)A.X, (int)A.Y] = 2;
                        continue;
                    }
                    if(HH[(int)B.X,(int)B.Y-1]>=0&&HH[(int)B.X,(int)B.Y-1]<=1)//只有下边有点
                    {
                        PointF tt = new PointF((int)B.X + (float)HH[(int)B.X, (int)B.Y - 1], (float)(B.Y - 1));
                        A = B;
                        B = tt;
                        isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                        HH[(int)A.X, (int)A.Y] = 2;
                        continue;
                    }
                }
                if ((int)B.Y < B.Y)//自右向左追踪
                {
                    if (HH[(int)B.X - 1, (int)B.Y + 1] >= 0 && HH[(int)B.X - 1, (int)B.Y + 1]<=1)//上边有点
                    {
                        if(HH[(int)B.X-1,(int)B.Y]>=0&&HH[(int)B.X-1,(int)B.Y]<=1)//三边都有点
                        {
                            double disd = Math.Sqrt(((B.Y - (int)B.Y) * yinterval) * ((B.Y - (int)B.Y) * yinterval) + xinterval * xinterval * (1 - HH[(int)B.X - 1, (int)B.Y]) * (1 - HH[(int)B.X - 1, (int)B.Y]));
                            double disu = Math.Sqrt(((1 + (int)B.Y - B.Y) * yinterval) * ((1 + (int)B.Y - B.Y) * yinterval) + (1 - HH[(int)B.X - 1, (int)B.Y + 1]) * xinterval * (1 - HH[(int)B.X - 1, (int)B.Y + 1]) * xinterval);
                            if (disd > disu)//判断该连上边还是下边
                            {
                                PointF tt = new PointF((float)(B.X-1 + HH[(int)B.X-1, (int)B.Y + 1]), (int)B.Y + 1);
                                A = B;
                                B = tt;
                                isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                                SS[(int)A.X, (int)A.Y] = 2;
                                continue;
                            }
                            else
                            {
                                PointF tt = new PointF((float)(B.X - 1 + HH[(int)B.X - 1, (int)B.Y]), (int)B.Y);
                                A = B;
                                B = tt;
                                isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                                SS[(int)A.X, (int)A.Y] = 2;
                                continue;
                            }
                        }
                        else//只有上边有点
                        {
                            PointF tt = new PointF((float)(B.X - 1 + HH[(int)B.X - 1, (int)B.Y + 1]), (int)B.Y + 1);
                            A = B;
                            B = tt;
                            isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                            SS[(int)A.X, (int)A.Y] = 2;
                            continue;
                        }                        
                    }
                    if (HH[(int)B.X - 1, (int)B.Y] >= 0 && HH[(int)B.X - 1, (int)B.Y] <= 1)//只有下边有点
                    {
                        PointF tt = new PointF((float)(B.X - 1 + HH[(int)B.X - 1, (int)B.Y]), (int)B.Y);
                        A = B;
                        B = tt;
                        isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                        SS[(int)A.X, (int)A.Y] = 2;
                        continue;
                    }
                    if (SS[(int)B.X - 1, (int)B.Y] >= 0 && SS[(int)B.X - 1, (int)B.Y]<=1)//只有左边有点
                    {
                        PointF tt = new PointF((float)(B.X - 1), (int)B.Y + (float)SS[(int)B.X - 1, (int)B.Y]);
                        A = B;
                        B = tt;
                        isol.Add(new PointF((float)(B.X * xinterval * scale + minx), (float)(B.Y * yinterval * scale + miny)));
                        SS[(int)A.X, (int)A.Y] = 2;
                        continue;
                    }
                }
                //如果四种情况都不是，说明是闭曲线，加入开始的起点，追踪完毕
                isol.Add(new PointF(isol[0].X,isol[0].Y));
                break;
            }
            //追踪的最后一段曲线的HH或者SS矩阵赋值为2
            if(B.X-(int)B.X==0)
                SS[(int)B.X,(int)B.Y]=2;
            else
                HH[(int)B.X, (int)B.Y] = 2;
            return isol;
        }

        private void 生成等值线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TINFlag == false)
            {
                MessageBox.Show("还未生成三角网！");
                return;
            }
            AdvancedGIS.DialogForm.IsolineParasDialog ipd = new DialogForm.IsolineParasDialog(this,false);
            ipd.Show(this);
        }
        /// <summary>
        /// TIN模型生成等值线
        /// </summary>
        /// <param name="val">等值线起始值</param>
        /// <param name="IsoInterval">等值线间隔</param>
        /// <param name="isSmooth">是否光滑</param>
        public void generateIsoLineTIN(double val, double IsoInterval,bool isSmooth)
        {
            List<List<PointF>> res = new List<List<PointF>>();//存储结果
            //计算区域内存在的值最小的等值线
            double HMax = double.MinValue, HMin = double.MaxValue;
            for (int i = 0; i<data.Count; i++)
            {
                if (HMax < data[i].getZ())
                    HMax = data[i].getZ();
                if (HMin > data[i].getZ())
                    HMin = data[i].getZ();
            }
            if (val > HMax)
            {
                if ((int)((val - HMax) / IsoInterval) == (int)((val - HMin) / IsoInterval) && (int)((val - HMax) / IsoInterval) != (val - HMax) / IsoInterval)
                    return;
                else
                {
                    while (val > HMin)
                    {
                        val = val - IsoInterval;
                    }
                    val = val + IsoInterval;
                }
            }
            else if (val < HMin)
            {
                if ((int)((HMin - val) / IsoInterval) == (int)((HMax - val) / IsoInterval) && (int)((HMin - val) / IsoInterval) != (HMin - val) / IsoInterval)
                    return;
                else
                {
                    while (val < HMin)
                    {
                        val = val + IsoInterval;
                    }
                }
            }
            else
            {
                while (val > HMin)
                {
                    val = val - IsoInterval;
                }
                val = val + IsoInterval;
            }
            //对每个等值线的值追踪等值线
            for (double HZ = val; HZ <= HMax; HZ = HZ + IsoInterval)
            {
                //计算HH矩阵
                double[] HH = new double[triArc.Count];
                for(int i=0;i<triArc.Count;i++)
                {
                    HH[i]=(HZ-data[triArc[i].startPoint].getZ())/(data[triArc[i].endPoint].getZ()-data[triArc[i].startPoint].getZ());
                    if(HH[i]==1)
                        HH[i]=0.9999;
                    else if(HH[i]==0)
                        HH[i]=0.0001;
                    else if(HH[i]>1||HH[i]<0)
                        HH[i]=2;
                }

                //找开曲线线头
                for(int i=0;i<TINConvex.Count;i++)
                {
                    if(HH[TINConvex[i]]<=1&&HH[TINConvex[i]]>=0)
                    {
                        res.Add(GetOneIsolineTin(TINConvex[i], HH));
                    }
                }
                //找闭曲线线头
                for(int i=0;i<triArc.Count;i++)
                {
                    if(HH[i]>=0&&HH[i]<=1)
                    {
                        res.Add(GetOneIsolineTin(i, HH));
                    }
                }
            }
            //绘制等值线
            Graphics g = pictureBox1.CreateGraphics();
            Pen myPen = new Pen(Color.Red);
            foreach (List<PointF> lp in res)
            {
                PointF[] temp = new PointF[lp.Count];
                for (int i = 0; i < lp.Count; i++)
                {
                    PointF tmp2 = new PointF((float)((lp[i].X - minx) / scale + margin), (float)(pictureBox1.Height - margin - (lp[i].Y - miny) / scale));
                    temp[i] = tmp2;
                }
                if (isSmooth)//绘制光滑等值线
                    g.DrawCurve(myPen, temp);
                else//不光滑等值线
                    g.DrawLines(myPen, temp);
            }
            myPen.Dispose();
            g.Dispose();
            return;
        }

        private PointF[] splin(PointF p1,PointF p2,List<PointF> lp)
        {
            PointF[]res=new PointF[50];

            return res;
        }
        /// <summary>
        /// 根据线头追踪一条等值线
        /// </summary>
        /// <param name="arcStart">线头</param>
        /// <param name="HH">HH矩阵</param>
        /// <returns>追踪的等值线</returns>
        public List<PointF> GetOneIsolineTin(int arcStart,double[]HH)
        {
            List<PointF> res = new List<PointF>();//存结果
            List<int> queue=new List<int>();//追踪队列
            queue.Add(arcStart);
            res.Add(new PointF((float)(data[triArc[arcStart].startPoint].getX()+HH[arcStart]*(data[triArc[arcStart].endPoint].getX()-data[triArc[arcStart].startPoint].getX())),(float)(data[triArc[arcStart].startPoint].getY()+HH[arcStart]*(data[triArc[arcStart].endPoint].getY()-data[triArc[arcStart].startPoint].getY()))));
            HH[arcStart]=2;
            while(queue.Count>0)//追踪队列不空，追踪未结束
            {
                bool ischanged=false;
                //找边所在的三角形
                if(triArc[queue[0]].tri[0]!=-1)
                {
                    Triangle tmpT=TINRes[triArc[queue[0]].tri[0]];
                    //分别找三角形三边是否存在当前点。若有则加入等值线，，所在边加入队列
                    if(HH[tmpT.arcID1]>=0&&HH[tmpT.arcID1]<=1)
                    {
                        queue.Add(tmpT.arcID1);
                        res.Add(new PointF((float)(data[triArc[tmpT.arcID1].startPoint].getX() + HH[tmpT.arcID1] * (data[triArc[tmpT.arcID1].endPoint].getX() - data[triArc[tmpT.arcID1].startPoint].getX())), (float)(data[triArc[tmpT.arcID1].startPoint].getY() + HH[tmpT.arcID1] * (data[triArc[tmpT.arcID1].endPoint].getY() - data[triArc[tmpT.arcID1].startPoint].getY()))));
                        HH[tmpT.arcID1] = 2;
                        ischanged=true;
                    }
                    else if (HH[tmpT.arcID2] >= 0 && HH[tmpT.arcID2] <= 1)
                    {
                        queue.Add(tmpT.arcID2);
                        res.Add(new PointF((float)(data[triArc[tmpT.arcID2].startPoint].getX() + HH[tmpT.arcID2] * (data[triArc[tmpT.arcID2].endPoint].getX() - data[triArc[tmpT.arcID2].startPoint].getX())), (float)(data[triArc[tmpT.arcID2].startPoint].getY() + HH[tmpT.arcID2] * (data[triArc[tmpT.arcID2].endPoint].getY() - data[triArc[tmpT.arcID2].startPoint].getY()))));
                        HH[tmpT.arcID2] = 2;
                        ischanged=true;
                    }
                    else if (HH[tmpT.arcID3] >= 0 && HH[tmpT.arcID3] <= 1)
                    {
                        queue.Add(tmpT.arcID3);
                        res.Add(new PointF((float)(data[triArc[tmpT.arcID3].startPoint].getX() + HH[tmpT.arcID3] * (data[triArc[tmpT.arcID3].endPoint].getX() - data[triArc[tmpT.arcID3].startPoint].getX())), (float)(data[triArc[tmpT.arcID3].startPoint].getY() + HH[tmpT.arcID3] * (data[triArc[tmpT.arcID3].endPoint].getY() - data[triArc[tmpT.arcID3].startPoint].getY()))));
                        HH[tmpT.arcID3] = 2;
                        ischanged=true;
                    }
                }
                if(triArc[queue[0]].tri[1]!=-1)
                {
                    Triangle tmpT = TINRes[triArc[queue[0]].tri[1]];
                    //分别找三角形三边是否存在当前点。若有则加入等值线，，所在边加入队列
                    if (HH[tmpT.arcID1] >= 0 && HH[tmpT.arcID1] <= 1)
                    {
                        queue.Add(tmpT.arcID1);
                        res.Add(new PointF((float)(data[triArc[tmpT.arcID1].startPoint].getX() + HH[tmpT.arcID1] * (data[triArc[tmpT.arcID1].endPoint].getX() - data[triArc[tmpT.arcID1].startPoint].getX())), (float)(data[triArc[tmpT.arcID1].startPoint].getY() + HH[tmpT.arcID1] * (data[triArc[tmpT.arcID1].endPoint].getY() - data[triArc[tmpT.arcID1].startPoint].getY()))));
                        HH[tmpT.arcID1] = 2;
                        ischanged=true;
                    }
                    else if (HH[tmpT.arcID2] >= 0 && HH[tmpT.arcID2] <= 1)
                    {
                        queue.Add(tmpT.arcID2);
                        res.Add(new PointF((float)(data[triArc[tmpT.arcID2].startPoint].getX() + HH[tmpT.arcID2] * (data[triArc[tmpT.arcID2].endPoint].getX() - data[triArc[tmpT.arcID2].startPoint].getX())), (float)(data[triArc[tmpT.arcID2].startPoint].getY() + HH[tmpT.arcID2] * (data[triArc[tmpT.arcID2].endPoint].getY() - data[triArc[tmpT.arcID2].startPoint].getY()))));
                        HH[tmpT.arcID2] = 2;
                        ischanged=true;
                    }
                    else if (HH[tmpT.arcID3] >= 0 && HH[tmpT.arcID3] <= 1)
                    {
                        queue.Add(tmpT.arcID3);
                        res.Add(new PointF((float)(data[triArc[tmpT.arcID3].startPoint].getX() + HH[tmpT.arcID3] * (data[triArc[tmpT.arcID3].endPoint].getX() - data[triArc[tmpT.arcID3].startPoint].getX())), (float)(data[triArc[tmpT.arcID3].startPoint].getY() + HH[tmpT.arcID3] * (data[triArc[tmpT.arcID3].endPoint].getY() - data[triArc[tmpT.arcID3].startPoint].getY()))));
                        HH[tmpT.arcID3] = 2;
                        ischanged=true;
                    }
                }
                if (triArc[queue[0]].tri[1] != -1 && triArc[queue[0]].tri[0] != -1&&ischanged==false)//闭曲线
                {
                    res.Add(new PointF(res[0].X,res[0].Y));
                }
                queue.RemoveAt(0);
            }
            return res;
        }

        private void 生成凸包ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (data.Count == 0)
            {
                MessageBox.Show("未导入点数据！");
                return;
            }
            pictureBox1.Refresh();
            DrawOriginData();

            List<int> convex = AdvancedGIS.algorithm.TINGenerator.generateConvex(data);
            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush myBrush = new SolidBrush(Color.Red);
            Pen myPen = new Pen(Color.Red);
            for (int i = 0; i < convex.Count; i++)
            {
                Dt tmp = CoordinateConvert(data[convex[i]]);
                g.FillEllipse(myBrush, new RectangleF((float)tmp.getX() - 2, (float)tmp.getY() - 2, 5, 5));
                g.DrawLine(myPen, new PointF((float)tmp.getX(), (float)tmp.getY()), new PointF((float)CoordinateConvert(data[convex[(i + 1) % convex.Count]]).getX(), (float)CoordinateConvert(data[convex[(i + 1) % convex.Count]]).getY()));
            }
        }

        private void 建立拓扑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridIsoline == false)
                MessageBox.Show("没有利用格网生成的等值线！");
            gridFlag = false;
            mp.Clear();
            ma.Clear();
            mpol.Clear();
            initData();//初始化基础的点和线
            breakLine();//线之间两两打断
            for(int i = 0; i < ma.Count;i++)//给所有点生成拓扑关系
            {
                mp[ma[i].startPoint].lineNum++;
                mp[ma[i].startPoint].relLine.Add(i);
                mp[ma[i].endPoint].lineNum++;
                mp[ma[i].endPoint].relLine.Add(-i-1);
                for(int j=0;j<ma[i].midPointNum;j++)
                {
                    mp[ma[i].midPoint[j]].lineNum++;
                    mp[ma[i].midPoint[j]].relLine.Add(i);
                }
            }
            trackPolygon();//追踪多边形并生成拓扑
            showTopo();//显示拓扑生成结果，用不同颜色表示不同多边形
            topo = true;
        }
        /// <summary>
        /// 加入原始点和线
        /// </summary>
        private void initData()
        {
            //加入点集外包矩形等的基础数据
            double EnvMaxx = double.MinValue, EnvMinx = double.MaxValue, EnvMaxy = double.MinValue, EnvMiny = double.MaxValue;
            for (int i = 0; i < data.Count; i++)
            {
                if (EnvMaxx < data[i].getX())
                    EnvMaxx = data[i].getX();
                if (EnvMinx > data[i].getX())
                    EnvMinx = data[i].getX();
                if (EnvMaxy < data[i].getY())
                    EnvMaxy = data[i].getY();
                if (EnvMiny > data[i].getY())
                    EnvMiny = data[i].getY();
            }
            mp.Add(new MyPoint(EnvMinx, EnvMaxy));
            mp.Add(new MyPoint(0.5 * (EnvMinx + EnvMaxx), EnvMaxy));
            mp.Add(new MyPoint(EnvMaxx, EnvMaxy));
            mp.Add(new MyPoint(EnvMinx, 0.5 * (EnvMaxy + EnvMiny)));
            mp.Add(new MyPoint(EnvMaxx, 0.5 * (EnvMaxy + EnvMiny)));
            mp.Add(new MyPoint(EnvMinx, EnvMiny));
            mp.Add(new MyPoint(0.5 * (EnvMinx + EnvMaxx), EnvMiny));
            mp.Add(new MyPoint(EnvMaxx, EnvMiny));

            List<int> arctmp = new List<int>();
            arctmp.Add(0);
            arctmp.Add(2);
            ma.Add(new MyArc(arctmp));
            arctmp.Clear();
            arctmp.Add(0);
            arctmp.Add(5);
            ma.Add(new MyArc(arctmp));
            arctmp.Clear();
            arctmp.Add(2);
            arctmp.Add(7);
            ma.Add(new MyArc(arctmp));
            arctmp.Clear();
            arctmp.Add(5);
            arctmp.Add(7);
            ma.Add(new MyArc(arctmp));
            arctmp.Clear();
            arctmp.Add(0);
            arctmp.Add(7);
            ma.Add(new MyArc(arctmp));
            arctmp.Clear();
            arctmp.Add(2);
            arctmp.Add(5);
            ma.Add(new MyArc(arctmp));
            arctmp.Clear();
            arctmp.Add(3);
            arctmp.Add(4);
            ma.Add(new MyArc(arctmp));
            arctmp.Clear();
            arctmp.Add(1);
            arctmp.Add(6);
            ma.Add(new MyArc(arctmp));
            //加入格网生成的等值线的基础数据
            for (int i = 0; i < resGrid.Count; i++)
            {
                arctmp.Clear();
                for (int j = 0; j < resGrid[i].Count - 1; j++)
                {
                    arctmp.Add(addOnePointToMP(new MyPoint(resGrid[i][j].X, resGrid[i][j].Y)));
                }
                if (resGrid[i][resGrid[i].Count - 1].X == resGrid[i][0].X && resGrid[i][resGrid[i].Count - 1].Y == resGrid[i][0].Y)
                {
                    arctmp.Add(arctmp[0]);
                }
                else
                {
                    arctmp.Add(addOnePointToMP(new MyPoint(resGrid[i][resGrid[i].Count - 1].X, resGrid[i][resGrid[i].Count - 1].Y)));
                }
                ma.Add(new MyArc(arctmp));
            }
        }
        /// <summary>
        /// 线段之间两两打断
        /// </summary>
        private void breakLine()
        {
            for (int i = 0; i < ma.Count - 1; i++)
            {
                for (int j = i + 1; j < ma.Count; j++)
                {
                    if (ma[i].midPointNum+2 == 2)
                    {
                        if (ma[j].midPointNum+2 == 2)
                            j=breakTwoTwo(i, j);//两点线段之间打断
                        else
                            j=breakTwoMore(i, j);//两点和多点
                    }
                    else
                    {
                        if (ma[j].midPointNum+2 == 2)
                            j = breakTwoMore(j, i);//多点和两点
                        else
                            j=breakMoreMore(i, j);//多点和多点
                    }
                }
            }
        }
       
        private int breakTwoTwo(int i,int j)
        {
            List<int> pArray = new List<int>();
            MyPoint p1 = mp[ma[i].startPoint], p2 = mp[ma[i].endPoint],p3 = mp[ma[j].startPoint],p4 = mp[ma[j].endPoint];
            MyPoint tres = CalIntersect(p1, p2, p3, p4);
            if (tres != null)
            {
                if (!isEqual(tres, p1) && !isEqual(tres, p2))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        int indexMP = addOnePointToMP(tres);
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        return i;
                    }
                    else if (isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].endPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        return i;
                    }
                }
                else if (isEqual(tres, p2))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].endPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        return j-1;
                    }
                }
                else
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        return j - 1;
                    }
                }
            }
            return j;
        }

        private int breakTwoMore(int i,int j)
        {
            List<int> pArray = new List<int>();
            MyPoint p1 = mp[ma[i].startPoint], p2 = mp[ma[i].endPoint], p3, p4;
            p3 = mp[ma[j].startPoint];
            p4 = mp[ma[j].midPoint[0]];
            MyPoint tres = CalIntersect(p1, p2, p3, p4);
            if (tres != null)//两点对开头
            {
                if (!isEqual(tres, p1) && !isEqual(tres, p2))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        int indexMP = addOnePointToMP(tres);
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                        {
                            pArray.Add(ma[j].midPoint[k]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        if (i < j)
                        {
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else
                        {
                            ma.RemoveAt(i);
                            ma.RemoveAt(j);
                            return j;
                        } 
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        if (i < j)
                            return i;
                        else
                            return i - 1;
                    }
                }
                else if (isEqual(tres, p1))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                        {
                            pArray.Add(ma[j].midPoint[k]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        if (i < j)
                            return j - 1;
                        else
                            return j;
                    }
                }
                else
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].endPoint);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                        {
                            pArray.Add(ma[j].midPoint[k]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        if (i < j)
                            return j - 1;
                        else
                            return j;
                    }
                }
            }

            p3 = mp[ma[j].midPoint[ma[j].midPointNum - 1]];
            p4 = mp[ma[j].endPoint];
            tres = CalIntersect(p1, p2, p3, p4);
            if (tres != null)//两点与后面
            {
                if (!isEqual(tres, p1) && !isEqual(tres, p2))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        int indexMP = addOnePointToMP(tres);
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                        {
                            pArray.Add(ma[j].midPoint[k]);
                        }
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        if (i < j)
                        {
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else
                        {
                            ma.RemoveAt(i);
                            ma.RemoveAt(j);
                            return j;
                        }
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                        {
                            pArray.Add(ma[j].midPoint[k]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        if (i < j)
                        {
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else
                        {
                            ma.RemoveAt(i);
                            ma.RemoveAt(j);
                            return j;
                        }
                    }
                    else
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].endPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        if (i < j)
                            return i;
                        else
                            return i - 1;
                    }
                }
                else if (isEqual(tres, p1))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                        {
                            pArray.Add(ma[j].midPoint[k]);
                        }
                        pArray.Add(ma[i].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        if (i < j)
                            return j - 1;
                        else
                            return j;
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                            pArray.Add(ma[j].midPoint[k]);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        if (i < j)
                            return j - 1;
                        else
                            return j;
                    }
                }
                else
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                        {
                            pArray.Add(ma[j].midPoint[k]);
                        }
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].endPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        if (i < j)
                            return j - 1;
                        else
                            return j;
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int k = 0; k < ma[j].midPointNum; k++)
                            pArray.Add(ma[j].midPoint[k]);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        if (i < j)
                            return j - 1;
                        else
                            return j;
                    }
                }
            }
            //两个点和中间
            for (int s = 0; s < ma[j].midPointNum - 1; s++)
            {
                p3 = mp[ma[j].midPoint[s]];
                p4 = mp[ma[j].midPoint[s + 1]];
                tres = CalIntersect(p1, p2, p3, p4);
                if (tres != null)
                {
                    if (!isEqual(tres, p1) && !isEqual(tres, p2))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            int indexMP = addOnePointToMP(tres);
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            pArray.Add(indexMP);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(indexMP);
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int k = 0; k < s + 1; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(indexMP);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(indexMP);
                            for (int k = s + 1; k < ma[j].midPointNum; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            if (i < j)
                            {
                                ma.RemoveAt(j);
                                ma.RemoveAt(i);
                                return i;
                            }
                            else
                            {
                                ma.RemoveAt(i);
                                ma.RemoveAt(j);
                                return j;
                            }
                        }
                        else if (isEqual(tres, p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            pArray.Add(ma[j].midPoint[s]);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].midPoint[s]);
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int k = 0; k < s + 1; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int k = s; k < ma[j].midPointNum; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            if (i < j)
                            {
                                ma.RemoveAt(j);
                                ma.RemoveAt(i);
                                return i;
                            }
                            else
                            {
                                ma.RemoveAt(i);
                                ma.RemoveAt(j);
                                return j;
                            }
                        }
                    }
                    else if (isEqual(tres, p1))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int k = 0; k < s + 1; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(ma[i].startPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int k = s + 1; k < ma[j].midPointNum; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            if (i < j)
                                return j - 1;
                            else
                                return j;
                        }
                        else if (isEqual(tres, p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int k = 0; k < s + 1; k++)
                                pArray.Add(ma[j].midPoint[k]);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int k = s ; k < ma[j].midPointNum; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            if (i < j)
                                return j - 1;
                            else
                                return j;
                        }
                    }
                    else
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int k = 0; k < s + 1; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].endPoint);
                            for (int k = s + 1; k < ma[j].midPointNum; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            if (i < j)
                                return j - 1;
                            else
                                return j;
                        }
                        else if (isEqual(tres, p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int k = 0; k < s + 1; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int k = s; k < ma[j].midPointNum; k++)
                            {
                                pArray.Add(ma[j].midPoint[k]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            if (i < j)
                                return j - 1;
                            else
                                return j;
                        }
                    }
                }
            }
            if(i<j)
                return j;
            else
                return i;
        }
        
        //private int breakMoreTwo(int i,int j)
        //{
        //    return j;
        //}
        private int breakMoreMore(int i,int j)
        {
            List<int> pArray = new List<int>();
            //开头
            MyPoint p1 = mp[ma[i].startPoint], p2 = mp[ma[i].midPoint[0]], p3, p4;
            p3 = mp[ma[j].startPoint];
            p4 = mp[ma[j].midPoint[0]];
            MyPoint tres = CalIntersect(p1, p2, p3, p4);
            if(tres!=null)//开头和开头
            {
                if(!isEqual(tres,p1)&&!isEqual(tres,p2))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        int indexMP = addOnePointToMP(tres);
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        for (int s = 0; s < ma[i].midPointNum;s++)
                        {
                            pArray.Add(ma[i].midPoint[s]);
                        }
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        for (int s = 0; s < ma[j].midPointNum; s++)
                        {
                            pArray.Add(ma[j].midPoint[s]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int s = 0; s < ma[i].midPointNum; s++)
                        {
                            pArray.Add(ma[i].midPoint[s]);
                        }
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        return i;
                    }
                }
                else if(isEqual(tres,p1))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int s = 0; s < ma[j].midPointNum; s++)
                        {
                            pArray.Add(ma[j].midPoint[s]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        return j - 1;
                    }
                }
            }

            p3 = mp[ma[j].midPoint[ma[j].midPointNum-1]];
            p4 = mp[ma[j].endPoint];
            tres = CalIntersect(p1, p2, p3, p4);
            if (tres != null)//开头和结尾
            {
                if (!isEqual(tres, p1) && !isEqual(tres, p2))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        int indexMP = addOnePointToMP(tres);
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        for (int s = 0; s < ma[i].midPointNum; s++)
                        {
                            pArray.Add(ma[i].midPoint[s]);
                        }
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int s = 0; s < ma[j].midPointNum; s++)
                        {
                            pArray.Add(ma[j].midPoint[s]);
                        }
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        for (int s = 0; s < ma[i].midPointNum; s++)
                        {
                            pArray.Add(ma[i].midPoint[s]);
                        }
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int s = 0; s < ma[j].midPointNum; s++)
                        {
                            pArray.Add(ma[j].midPoint[s]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum-1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].endPoint);
                        for (int s = 0; s < ma[i].midPointNum; s++)
                        {
                            pArray.Add(ma[i].midPoint[s]);
                        }
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        return i;
                    }
                }
                else if (isEqual(tres, p1))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int s = 0; s < ma[j].midPointNum; s++)
                        {
                            pArray.Add(ma[j].midPoint[s]);
                        }
                        pArray.Add(ma[i].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        return j - 1;
                    }
                    else if(isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int s = 0; s < ma[j].midPointNum; s++)
                        {
                            pArray.Add(ma[j].midPoint[s]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum-1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        return j - 1;
                    }
                }
            }
            for (int s = 0; s < ma[j].midPointNum - 1;s++)
            {
                p3 = mp[ma[j].midPoint[s]];
                p4 = mp[ma[j].midPoint[s+1]];
                tres = CalIntersect(p1, p2, p3, p4);
                if (tres != null)//开头和中间
                {
                    if (!isEqual(tres, p1) && !isEqual(tres, p2))
                    {
                         if (!isEqual(tres, p3) && !isEqual(tres, p4))
                         {
                             int indexMP = addOnePointToMP(tres);
                             pArray.Clear();
                             pArray.Add(ma[i].startPoint);
                             pArray.Add(indexMP);
                             ma.Add(new MyArc(pArray));
                             pArray.Clear();
                             pArray.Add(indexMP);
                             for (int ss = 0; ss < ma[i].midPointNum; ss++)
                             {
                                 pArray.Add(ma[i].midPoint[ss]);
                             }
                             pArray.Add(ma[i].endPoint);
                             ma.Add(new MyArc(pArray));
                             pArray.Clear();
                             pArray.Add(ma[j].startPoint);
                             for (int ss = 0; ss < s+1; ss++)
                             {
                                 pArray.Add(ma[j].midPoint[ss]);
                             }
                             pArray.Add(indexMP);
                             ma.Add(new MyArc(pArray));
                             pArray.Clear();
                             pArray.Add(indexMP);
                             for (int ss = s+1; ss < ma[j].midPointNum; ss++)
                             {
                                 pArray.Add(ma[j].midPoint[ss]);
                             }
                             pArray.Add(ma[j].endPoint);
                             ma.Add(new MyArc(pArray));
                             ma.RemoveAt(j);
                             ma.RemoveAt(i);
                             return i;
                         }
                         else if(isEqual(tres, p3))
                         {
                             pArray.Clear();
                             pArray.Add(ma[i].startPoint);
                             pArray.Add(ma[j].midPoint[s]);
                             ma.Add(new MyArc(pArray));
                             pArray.Clear();
                             pArray.Add(ma[j].midPoint[s]);
                             for (int ss = 0; ss < ma[i].midPointNum; ss++)
                             {
                                 pArray.Add(ma[i].midPoint[ss]);
                             }
                             pArray.Add(ma[i].endPoint);
                             ma.Add(new MyArc(pArray));
                             pArray.Clear();
                             pArray.Add(ma[j].startPoint);
                             for (int ss = 0; ss < s + 1; ss++)
                             {
                                 pArray.Add(ma[j].midPoint[ss]);
                             }
                             ma.Add(new MyArc(pArray));
                             pArray.Clear();
                             for (int ss = s + 1; ss < ma[j].midPointNum; ss++)
                             {
                                 pArray.Add(ma[j].midPoint[ss]);
                             }
                             pArray.Add(ma[j].endPoint);
                             ma.Add(new MyArc(pArray));
                             ma.RemoveAt(j);
                             ma.RemoveAt(i);
                             return i;
                         }
                    }
                    else if(isEqual(tres, p1))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {  
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int ss = 0; ss < s + 1; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[i].startPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int ss = s + 1; ss < ma[j].midPointNum; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            return j-1;
                        }
                        else if (isEqual(tres, p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int ss = 0; ss < s + 1; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int ss = s + 1; ss < ma[j].midPointNum; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            return j - 1;
                        }
                    }
                }
            }
            //尾部
            p1 = mp[ma[i].midPoint[ma[i].midPointNum - 1]];
            p2 = mp[ma[i].endPoint];
            p3 = mp[ma[j].startPoint];
            p4 = mp[ma[j].midPoint[0]];
            tres = CalIntersect(p1, p2, p3, p4);
            if (tres != null)//尾部对开头
            {
                if (!isEqual(tres, p1) && !isEqual(tres, p2))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        int indexMP = addOnePointToMP(tres);
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else if(isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        pArray.Add(ma[j].startPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        return i;
                    }
                }
                else if(isEqual(tres, p1))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum-1]);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        return i;
                    }
                }
                else
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].endPoint);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        return j - 1;
                    }
                }
            }
            p3 = mp[ma[j].midPoint[ma[j].midPointNum - 1]];
            p4 = mp[ma[j].endPoint];
            tres = CalIntersect(p1, p2, p3, p4);
            if (tres != null)//尾部和尾部
            {
                if (!isEqual(tres, p1) && !isEqual(tres, p2))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        int indexMP = addOnePointToMP(tres);
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        pArray.Add(indexMP);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(indexMP);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else if(isEqual(tres,p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum-1]);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].endPoint);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        return i;
                    }
                }
                else if(isEqual(tres,p1))
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum-1]);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        ma.RemoveAt(i);
                        return i;
                    }
                    else
                    {
                        pArray.Clear();
                        pArray.Add(ma[i].startPoint);
                        for (int ss = 0; ss < ma[i].midPointNum; ss++)
                        {
                            pArray.Add(ma[i].midPoint[ss]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(i);
                        return i;
                    }
                }
                else
                {
                    if (!isEqual(tres, p3) && !isEqual(tres, p4))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        pArray.Add(ma[i].endPoint);
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[i].endPoint);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        return j-1;
                    }
                    else if (isEqual(tres, p3))
                    {
                        pArray.Clear();
                        pArray.Add(ma[j].startPoint);
                        for (int ss = 0; ss < ma[j].midPointNum; ss++)
                        {
                            pArray.Add(ma[j].midPoint[ss]);
                        }
                        ma.Add(new MyArc(pArray));
                        pArray.Clear();
                        pArray.Add(ma[j].midPoint[ma[j].midPointNum-1]);
                        pArray.Add(ma[j].endPoint);
                        ma.Add(new MyArc(pArray));
                        ma.RemoveAt(j);
                        return j - 1;
                    }
                }
            }
            //后面对中间
            for (int s = 0; s < ma[j].midPointNum - 1; s++)
            {
                p3 = mp[ma[j].midPoint[s]];
                p4 = mp[ma[j].midPoint[s + 1]];
                tres = CalIntersect(p1, p2, p3, p4);
                if (tres != null)
                {
                    if (!isEqual(tres, p1) && !isEqual(tres, p2))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            int indexMP = addOnePointToMP(tres);
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int ss = 0; ss < ma[i].midPointNum; ss++)
                            {
                                pArray.Add(ma[i].midPoint[ss]);
                            }
                            pArray.Add(indexMP);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(indexMP);
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int ss = 0; ss < s+1; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(indexMP);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(indexMP); 
                            for (int ss = s+1; ss < ma[j].midPointNum; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[j].endPoint); 
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else if (isEqual(tres, p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int ss = 0; ss < ma[i].midPointNum; ss++)
                            {
                                pArray.Add(ma[i].midPoint[ss]);
                            }
                            pArray.Add(ma[j].midPoint[s]);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].midPoint[s]);
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int ss = 0; ss < s + 1; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int ss = s; ss < ma[j].midPointNum; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                    }
                    else if(isEqual(tres,p1))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int ss = 0; ss < ma[i].midPointNum; ss++)
                            {
                                pArray.Add(ma[i].midPoint[ss]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].midPoint[ma[i].midPointNum-1]);
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int ss = 0; ss < s + 1; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                            for (int ss = s + 1; ss < ma[j].midPointNum; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else if (isEqual(tres, p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int ss = 0; ss < ma[i].midPointNum; ss++)
                            {
                                pArray.Add(ma[i].midPoint[ss]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].midPoint[ma[i].midPointNum - 1]);
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int ss = 0; ss < s + 1; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int ss = s; ss < ma[j].midPointNum; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                    }
                    else
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int ss = 0; ss < s + 1; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].endPoint);
                            for (int ss = s + 1; ss < ma[j].midPointNum; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            return j - 1;
                        }
                        else if (isEqual(tres, p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int ss = 0; ss < s + 1; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int ss = s; ss < ma[j].midPointNum; ss++)
                            {
                                pArray.Add(ma[j].midPoint[ss]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            return j - 1;
                        }
                    }
                }
            }

            //还未检查
            //中间
            for (int ss = 0; ss < ma[i].midPointNum-1;ss++ )
            {
                p1 = mp[ma[i].midPoint[ss]];
                p2 = mp[ma[i].midPoint[ss+1]];
                p3 = mp[ma[j].startPoint];
                p4 = mp[ma[j].midPoint[0]];
                tres = CalIntersect(p1, p2, p3, p4);
                if (tres != null)//中间和开头
                {
                    if (!isEqual(tres, p1) && !isEqual(tres, p2))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            int indexMP = addOnePointToMP(tres);
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss+1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(indexMP);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(indexMP);
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            pArray.Add(indexMP);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(indexMP);
                            for (int s = 0; s < ma[j].midPointNum; s++)
                            {
                                pArray.Add(ma[j].midPoint[s]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else if(isEqual(tres,p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss + 1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[j].startPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(i);
                            return i;
                        }
                    }
                    else if(isEqual(tres,p1))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss+1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            pArray.Add(ma[i].midPoint[ss]);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].midPoint[ss]);
                            for (int s = 0; s < ma[j].midPointNum; s++)
                            {
                                pArray.Add(ma[j].midPoint[s]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else if(isEqual(tres,p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss + 1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(i);
                            return i;
                        }
                    }
                }
                p3 = mp[ma[j].midPoint[ma[j].midPointNum - 1]];
                p4 = mp[ma[j].endPoint];
                tres = CalIntersect(p1, p2, p3, p4);
                if (tres != null)//中间和结尾
                {
                    if (!isEqual(tres, p1) && !isEqual(tres, p2))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            int indexMP = addOnePointToMP(tres);
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss + 1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(indexMP);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(indexMP);
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int s = 0; s < ma[j].midPointNum; s++)
                            {
                                pArray.Add(ma[j].midPoint[s]);
                            }
                            pArray.Add(indexMP);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(indexMP);
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else if (isEqual(tres, p3))
                        {
                            int indexMP = addOnePointToMP(tres);
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss + 1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[j].midPoint[ma[j].midPointNum-1]);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int s = 0; s < ma[j].midPointNum; s++)
                            {
                                pArray.Add(ma[j].midPoint[s]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].midPoint[ma[j].midPointNum - 1]);
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else
                        {
                            int indexMP = addOnePointToMP(tres);
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss + 1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].endPoint);
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(i);
                            return i;
                        }
                    }
                    else if (isEqual(tres, p1))
                    {
                        if (!isEqual(tres, p3) && !isEqual(tres, p4))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss + 1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int s = 0; s < ma[j].midPointNum; s++)
                            {
                                pArray.Add(ma[j].midPoint[s]);
                            }
                            pArray.Add(ma[i].midPoint[ss]);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].midPoint[ss]);
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else if (isEqual(tres, p3))
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss + 1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[j].startPoint);
                            for (int s = 0; s < ma[j].midPointNum; s++)
                            {
                                pArray.Add(ma[j].midPoint[s]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            pArray.Add(ma[i].midPoint[ma[i].midPointNum-1]);
                            pArray.Add(ma[j].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(j);
                            ma.RemoveAt(i);
                            return i;
                        }
                        else
                        {
                            pArray.Clear();
                            pArray.Add(ma[i].startPoint);
                            for (int s = 0; s < ss + 1; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            ma.Add(new MyArc(pArray));
                            pArray.Clear();
                            for (int s = ss + 1; s < ma[i].midPointNum; s++)
                            {
                                pArray.Add(ma[i].midPoint[s]);
                            }
                            pArray.Add(ma[i].endPoint);
                            ma.Add(new MyArc(pArray));
                            ma.RemoveAt(i);
                            return i;
                        }
                    }
                }
                for (int s = 0; s < ma[j].midPointNum - 1; s++)
                {
                    p3 = mp[ma[j].midPoint[s]];
                    p4 = mp[ma[j].midPoint[s + 1]];
                    tres = CalIntersect(p1, p2, p3, p4);
                    if (tres != null)//中间对中间
                    {
                        if (!isEqual(tres, p1) && !isEqual(tres, p2))
                        {
                            if (!isEqual(tres, p3) && !isEqual(tres, p4))
                            {
                                int indexMP = addOnePointToMP(tres);
                                pArray.Clear();
                                pArray.Add(ma[i].startPoint);
                                for (int sk = 0; sk < ss + 1; sk++)
                                {
                                    pArray.Add(ma[i].midPoint[sk]);
                                }
                                pArray.Add(indexMP);
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                pArray.Add(indexMP);
                                for (int sk = ss + 1; sk < ma[i].midPointNum; sk++)
                                {
                                    pArray.Add(ma[i].midPoint[sk]);
                                }
                                pArray.Add(ma[i].endPoint);
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                pArray.Add(ma[j].startPoint);
                                for (int sk = 0; sk < s+1; s++)
                                {
                                    pArray.Add(ma[j].midPoint[sk]);
                                }
                                pArray.Add(indexMP);
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                pArray.Add(indexMP);
                                for (int sk = s+1; sk < ma[j].midPointNum; s++)
                                {
                                    pArray.Add(ma[j].midPoint[sk]);
                                }
                                pArray.Add(ma[j].endPoint);
                                ma.Add(new MyArc(pArray));
                                ma.RemoveAt(j);
                                ma.RemoveAt(i);
                                return i;
                            }
                            else if (isEqual(tres, p3))
                            {
                                pArray.Clear();
                                pArray.Add(ma[i].startPoint);
                                for (int sk = 0; sk < ss + 1; sk++)
                                {
                                    pArray.Add(ma[i].midPoint[sk]);
                                }
                                pArray.Add(ma[j].midPoint[s]);
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                pArray.Add(ma[j].midPoint[s]);
                                for (int sk = ss + 1; sk < ma[i].midPointNum; sk++)
                                {
                                    pArray.Add(ma[i].midPoint[sk]);
                                }
                                pArray.Add(ma[i].endPoint);
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                pArray.Add(ma[j].startPoint);
                                for (int sk = 0; sk < s + 1; s++)
                                {
                                    pArray.Add(ma[j].midPoint[sk]);
                                }
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                for (int sk = s; sk < ma[j].midPointNum; s++)
                                {
                                    pArray.Add(ma[j].midPoint[sk]);
                                }
                                pArray.Add(ma[j].endPoint);
                                ma.Add(new MyArc(pArray));
                                ma.RemoveAt(j);
                                ma.RemoveAt(i);
                                return i;
                            }
                        }
                        else if (isEqual(tres, p1))
                        {
                            if (!isEqual(tres, p3) && !isEqual(tres, p4))
                            {
                                pArray.Clear();
                                pArray.Add(ma[i].startPoint);
                                for (int sk = 0; sk < ss + 1; sk++)
                                {
                                    pArray.Add(ma[i].midPoint[sk]);
                                }
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                for (int sk = ss; sk < ma[i].midPointNum; sk++)
                                {
                                    pArray.Add(ma[i].midPoint[sk]);
                                }
                                pArray.Add(ma[i].endPoint);
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                pArray.Add(ma[j].startPoint);
                                for (int sk = 0; sk < s + 1; s++)
                                {
                                    pArray.Add(ma[j].midPoint[sk]);
                                }
                                pArray.Add(ma[i].midPoint[ss]);
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                pArray.Add(ma[i].midPoint[ss]);
                                for (int sk = s+1; sk < ma[j].midPointNum; s++)
                                {
                                    pArray.Add(ma[j].midPoint[sk]);
                                }
                                pArray.Add(ma[j].endPoint);
                                ma.Add(new MyArc(pArray));
                                ma.RemoveAt(j);
                                ma.RemoveAt(i);
                                return i;
                            }
                            else if (isEqual(tres, p3))
                            {
                                pArray.Clear();
                                pArray.Add(ma[i].startPoint);
                                for (int sk = 0; sk < ss + 1; sk++)
                                {
                                    pArray.Add(ma[i].midPoint[sk]);
                                }
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                for (int sk = ss; sk < ma[i].midPointNum; sk++)
                                {
                                    pArray.Add(ma[i].midPoint[sk]);
                                }
                                pArray.Add(ma[i].endPoint);
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                pArray.Add(ma[j].startPoint);
                                for (int sk = 0; sk < s + 1; s++)
                                {
                                    pArray.Add(ma[j].midPoint[sk]);
                                }
                                ma.Add(new MyArc(pArray));
                                pArray.Clear();
                                for (int sk = s; sk < ma[j].midPointNum; s++)
                                {
                                    pArray.Add(ma[j].midPoint[sk]);
                                }
                                pArray.Add(ma[j].endPoint);
                                ma.Add(new MyArc(pArray));
                                ma.RemoveAt(j);
                                ma.RemoveAt(i);
                                return i;
                            }
                        }
                    }
                }
            }
            return j;
        }
        
        private int addOnePointToMP(MyPoint p)
        {
            for(int i=0;i<mp.Count;i++)
            {
                if (Math.Abs(p.x -mp[i].x)<0.1 && Math.Abs(p.y - mp[i].y)<0.1)
                    return i;
            }
            mp.Add(p);
            return mp.Count - 1;
        }
        private bool isEqual(MyPoint p1,MyPoint p2)
        {
            if (Math.Abs(p1.x - p2.x)<0.1 && Math.Abs(p1.y - p2.y)<0.1)
                return true;
            else
                return false;
        }

        private bool isInEnv(MyPoint p,double EnvMaxx,double EnvMinx,double EnvMaxy,double EnvMiny)
        {
            if (p.x < EnvMaxx && p.x > EnvMinx && p.y > EnvMiny && p.y < EnvMaxy)
                return true;
            return false;
        }
        /// <summary>
        /// 生成多边形
        /// </summary>
        private void trackPolygon()
        {
            double EnvMaxx = double.MinValue, EnvMinx = double.MaxValue, EnvMaxy = double.MinValue, EnvMiny = double.MaxValue;
            for (int i = 0; i < data.Count; i++)
            {
                if (EnvMaxx < data[i].getX())
                    EnvMaxx = data[i].getX();
                if (EnvMinx > data[i].getX())
                    EnvMinx = data[i].getX();
                if (EnvMaxy < data[i].getY())
                    EnvMaxy = data[i].getY();
                if (EnvMiny > data[i].getY())
                    EnvMiny = data[i].getY();
            }

            int[] trackTimes = new int[ma.Count];//记录某弧段被用了几次，是顺时针还是逆时针
            for (int i = 0; i < ma.Count; i++)
                trackTimes[i] = 0;

            for (int i = 0; i < mp.Count; i++)
            {
                if (mp[i].lineNum > 1&&isInEnv(mp[i],EnvMaxx,EnvMinx,EnvMaxy,EnvMiny))//是内部结点
                {
                    //将结点拓扑关联的线段按照角度从小到大排列
                    for (int j = 0; j < mp[i].lineNum - 1; j++)
                    {
                        for (int k = j + 1; k < mp[i].lineNum; k++)
                        {
                            if (calAngle(i,j) > calAngle(i,k))
                            {
                                int tt = mp[i].relLine[j];
                                mp[i].relLine[j] = mp[i].relLine[k];
                                mp[i].relLine[k] = tt;
                            }
                        }
                    }
                    for (int j = 0; j < mp[i].lineNum; j++)
                    {
                        if (Math.Abs(trackTimes[convert(mp[i].relLine[j])]) != 2 && Math.Abs(trackTimes[convert(mp[i].relLine[(j + 1) % mp[i].lineNum])]) != 2)
                        {
                            List<int> alist = new List<int>();
                            int tempArc = mp[i].relLine[(j + 1) % mp[i].lineNum];
                            while ((tempArc >= 0 && trackTimes[convert(tempArc)] <= 0 && trackTimes[convert(tempArc)] > -2) || (tempArc < 0 && trackTimes[convert(tempArc)] >= 0 && trackTimes[convert(tempArc)]<2))
                            {
                                alist.Add(tempArc);
                                if (trackTimes[convert(tempArc)] > 0)
                                    trackTimes[convert(tempArc)]++;
                                else if (trackTimes[convert(tempArc)] < 0)
                                    trackTimes[convert(tempArc)]--;
                                else
                                    trackTimes[convert(tempArc)] = tempArc >= 0 ? 1 : -1;

                                int nextP;
                                if (tempArc >= 0)//找末结点
                                {
                                    nextP = ma[tempArc].endPoint;
                                    ma[tempArc].rightPoly = mpol.Count;
                                }
                                else//找首节点
                                {
                                    nextP = ma[convert(tempArc)].startPoint;
                                    ma[convert(tempArc)].leftPoly = mpol.Count;
                                }
                                for (int l = 0; l < mp[nextP].lineNum - 1; l++)
                                {
                                    for (int k = l + 1; k < mp[nextP].lineNum; k++)
                                    {
                                        if (calAngle(nextP,l) > calAngle(nextP,k))
                                        {
                                            int tt = mp[nextP].relLine[l];
                                            mp[nextP].relLine[l] = mp[nextP].relLine[k];
                                            mp[nextP].relLine[k] = tt;
                                        }
                                    }
                                }
                                for(int k=0;k<mp[nextP].lineNum;k++)
                                {
                                    if(Math.Abs(convert(tempArc))==Math.Abs(convert(mp[nextP].relLine[k])))
                                    {
                                        tempArc = mp[nextP].relLine[(k + 1) % mp[nextP].lineNum];
                                        break;
                                    }
                                }
                            }
                            if (alist.Count != 0)
                                mpol.Add(new MyPolygon(alist));
                        }
                    }
                }
            }
            //生成多边形的拓扑关系
            for(int i=0;i<mpol.Count;i++)
            {
                for(int j=0;j<mpol[i].arcList.Count;j++)
                {
                    if(ma[convert(mpol[i].arcList[j])].leftPoly==i&&ma[convert(mpol[i].arcList[j])].rightPoly!=-1)
                    {
                        mpol[i].adjPoly.Add(ma[convert(mpol[i].arcList[j])].rightPoly);
                        mpol[i].adjPolyNum++;
                    }
                    if (ma[convert(mpol[i].arcList[j])].rightPoly == i && ma[convert(mpol[i].arcList[j])].leftPoly != -1)
                    {
                        mpol[i].adjPoly.Add(ma[convert(mpol[i].arcList[j])].leftPoly);
                        mpol[i].adjPolyNum++;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i">第i个结点</param>
        /// <param name="j">第j条弧段</param>
        /// <returns></returns>
        public double calAngle(int i,int j)
        {
            double cos1,sin1;
            if (ma[convert(mp[i].relLine[j])].midPointNum == 0)
            {
                if (mp[i].relLine[j] >= 0)
                {
                    cos1 = cos(mp[i], new MyPoint(mp[i].x + 1, mp[i].y), mp[ma[convert(mp[i].relLine[j])].endPoint]);
                    sin1 = sin(mp[i], new MyPoint(mp[i].x + 1, mp[i].y), mp[ma[convert(mp[i].relLine[j])].endPoint]);
                }
                else
                {
                    cos1 = cos(mp[i], new MyPoint(mp[i].x + 1, mp[i].y), mp[ma[convert(mp[i].relLine[j])].startPoint]);
                    sin1 = sin(mp[i], new MyPoint(mp[i].x + 1, mp[i].y), mp[ma[convert(mp[i].relLine[j])].startPoint]);
                }
            }
            else
            {
                if (mp[i].relLine[j] >= 0)
                {
                    cos1 = cos(mp[i], new MyPoint(mp[i].x + 1, mp[i].y), mp[ma[mp[i].relLine[j]].midPoint[0]]);
                    sin1 = sin(mp[i], new MyPoint(mp[i].x + 1, mp[i].y), mp[ma[mp[i].relLine[j]].midPoint[0]]);
                }
                else
                {
                    cos1 = cos(mp[i], new MyPoint(mp[i].x + 1, mp[i].y), mp[ma[convert(mp[i].relLine[j])].midPoint[ma[convert(mp[i].relLine[j])].midPointNum - 1]]);
                    sin1 = sin(mp[i], new MyPoint(mp[i].x + 1, mp[i].y), mp[ma[convert(mp[i].relLine[j])].midPoint[ma[convert(mp[i].relLine[j])].midPointNum - 1]]);
                }
            }
            if (sin1 >= 0)
                return Math.Acos(cos1);
            else
                return (Math.Acos(-cos1) + 180);
        }

        public int convert(int s)
        {
            return s >= 0 ? s : (-s - 1);
        }
        /// <summary>
        /// 绘制拓扑数据
        /// </summary>
        private void showTopo()
        {
            pictureBox1.Refresh();
            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush myBrush = new SolidBrush(Color.Red);
            // Pen myPen = new Pen(Color.Red);
            for (int i = 0; i < mpol.Count; i++)
            {
                myBrush.Color = Color.FromArgb(125,(i*35)%256,(i*76)%256,(i*90)%256);
                int pointNumPoly = 0;
                for (int j = 0; j < mpol[i].arcList.Count; j++)
                {
                    pointNumPoly += ma[convert(mpol[i].arcList[j])].midPointNum + 1;
                }
                Point[] tmp = new Point[pointNumPoly];
                pointNumPoly = 0;
                for (int j = 0; j < mpol[i].arcList.Count; j++)
                {
                    if (mpol[i].arcList[j] >= 0)
                    {
                        tmp[pointNumPoly] = new Point((int)((mp[ma[convert(mpol[i].arcList[j])].startPoint].x - minx) / scale + margin), (int)(pictureBox1.Height - (mp[ma[convert(mpol[i].arcList[j])].startPoint].y - miny) / scale - margin));
                        pointNumPoly++;
                        for (int k = 0; k < ma[convert(mpol[i].arcList[j])].midPointNum; k++)
                        {
                            tmp[pointNumPoly] = new Point((int)((mp[ma[convert(mpol[i].arcList[j])].midPoint[k]].x - minx) / scale + margin), (int)(pictureBox1.Height - (mp[ma[convert(mpol[i].arcList[j])].midPoint[k]].y - miny) / scale - margin));
                            pointNumPoly++;
                        }
                    }
                    else
                    {
                        tmp[pointNumPoly] = new Point((int)((mp[ma[convert(mpol[i].arcList[j])].endPoint].x - minx) / scale + margin), (int)(pictureBox1.Height - (mp[ma[convert(mpol[i].arcList[j])].endPoint].y - miny) / scale - margin));
                        pointNumPoly++;
                        for (int k = ma[convert(mpol[i].arcList[j])].midPointNum-1; k >=0; k--)
                        {
                            tmp[pointNumPoly] = new Point((int)((mp[ma[convert(mpol[i].arcList[j])].midPoint[k]].x - minx) / scale + margin), (int)(pictureBox1.Height - (mp[ma[convert(mpol[i].arcList[j])].midPoint[k]].y - miny) / scale - margin));
                            pointNumPoly++;
                        }
                    }
                    
                }
                g.FillPolygon(myBrush, tmp);
            }
            // myPen.Dispose();
            myBrush.Dispose();
            g.Dispose();
        }
        public double cos(MyPoint p,MyPoint p1,MyPoint p2)
        {
            double tmp1 = (p1.x - p.x) * (p2.x - p.x) + (p1.y - p.y) * (p2.y - p.y);
            double tmp2 = Math.Sqrt((p1.x - p.x) * (p1.x - p.x) + (p1.y - p.y) * (p1.y - p.y));
            double tmp3 = Math.Sqrt((p2.x - p.x) * (p2.x - p.x) + (p2.y - p.y) * (p2.y - p.y));
            return tmp1 / tmp2 / tmp3;
        }

        public double sin(MyPoint p, MyPoint p1, MyPoint p2)
        {
            double tmp1 = (p1.x - p.x) * (p2.y - p.y) - (p2.x - p.x) * (p1.y - p.y);
            double tmp2 = Math.Sqrt((p1.x - p.x) * (p1.x - p.x) + (p1.y - p.y) * (p1.y - p.y));
            double tmp3 = Math.Sqrt((p2.x - p.x) * (p2.x - p.x) + (p2.y - p.y) * (p2.y - p.y));
            return tmp1 / tmp2 / tmp3;
        }

        public MyPoint CalIntersect(MyPoint p1,MyPoint p2,MyPoint p3,MyPoint p4)
        {
            if(Math.Abs(p1.x-p2.x)<0.1||Math.Abs(p3.x-p4.x)<0.1)
            {
                if (Math.Abs(p1.x - p2.x) > 0.1)
                {
                    double k1 = (p1.y - p2.y) / (p1.x - p2.x);
                    double resy = k1 * (p3.x - p1.x) + p1.y;
                    if ((p3.x - p1.x) * (p3.x - p2.x) <=0 && (resy - p3.y) * (resy - p4.y) <= 0)
                    {
                        return new MyPoint(p3.x, resy);
                    }
                    return null;
                }
                else if (Math.Abs(p3.x - p4.x) >0.1)
                {
                    double k2 = (p3.y - p4.y) / (p3.x - p4.x);
                    double resy = k2 * (p1.x - p3.x) + p3.y;
                    if ((p1.x - p3.x) * (p1.x - p4.x) <=0 && (resy - p1.y) * (resy - p2.y) <= 0)
                    {
                        return new MyPoint(p1.x, resy);
                    }
                    return null;
                }
                else
                    return null;
            }
            else
            {
                double k1=(p1.y-p2.y)/(p1.x-p2.x);
                double k2=(p3.y-p4.y)/(p3.x-p4.x);
                if (Math.Abs(k1 - k2)<0.1)
                    return null;
                double resx=(p3.y-p1.y+k1*p1.x-k2*p3.x)/(k1-k2);
                if ((resx - p1.x) * (resx - p2.x) <=0 && (resx - p3.x) * (resx - p4.x) <= 0)
                {
                    double resy = k1 * (resx - p1.x) + p1.y;
                    return new MyPoint(resx,resy);
                }
                return null;
            }
        }
        
        private void 导出拓扑数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdvancedGIS.DialogForm.TopologyForm TF = new DialogForm.TopologyForm(this);
            TF.ShowDialog(this);
        }
    }
}
