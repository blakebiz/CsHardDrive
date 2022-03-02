using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
 using HardDrive;
using Directory = HardDrive.Directory;
using File = HardDrive.File;

class Program
{
    static void Main(string[] args)
    {
        // TODO test saving files to new _Hdd format
        //filter_excel();
        
        // Excel to hdd object
        // excel_to_hdd_file();

        // Directory to List<string> of file paths to HDD object
        // dir_to_str();

        // Wiki Data Parser
        // WDParser.init();

        // Polution data to hdd:
        // pollution_to_hdd();

        // Flickr data to hdd:
        // flickr_to_hdd();

        // To test out the inventory system:
        // try_inventory();

        // To test out updated search functionality:
        // search_hdd();

        // To try out flight data to Hdd:
        // flights_to_hdd();

        // To try out flight data parser:
        // parse_flights();

        // To try out seeing how much data has been gathered on each country:
        // examine_csv();

        // To try out global covid csv to hdd:
        // csv_to_hdd();

        // To try out country parser:
        // parse_countries();

        // To try out csv parser:
        // parse_csv();

        // To try out keyhints:
        // keyhints();

        // To try out the harddrive class:
        // test_hdd()

    }

    static void filter_excel()
    {
        string path = @"C:\Users\blake\First VR Game\3dPointsTest1.csv";
        Spreadsheet sheet = Spreadsheet.LoadFile(path);

        List<List<object>> filtered = sheet.graph_3D(0, 1, 2, path, 1, 5);
        var dict = new Dictionary<string, List<List<object>>>();
        dict.Add(path, filtered);
        Spreadsheet fsheet = new Spreadsheet(dict, path);

        string filename = fsheet.StoreHdd();
        Console.WriteLine(filename);
    }
    

    static void excel_to_hdd_file()
    {
        List<string> filters = new List<string>() {".xlsm", ".xlsx"};
        string path = @"C:\Users\blake\RiderProjects\Research_Env1.1\Research_Env1.1";
        List<string> results = PathToHDD.filterDirs(path, filters);
        List<IHddObject> files = new List<IHddObject>();
        Console.WriteLine("Creating Files");
        foreach (string result in results)
        {
            // Console.WriteLine(result);
            File file = new File(result);
            files.Add(file);
            // foreach (string s in (string[])file.SourceReference)
            // {
            //     Console.WriteLine(s)
            // }
            // Console.WriteLine();
        }
        Console.WriteLine("Files Created");
        Directory directory = new Directory(files, path);
        foreach (File file in directory.Files)
        {
            if (file.Extension == ".xlsx" || file.Extension == ".xlsm")
            {
                Dictionary<string, List<List<object>>> spreadsheet;
                try
                {
                    spreadsheet = (Spreadsheet) file.SourceReference;
                }
                catch (InvalidCastException)
                {
                    Console.WriteLine("error");
                    continue;
                }

                Console.WriteLine(spreadsheet);
                if (!ReferenceEquals(null, spreadsheet))
                {
                    Spreadsheet sheet = new Spreadsheet(spreadsheet, file.Path);
                    foreach (KeyValuePair<string, List<List<object>>> kv in spreadsheet)
                    {
                        if (kv.Key == "Daily Cash Record")
                        {
                            
                            List<List<object>> frow = sheet.graph_3D(2, 3, 4, "Daily Cash Record");
                            
                            Console.Write($"\n\n{kv.Key}\n\n");
                            foreach (List<object> row in frow)
                            {
                                Console.Write('[');
                                foreach (object item in row)
                                {
                                    try
                                    {
                                        Console.Write(item.ToString() + ", ");
                                    }
                                    catch (System.NullReferenceException)
                                    {
                                        Console.WriteLine("NULLLLLLLLLL");
                                        Console.Write("null, ");
                                    }
                                }

                                Console.WriteLine(']');
                            }
                        }
                        
                    }
                }
            }
            
        }
        
        // Hdd_Dir dir = new Hdd_Dir();
        // dir.hdd_to_dir(hdd);

    }

    static void dir_to_str()
    {
        List<string> filters = new List<string>() {".xlsm", ".xlsx", ".txt", ".csv"};
        List<string> results = PathToHDD.filterDirs(@"C:\Users\blake\RiderProjects\Research_Env1.0\Research_Env1.0", filters);
        List<IHddObject> files = new List<IHddObject>();
        foreach (string result in results)
        {
            files.Add(new File(result));
        }
        Directory directory = new Directory(files);
        // Console.WriteLine(hdd);
    }
    

    static void search_hdd()
    {
        Directory mainDirectory = create_hdd();
        // Create a list of strings to search for in the hard drive
        List<string> search = new List<string>() {"ar", "buggy"};
        
        // Print out all of the files to be stored in the Hdd
        Console.WriteLine("Files in main hard drive:");
        foreach (File file in mainDirectory.Files) { Console.WriteLine(file); }
        Console.WriteLine();
        
        // Create a copy to perform searches on so the main keeps all original data
        Directory copy = mainDirectory.Copy();

        // Perform a "strict" search or an "and" search meaning files found must contain every tag in the search list
        // If apply is not set to false, the search is applied to the harddrive, erasing any irrelevant info
        copy.LooseTagsBySet(search);

        Console.WriteLine("Search results:");
        foreach (File file in copy.Files) { Console.WriteLine(file); }
        Console.WriteLine();
        
        
        // --------------- How to use new filter functions -------------------------
        // Extract filters from copy, this is all that's needed to store the data
        List<List<object>> filters = copy.Filters;
        // Destroy old hard drive to prove it's no longer needed
        copy = null;
        // Make a fresh copy of the main hard rive with all of original data
        copy = mainDirectory.Copy();
        // Apply the filters previously extracted to recreate previously destroyed hard drive
        copy.ApplyFilters(filters);
        
        Console.WriteLine("Recovered hard drive:");
        foreach (File file in copy.Files) { Console.WriteLine(file); }
        Console.WriteLine();
        
        // You can also clear the filters from a harddrive if you like, idk why you would need to,but you can
        // copy.clear_filters();
        

    }

    static Directory create_hdd(int amt = 10)
    {
        // Create an array of tags
        string[] tags = {"desert", "ar", "pistol", "car", "buggy", "dune", "piercing"};
        
        // Create an array of "sample tags", these will be arrays with a random amount of the tags above
        // To change how many change amount, default is 10 so it's easy to read/see it's working
        string[][] tag_sample = sample(tags, amount:amt);
        
        // Create a list to store Hdd_File objects in to be stored in the Hdd (Hard Drive) object
        List<IHddObject> files = new List<IHddObject>();
        
        int count = 0;
        // Loop through sample tags and make a Hdd_File object from their unique tags and give it the id of 
        // the variable count which will then be incremented.
        foreach (string[] tsample in tag_sample) { files.Add(new File(count++, tsample)); }

        // Initiate a Hdd object with the files created above
        return new Directory(files);
    }
    

    static void parse_flights()
    {
        string pt = @"C:\Users\blake\RiderProjects\Research_Env1.0\Research_Env1.0\flightlist_20200401_20200430.csv";
        string del = ",";
        List<Dictionary<string, object>> results = CSVParser.parse_rows(pt, del, new HashSet<int>(){0});
        
        
        // Print results
        foreach (Dictionary<string, object> d in results)
        {
            foreach (KeyValuePair<string, object> kv in d)
            {
                Console.WriteLine(kv.Key + ", " + kv.Value);
            }
            Console.WriteLine();
        }
    }
    

    static void parse_csv()
    {
        string pt = @"C:\Users\blake\RiderProjects\HardDrive\HardDrive\time_series_covid19_confirmed_global.csv";
        CSVParser.parse(pt, ",");
    }

    static void word_search()
    {
        // Create an array of tags
        string[] tags = {"desert", "ar", "pistol", "car", "buggy", "dune", "piercing"};
        
        // Create an array of "sample tags", these will be arrays with a random amount of the tags above
        // To change how many change amount, default is 10 so it's easy to read/see it's working
        string[][] tag_sample = sample(tags, amount:10);

        // Create a list to store Hdd_File objects in to be stored in the Hdd (Hard Drive) object
        List<IHddObject> files = new List<IHddObject>();
        
        int count = 0;
        // Loop through sample tags and make a Hdd_File object from their unique tags and give it the id of 
        // the variable count which will then be incremented.
        foreach (string[] tsample in tag_sample) { files.Add(new File(count++, tsample)); }

        Directory harddrive = new Directory(files);
        Console.WriteLine("Tags:");
        foreach (string tag in harddrive.Tags)
        {
            Console.WriteLine(tag);
        }
        Console.WriteLine("done");

        string result = harddrive.GuessWord("ca");
        Console.WriteLine(result);

    }

    static void keyhints()
    {
        // Make a list of tags to search through
        List<string> tags = new List<string>{"desert", "ar", "pistol", "car", "buggy", "dune", "piercing"};
        HDDKeyhinter kh = new HDDKeyhinter(tags);
        
        // Type a letter (expected results: ar)
        List<string> results = kh.input('a');
        Console.WriteLine("printing results:");
        foreach (string result in results) { Console.WriteLine(result); }
        Console.WriteLine("results printed.\n");
        
        // Backspace (expected results: full original list)
        results = kh.backspace();
        Console.WriteLine("printing results:");
        foreach (string result in results) { Console.WriteLine(result); }
        Console.WriteLine("results printed.\n");
        
        // Type another letter (expected results: desert, dune)
        results = kh.input('d');
        Console.WriteLine("printing results:");
        foreach (string result in results) { Console.WriteLine(result); }
        Console.WriteLine("results printed.\n");
        
        // Type another letter (expected results: desert)
        results = kh.input('e');
        Console.WriteLine("printing results:");
        foreach (string result in results) { Console.WriteLine(result); }
        Console.WriteLine("results printed.\n");
        

    }

    static void test_harddrive()
    {
        // Create an array of tags
        string[] tags = {"desert", "ar", "pistol", "car", "buggy", "dune", "piercing"};
        
        // Create an array of "sample tags", these will be arrays with a random amount of the tags above
        // To change how many change amount, default is 10 so it's easy to read/see it's working
        string[][] tag_sample = sample(tags, amount:10);
        
        // Create a list to store Hdd_File objects in to be stored in the Hdd (Hard Drive) object
        List<IHddObject> files = new List<IHddObject>();
        
        int count = 0;
        // Loop through sample tags and make a Hdd_File object from their unique tags and give it the id of 
        // the variable count which will then be incremented.
        foreach (string[] tsample in tag_sample) { files.Add(new File(count++, tsample)); }
        
        // Print out all of the files to be stored in the Hdd
        foreach (File file in files) { Console.WriteLine(file); }
        Console.WriteLine("\nDone making files.");
        
        // Initiate a Hdd object with the files created above
        Directory harddrive = new Directory(files);
        
        // Create a list of strings to search for in the hard drive
        List<string> search = new List<string>() {"ar", "buggy"};
        
        // Perform a "strict" search or an "and" search meaning files found must contain every tag in the search list
        List<IHddObject> results = harddrive.StrictTagsBySet(search, false);
        
        // Print out results of strict search
        Console.WriteLine("results of strict search");
        foreach (File file in results) { Console.WriteLine(file); }
        Console.WriteLine();
        
        // Perform a "loose" search or an "or" search meaning files found must contain only one of the tags in the search
        List<IHddObject> results1 = harddrive.LooseTagsBySet(search, apply: false);
        
        // Print out results of loose search
        Console.WriteLine("results of loose search");
        foreach (File file in results1) { Console.WriteLine(file); }
        Console.WriteLine();
        
        // Grab first result of loose search to do a "relevancy" search for
        File rel = (File)results1[0];
        
        // Perform a relevancy search
        List<File> results2 = harddrive.GetRelevant(rel, 3);
        
        // Print out results of relevancy search
        Console.Write("Performing a relevancy search on: ");
        Console.WriteLine(rel);
        foreach (File file in results2) { Console.WriteLine(file); }

    }

    static string[][] sample(string[] tags, int amount = 10)
    {
        Random rand = new Random();
        string[][] tag_list = new string[amount][];
        for (int i = 0; i < amount; i++)
        {
            int amt = rand.Next(1, tags.Length - 1);
            int[] choices = new int[amt];
            for (int x = 0; x < amt; x++)
            {
                int choice = rand.Next(tags.Length - 1);
                while (choices.Contains(choice)) { choice = rand.Next(tags.Length - 1); }
                choices[x] = choice;
            }

            string[] selections = new string[amt];
            for (int x = 0; x < amt; x++)
            {
                selections[x] = tags[choices[x]];
            }

            tag_list[i] = selections;
        }

        return tag_list;
    }
}
