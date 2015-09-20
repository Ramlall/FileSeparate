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
            // Tell the user what's up.
            System.Console.WriteLine("We're about to separate your text file of registry info.");
            System.Console.WriteLine("Make sure the file is called input.txt and is in the working directory.");

            // Create an empty string for which line we're on. We'll use this to read the lines later.
            string line;

            // Define our input file and output files and their locations, and their variable names.
            StreamReader input = new StreamReader("input.txt");
            StreamWriter paths = new StreamWriter("justpaths.txt");
            StreamWriter valuenames = new StreamWriter("justvaluenames.txt");
            StreamWriter values = new StreamWriter("justvalues.txt");
            StreamWriter parentKeys = new StreamWriter("justparentkeys.txt");

            // This tells us which line of the file we're on. 
            int lineCount = File.ReadLines("input.txt").Count();

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
                    parentKeys.WriteLine();
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
                        parentKeys.Write("tempList.Add(");

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
                        
                        // Handle if it's an HKU
                        if (line[0] == 'H' && line[1] == 'K' && line[2] == 'U')
                            {
                            // Then the paths starts at a different place & our parentKey is different.

                            // Set the parentKey to Users.
                            parentKeys.Write("Registry.Users");

                            // Write to the "paths" output file the subkey.
                            for (int k = 4; k <= h; k++) // Will go from HKU\Whatever\    and then value name is left out.
                                {
                                paths.Write(line[k]);
                                }
                            }

                        // Handle if it's an HKLM
                        if (line[0] == 'H' && line[1] == 'K' && line[2] == 'L' && line[3] == 'M')
                            {
                            // Then the paths starts at a different place & our parentKey is different.

                            // Set the parentKey to Users.
                            parentKeys.Write("Registry.LocalMachine");

                            // Write to the "paths" output file the subkey.
                            for (int k = 5; k <= h; k++) // Will go from HKU\Whatever\    and then value name is left out.
                                {
                                paths.Write(line[k]);
                                }
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
                        parentKeys.WriteLine(");");
                        break;
					    }

                    // If we get to the end of a non-blank line that isn't a value. (It could be a key to be added)
                    // We handle organizational lines and Keys to be Added paths here since they're very simple. 
                    if (w == line.Count() - 1)
					    {
                        // If the line doesn't begin with a /. If it has a / then we know we designated it
                        // as a comment for organizational purposes when we organized the input file.
						if (line[0] != '/')
						    {
                            // Setup the list format for the path of the string (for the key to be added)
							paths.Write("tempList.Add(@\"");
                            parentKeys.Write("tempList.Add(");

                            // If it's HKU
                            if (line[0] == 'H' && line[1] == 'K' && line[2] == 'U')
                                {
                                // Write the parentkey to parentkey file.
                                parentKeys.Write("Registry.Users");

                                // Write to paths starting from a different index.
                                for (int n = 4; n < line.Count(); n++)
                                    {
                                    paths.Write(line[n]);
                                    }
                                }
                            // If it's HKLM
                            if (line[0] == 'H' && line[1] == 'K' && line[2] == 'L' && line[3] == 'M')
                                {
                                // Write the parentkey to parentkey file.
                                parentKeys.Write("Registry.LocalMachine");

                                // Write to paths starting from a different index.
                                for (int n = 5; n < line.Count(); n++)
                                    {
                                    paths.Write(line[n]);
                                    }
                                }

                            // Now we can close up our adding to the list format for the Path.
                            paths.Write("\");");
                            parentKeys.WriteLine(");");
                            }

                        // If the beginning of the line is a comment, we'll also add it to the Values and ValueNames
                        // file for the sake of organization and keeping line numbers consistent.
                        if (line[0] == '/')
                            {
                            for (int h = 0; h < line.Count(); h++)
                                {
                                paths.Write(line[h]);
                                values.Write(line[h]);
                                valuenames.Write(line[h]);
                                parentKeys.Write(line[h]);
                                }
                            }
                        
                        // Go to the next line.
						values.WriteLine();
                        valuenames.WriteLine();
						paths.WriteLine();
                        parentKeys.WriteLine();
						break;
					    }
				    }
			    }

			input.Close();
			paths.Close();
            valuenames.Close();
			values.Close();
            parentKeys.Close();

		    }
	    }
    }
