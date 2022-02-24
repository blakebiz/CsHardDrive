
using System;
using System.Collections.Generic;
using System.IO;
using HardDrive;

public class HddDir : IHddObject
{
    public List<HddFile> Files { get; }
    public List<HddDir> Dirs { get; }


    public HddDir hdd_to_dir(Hdd hdd)
    {
        // Console.WriteLine(hdd.Path);
        Dictionary<string, int> results = new Dictionary<string, int>();
        foreach (HddFile file in hdd.Files)
        {
            int count = 0;
            foreach (char c in file.Path) 
                if (c == '\\') count++;
            results[file.Path] = count;
            Console.WriteLine(file.Path);
            Console.WriteLine(count);
        }

        return this;
    }


}
