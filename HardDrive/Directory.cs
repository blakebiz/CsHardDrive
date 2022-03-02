
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace HardDrive
{


    public class Directory : IHddObject
    {
        public List<IHddObject> Files { get; private set; } = new List<IHddObject>();
        public List<string> Tags { get; private set; }= new List<string>();
        public List<List<object>> Filters { get; private set; } = new List<List<object>>();
        public string? Path { get; private set; }
        
        
        public Directory(string path)
        {
            this.Path = path;
        }

        public Directory(List<IHddObject> file_list)
        {
            this.Files = file_list;
            this.SetTags();
        }

        public Directory(List<IHddObject> file_list, string path)
        {
            this.Files = file_list;
            this.SetTags();
            this.Path = path;
        }

        public Directory(Directory directory)
        {
            // TODO convert from shallow copy to hard copy
            this.Files = directory.Files;
            this.Tags = directory.Tags;
            this.Filters = directory.Filters;
            this.Path = directory.Path;
        }

        public Directory(Directory directory, string path)
        {
            this.Files = directory.Files;
            this.SetTags();
            this.Path = path;
        }

        public Directory()
        {
        }

        public Directory(List<string> paths)
        {
            foreach (string path in paths)
            {
                this.Files.Add(new File(path));
            }
            this.SetTags();
        }

        /// <summary>
        /// allows manually overriding the files of the hard drive
        /// </summary>
        /// <param name="files"> The list of hdd files/directories to set as the files of the hard drive </param>
        public void SetFiles(List<IHddObject> files)
        {
            this.Files = files;
            this.SetTags();
        }

        /// <summary>
        /// This method sets the tags of the files in the hdd.
        /// </summary>
        /// <remarks> Tags are used to search through files for given tag(s) </remarks>
        /// <param name="files"></param>
        /// <param name="clearOld"> Whether or not to clear the previous tags attached to the object.
        /// You should probably never touch this parameter</param>
        public void SetTags(List<IHddObject>? files = null, bool clearOld = true)
        {
            files ??= this.Files;
            // TODO This method needs to be updated to work with Hdd objects
            if (clearOld) this.Tags = new List<string>();
            foreach (IHddObject file in this.Files)
            {
                if (file is File hddFile)
                {
                    foreach (string tag in hddFile.Tags)
                    {
                        if (!this.Tags.Contains(tag.ToLower()))
                        {
                            this.Tags.Add(tag.ToLower());
                        }
                    }
                }
                else if (file is Directory hdd)
                {
                    this.SetTags(hdd.Files, false);
                }
            }
        }

        /// <summary>
        /// clears all data from the hard drive
        /// </summary>
        public void ClearData()
        {
            this.Files = new List<IHddObject>();
            this.Tags = new List<string>();
            this.Filters = new List<List<object>>();
            this.Path = null;
        }

        /// <summary>
        /// Accepts a list of filters and applies them to the hard drive
        /// </summary>
        /// <param name="filters"> A list of filters to filter the hard drive. Some methods in the class like
        /// StrictTagsBySet will return this. You likely never want to manually make this list unless you contact
        /// Blake first. </param>
        /// <returns> New Hdd object with the filters applied to it </returns>
        public Directory ApplyFilters(List<List<object>> filters)
        {
            foreach (List<object> search in filters)
            {
                switch ((string) search[0])
                {
                    case "stbs":
                        this.StrictTagsBySet((List<string>) search[1]);
                        break;
                    case "ltbs":
                        this.LooseTagsBySet((List<string>) search[1]);
                        break;
                }
            }

            return this;
        }

        /// <summary>
        /// clears all filters from the hard drive
        /// </summary>
        public void ClearFilters()
        {
            this.Filters = new List<List<object>>();
        }
        
        /// <summary>
        /// Returns a copy of the hard drive. This is useful especially for filtering so you can preserve the original
        /// data and apply filters to it.
        /// </summary>
        /// <returns> A fresh instance of a hard drive with all of the information being the same </returns>
        public Directory Copy()
        {
            return new Directory(this);
        }

        /// <summary>
        /// Gets all files in a directory and all subdirectories and returns them as a flattened list
        /// </summary>
        /// <param name="dir"> The hdd dir to search for files </param>
        /// <returns> List of HddFile objects found in the directories </returns>
        List<File> GetFiles()
        {
            List<File> files = new List<File>();
            foreach (IHddObject file in this.Files)
            {
                if (file is File hddFile)
                {
                    files.Add(hddFile);
                }
                else if (file is Directory hdd)
                {
                    files.AddRange(this.GetFiles());
                }
            }

            return files;
        }
        

        /// <summary>
        /// Searches the hard drive for files with **all** of the given tags. For now this will delete
        /// any hard drive directories and only the files will be preserved.
        /// </summary>
        /// <param name="tags"> A list of tags to search for (case insensitive) </param>
        /// <param name="apply"> Whether or not to apply the changes to the hard drive </param>
        /// <param name="log"> Whether or not to log the changes to be able to repeat these searches later.
        /// If apply is false this parameter is meaningless as the changes aren't applied.
        /// You probably don't want to touch this parameter. </param>
        /// <returns> Returns the new list of files </returns>
        public List<IHddObject> StrictTagsBySet(List<string> tags, bool apply = true, bool log = true)
        {
            // TODO update to not delete Hdd objects
            
            // if user wants to apply filters and log the search filters. If user doesn't want to apply,
            // then we don't want to log the search filters.
            if (log && apply)
            {
                // Log search
                Filters.Add(new List<object>() {"stbs", tags});
            }

            // Initialize empty list to store results in
            List<IHddObject> results = new List<IHddObject>();

            // If given empty tags list then return empty results list
            if (tags.Count == 0)
            {
                return results;
            }
            // code added to handle hard drive directories but needs to be reworked
            // just gets all files from any directories but this causes the directory to be deleted
            List<File> files = this.GetFiles();
            
            // iterate through the files
            foreach (File file in files)
            {
                // whether or not the file has all the tags
                bool hasAllTags = true;
                // iterate through the tags needed to be found
                foreach (string tag in tags)
                {

                    // if the file has the tag
                    if (!file.Tags.Contains(tag.ToLower()))
                    {
                        hasAllTags = false;
                        break;
                    }
                }
                // if the file has all the tags
                if (hasAllTags)
                {
                    // add the file to the results
                    results.Add(file);
                }
            }
            
            
            if (apply)
            {
                this.Files = results;
                this.SetTags();
            }

            return results;
        }

        /// <summary>
        /// Searches the hard drive for files with **one** of the given tags. For now this will delete
        /// any hard drive directories and only the files will be preserved.
        /// </summary>
        /// <param name="tags"> A list of tags to search for (case insensitive) </param>
        /// <param name="apply"> Whether or not to apply the changes to the hard drive </param>
        /// <param name="log"> Whether or not to log the changes to be able to repeat these searches later.
        /// If apply is false this parameter is meaningless as the changes aren't applied.
        /// You probably don't want to touch this parameter. </param>
        /// <returns> Returns the new list of files </returns>
        public List<IHddObject> LooseTagsBySet(List<string> tags, bool apply = true, bool log = true)
        {
            if (log && apply)
            {
                // Log search
                Filters.Add(new List<object>() {"ltbs", tags});
            }

            // Initialize empty list to store results in
            List<IHddObject> results = new List<IHddObject>();

            // If given empty tags list then return empty results list
            if (tags.Count == 0)
            {
                return results;
            }

            // Iterate through all files
            foreach (File file in this.Files)
            {
                // Iterate through given tags
                foreach (string tag in tags)
                {
                    // If the file has the tag then add it to the results and break out of the inner loop as file is valid
                    if (file.Tags.Contains(tag.ToLower()))
                    {
                        results.Add(file);
                        break;
                    }
                }
            }

            if (apply)
            {
                this.Files = results;
                this.SetTags();
            }

            return results;
        }

        /// <summary>
        /// Returns a list of files that have the most tags in common with the given file
        /// </summary>
        /// <param name="file"> The file to compare others to </param>
        /// <param name="count"> The amount of relevant files to return </param>
        /// <returns></returns>
        public List<File> GetRelevant(File file, int count = 5)
        {
            // Initialize empty list to store results in
            List<File> results = new List<File>();
            // Get count of similar tags for each file
            foreach (File f in Files)
            {
                // if not the original file
                if (f != file)
                {
                    // loop through original files tags
                    foreach (string tag in f.Tags)
                    {
                        // if original file tag in current files tags, increment it's relevance
                        if (file.Tags.Contains(tag.ToLower()))
                        {
                            f.Relevance++;
                        }
                    }

                    results.Add(f);
                }
            }


            List<File> top = new List<File>();
            // Get max from list <count> times
            for (int i = 0; i < count; i++)
            {
                // if no results left, return however many have been grabbed so far
                if (results.Count == 0)
                {
                    return top;
                }

                File max = results[0];
                int ind = 0, counter = 0;

                // basic get max alg (n complexity)
                foreach (File f in results)
                {
                    if (f.Relevance > max.Relevance)
                    {
                        max = f;
                        ind = counter;
                    }

                    counter++;
                }

                // reset relevance for next search
                max.Relevance = 0;

                // add result to return list
                top.Add(max);
                // remove result from list so no dupes
                results.RemoveAt(ind);
            }

            // reset relevance for next search
            foreach (File f in results)
            {
                f.Relevance = 0;
            }

            return top;

        }

        /// <summary>
        /// Tries to guess the most relevant tags given a word. This is intended to be used by a search bar that
        /// wants to offer auto complete suggestions when searching through files. See HDDKeyhinter.cs for an easier
        /// interface to use that allows guessing character by character.
        /// </summary>
        /// <param name="word"> The word to search for </param>
        /// <returns> The closest tag to the given word</returns>
        public string GuessWord(string word)
        {
            int max = -1;
            string result = null;
            foreach (string tag in this.Tags)
            {
                int rel = 0;
                foreach (char l in word.ToLower())
                {
                    if (tag.ToLower().Contains(l))
                    {
                        rel++;
                    }
                }

                if (rel > max)
                {
                    max = rel;
                    result = tag;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts all drives and files to strings seperated by newlines
        /// </summary>
        public override string ToString()
        {
            string value = "";
            foreach (IHddObject file in this.Files)
            {
                try
                {
                    value += file + "\n";
                }
                catch (NullReferenceException)
                {
                }
            }
            return value;
        }

        /// <summary>
        /// NOT IMPLEMENTED YET
        /// Saves all files in the hard drive to a directory on client device
        /// </summary>
        /// <param name="directoryName"> what to name the directory this data is stored in. If no filename is given,
        /// defaults to this.Path
        /// </param>
        /// <returns> The directory name the directory is saved to </returns>
        public string ExportHdd(string? directoryName = null)
        {
            /*
             TODO implement this method
             This method should export the current Hdd to a directory of it's contents.
             */
            
            // if no filename given, use default
            // this line is basically saying if filename is null, set filename to this.path
            directoryName ??= this.Path;
            if (directoryName == null)
            {
                throw new ArgumentNullException("directoryName", "No directory name given");
            }
            // code for saving files goes here
            return directoryName;
        }
        
    }
}
