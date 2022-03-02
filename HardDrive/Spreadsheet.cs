using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace HardDrive
{
    public class Spreadsheet
    {
        private Dictionary<string, List<List<object>>> worksheets;
        private string path;

        public Spreadsheet(Dictionary<string, List<List<object>>> worksheets, string save_name)
        {
            // worksheets is a dictionary containing strings as worksheet names and 2D arrays containing the data of each worksheets
            // save_name is the name to save the file as
            this.worksheets = worksheets;
            this.path = save_name;
        }

        public Spreadsheet(List<List<object>> csv, string save_name)
        {
            // worksheets is a 2D array containing the data of the csv
            // save_name is the name to save the file as
            Dictionary<string, List<List<object>>> worksheets = new Dictionary<string, List<List<object>>>();
            worksheets.Add(save_name, csv);
            this.worksheets = worksheets;
            this.path = save_name;
        }


        public static implicit operator Dictionary<string, List<List<object>>>(Spreadsheet sheet)
        {
            return sheet.worksheets;
        }

        public List<List<object>> graph_3D(int col1 = 0, int col2 = 1, int col3 = 2, string sheet_name = "", int min_row = 0, int max_row = -1)
        {
            if (sheet_name == "")
            {
                foreach (string key in this.worksheets.Keys)
                {
                    sheet_name = key; break;
                }
            }
            List<List<object>> filtered_values = new List<List<object>>();
            int[] indexes = {col1, col2, col3};
            if (this.worksheets.ContainsKey(sheet_name))
            {
                if (max_row == -1)
                {
                    max_row = this.worksheets[sheet_name].Count;
                }
                List<List<object>> worksheet = this.worksheets[sheet_name];
                filtered_values.Add(new List<object>{"null", "null", "null"});

                for (int r = 0; r < worksheet.Count; r++)
                {
                    List<object> row = worksheet[r];
                    for (int c = 0; c < row.Count; c++)
                    {
                        if (indexes.Contains(c) && r >= min_row && r <= max_row)
                        {
                            if (!filtered_values[filtered_values.Count-1].Contains("null"))
                            {
                                filtered_values.Add(new List<object>{"null", "null", "null"});
                            }

                            if ((string)row[c] == "null")
                            {
                                filtered_values[filtered_values.Count-1][Array.IndexOf(indexes, c)] = 0;
                            }
                            else
                            {
                                if (row[c] is Double)
                                {
                                    filtered_values[filtered_values.Count-1][Array.IndexOf(indexes, c)] = row[c];
                                }
                                else
                                {
                                    try
                                    {
                                        filtered_values[filtered_values.Count - 1][Array.IndexOf(indexes, c)] =
                                            Double.Parse(row[c].ToString());
                                    }
                                    catch (System.FormatException)
                                    {
                                        filtered_values[filtered_values.Count - 1][Array.IndexOf(indexes, c)] = 0;
                                    }
                                }
                            }
                        }
                    }
                    if (r > max_row) { break; }
                }
            }
            return filtered_values;
        }

        public List<List<float>> graphf_3D(int col1 = 0, int col2 = 1, int col3 = 2, string sheet_name = "", int min_row = 0, int max_row = -1)
        {
            List<List<float>> rv = new List<List<float>>();
            foreach (List<object> row in graph_3D(col1, col2, col3, sheet_name, min_row, max_row))
            {
                List<float> halfway = new List<float>();
                foreach (object o in row)
                {
                    try
                    {
                        halfway.Add((float)o);
                        
                    }
                    catch (InvalidCastException)
                    {
                        try
                        {
                            halfway.Add(float.Parse(o.ToString()));
                        }
                        catch (FormatException) { }
                    }
                    
                }
                rv.Add(halfway);
            }
            return rv;
        }





        public string StoreHdd()
        {
            /*
             Storage Conventions:
             
             All files are stored in a folder named HddStorage
             All csv files are stored with the postfix "_HddCsv.csv"
             All Excel files (.xlsx, .xlsm) are stored as directories containing csv files that have their worksheets
             All Excel directories are stored with the postfix "_HddXLFolder
             
             WARNING:
             All commas in the data are replaced with "|~|" and all newline characters "\n" are replaced with "~|~"
             So on the off chance you have either of those character sequences in your excel/csv data there
             might be issues. This would be much less common than commas or newline characters though hence my decision.
             */
            string filename = this.path;
            if (filename == "" || ReferenceEquals(filename, null))
            {
                throw new FileNotFoundException("Blank file name or no filename is not acceptable!");
            }

            if (filename.Contains("."))
            {
                filename = filename.Substring(0, filename.LastIndexOf("."));
            }

            if (filename.Contains("\\"))
            {
                int index = filename.LastIndexOf("\\");
                filename = filename.Substring(index + 1, filename.Length - index - 1);
            }
            if (filename.Contains("/"))
            {
                int index = filename.LastIndexOf("/");
                filename = filename.Substring(index + 1, filename.Length - index - 1);
            }
            if (this.worksheets.Count > 0)
            {
                if (!System.IO.Directory.Exists("HddStorage")) System.IO.Directory.CreateDirectory("HddStorage");
                if (this.worksheets.Count == 1)
                {
                    foreach (KeyValuePair<string, List<List<object>>> kv in worksheets)
                    {
                        filename = GetValidPath($"HddStorage/{filename}", "_HddCsv.csv");
                        using (StreamWriter writer = new StreamWriter(filename))
                        {
                            foreach (List<object> row in kv.Value)
                            {
                                for (int i = 0; i < row.Count; i++)
                                {
                                    if (row[i].ToString().Contains(","))
                                        row[i] = row[i].ToString().Replace(",", "|~|");
                                    if (row[i].ToString().Contains("\n"))
                                        row[i] = row[i].ToString().Replace("\n", "~|~");
                                }
                                writer.Write(string.Join(",", row) + "\n");
                            }
                            writer.Close();
                        }
                    }
                }
                else
                {
                    
                    string foldername = "HddStorage/" + filename + "_HddXLFolder";
                    if (!System.IO.Directory.Exists(foldername))
                        System.IO.Directory.CreateDirectory(foldername);
                    foreach (KeyValuePair<string, List<List<object>>> kv in worksheets)
                    {
                        string fname = GetValidPath($"{foldername}/{kv.Key}", "_HddCsv.csv");
                        using (StreamWriter writer = new StreamWriter(fname))
                        {
                            foreach (List<object> row in kv.Value)
                            {
                                for (int i = 0; i < row.Count; i++)
                                {
                                    if (row[i].ToString().Contains(","))
                                        row[i] = row[i].ToString().Replace(",", "|~|");
                                    if (row[i].ToString().Contains("\n"))
                                        row[i] = row[i].ToString().Replace("\n", "~|~");
                                }
                                writer.Write(string.Join(",", row) + "\n");
                            }
                            writer.Close();
                        }
                    }
                    return foldername;
                }
                
            }
            return filename;
        }


        public static Spreadsheet LoadFile(string path)
        {
            Dictionary<string, List<List<object>>> spreadsheet = new Dictionary<string, List<List<object>>>();
            if (path.EndsWith("_HddXLFolder"))
            {
                
                foreach (string file in System.IO.Directory.EnumerateFiles(path))
                {
                    spreadsheet.Add(file, CSVParser.parse_list(path, ","));
                }
            }
            else if (path.EndsWith(".csv"))
            {
                spreadsheet.Add(path, CSVParser.parse_list(path, ","));
            }
            //else if (path.EndsWith(".xlsx") || path.EndsWith(".xlsm"))
            //{
                // "xlsx and xlsm files must be parsed before run-time for time being."
            //}
            else
            {
                throw new Exception("Invalid file type given. Only .csv, _HddCsv.csv, and _HddXLFolder are accepted");
            }
            return new Spreadsheet(spreadsheet, path);
        }
        
        
        public static Tuple<int, int> GetAddr(string address)
        {
            string col = "", row = "";
            foreach (char c in address)
            {
                if (char.IsNumber(c))
                {
                    row += c;
                }
                else
                {
                    col += c;
                }
            }

            int rown = int.Parse(row);
            int coln = FromCol(col);
            return new Tuple<int, int>(rown, coln);
        }

        public static string GetValidPath(string beginning, string ending)
        {
            string fname = beginning + ending;
            int file_count = 0;
            while (System.IO.File.Exists(fname))
            {
                file_count++;
                fname = $"{beginning}({file_count}){ending}";
            }

            return fname;
        }


        public static int FromCol(string col)
        {
            int coln = -1;
            for (int count = 0; count < col.Length; count++)
            {
                coln += (int)((char.ToUpper(col[count]) - 64) * Math.Pow(26, col.Length - 1 - count));
            }
            return coln;
        }


        public static string ToCol(int col)
        {
            string out_col = "";
            int rem = col;
            while (col > 25)
            {
                out_col += (char) (col / 26 + 64);
                rem = col % 26;
                col = (int) (col / 26);
            }

            out_col += (char) (rem + 65);
            return out_col;
        }
        
        public Dictionary<string, List<List<object>>> Worksheets
        {
            get => worksheets;
            set => worksheets = value;
        }
    }
}