using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace chart1
{
    internal class Series
    {
        public static implicit operator Series(System.Windows.Forms.DataVisualization.Charting.Series v)
        {
            throw new NotImplementedException();
        }
    }
}