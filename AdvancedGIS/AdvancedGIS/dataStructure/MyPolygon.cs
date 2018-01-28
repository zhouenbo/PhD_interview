using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedGIS.dataStructure
{
    public class MyPolygon
    {
        public static int count = -1;
        public int ID;
        public int arcNum;
        public List<int> arcList = new List<int>();
        public int adjPolyNum = -1;
        public List<int> adjPoly = new List<int>();

        public MyPolygon(List<int> d)
        {
            count++;
            ID = count;
            arcNum = d.Count;
            for(int i=0;i<d.Count;i++)
            {
                arcList.Add(d[i]);
            }
        }

        public int convert(int s)
        {
            return s >= 0 ? s : (-s - 1);
        }
        //计算多边形周长
        public double GetPerimeter(List<MyPoint> mp,List<MyArc> ma)
        {
            double res = 0;
            //假设点线面都按照ID升序排列
            for(int i=0;i<arcNum;i++)
            {
                res += ma[convert(arcList[i])].GetLength(mp);
            }
            return res;
        }
        //计算多边形的面积
        public double GetArea(List<MyPoint> mp, List<MyArc> ma)
        {
            double res=0;
            MyPoint tmp = mp[ma[convert(arcList[arcList.Count - 1])].startPoint];
            foreach(int i in arcList)
            {
                res += TriArea(tmp, mp[ma[convert(i)].startPoint], mp[ma[convert(i)].endPoint]);
            }
            return res;
        }
        //计算三角形面积
        public double TriArea(MyPoint p1,MyPoint p2,MyPoint p3)
        {
            return 0.5*Math.Abs((p2.x-p1.x)*(p3.y-p1.y)-(p2.y-p1.y)*(p3.x-p1.x));
        }
    }
}
