using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvancedGIS.dataStructure;

namespace AdvancedGIS.algorithm
{
    class TINGenerator
    {
        /// <summary>
        /// 根据点集生成凸包，Graham's Scan算法
        /// </summary>
        /// <param name="od">点集</param>
        /// <returns>凸包点序列</returns>
        public static List<int> generateConvex(List<Dt> od)
        {
            List<int> res = new List<int>();
            List<double> tmp1 = new List<double>();
            List<int> tmp2 = new List<int>();
            //找到最低点的序号
            int minYIndex=0;
            for (int i = 0; i < od.Count;i++ )
            {
                if (od[i].getY() < od[minYIndex].getY() || (od[i].getY() == od[minYIndex].getY() && od[i].getX() < od[minYIndex].getX()))
                    minYIndex = i;
            }
            tmp1.Add(-1);
            tmp2.Add(minYIndex);
            //将所有点按照和最低点构成的向量的角度从大到小排列
            for (int i = 0; i < od.Count;i++ )
            {
                if(i!=minYIndex)
                {
                    double cos = (od[i].getX() - od[minYIndex].getX()) / Math.Sqrt((od[i].getX() - od[minYIndex].getX()) * (od[i].getX() - od[minYIndex].getX()) + (od[i].getY() - od[minYIndex].getY()) * (od[i].getY() - od[minYIndex].getY()));
                    int j;
                    for(j=0;j<tmp1.Count;j++)
                    {
                        if(cos<tmp1[j])
                        {
                            tmp1.Insert(j,cos);
                            tmp2.Insert(j,i);
                            break;
                        }
                    }
                    if(j==tmp1.Count)
                    {
                        tmp1.Add(cos);
                        tmp2.Add(i);
                    }
                }
            }
            //开始追踪凸包多边形
            res.Add(tmp2[0]);
            res.Add(tmp2[1]);
            for (int i = 2; i < tmp2.Count;i++ )
            {
                //计算两个向量的叉积
                double s = (od[res[res.Count - 1]].getX() - od[res[res.Count - 2]].getX()) * (od[tmp2[i]].getY() - od[res[res.Count - 1]].getY()) - (od[res[res.Count - 1]].getY() - od[res[res.Count - 2]].getY()) * (od[tmp2[i]].getX() - od[res[res.Count - 1]].getX());
                while(s>0)//叉积大于零，需要删去之前的点
                {
                    res.RemoveAt(res.Count - 1);
                    s = (od[res[res.Count - 1]].getX() - od[res[res.Count - 2]].getX()) * (od[tmp2[i]].getY() - od[res[res.Count - 1]].getY()) - (od[res[res.Count - 1]].getY() - od[res[res.Count - 2]].getY()) * (od[tmp2[i]].getX() - od[res[res.Count - 1]].getX());
                }
                res.Add(tmp2[i]);
            }

            return res;
        }
        /// <summary>
        /// 生长法生成TIN
        /// </summary>
        /// <param name="data">原始点集</param>
        /// <param name="convex">点集凸包</param>
        /// <param name="arcList">TIN的边的列表</param>
        /// <param name="TINConvex">边描述的TIN凸包</param>
        /// <param name="res">TIN三角网</param>
        public static void generateTIN(List<Dt> data, List<int> convex, List<Arc> arcList, List<int> TINConvex, List<AdvancedGIS.dataStructure.Triangle> res)
        {
            res.Clear();
            arcList.Clear();
            TINConvex.Clear();
            List<int> queue = new List<int>();//边的队列
            Arc at = new dataStructure.Arc(0, convex[0] > convex[1] ? convex[1] : convex[0], convex[0] > convex[1] ? convex[0] : convex[1], 0);
            at.po=true;
            at.po = zhengfuqu(at,data,data[convex[convex.Count-1]]);
            arcList.Add(at);
            queue.Add(0);//加入初始生长边
            while(queue.Count>0)//队列不空，继续生长
            {
                double tmpCos=double.MaxValue;
                int ind=-1;
                for(int i=0;i<data.Count;i++)//求位于当前生长边一侧的张角最大的点的序号
                {
                    if (i != arcList[queue[0]].startPoint && i != arcList[queue[0]].endPoint && zhengfuqu(arcList[queue[0]],data,data[i]))
                    {
                        double tt=COS(data[arcList[queue[0]].startPoint], data[arcList[queue[0]].endPoint],data[i]);
                        if (tt < tmpCos)
                        {
                            tmpCos = tt;
                            ind = i;
                        }
                    }
                }
                if(ind<0)//在追踪边一侧未找到点，删除此边
                {
                    arcList[queue[0]].useTimes++;
                    queue.RemoveAt(0);
                    continue;
                }
                //找到了点，则向队列里新加两边，得到一个三角形
                arcList[queue[0]].useTimes++;
                int sp=arcList[queue[0]].startPoint > ind ? ind : arcList[queue[0]].startPoint;
                int ep=arcList[queue[0]].startPoint > ind ? arcList[queue[0]].startPoint:ind;
                int k1;
                for(k1=0;k1<arcList.Count;k1++)
                {
                    if (arcList[k1].startPoint == sp && arcList[k1].endPoint == ep)
                    {
                        arcList[k1].useTimes++;
                        break;
                    }
                }
                if (k1 == arcList.Count)
                {
                    at = new AdvancedGIS.dataStructure.Arc(arcList.Count, sp, ep, 1);
                    at.po=false;
                    at.po = zhengfuqu(at, data, data[arcList[queue[0]].endPoint]);
                    arcList.Add(at);
                }
                
                sp = arcList[queue[0]].endPoint > ind ? ind : arcList[queue[0]].endPoint;
                ep = arcList[queue[0]].endPoint > ind ? arcList[queue[0]].endPoint : ind;
                int k2;
                for (k2 = 0; k2 < arcList.Count; k2++)
                {
                    if (arcList[k2].startPoint == sp && arcList[k2].endPoint == ep)
                    {
                        arcList[k2].useTimes++;
                        break;
                    }
                }
                if (k2 == arcList.Count)
                {
                    at = new AdvancedGIS.dataStructure.Arc(arcList.Count, sp, ep, 1);
                    at.po = false;
                    at.po = zhengfuqu(at,data,data[arcList[queue[0]].startPoint]);
                    arcList.Add(at);
                }

                int[] tmpID = new int[3];
                tmpID[0]= queue[0];
                tmpID[1] = k1;
                tmpID[2] = k2;
                for(int i=0;i<tmpID.Length-1;i++)
                {
                    for(int j=i+1;j<tmpID.Length;j++)
                    {
                        if(tmpID[i]>tmpID[j])
                        {
                            int ttt = tmpID[i];
                            tmpID[i] = tmpID[j];
                            tmpID[j] = ttt;
                        }
                    }
                }
                Triangle tmpTri = new AdvancedGIS.dataStructure.Triangle(arcList[queue[0]].startPoint, arcList[queue[0]].endPoint, ind);
                tmpTri.arcID1 = tmpID[0];
                tmpTri.arcID2 = tmpID[1];
                tmpTri.arcID3 = tmpID[2];
                int kk;
                for (kk = 0; kk < res.Count;kk++ )
                {
                    if (res[kk].arcID1 == tmpTri.arcID1 && res[kk].arcID2 == tmpTri.arcID2 && res[kk].arcID3 == tmpTri.arcID3)
                        break;
                }
                if (kk == res.Count)
                    res.Add(tmpTri);
                if (arcList[k1].useTimes < 2)
                    queue.Add(k1);
                if (arcList[k2].useTimes < 2)
                    queue.Add(k2);
                queue.RemoveAt(0);
            }
            //三角网生成结束，下面生成拓扑关系
            //给边加入三角形信息
            for(int i=0;i<res.Count;i++)
            {
                if (arcList[res[i].arcID1].tri[0] == -1)
                    arcList[res[i].arcID1].tri[0] = i;
                else if (arcList[res[i].arcID1].tri[1] == -1)
                    arcList[res[i].arcID1].tri[1] = i;
                if (arcList[res[i].arcID2].tri[0] == -1)
                    arcList[res[i].arcID2].tri[0] = i;
                else if (arcList[res[i].arcID2].tri[1] == -1)
                    arcList[res[i].arcID2].tri[1] = i;
                if (arcList[res[i].arcID3].tri[0] == -1)
                    arcList[res[i].arcID3].tri[0] = i;
                else if (arcList[res[i].arcID3].tri[1] == -1)
                    arcList[res[i].arcID3].tri[1] = i;
            }
            //生成以边为基础的点集凸包
            for (int i = 0; i < convex.Count;i++)
            {
                int minPP = convex[i] > convex[(i + 1) % convex.Count] ? convex[(i + 1) % convex.Count] : convex[i];
                int maxPP = convex[i] <= convex[(i + 1) % convex.Count] ? convex[(i + 1) % convex.Count] : convex[i];
                for(int j=0;j<arcList.Count;j++)
                {
                    if (minPP == arcList[j].startPoint && maxPP == arcList[j].endPoint)
                        TINConvex.Add(j);
                }
            }
            return;
        }

        public static double COS(Dt p1,Dt p2,Dt p3)
        {
            double tmp1 = (p1.getX() - p3.getX()) * (p2.getX() - p3.getX()) + (p1.getY() - p3.getY()) * (p2.getY() - p3.getY());
            double tmp2 = Math.Sqrt((p3.getX() - p1.getX()) *(p3.getX() - p1.getX())+(p3.getY() - p1.getY()) * (p3.getY() - p1.getY()));
            double tmp3 = Math.Sqrt((p2.getX() - p3.getX()) * (p2.getX() - p3.getX()) + (p2.getY() - p3.getY()) * (p2.getY() - p3.getY()));
            return tmp1/tmp2/tmp3;
        }

        public static bool zhengfuqu(Arc a,List<Dt> data,Dt p3)
        {
            if(data[a.startPoint].getX()==data[a.endPoint].getX())
            {
                if (p3.getX() > data[a.startPoint].getX())
                    return !a.po;
                else
                    return a.po;
            }
            else
            {
                double tmp = (data[a.startPoint].getY() - data[a.endPoint].getY()) / (data[a.startPoint].getX() - data[a.endPoint].getX()) * (p3.getX() - data[a.endPoint].getX()) + data[a.endPoint].getY();
                return (tmp-p3.getY())> 0 ? (!a.po) : a.po;
            }
        }

    }
}


