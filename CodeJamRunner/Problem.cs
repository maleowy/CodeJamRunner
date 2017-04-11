using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeJamRunner.Enums;

namespace CodeJamRunner
{
	public class Problem
	{
	    public static readonly Dictionary<FileTypeEnum, string> FileTypes = new Dictionary<FileTypeEnum, string> {
			{ FileTypeEnum.Small, "{0}-small-attempt{1}.in" },
            { FileTypeEnum.Small1, "{0}-small-1-attempt{1}.in" },
            { FileTypeEnum.Small2, "{0}-small-2-attempt{1}.in" },
            { FileTypeEnum.Large, "{0}-large.in" },
			{ FileTypeEnum.SmallPractice, "{0}-small-practice.in" },
            { FileTypeEnum.SmallPractice1, "{0}-small-practice-1.in" },
            { FileTypeEnum.SmallPractice2, "{0}-small-practice-2.in" },
            { FileTypeEnum.LargePractice, "{0}-large-practice.in" },
			{ FileTypeEnum.Test, "test.in" }
		};

		public FileTypeEnum FileType { get; set; }
		public int Attempt { get; set; }
        private readonly ITestCase _testCase;
        private readonly string[] _newLines = { Environment.NewLine };

        public Problem(ITestCase testCase)
		{
			_testCase = testCase;
		}

		public void Run()
        {
            Console.WriteLine(_testCase.GetTitle());

            DateTime started = DateTime.Now;
            Console.WriteLine("Started: " + started.ToString("hh:mm:ss"));

            Process();

            DateTime ended = DateTime.Now;
            Console.WriteLine("Ended: " + ended.ToString("hh:mm:ss"));

            TimeSpan total = ended.Subtract(started);

            Console.WriteLine("Total: {0} minutes, {1} seconds, {2} milliseconds", total.Minutes, total.Seconds, total.Milliseconds);

            Console.ReadLine();
        }

        private void Process()
        {
            string codeFile = GetCodeFileName();
            string testFile = GetTestFileName();

            string fn = Path.Combine(GetProblemDirectory(), testFile);

            var divider = new Divider(fn);

            List<string> testCases = divider.Divide(_testCase.LinesPerTestCase());

            var ms = new MemoryStream();

            ProcessReal(fn, testCases, ms);
            ProcessTest(fn, ms);
            CopyToDesktop(codeFile, testFile);
        }

        private void CopyToDesktop(string codeFile, string testFile)
        {
            var desktop = GetDesktopDictionary();
            var problemDir = GetProblemDirectory();

            if (!Directory.Exists(desktop))
            {
                Directory.CreateDirectory(desktop);
            }
            else
            {
                var di = new DirectoryInfo(desktop);

                foreach (FileInfo file in di.GetFiles("*.cs"))
                {
                    file.Delete();
                }

                foreach (FileInfo file in di.GetFiles("*.out"))
                {
                    file.Delete();
                }
            }

            File.Copy(Path.Combine(problemDir, codeFile), Path.Combine(desktop, codeFile), true);
            testFile = testFile.Replace(".in", ".out");
            File.Copy(Path.Combine(problemDir, testFile), Path.Combine(desktop, testFile), true);
        }

        private void ProcessReal(string fn, List<string> testCases, MemoryStream ms)
        {
            using (StreamWriter writer = new StreamWriter(FileType == FileTypeEnum.Test ? (Stream)ms : (Stream)new FileStream(fn.Replace(".in", ".out"), FileMode.Create)))
            {
                for (int i = 0; i < testCases.Count; i++)
                {
                    DateTime now = DateTime.Now;

                    List<string> lines = testCases[i].Split(_newLines, StringSplitOptions.None).ToList();
                    string answer = $"Case #{i + 1}: {_testCase.Run(lines)}";
                    writer.WriteLine(answer);

                    var timeSpan = DateTime.Now.Subtract(now);
                    Console.WriteLine("{0} ({1} min, {2} s, {3} ms)", answer, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                }
            }
        }

        private void ProcessTest(string fn, MemoryStream ms)
        {
            if (FileType != FileTypeEnum.Test)
                return;

            if (!File.Exists(fn.Replace(".in", ".out")))
            {
                throw new Exception("Output test file does not exists");
            }

            List<string> outMemoryList = Regex.Split(Encoding.Default.GetString(ms.ToArray()), @"Case ").Skip(1).ToList();
            List<string> outFileList = Regex.Split(File.ReadAllText(fn.Replace(".in", ".out")), @"Case ").Skip(1).ToList();

            if (!outFileList[outFileList.Count - 1].EndsWith(Environment.NewLine))
                outFileList[outFileList.Count - 1] += Environment.NewLine;

            bool correct = true;

            Console.WriteLine("\n-----------------------\n");

            for (int i = 0; i < outFileList.Count(); i++)
            {
                if (outMemoryList[i].Equals(outFileList[i]) == false)
                {
                    Console.WriteLine("#{0} -wrong-> {1} -correct-> {2}", i + 1, outMemoryList[i].Substring(outMemoryList[i].IndexOf(':') + 1), outFileList[i].Substring(outFileList[i].IndexOf(':') + 1));
                    correct = false;
                }
            }

            Console.WriteLine(correct ? "Test succeded!" : "Test failed!");
        }

        private string GetDesktopDictionary()
	    {
	        var dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var name = _testCase.GetType().Name;

            return Path.Combine(dir, name[name.Length - 1].ToString());
	    }

        private string GetProblemDirectory()
        {
            var fn = _testCase.GetType().Name.Replace("CodeJam_", "").Replace("_", "/");
            var dir = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\", "/").Replace(@"/bin/Debug", "");

            fn = string.Format(dir + fn);
            return fn;
        }

        private string GetCodeFileName()
        {
            var name = _testCase.GetType().Name;
            return name + ".cs";
        }

        private string GetTestFileName()
	    {
	        var name = _testCase.GetType().Name;
            return string.Format(FileTypes[FileType], name[name.Length - 1], Attempt);
	    }
    }
}

