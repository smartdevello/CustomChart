using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace CustomChart
{
    public class ChartData
    {
        [Name("source file name")]
        public string source_file { get; set; }

        [Name("processed file name")]
        public string processed_file { get; set; }

        [Name("deviation value(micron)")]
        public string _deviation { get; set; }

        [Name("deviation direction")]
        public string deviation_direction { get; set; }

        [Name("percentage(%)")]
        public string _percentage { get; set; }

        public int deviation;
        public int percentage;

    }
}
