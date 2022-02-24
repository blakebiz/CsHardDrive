using System;
using System.Collections.Generic;


namespace HardDrive
{

    public class HddFile: IHddObject
    {
        private int id;

        private HashSet<string> tags = new HashSet<string>();

        // stores the data in the file
        private object source_reference;

        // for storing the path to original file
        private string path;

        // for storing the type of file
        private string extension;

        // this should probably be moved elsewhere, just don't touch this attribute
        private int relevance = 0;

        // file types allowed to be stored by an HddFile (whatever we have support built for so far
        HashSet<string> image_extensions = new HashSet<string>() {".xlsm", ".xlsx", ".csv", ".txt"};



        public HddFile(string[] tags)
        {
            foreach (string tag in tags)
            {
                this.tags.Add(tag.ToLower());
            }
        }

        public HddFile(List<string> tags)
        {
            foreach (string tag in tags)
            {
                this.tags.Add(tag.ToLower());
            }
        }

        public HddFile(string[] tags, object source_ref)
        {
            foreach (string tag in tags)
            {
                this.tags.Add(tag.ToLower());
            }

            this.source_reference = source_ref;
        }

        public HddFile(List<string> tags, object source_ref)
        {
            foreach (string tag in tags)
            {
                this.tags.Add(tag.ToLower());
            }

            this.source_reference = source_ref;
        }

        public HddFile(object source_ref)
        {
            this.source_reference = source_ref;
        }

        public HddFile(int id, string[] tags)
        {
            this.id = id;
            foreach (string tag in tags)
            {
                this.tags.Add(tag.ToLower());
            }
        }

        public HddFile(string path, bool convert = true)
        {
            this.path = path;
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
            if (this.path.EndsWith(".txt"))
            {
                this.txtToFile();
            }
            else if (this.path.EndsWith(".xlsm") || this.path.EndsWith(".xlsx") || this.path.EndsWith(".csv"))
            {
                this.source_reference = Spreadsheet.LoadFile(this.path);
            }
            else
            {
                throw new Exception("Invalid file type given");
            }

            int start = this.path.LastIndexOf('\\');
            int stop = this.path.LastIndexOf('.');
            this.extension = this.path.Substring(stop);
            string file_name = this.path.Substring(start + 1, stop - start - 1);
            if (ReferenceEquals(null, this.tags))
            {
                this.tags = new HashSet<string>() {file_name, this.path};
            }
            else
            {
                this.tags.Add(file_name);
                this.tags.Add(this.path);
            }
            
        }

        public override string ToString()
        {
            string vals = "{";
            int count = 0;
            foreach (string tag in this.tags)
            {
                vals += tag;
                count++;
                if (count != this.tags.Count)
                {
                    vals += ", ";
                }
            }

            vals += "}";
            return this.id + ", " + vals;
        }

        /// <summary>
        /// Reads the data from the .txt file and stores it in the source_reference attribute.
        /// </summary>
        public void txtToFile()
        {
            // Sets file name and file path as tag by default
            this.SourceReference = System.IO.File.ReadAllLines(this.path);
            
        }

        /// <summary>
        /// Reads the data from the .csv file and stores it in the source_reference attribute.
        /// </summary>
        /// <param name="delimiter"> The delimiter in the csv file </param>
        public void csvToFile(string delimiter = ",")
        {
            this.source_reference = CSVParser.parse(this.path, ",");
        }


        // Getters/Setters
        public int Id
        {
            get => id;
            set => id = value;
        }

        public object SourceReference
        {
            get => source_reference;
            set => source_reference = value;
        }

        public HashSet<string> Tags
        {
            get => tags;
            set => tags = value;
        }
        
        public int Relevance
        {
            get => relevance;
            set => relevance = value;
        }

        public string Path
        {
            get => path;
            set => path = value;
        }

        public string Extension
        {
            get => extension;
            set => extension = value;
        }

    }
}