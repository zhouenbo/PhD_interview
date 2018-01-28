using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Drawing;

namespace AdvancedGIS.dataStructure
{
    public class MyArc
    {
        public static int count = -1;
        public int ID;
        public int startPoint;
        public int endPoint;
        public int midPointNum;
        public List<int> midPoint=new List<int>();
        public int leftPoly;
        public int rightPoly;

        public MyArc(List<int> d)
        {
            count++;
            ID = count;
            startPoint = d[0];
            endPoint = d[d.Count - 1];
            for(int i=1;i<d.Count-1;i++)
            {
                midPoint.Add(d[i]);
            }
            midPointNum = midPoint.Count;

            leftPoly = -1;
            rightPoly = -1;
        }

        public double GetLength(List<MyPoint> mp)
        {
            double res = 0;
            MyPoint p1 = mp[startPoint];
            MyPoint p2;
            for(int i=0;i<midPointNum;i++)
            {
                p2 = mp[i];
                res += Math.Sqrt((p1.x-p2.x)*(p1.x-p2.x)+(p1.y-p2.y)*(p1.y-p2.y));
                p1 = p2;
            }
            p2 = mp[endPoint];
            res += Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
            return res;
        }
    }
}
