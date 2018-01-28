using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedGIS.dataStructure
{
    public class Dt
    {
        private int index;
        private string name;
        private double x;
        private double y;
        private double z;

        public int getIndex()
        {
            return index;
        }
        public string getName()
        {
            return name;
        }
        public double getX()
        {
            return x;
        }
        public double getY()
        {
            return y;
        }
        public double getZ()
        {
            return z;
        }

        public void setIndex(int x)
        {
            if (x >= 0)
                index = x;
            else
                throw new Exception("数据序号为负！");
        }
        public void setName(string s)
        {
            name = s;
        }
        public void setX(double tmp)
        {
            x = tmp;
        }
        public void setY(double tmp)
        {
            y = tmp;
        }
        public void setZ(double tmp)
        {
            z = tmp;
        }
    }

}
