using System;
using System.Collections.Generic;
using System.Text;

namespace DataStorage
{
    public class Monitor
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
        public string image = "";
    }

    public class MonInfo
    {
        public int count = 0;
        public List<Monitor> monitors = new List<Monitor>();

        public int yMin = int.MaxValue, yMax = int.MinValue;
        public int xMin = int.MaxValue, xMax = int.MinValue;
        public int height = 0;
        public int width = 0;


        public MonInfo() { }
        public MonInfo(string args) { parseString(args); }



        public void parseString(string str)
        {
            string[] args = str.Split(' ');

            int count = int.Parse(args[0]);

            for(int i=1; i<(count)*4+1; i+=4)
            {
                Monitor mon = new Monitor();

                // parse string
                mon.left = int.Parse(args[i]);
                mon.top = int.Parse(args[i + 1]);
                mon.right = int.Parse(args[i + 2]);
                mon.bottom = int.Parse(args[i + 3]);


                // update maximum values of virtual screen
                yMin = min(yMin, mon.top);
                yMax = max(yMax, mon.bottom);
                xMin = min(xMin, mon.left);
                xMax = max(xMax, mon.right);

                monitors.Add(mon);
            }

            width = xMax = xMin;
            height = yMax - yMin;
        }


        private int min(int a, int b)
        {
            return a < b ? a : b;
        }
        private int max(int a, int b)
        {
            return a > b ? a : b;
        }

    }
}
