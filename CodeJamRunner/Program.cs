using System;
using System.IO;
using CodeJamRunner.Enums;

namespace CodeJamRunner
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string YEAR = "2017";
			RoundEnum ROUND = RoundEnum.QualificationRound;
			ProblemEnum PROBLEM = ProblemEnum.A;
		    FileTypeEnum FILE_TYPE = FileTypeEnum.Test;
			int ATTEMPT = 0;

		    Run(YEAR, ROUND, PROBLEM, FILE_TYPE, ATTEMPT);
		}

	    private static void Run(string year, RoundEnum round, ProblemEnum problem, FileTypeEnum fileType, int attempt)
	    {
            Prepare(year, round, problem);

            Type testCaseType = Type.GetType($"CodeJamRunner.CodeJam_{year}_{round}_{problem}");

	        try
	        {
	            var testCase = (ITestCase) Activator.CreateInstance(testCaseType);

                new Problem(testCase)
                {
                    FileType = fileType,
                    Attempt = attempt
                }.Run();
            }
	        catch
	        {
	            Console.WriteLine("Show All Files and add generated folder to the Solution");
	            Console.ReadLine();
	        }
        }

	    private static void Prepare(string year, RoundEnum round, ProblemEnum problem)
	    {
	        var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, year, round.ToString(), problem.ToString());
	        dir = dir.Replace("\\bin\\Debug", "");

	        Directory.CreateDirectory(dir);

	        var filePath = Path.Combine(dir, $"CodeJam_{year}_{round}_{problem}.cs");

            if (File.Exists(filePath))
                return;

            File.WriteAllText(filePath, @"using System;
using System.Collections.Generic;

namespace CodeJamRunner
{
	public class CodeJam_" + year + "_" + round + "_" + problem + @" : ITestCase
	{
		public string GetTitle()
		{
			return """ + problem + @""";
        }

        public Func<string, int> LinesPerTestCase()
        {
            return x => 1;
        }

        public string Run(List<string> lines)
        {
            return """ + problem + @""";
        }
    }
}");

            File.WriteAllText(Path.Combine(dir, "test.in"), @"3
1
2
3");

            File.WriteAllText(Path.Combine(dir, "test.out"), @"Case #1: A
Case #2: A
Case #3: A");
        }
	}
}