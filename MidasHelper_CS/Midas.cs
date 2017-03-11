using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MidasHelper_CS
{
    public class MidasNode
    {
        public double x;
        public double y;
        public double z;
        public int num;
        public MidasNode()
        {
            x = new double();
            y = new double();
            z = new double();
            num = new int();
        }
        public MidasNode Copy()
        {
            MidasNode copy = new MidasNode();
            copy.num = num;
            copy.x = x;
            copy.y = y;
            copy.z = z;
            return copy;
        }
        //public operator()
        //{

        //}
    }

    public class MidasElement
    {
        public MidasNode fNode = null;
        public MidasNode bNode = null;
        public int num;
        public MidasElement()
        {
            num = new int();
            fNode = new MidasNode();
            bNode = new MidasNode();
        }
        public MidasElement Copy()
        {
            MidasElement copy = new MidasElement();
            copy.num = num;
            copy.fNode = fNode.Copy();
            copy.bNode = bNode.Copy();
            return copy;
        }
    }
}
