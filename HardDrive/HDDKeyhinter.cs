using System;
using System.Collections.Generic;

public class HDDKeyhinter
{
    private List<string> tags;
    private List<string> results;
    private List<List<string>> history;
    private int index;


    public HDDKeyhinter(List<string> tags)
    {
        this.tags = tags;
        this.index = 0;
        this.history = new List<List<string>>();
    }

    public List<string> input(char letter)
    {
        // If first time typing
        if (this.index == 0)
        {
            this.results = new List<string>();
            // Iterate through entire list of tags, fresh search
            foreach (string tag in tags)
            {
                if (tag[this.index] == letter)
                {
                    results.Add(tag);
                }
            }
        }
        else
        {
            // Make temp list to store new results in
            List<string> temp = new List<string>();
            // Iterate through current list of tags
            foreach (string tag in results)
            {
                // If not past length of word and letter is as expected, add to new results
                if (!(this.index > tag.Length-1)) { if (tag[this.index] == letter) { temp.Add(tag); } }
            }
            // add old results to history
            this.history.Add(this.results);
            // set old results to new results
            this.results = temp;
            temp = null;
        }
        
        this.index++;
        return this.results;
    }

    public List<string> backspace()
    {
        // If nothing typed, do nothing
        if (this.index == 0) { this.index--; return this.results; }
        // If at index 1 then return entire original list of tags
        if (this.index == 1)
        {
            this.results = this.tags;
        }
        else
        {
            // set current results to last results
            this.results = this.history[this.history.Count - 1];
            // delete last result from history
            this.results.RemoveAt(this.history.Count - 1);
        }

        this.index--;
        return this.results;
    }
    
    // Getters and Setters
    public List<string> Tags { get => tags; set => tags = value; }
}