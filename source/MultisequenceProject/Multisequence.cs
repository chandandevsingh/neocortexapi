using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoCortexApi;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;

namespace MultisequenceProject
{
    public class Multisequence
    {
        static readonly string dataset = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + @"\Dataset\rec-center-hourly-exp.csv");

        public void Learning()
        {

            Console.WriteLine("Reading Dataset..");
            var csvData = HelperMethods.ReadCSV(dataset);

            Console.WriteLine("Encoding Dataset...");
            var encodedData = HelperMethods.EncodeDataset(csvData);

            Console.WriteLine("Learning...");
            //implement learning
            Console.WriteLine("Done Learning");

        }
    }
}
