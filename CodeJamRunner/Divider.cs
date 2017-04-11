using System;
using System.Collections.Generic;
using System.IO;

namespace CodeJamRunner
{
	public class Divider
	{
	    public readonly string Path;

		public Divider (string path)
		{
			if (!File.Exists (path)) 
			{
				throw new Exception ("File not found");
			}

			Path = path;
		}

		public List<string> Divide(Func<string, int> func)
		{
		    int numberOfTestCases;
            var list = new List<string> ();

			using (StreamReader reader = new StreamReader(Path))
			{
				string firstLineOfFile = reader.ReadLine();

				if (string.IsNullOrEmpty (firstLineOfFile) || 
					firstLineOfFile.Equals (Environment.NewLine)) {
					throw new Exception ("Test file empty");
				}

				numberOfTestCases = int.Parse(firstLineOfFile);

				while (!reader.EndOfStream) 
				{
					string first = reader.ReadLine ();
					var subList = new List<string> { first };
					int nr = func (first) - 1;

					for (int i=0; i < nr; i++) {
						subList.Add(reader.ReadLine ());
					}

					list.Add (string.Join(Environment.NewLine, subList));
				}
			}

			if (numberOfTestCases != list.Count) 
			{
				throw new Exception ("Number of tests mismatch");
			}

			return list;
		}
	}
}

