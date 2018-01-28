using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedGIS.dataStructure
{
    public class Arc
    {
        public int ID;
        public int startPoint;
        public int endPoint;
        public int useTimes;
        public bool po;
        public int[] tri=new int[2];

        public Arc(int id,int sp,int ep,int ut)
        {
            ID = id;
            startPoint = sp;
            endPoint = ep;
            useTimes = ut;
            po = false;
            tri[0]=-1;
            tri[1]=-1;
        }
    }
}
