using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FileOps
{
	class Program
	{
		static void Main(string[] args)
		{
			string line;

			// Read the file and display it line by line.
			System.IO.StreamReader file = new System.IO.StreamReader(@"c:\work\FileSeparation\input.txt");
			StreamWriter paths = new StreamWriter(@"c:\work\FileSeparation\justpaths.txt");
			StreamWriter values = new StreamWriter(@"c:\work\FileSeparation\justvalues.txt");


			int lineCount = File.ReadLines(@"c:\work\FileSeparation\input.txt").Count();
			//Console.WriteLine(lineCount);

			for (int d = 0; d < lineCount; d++)
			{
				line = file.ReadLine();
				//Console.WriteLine(d);

				// If it's a blank line.
				if (line.Count() == 0)
				{
					paths.WriteLine();
					values.WriteLine();
					continue;
				}

				// HKLM lines are too long.
				if (line.Count() >= 3 && line[0] == 'H' && line[1] == 'K' && line[2] == 'L' && line[3] == 'M')
				{
					paths.WriteLine("HEYYYYY OVER HERE. This was an HKLM line and it was too long to process. Do this one manually yourself! In the input it's line {0}!", d);
					values.WriteLine("HEYYYYY OVER HERE. This was an HKLM line and it was too long to process. Do this one manually yourself! In the input it's line {0}!", d);
					continue;
				}

				for (int w = 0; w < line.Count(); w++)
				{
					if (line[w] == ':' && line[w + 1] == ' ')
					{
						for (int k = 0; k < w; k++)
						{
							paths.Write(line[k]);
						}
						for (int m = w + 2; m < line.Count(); m++)
						{
							values.Write(line[m]);
						}
						paths.WriteLine();
						values.WriteLine();
						break;
					}

					// If we get to the end of a non-blank line.
					if (w == line.Count() - 1)
					{
						for (int n = 0; n < line.Count(); n++)
						{
							paths.Write(line[n]);
						}
						if (line[0] == '/')
						{
							for (int h = 0; h < line.Count(); h++)
							{
								values.Write(line[h]);
							}
						}

						values.WriteLine();
						paths.WriteLine();
						break;
					}
				}
			}

			file.Close();
			paths.Close();
			values.Close();
		}
	}
}