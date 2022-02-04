using NeoCortex;
using NeoCortexApi.Utility;
using NeoCortexApi.Encoders;
using NeoCortexApi;
using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static NeoCortexApiSample.MultiSequenceLearning;

namespace NeoCortexApiSample
{
    class Program
    {
        /// <summary>
        /// This sample shows a typical experiment code for SP and TM.
        /// You must start this code in debugger to follow the trace.
        /// and TM.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //
            // Starts experiment that demonstrates how to learn spatial patterns.
            //SpatialPatternLearning experiment = new SpatialPatternLearning();
            //experiment.Run();

            //
            // Starts experiment that demonstrates how to learn spatial patterns.
            //SequenceLearning experiment = new SequenceLearning();
            //experiment.Run();

            //RunMultiSimpleSequenceLearningExperiment();
            //RunMultiSequenceLearningExperiment();

            EncodeDateTime();
        }

        private static void RunMultiSimpleSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            sequences.Add("S1", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, }));
            sequences.Add("S2", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0 }));

            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);
         
        }

        private static void EncodeDateTime()
        {
            var folderName = Directory.CreateDirectory(nameof(EncodeDateTime)).Name;
            string filename;
            int minHour = 0, maxHour = 23, minDate = 1, maxDate = 31, minMonth = 1, maxMonth = 12;
            int[] daysOfMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            ScalarEncoder hourEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "N", 30},
                { "MinVal", (double)minHour},
                { "MaxVal", (double)maxHour + 1},
                { "Periodic", true},
                { "Name", "Hour of the day."},
                { "ClipInput", true},
            });


            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Hour of the day");
            Console.WriteLine(hourEncoder.TraceSimilarities());
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");


            ScalarEncoder dateEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "N", 36},
                { "MinVal", (double)minDate},
                { "MaxVal", (double)maxDate + 1},
                { "Periodic", true},
                { "Name", "Date of the month."},
                { "ClipInput", true},
            });



            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Date of the Month");
            Console.WriteLine(dateEncoder.TraceSimilarities());
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");


            ScalarEncoder monthEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 3},
                { "N", 15},
                { "MinVal", (double)minMonth},
                { "MaxVal", (double)maxMonth + 1},
                { "Periodic", true},
                { "Name", "Month of Year."},
                { "ClipInput", true},
            });


            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Month of Year");
            Console.WriteLine(monthEncoder.TraceSimilarities());
            Console.WriteLine("--------------------------------------------------------------------------------------------------------------");




            Dictionary<string, int[]> sdrDict = new Dictionary<string, int[]>();
            Dictionary<string, int[]> sdrImg = new Dictionary<string, int[]>();

            for (int month = minMonth; month <= maxMonth; month++)
            {
                for (int date = minDate; date <= maxDate; date++)
                {
                    for (int hour = minHour; hour <= maxHour; hour++)
                    {
                        if (date > daysOfMonth[month - 1])
                            break;

                        var sdrHour = hourEncoder.Encode(hour);
                        var sdrDate = dateEncoder.Encode(date);
                        var sdrMonth = monthEncoder.Encode(month);

                        List<int> sdrDateTime = new List<int>();

                        sdrDateTime.AddRange(sdrDate);
                        sdrDateTime.AddRange(sdrMonth);
                        sdrDateTime.AddRange(sdrHour);


                        string key = $"{date.ToString("00")}/{month.ToString("00")}-{hour.ToString("00")}:00";  /* eg : DD/MM-HH:00 20/01-15:00 */
                        //Console.WriteLine($"{key}");

                        sdrDict.Add(key, sdrDateTime.ToArray());

                        int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(sdrDateTime.ToArray(), (int)Math.Sqrt(sdrDateTime.ToArray().Length), (int)Math.Sqrt(sdrDateTime.ToArray().Length));
                        var twoDimArray = ArrayUtils.Transpose(twoDimenArray);



                        Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
                        Console.WriteLine($"DateTime: {key} \nSDR: {Helpers.StringifyVector(sdrDateTime.ToArray())} \nand 2D Array: ");
                        Print2DArray(twoDimArray);
                        Console.WriteLine("--------------------------------------------------------------------------------------------------------------");


                        filename = $"{date.ToString("00")}_{month.ToString("00")}_{hour.ToString("00")}_00" + ".png";
                        NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, Path.Combine(folderName, filename), Color.Black, Color.Gray, text: key.ToString());
                    }
                }
            }

        }

        private static void RunMultiSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            //sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 5.0, 4.0, 3.0, 7.0, 1.0, 9.0, 12.0, 11.0, 12.0, 13.0, 14.0, 11.0, 12.0, 14.0, 5.0, 7.0, 6.0, 9.0, 3.0, 4.0, 3.0, 4.0, 3.0, 4.0 }));
            //sequences.Add("S2", new List<double>(new double[] { 0.8, 2.0, 0.0, 3.0, 3.0, 4.0, 5.0, 6.0, 5.0, 7.0, 2.0, 7.0, 1.0, 9.0, 11.0, 11.0, 10.0, 13.0, 14.0, 11.0, 7.0, 6.0, 5.0, 7.0, 6.0, 5.0, 3.0, 2.0, 3.0, 4.0, 3.0, 4.0 }));

            sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 2.0, 3.0, 4.0, 2.0, 5.0, }));
            sequences.Add("S2", new List<double>(new double[] { 8.0, 1.0, 2.0, 9.0, 10.0, 7.0, 11.00 }));

            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            var list1 = new double[] { 1.0, 2.0, 3.0 };
            var list2 = new double[] { 2.0, 3.0, 4.0 };
            var list3 = new double[] { 8.0, 1.0, 2.0 };

            predictor.Reset();
            PredictNextElement(predictor, list1);

            predictor.Reset();
            PredictNextElement(predictor, list2);

            predictor.Reset();
            PredictNextElement(predictor, list3);
        }

        private static void PredictNextElement(HtmPredictionEngine predictor, double[] list)
        {
            Debug.WriteLine("------------------------------");

            foreach (var item in list)
            {
                var res = predictor.Predict(item);

                if (res.Count > 0)
                {
                    foreach (var pred in res)
                    {
                        Debug.WriteLine($"{pred.PredictedInput} - {pred.Similarity}");
                    }

                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');
                    Debug.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2[tokens.Length - 1]}");
                }
                else
                    Debug.WriteLine("Nothing predicted :(");
            }

            Debug.WriteLine("------------------------------");
        }
        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
