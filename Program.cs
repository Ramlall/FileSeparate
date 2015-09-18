using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

// Kay so. This program separates the strings in a text file. 
// This program is designed around Regshot and registry values. 
// We have an input file full of registry key lines regshot provides us (and a few stray lines for organization)
// For pure registry keys that need to be added or deleted, we format the string to be added to a list.
// For a value that has to be added/modified, we take the path part, format it to be added to a list and move that to a file,
// we take the value name, format it to be added to a list and put that in another file, and lastly we 
// take the actual value, convert it to it's property registry value kind and add that to a list in a different file.

namespace FileOps
    {
	class Program
	    {
		static void Main(string[] args)
		    {
            // Create an empty string for which line we're on. We'll use this to read the lines later.
			string line;

			// Define our input file and output files and their locations, and their variable names.
			StreamReader input = new System.IO.StreamReader(@"c:\work\FileSeparate\input.txt");
			StreamWriter paths = new StreamWriter(@"c:\work\FileSeparate\justpaths.txt");
            StreamWriter valuenames = new StreamWriter(@"c:\work\FileSeparate\justvaluenames.txt");
            StreamWriter values = new StreamWriter(@"c:\work\FileSeparate\justvalues.txt");

            // This tells us which line of the file we're on. 
			int lineCount = File.ReadLines(@"c:\work\FileSeparate\input.txt").Count();

            // Scrolling through all the lines in the file.
			for (int d = 0; d < lineCount; d++)
			    {
                // Read the input line and store it in the variable called "line"
				line = input.ReadLine();

				// If it's a blank line then put a blank line in our output files so the numbers of the lines all match up.
				if (line.Count() == 0)
				    {
					paths.WriteLine();
                    valuenames.WriteLine();
					values.WriteLine();
					continue;
				    }

                // HKLM lines are very long and take a long time to process programatically. There are few enough of them
                // that we'll manually type in the info, other wise the program running time will be high and we'll be waiting.
                if (line.Count() >= 3 && line[0] == 'H' && line[1] == 'K' && line[2] == 'L' && line[3] == 'M')
				    {
					paths.WriteLine("HEYYYYY OVER HERE. This was an HKLM line and it was too long to process. Do this one manually yourself! In the input it's line {0}!", d);
                    valuenames.WriteLine("HEYYYYY OVER HERE. This was an HKLM line and it was too long to process. Do this one manually yourself! In the input it's line {0}!", d);
                    values.WriteLine("HEYYYYY OVER HERE. This was an HKLM line and it was too long to process. Do this one manually yourself! In the input it's line {0}!", d);
					continue;
				    }

                // Check if the line is a value to be modified. If so we need to seperate the string into pieces.
				for (int w = 0; w < line.Count(); w++)
				    {
                    // We know it's a value modified because there's a semicolon followed by a space.
                    // We need to specify the space too because some registry paths have a semicolon in the string.
					if (line[w] == ':' && line[w + 1] == ' ')
					    {
                        // To the output files start by setting up the frame for adding to a list called tempList.
						paths.Write("tempList.Add(@\"");
                        valuenames.Write("tempList.Add(\"");
                        values.Write("tempList.Add(\"");

                        // Now we need to find where the path ends and the value begins.
                        int h;
                        // Starting at the semicolon that separates the path from the value,
                        for (h=w; w > 0; h--)
                            {
                            // Find the first "\" to the left. 
                            if (line[h]=='\\') // Two backslashes for C# syntax. We're actually just looking for one.
                                {
                                break;
                                }
                            }

                        // Write to the "paths" output file the beginning of the string. 
						for (int k = 0; k <= h; k++) // Will go from HKU\Whatever\    and then value name is left out.
						    {
							paths.Write(line[k]);
						    }
                        // Write to the "valuenames" output file everything after the space.
                        for (int p = h+1; p < w; p++) // Go from the first char after \ to the semicolon
                            {
                            valuenames.Write(line[p]);
                            }
                        // Write to the "values" output file everything after the space.
                        for (int m = w + 2; m < line.Count(); m++)
						    {
							values.Write(line[m]);
						    }
                        // Close up our AddToList framework.
						paths.WriteLine("\");");
                        valuenames.WriteLine("\");");
                        values.WriteLine("\");");
						break;
					    }

                    // If we get to the end of a non-blank line. 
                    // We handle organizational lines and Keys to be Added paths here since they're very simple. 
                    if (w == line.Count() - 1)
					    {
                        // If the line doesn't begin with a /. If it has a / then we know we designated it
                        // as a comment for organizational purposes when we organized the input file.
						if (line[0] != '/')
						    {
                            // Setup the list format for the path of the string (for the key to be added)
							paths.Write("tempList.Add(@\"");
						    }
                        // Write everything from that line into our string for our list in the Paths file. This isn't part
                        // of the if Block because we write the contents to the path file regardless of what it is.
                        for (int n = 0; n < line.Count(); n++)
						    {
							paths.Write(line[n]);
						    }
                        // Now we can close up our adding to the list format for the Path.
                        if (line[0] != '/')
                            {
                            paths.Write("\");");
                            }

                        // If the beginning of the line is a comment, we'll also add it to the Values and ValueNames
                        // file for the sake of organization and keeping line numbers consistent.
                        if (line[0] == '/')
						    {
							for (int h = 0; h < line.Count(); h++)
							    {
								values.Write(line[h]);
							    }
						    }
                        if (line[0] == '/')
                            {
                            for (int h = 0; h < line.Count(); h++)
                                {
                                valuenames.Write(line[h]);
                                }
                            }
                        
                        // Go to the next line.
						values.WriteLine();
                        valuenames.WriteLine();
						paths.WriteLine();
						break;
					    }
				    }
			    }

			input.Close();
			paths.Close();
            valuenames.Close();
			values.Close();
		    }
	    }
    }
