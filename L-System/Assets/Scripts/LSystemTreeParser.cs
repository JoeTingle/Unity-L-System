using System;
using System.Collections;
using System.Collections.Generic;

public static class LSystemTreeParser {

    /// <summary>
    /// Parsing Function which takes in the txt file as a string, with ouput variables
    /// Text file needs to be formatted correctly for it to parse fully
    /// </summary>
    /// <param name="content">The text file as a string</param>
    /// <param name="axiom">output variable for axiom, as a string instead of char. Converted back to char afterwards</param>
    /// <param name="angle">ouput variable for the angle, as a float</param>
    /// <param name="itterations">ouput variable for the number of itterations, as an integer</param>
    /// <param name="rules">output variable dictionary for the rules, with char and string</param>
    /// <param name="srules">output variable for the rules as a string</param>
    /// <param name="zaxis">output variable for z axis variation, as a int, must be in range of 0 to 10</param>
    /// <param name="size">output variable for size of l system tree, as a float</param>
    public static void ParseFile(string content, out string axiom, out float angle, out int itterations, out Dictionary<char, string> rules, out string srules, out int zaxis, out float size)
    {
        //Setting default values for all output variables
        axiom = "";
        angle = 0;
        itterations = 0;
        srules = "";
        zaxis = 0;
        size = 0;

        //Creating dictionary and string array to contain the content
        rules = new Dictionary<char, string>();
        string[] lines = content.Split('\n');

        //Itterating through each line and checking it is formatted correctly, then assigns the correct variable values
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (line.Length == 0)
                continue;
            else if (line.Length == 1 && line[0] == '\r')
                continue;
            else if (line[0] == '/' && line[1] == '/')
                continue;
            string value;
            if (line.IndexOf("axiom") != -1)
            {
                value = line.Substring(line.IndexOf("=") + 1);
                value = value.Trim();
                axiom = value;
            }
            else if (line.IndexOf("angle") != -1)
            {
                value = line.Substring(line.IndexOf("=") + 1);
                value = value.Trim();
                angle = float.Parse(value);
            }
            else if (line.IndexOf("itterations") != -1)
            {
                value = line.Substring(line.IndexOf("=") + 1);
                value = value.Trim();
                itterations = int.Parse(value);
            }
            else if (line.IndexOf("zaxis") != -1)
            {
                value = line.Substring(line.IndexOf("=") + 1);
                value = value.Trim();
                zaxis = int.Parse(value);
            }
            else if (line.IndexOf("size") != -1)
            {
                value = line.Substring(line.IndexOf("=") + 1);
                value = value.Trim();
                size = float.Parse(value);
            }
            else if (line.IndexOf("rules") != -1)
            {
                value = line.Substring(line.IndexOf("=") + 1);
                value = value.Trim();
                srules = value;
            }
            else
            {
                //Throws expection if the file is not in the correct format for it to be parsed
                throw new InvalidOperationException("File Not Formatted Correctly !");
            }
        }
    }

}
