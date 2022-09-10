using System.Diagnostics;

namespace AHundredGigaSort.Common.Logging;

public class ElapsedLogger : IDisposable
{
	private readonly Stopwatch _chrono = new();
	private readonly StreamWriter _writer;

	public ElapsedLogger(string path)
	{
		_writer = new StreamWriter(path);
	}

	public void Dispose()
	{
		_writer.Dispose();
	}

	public void Restart() => _chrono.Restart();

	public void WriteLog(string category, long period)
	{
		var ret = $"{category}\t{period}\t{_chrono.Elapsed.TotalMilliseconds}";
		_writer.WriteLine(ret);
	}
}