using System.Diagnostics;
using System.Text;
using XorShiftAddSharp;

namespace AHundredGigaSort.DataGenerator;


public class Generator
{
	private const uint KeyMask = 0x3FFF_FFFF;
	private const uint ValueMask = 0xFF;
	private const int CountMask = 0x000f_ffff;

	private const byte NewLine = 10;
	private const byte Comma = 44;

	private static readonly Character[] Characters;
	private static readonly Encoding Enc = new UTF8Encoding();

	private readonly HashSet<uint> _checker = new(100_000_000);
	private readonly XorShiftAdd _rnd;


	static Generator()
	{
		var list = new List<Character>();

		for (var i = 0; i < 10; i++)
		{
			var c = (char)(0xFF10 + i);
			list.Add(new Character(c));
		}

		for (var i = 0; i < 26; i++)
		{
			var c = (char)(0xFF21 + i);
			list.Add(new Character(c));

			c = (char)(0xFF41 + i);
			list.Add(new Character(c));
		}

		for (var i = 0; i < 83; i++)
		{
			var c = (char)(0x3041 + i);
			list.Add(new Character(c));
		}

		for (var i = 0; i < 86; i++)
		{
			var c = (char)(0x30A1 + i);
			list.Add(new Character(c));
		}

		for (var i = 0; i < 25; i++)
		{
			var c = (char)(0x2460 + i);
			list.Add(new Character(c));
		}

		Characters = list.ToArray();
		Span<byte> buff = stackalloc byte[1];
		var numerics = new (int, byte)[10];

		for (var i = 0; i < 10; i++)
		{
			var r = new Rune((char)(0x30 + i));
			r.EncodeToUtf8(buff);

			numerics[i] = (i, buff[0]);
		}
	}

	public Generator(uint seed)
	{
		_rnd = new XorShiftAdd(seed);
	}

	private void WriteKey(BinaryWriter writer)
	{
		uint key = 0;

		try
		{
			while (true)
			{
				key = _rnd.NextUnsignedInt() & KeyMask;
				if (key >= 100_000_000) continue;
				if (_checker.Add(key)) break;
			}

			Span<byte> buff = stackalloc byte[8];

			Enc.GetBytes($"{key:D8}", buff);
			writer.Write(buff);
			writer.Write(Comma);
		}
		catch (ArgumentException ex)
		{
			Console.WriteLine(ex);
			Console.WriteLine(key);
			Console.WriteLine("Press enter to exit");
			Console.ReadLine();
		}
	}

	private void WriteValue(BinaryWriter writer)
	{
		Span<byte> buff = stackalloc byte[3_001];

		for (var i = 0; i < 1000; i++)
		{
			var c = Characters[_rnd.NextUnsignedInt() & ValueMask];
			var offset = i * 3;
			buff[offset] = c.First;
			buff[offset + 1] = c.Second;
			buff[offset + 2] = c.Third;
		}

		buff[3000] = NewLine;

		writer.Write(buff);
	}

	public void WriteData(int size, BinaryWriter writer)
	{
		var chrono = new Stopwatch();
		chrono.Restart();

		for (var i = 0; i < size; i++)
		{
			WriteKey(writer);
			WriteValue(writer);
			if ((i & CountMask) == 0)
			{
				Console.WriteLine($"{chrono.Elapsed} {i} ({i / (double)size * 100:F2}%)");
			}
		}

		Console.WriteLine($"Took {chrono.Elapsed} to write out {size} records. ");
	}
}