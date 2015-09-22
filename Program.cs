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
            Console.WriteLine("We're about to separate your text file of registry info.");
            Console.WriteLine("Processing the input.txt file. Please wait, this will probably take a while.");
            Console.WriteLine("When this black box disappears the program is completed.");
            Console.WriteLine("Wait only until the program is completed to close this window.");
            Console.WriteLine("If you close the window early then the five files generated won't be complete.");

            // Create an empty string for which line we're on. We'll use this to read the lines later.
            string line;

            // Define our input file and output files and their locations, and their variable names.
            StreamReader input = new StreamReader("input.txt");
            StreamWriter paths = new StreamWriter("justpaths.txt");
            StreamWriter valuenames = new StreamWriter("justvaluenames.txt");
            StreamWriter values = new StreamWriter("justvalues.txt");
            StreamWriter valuekinds = new StreamWriter("justvaluekinds.txt");
            StreamWriter parentKeys = new StreamWriter("justparentkeys.txt");

            // This tells us which line of the file we're on. 
            int lineCount = File.ReadLines("input.txt").Count();

            // Scrolling through all the lines in the file.
			for (int d = 0; d < lineCount; d++)
			    {
                // Write the current line to the system.
                System.Console.WriteLine("We're on line number {0}", d+1);

                // Read the input line and store it in the variable called "line"
				line = input.ReadLine();

				// If it's a blank line then put a blank line in our output files so the numbers of the lines all match up.
				if (line.Count() == 0)
				    {
					paths.WriteLine();
                    valuenames.WriteLine();
					values.WriteLine();
                    valuekinds.WriteLine();
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
                        values.Write("tempList.Add(");
                        valuekinds.Write("tempList.Add(");
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

                        // "Value" Processing@@@@@@@@@@@@@@@@@@@@@
                        // Write the "values" to a string so we can process it.
                        string valueUnModified = "";
                        for (int m = w + 2; m < line.Count(); m++)
						    {
                            valueUnModified = valueUnModified + line[m];
						    }
                        // So now valueUnModified is a string of the value.
                        // Let's process it to determine the type of value it is (binary, string, or DWord)
                        
                        // If it starts with a quotation mark then it's a string.
                        if (valueUnModified[0] == '"')
                            {
                            // Write the valueKind as String to the output file
                            valuekinds.Write("RegistryValueKind.String");
                            // We can just write the string itself.
                            values.Write(valueUnModified);
                            }
                        // If it starts with a ' then it's a Multistring. 
                        // '1,http: 1,file: 1,ftp: 1,mailto: 1,news: "1,//www." 1,www. 2,windows'
                        // new string[] { "One", "Two", "Three" }
                        else if (valueUnModified[0] == '\'')
                            {
                            // Write the valueKind as String to the output file
                            valuekinds.Write("RegistryValueKind.MultiString");
                            values.Write("new string[] { \"");
                            // Go through the string and every time there's a space separate it.
                            for (int k = 1; k < valueUnModified.Count() - 1; k++) // Go from the char after ' to the one before the last '
                                {
                                if (valueUnModified[k] != ' ')
                                    { values.Write(valueUnModified[k]); }
                                else
                                    { values.Write("\", \""); }
                                }
                                values.Write("\" }");
                            }
                        // If the second character is an x then it's DWord
                        else if (valueUnModified[1] == 'x')
                            {
                            // Write the valueKind as DWord to the output file
                            valuekinds.Write("RegistryValueKind.DWord");
                            // We can just write the string itself for DWord
                            values.Write(valueUnModified);
                            }
                        // Otherwise it's just binary.
                        else
                            {
                            // Write the valueKind as Binary to the output file
                            valuekinds.Write("RegistryValueKind.Binary");

                            // Our valueUnModified looks like "20 3F FF" currently.
                            // It has to look like   new byte[] { 0x20, 0x3F, 0xFF }    when we're done

                            // Make a new string
                            string valueModified = "new byte[] {";
                            
                            // Go through the unmodified string adding in the numbers.
                            for (int k = 0; k < valueUnModified.Count() ;  )
                                {
                                // Add 0x##,
                                valueModified = valueModified + "0x" + valueUnModified[k] + valueUnModified[k+1];
                                // Our next stop is 3 spots ahead. (That is, skip the 2 numbers we just processed + the blank space)
                                k = k + 3;

                                // If we have more numbers to process then add a ", "
                                if (k < valueUnModified.Count() )
                                    {
                                    valueModified = valueModified + ", ";
                                    }
                                }

                            // Close it up.
                            valueModified = valueModified + "}";
                            // Write the modified string to our values output file.
                            values.Write(valueModified);
                            }

                        // END OF VALUE PROCESSING @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                        // Close up our framework for adding to the temp list.
                        paths.WriteLine("\");");
                        valuenames.WriteLine("\");");
                        values.WriteLine(");");
                        valuekinds.WriteLine(");");
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
                            parentKeys.Write(");");
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
                                valuekinds.Write(line[h]);
                                parentKeys.Write(line[h]);
                                }
                            }
                        
                        // Go to the next line.
						values.WriteLine();
                        valuekinds.WriteLine();
						paths.WriteLine();
                        valuenames.WriteLine();
                        parentKeys.WriteLine();
						break;
					    }
				    }
			    }

			input.Close();
			paths.Close();
            valuenames.Close();
			values.Close();
            valuekinds.Close();
            parentKeys.Close();

		    }
	    }
    }
