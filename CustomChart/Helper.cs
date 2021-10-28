using System;
using System.Collections.Generic;
using System.Text;

namespace CustomChart
{
    public class Helper
    {
        public static double DegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180.0) * degrees;
            return radians;
        }
    }
}
