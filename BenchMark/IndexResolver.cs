using System.Diagnostics;

namespace Benchmark;

public class IndexResolver
{
	private readonly Stopwatch _chrono = new();
	private readonly int _mask;
	private readonly (int idx, int pos)[] _table;
	private readonly StreamWriter _writer;
	private int _index = -1;

	public IndexResolver(string path, string logPath, int mask)
	{
		using var str = new FileStream(path, FileMode.Open, FileAccess.Read);
		using var rdr = new BinaryReader(str);

		_table = new (int, int)[Share.Size];

		for (var i = 0; i < _table.Length; i++) _table[i] = (rdr.ReadInt32(), rdr.ReadInt32());

		_mask = mask;
		_writer = new StreamWriter(logPath);
	}

	public void RestartChronograph()
	{
		_chrono.Restart();
	}

	public bool TryRead(out int position, out int idx)
	{
		var i = Interlocked.Increment(ref _index);
		if (i >= _table.Length)
		{
			_writer.Dispose();
			idx = i;
			position = -1;
			return false;
		}

		if ((i & _mask) == 0)
		{
			Console.WriteLine(
				$"{i}/{_table.Length} {(double)i / _table.Length * 100:f2}% {_chrono.Elapsed} {_chrono.ElapsedMilliseconds / (i + 1.0):f2}(ms)/perRecord");
			_writer.WriteLine($"{i}\t{_chrono.ElapsedMilliseconds}");
		}

		idx = i;
		position = _table[i].pos;
		return true;
	}
}