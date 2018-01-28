using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedGIS.dataStructure
{
    public class Triangle
    {
        public int ID1, ID2, ID3;
        public int arcID1, arcID2, arcID3;

        public Triangle(int id1,int id2,int id3)
        {
            ID1 = id1;
            ID2 = id2;
            ID3 = id3;
            arcID1 = -1;
            arcID2 = -1;
            arcID3 = -1;
        }
    }
}
