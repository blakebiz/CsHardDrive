﻿
using System;
using System.Collections.Generic;
using System.IO;
using HardDrive;
using Directory = System.IO.Directory;

public class PathToHDD
{

    public static List<string> dirSearch(string sDir) 
    {
        List<string> filesFound = new List<string>();
        try
        {
            foreach (string f in Directory.GetFiles(sDir))
            {
                filesFound.Add(f);
            }
            foreach (string d in Directory.GetDirectories(sDir))
            {
                foreach (string file in dirSearch(d))
                {
                    filesFound.Add(file);
                }
            }
        }
        catch (System.Exception excpt)
        {
            Console.WriteLine(excpt.Message);
        }
        return filesFound;
    }

    public static List<string> filterDirs(string sDir, List<string> filters, bool getDirs=false)
    {
        List<string> filesFound = new List<string>();
        try
        {
            foreach (string f in Directory.GetFiles(sDir))
            {
                foreach (string filter in filters)
                {
                    if (f.EndsWith(filter))
                    {
                        filesFound.Add(f); break;
                    }
                }
            }
            foreach (string d in Directory.GetDirectories(sDir))
            {
                if (getDirs) { filesFound.Add(d); }
                List<string> results = filterDirs(d, filters, getDirs);
                foreach (string file in results)
                {
                    filesFound.Add(file);
                }
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return filesFound;

    }
}

