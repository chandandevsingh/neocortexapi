using System;
using System.Globalization;
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
    public class HelperMethods
    {
        public static List<Dictionary<string, string>> ReadCSV(string csvFilePath)
        {
            List<Dictionary<string, string>> sequencesCollection = new List<Dictionary<string, string>>();

            int keyForUniqueIndexes = 0;

            int count = 0, maxCount = 0;

            bool firstTime = true;

            if (File.Exists(csvFilePath))
            {
                using (StreamReader reader = new StreamReader(csvFilePath))
                {
                    Dictionary<string, string> sequence = new Dictionary<string, string>();
                    while (reader.Peek() >= 0)
                    {
                        var line = reader.ReadLine();
                        string[] values = line.Split(",");

                        keyForUniqueIndexes++;
                        count++;

                        var columnDateTime = values[0];
                        var columnPower = values[1];

                        string[] splitDateTime = columnDateTime.Split(" ");

                        string[] date = splitDateTime[0].Split("/");
                        int dd = int.Parse(date[1]);
                        int MM = int.Parse(date[0]);
                        int yy = int.Parse(date[2]);

                        if (firstTime)
                        {
                            maxCount = 24;
                            firstTime = false;
                        }

                        string[] time = splitDateTime[1].Split(":");
                        int hh = int.Parse(time[0]);
                        int mm = int.Parse(time[1]);

                        string dateTime = dd.ToString("00") + "/" + MM.ToString("00") + "/" + yy.ToString("00") + " " + hh.ToString("00") + ":" + mm.ToString("00");

                        if (sequence.ContainsKey(columnPower))
                        {
                            var newKey = columnPower + "," + keyForUniqueIndexes;
                            sequence.Add(newKey, dateTime);
                        }
                        else
                            sequence.Add(columnPower, dateTime);

                        if (count >= maxCount)
                        {
                            count = 0;
                            maxCount = 0;
                            firstTime = true;

                            sequencesCollection.Add(sequence);
                            sequence = new Dictionary<string, string>();
                        }
                    }
                }

                return sequencesCollection;
            }

            return null;
        }

        public static ScalarEncoder FetchDayEncoder()
        {
            ScalarEncoder dayEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 9},
                { "N", 40},
                { "MinVal", (double)1}, // Min value = (1).
                { "MaxVal", (double)32}, // Max value = (31).
                { "Periodic", true},
                { "Name", "Date"},
                { "ClipInput", true},
           });

            return dayEncoder;
        }

        public static ScalarEncoder FetchMonthEncoder()
        {
            ScalarEncoder monthEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "N", 17},
                { "MinVal", (double)1}, // Min value = (1).
                { "MaxVal", (double)13}, // Max value = (12).
                { "Periodic", true},
                { "Name", "Month"},
                { "ClipInput", true},
            });
            return monthEncoder;
        }

        public static ScalarEncoder FetchHourEncoder()
        {
            ScalarEncoder hourEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 9},
                { "N", 34},
                { "MinVal", (double)0},
                { "MaxVal", (double)23 + 1},
                { "Periodic", true},
                { "Name", "Hour of the day."},
                { "ClipInput", true},
            });
            return hourEncoder;
        }

        public static ScalarEncoder FetchYearEncoder()
        {
            ScalarEncoder yearEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 5},
                { "N", 9},
                { "MinVal", (double)2009}, // Min value = (2009).
                { "MaxVal", (double)2012}, // Max value = (2012).
                { "Periodic", false},
                { "Name", "Year"},
                { "ClipInput", true},
            });
            return yearEncoder;
        }

        public static MultiEncoder FetchDateTimeEncoder()
        {
            EncoderBase hourEncoder = FetchHourEncoder();
            EncoderBase dayEncoder = FetchDayEncoder();
            EncoderBase monthEncoder = FetchMonthEncoder();
            EncoderBase yearEncoder = FetchYearEncoder();

            List<EncoderBase> datetime = new List<EncoderBase>();
            datetime.Add(hourEncoder);
            datetime.Add(dayEncoder);
            datetime.Add(monthEncoder);
            datetime.Add(yearEncoder);

            MultiEncoder datetimeEncoder = new MultiEncoder(datetime);

            return datetimeEncoder;
        }

        public static List<Dictionary<string, int[]>> EncodeDataset(List<Dictionary<string, string>> data)
        {
            List<Dictionary<string, int[]>> listOfSDR = new List<Dictionary<string, int[]>>();

            ScalarEncoder hourEncoder = FetchHourEncoder();
            ScalarEncoder dayEncoder = FetchDayEncoder();
            ScalarEncoder monthEncoder = FetchMonthEncoder();

            foreach (var sequence in data)
            {
                var tempDic = new Dictionary<string, int[]>();

                foreach (var keyValuePair in sequence)
                {
                    var label = keyValuePair.Key;
                    var value = keyValuePair.Value;

                    string[] formats = { "dd/MM/yy hh:mm" };
                    //DateTime dateTime = DateTime.ParseExact(value, formats, CultureInfo.InvariantCulture);
                    DateTime dateTime = DateTime.Parse(value);
                    int day = dateTime.Day;
                    int month = dateTime.Month;
                    int year = dateTime.Year;
                    int hour = dateTime.Hour;

                    int[] sdr = new int[0];

                    sdr = sdr.Concat(dayEncoder.Encode(day)).ToArray();
                    sdr = sdr.Concat(monthEncoder.Encode(month)).ToArray();
                    sdr = sdr.Concat(hourEncoder.Encode(hour)).ToArray();

                    tempDic.Add(label, sdr);
                }
                listOfSDR.Add(tempDic);
            }

            return listOfSDR;
        }

    }
}
