using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SourceManipulation
{
    #region GET_TRIMED_SOURCE_LINES
    /// <summary>
    /// Trims source, removes empty lines and removes comments
    /// </summary>
    public static List<string> GetTrimedSourceLines(string source)
    {
        ///split by new line and trim all white spaces
        var lines = source
            .Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim(' ', '\t', '\r', '\t', '\v'))
            .ToList();

        /// removing all empty lines
        lines = lines.Where(x => x != "").ToList();

        ///removing line comments
        for (int i = 0; i < lines.Count; i++)
        {
            var indexOfLineComment = lines[i].IndexOf("//");
            if (indexOfLineComment != -1)
            {
                lines[i] = lines[i].Substring(0, indexOfLineComment);
            }
        }
        ///...
        ///
        #region REMOVE /**/ COMMENTS
        ///collection the loactions of all /**/ comments
        var openingBrakets = new List<CommentInfo>();
        var closingBrackets = new List<CommentInfo>();
        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            var indexOfOpeningBraket = line.IndexOf("/*");
            while (indexOfOpeningBraket != -1)
            {
                openingBrakets.Add(new CommentInfo(i, indexOfOpeningBraket));
                indexOfOpeningBraket = line.IndexOf("/*", indexOfOpeningBraket + 1);
            }

            var indexOfClosingBraket = line.IndexOf("*/");
            while (indexOfClosingBraket != -1)
            {
                closingBrackets.Add(new CommentInfo(i, indexOfClosingBraket));
                indexOfClosingBraket = line.IndexOf("*/", indexOfClosingBraket + 1);
            }
        }

        ///If we have /**/ comments, we romove them from the source
        if (openingBrakets.Count > 0 && closingBrackets.Count > 0)
        {
            ///Comment brackets mismatch///Questionable if necessary
            if (openingBrakets.First().CompareTo(closingBrackets.First()) > 0)
            {
                Debug.Log("First closing comment bracket before first opening comment bracket!");
                return null;
            }

            if (openingBrakets.Last().CompareTo(closingBrackets.Last()) > 0)
            {
                Debug.Log("Last opening comment bracket is after last closing comment bracket!");
                return null;
            }
            ///...

            var openingBraket = openingBrakets.First();

            while (openingBraket != null)
            {
                var closingBracket = closingBrackets.Where(x => x.CompareTo(openingBraket) > 0).FirstOrDefault();

                if (closingBracket == null)
                {
                    Debug.Log("Braket mismath");
                    return null;
                }

                ///delete the whole comented lines
                if (closingBracket.Line - openingBraket.Line > 1)
                {
                    for (int i = openingBraket.Line + 1; i < closingBracket.Line; i++)
                    {
                        lines[i] = "";
                    }
                }
                ///...

                /*Delete the comment from partially commented lines*/
                //Debug.Log(openingBraket + "|" + closingBracket);
                //Debug.Log(closingBracket.Col + 2 + " " + lines[closingBracket.Line].Length);

                ///If lines open and close same line -> we cut out the comment and append spaces that will 
                ///later be removed so the data for the next comment on the same line is the same 
                ///as at the point where it was collected
                if (closingBracket.Line == openingBraket.Line)
                {
                    var lineLenght = lines[openingBraket.Line].Length;
                    lines[openingBraket.Line] = lines[openingBraket.Line].Substring(0, openingBraket.Col) + lines[closingBracket.Line].Substring(closingBracket.Col + 2);
                    var newLineLenght = lines[openingBraket.Line].Length;
                    var dif = lineLenght - newLineLenght;
                    lines[openingBraket.Line] = new string(' ', dif) + lines[openingBraket.Line];
                }
                else
                {
                    lines[closingBracket.Line] = lines[closingBracket.Line].Substring(closingBracket.Col + 2);
                    lines[openingBraket.Line] = lines[openingBraket.Line].Substring(0, openingBraket.Col);
                }

                openingBraket = openingBrakets.Where(x => x.CompareTo(closingBracket) > 0).FirstOrDefault();
            }
        }
        #endregion

        ///trim to remove the spaces added for the multiple comments per line cenario
        lines = lines.Select(x => x.Trim(' ', '\t', '\r', '\t', '\v')).ToList();
        ///final trim to remove line comments that started at the beginning of the line
        lines = lines.Where(x => x != "").ToList();

        return lines;
    }

    class CommentInfo : IComparable<CommentInfo>
    {
        public CommentInfo(int line, int col)
        {
            Line = line;
            Col = col;
        }

        public int Line { get; set; }
        public int Col { get; set; }

        public int CompareTo(CommentInfo other)
        {
            var lineComp = this.Line.CompareTo(other.Line);
            if (lineComp == 0)
            {
                return this.Col.CompareTo(other.Col);
            }

            return lineComp;
        }

        public override string ToString()
        {
            return $"Line: {this.Line} Col: {this.Col}";
        }
    }
    #endregion

    #region ADD_SELF_ATTACH_TO_SOURCE
    /// <summary>
    /// Finds the closing bracket and just above it insert the Attach method, so 
    /// it does not have to be pre added to every source the user submits 
    /// </summary>
    public static string AddSelfAttachToSource(string source)
    {
        /// spliting the code by lines and trims then as well as remove empty lines,
        /// it is assumed that brackets {,} will awlays be on new lines
        var lines = GetTrimedSourceLines(source).ToList();

        ///If last line is not soly } after trim return null
        if (lines[lines.Count - 1] != "}")
        {
            Debug.LogError("File does not end with \"}\"");
            return null;
        }

        ///If can not find opening bracket return null
        var openingBraketLineNumber = lines.IndexOf("{");
        if (openingBraketLineNumber == -1)
        {
            Debug.Log("Can not find opening bracket braket!");
            return null;
        }

        ///The line right above is expected to be the class declaration line, 
        ///from which we can extract the name of the class
        var classLine = lines[openingBraketLineNumber - 1];

        ///we get the parts of the class declaration line
        var classDeclarationLineParts = classLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        ///we find the index of class inside the above class declaration line parts
        var indexOfClass = Array.IndexOf(classDeclarationLineParts, "class");

        ///if there is no class keyword in this line we return null
        if (indexOfClass == -1)
        {
            Debug.Log("Can not find class keyword in the class declaration line!");
            return null;
        }

        ///if the class keyword is the last item in the parts array, return null since we can not get name
        if (classDeclarationLineParts.Length - 1 == indexOfClass)
        {
            Debug.Log("Can not find the name of the class after the class keyword!");
            return null;
        }

        var name = string.Empty;
        ///we get tha part just after class, as it should contein the class name
        var namePart = classDeclarationLineParts[indexOfClass + 1];
        ///if that contains :, which is valid inheratance syntaxis, we take just the part before : which is the name
        if (namePart.Contains(":"))
        {
            name = namePart.Substring(0, namePart.IndexOf(":"));
        }
        ///if not, we just take the whole thing as the name
        else
        {
            name = namePart;
        }

        if (name.Length == 0)
        {
            Debug.Log("Name not found!");
            return null;
        }

        /// we insert that bit of text at the end of the class so we can later use reflection on that fucntion 
        /// and use to attach the whole class to gameobject and return the instance of the class that is attached
        var toInsrt = $@"//--ATTACH--METHOD--HERE
public static {name} Attach(GameObject obj)
{{
    return obj.AddComponent<{name}>();
}}";

        string[] classDecLines = lines.Where(x => x.StartsWith("public class") || x.StartsWith("private class") || x.StartsWith("class")).ToArray();

        if (classDecLines.Length == 1)
        {
            lines.Insert(lines.Count - 1, toInsrt);
        }
        else
        {
            int ind = Array.IndexOf(lines.ToArray(), classDecLines[1]) - 1;
            lines.Insert(ind, toInsrt);
        }

        ///joining the whole text together
        var result = string.Join("\n", lines);

        return result;
    }
    #endregion
}

