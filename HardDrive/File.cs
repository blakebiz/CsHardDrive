using System;
using System.Collections.Generic;


namespace HardDrive
{

    public class File: IHddObject
    {
        public int Id { get; }

        public HashSet<string> Tags { get; private set; } = new HashSet<string>();

        // stores the data in the file
        public object SourceReference { get; private set; }

        // for storing the path to original file
        public string Path { get; }

        // for storing the type of file
        public string Extension { get; private set;}

        // this should probably be moved elsewhere, just don't touch this attribute
        public int Relevance { get; set; } = 0;

        // file types allowed to be stored by an HddFile (whatever we have support built for so far
        public HashSet<string> ImageExtensions { get; }= new HashSet<string>() {".xlsm", ".xlsx", ".csv", ".txt"};

        
        public File(string[] tags)
        {
            foreach (string tag in tags)
            {
                this.Tags.Add(tag.ToLower());
            }
        }

        public File(List<string> tags)
        {
            foreach (string tag in tags)
            {
                this.Tags.Add(tag.ToLower());
            }
        }

        public File(string[] tags, object sourceRef)
        {
            foreach (string tag in tags)
            {
                this.Tags.Add(tag.ToLower());
            }

            this.SourceReference = sourceRef;
        }

        public File(List<string> tags, object sourceRef)
        {
            foreach (string tag in tags)
            {
                this.Tags.Add(tag.ToLower());
            }

            this.SourceReference = sourceRef;
        }

        public File(object sourceRef)
        {
            this.SourceReference = sourceRef;
        }

        public File(int id, string[] tags)
        {
            this.Id = id;
            foreach (string tag in tags)
            {
                this.Tags.Add(tag.ToLower());
            }
        }

        public File(string path, bool convert = true)
        {
            this.Path = path;
            if (convert)
            {
                this.FetchData();
            }
        }

        /// <summary>
        /// Reads the data from the file and stores it in the source_reference attribute.
        /// If given an unsupported file type, throws an exception.
        /// </summary>
        public void FetchData()
        {
            if (this.Path.EndsWith(".txt"))
            {
                this.TxtToFile();
            }
            else if (this.Path.EndsWith(".xlsm") || this.Path.EndsWith(".xlsx") || this.Path.EndsWith(".csv"))
            {
                this.SourceReference = Spreadsheet.LoadFile(this.Path);
            }
            else
            {
                throw new Exception("Invalid file type given");
            }

            int start = this.Path.LastIndexOf('\\');
            int stop = this.Path.LastIndexOf('.');
            this.Extension = this.Path.Substring(stop);
            string file_name = this.Path.Substring(start + 1, stop - start - 1);
            if (ReferenceEquals(null, this.Tags))
            {
                this.Tags = new HashSet<string>() {file_name, this.Path};
            }
            else
            {
                this.Tags.Add(file_name);
                this.Tags.Add(this.Path);
            }
            
        }

        public override string ToString()
        {
            string vals = "{";
            int count = 0;
            foreach (string tag in this.Tags)
            {
                vals += tag;
                count++;
                if (count != this.Tags.Count)
                {
                    vals += ", ";
                }
            }

            vals += "}";
            return this.Id + ", " + vals;
        }

        /// <summary>
        /// Reads the data from the .txt file and stores it in the source_reference attribute.
        /// </summary>
        public void TxtToFile()
        {
            // Sets file name and file path as tag by default
            this.SourceReference = System.IO.File.ReadAllLines(this.Path);
            
        }

        /// <summary>
        /// Reads the data from the .csv file and stores it in the source_reference attribute.
        /// </summary>
        /// <param name="delimiter"> The delimiter in the csv file </param>
        public void CsvToFile(string delimiter = ",")
        {
            this.SourceReference = CSVParser.parse(this.Path, ",");
        }


        // Getters/Setters
        // public int Id
        // {
        //     get => id;
        //     set => id = value;
        // }
        //
        // public object SourceReference
        // {
        //     get => sourceReference;
        //     set => sourceReference = value;
        // }
        //
        // public HashSet<string> Tags
        // {
        //     get => tags;
        //     set => tags = value;
        // }
        //
        // public int Relevance
        // {
        //     get => relevance;
        //     set => relevance = value;
        // }
        //
        // public string Path
        // {
        //     get => path;
        //     set => path = value;
        // }
        //
        // public string Extension
        // {
        //     get => extension;
        //     set => extension = value;
        // }

    }
}