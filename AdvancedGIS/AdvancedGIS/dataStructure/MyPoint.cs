using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedGIS.dataStructure
{
    public class MyPoint
    {
        public static int count = -1;
        public int ID;
        public double x;
        public double y;
        public int lineNum;
        public List<int> relLine=new List<int>();

        public MyPoint(double xx,double yy)
        {
            count++;
            ID = count;
            x = xx;
            y = yy;
            lineNum = 0;
        }
    }
}
