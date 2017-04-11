using System;
using System.Collections.Generic;

namespace CodeJamRunner
{
	public interface ITestCase
	{
		string GetTitle();
		string Run(List<string> lines);
		Func<string, int> LinesPerTestCase ();
	}
}