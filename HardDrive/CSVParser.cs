using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace HardDrive
{


    public class CSVParser
    {

        public static Dictionary<string, List<object>> parse(string path, string delimiter)
        {
            Dictionary<string, List<object>> results = new Dictionary<string, List<object>>();
            List<string> headers = new List<string>();
            using (var reader = new StreamReader(path))
            {
                bool first = true;
                bool second = false;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    string[] split = { delimiter };
                    var values = line.Split(split, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (first)
                        {
                            headers.Add(values[i]);
                        }
                        else if (second)
                        {
                            List<object> column = new List<object>();
                            column.Add(values[i]);
                            results.Add(headers[i], column);
                        }
                        else
                        {
                            if (i < headers.Count)
                            {
                                results[headers[i]].Add(values[i]);
                            }
                            else
                            {
                                throw new Exception("Comma found in file or uneven file");
                            }
                        }
                    }

                    if (first)
                    {
                        first = false;
                        second = true;
                    }
                    else if (second)
                    {
                        second = false;
                    }
                }
            }

            return results;

        }

        public static List<Dictionary<string, object>> parse_rows(string path, string delimiter,
            HashSet<int> ignore = null)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();
            List<string> headers = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                int line_count = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] split = { delimiter };
                    string[] values = line.Split(split, StringSplitOptions.RemoveEmptyEntries);
                    results.Add(new Dictionary<string, object>());
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (line_count == 0)
                        {
                            headers.Add(values[i]);
                        }
                        else
                        {
                            bool check_passed = true;
                            if (ignore != null)
                            {
                                check_passed = !ignore.Contains(i);
                            }

                            if (check_passed)
                            {
                                if (i < headers.Count)
                                {
                                    results[line_count - 1].Add(headers[i], values[i]);
                                }
                                else
                                {
                                    throw new Exception("Comma found in file or uneven file");
                                }
                            }
                        }
                    }

                    line_count++;
                }
            }

            return results;

        }
        
        public static List<List<object>> parse_list(string path, string delimiter)
        {
            bool check_chars = path.ToLower().Contains("_hdd");
            List<List<object>> results = new List<List<object>>();
            using (StreamReader reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    List<object> row = new List<object>();
                    string[] split = { delimiter };
                    foreach (string item in reader.ReadLine().Split(split, StringSplitOptions.RemoveEmptyEntries))
                    {
                        // Console.WriteLine(item);
                        string fitem = item;
                        if (check_chars)
                        {
                            if (item.Contains("|~|"))
                                fitem = item.Replace("|~|", ",");
                            if (item.Contains("~|~"))
                                fitem = item.Replace("~|~", "\n");
                        }
                        row.Add(fitem);
                    }
                    results.Add(row);
                }
            }
            return results;

        }

    }
}