using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using AHundredGigaSort.Common;

namespace Benchmark;

public static class Share
{
	public const int Size = 3_600_000;
}

static class MainEntry
{
	private const string DataPath = @"/data/ssd250/dummy.txt";

	private static readonly IndexResolver Resolver = new(@"/home/tokeiya3/sorted.dat", @"/home/tokeiya3/two_read.tsv", 16383);
	
	static void TaskProc()
	{
		using var stream = new RecordReader(DataPath);
		Console.WriteLine("Task stream was opened.");
		var accum = 0;
		var rec = new Record();

		for (;;)
		{
			if (Resolver.TryRead(out var idx, out _))
			{
				stream.Fill(idx, rec);
				Thread.Sleep(10);
				accum += rec.Id;
			}
			else
			{
				break;
			}
		}

		Console.WriteLine($"Task:{accum}");
	}
	
	public static void Main(string[] args)
	{
		const int mask = 0xffff;
		
		using var stream=new FileStream("/data/ssd250/dummy.txt", FileMode.Open, FileAccess.Read, FileShare.Read, 30100, FileOptions.SequentialScan);
		Span<byte> buff = stackalloc byte[3010];

		var accum = 0;
		for (int i = 0; i < 5_000_000; i++)
		{
			if((i&mask)==0) Console.WriteLine(i);
			stream.Read(buff);
			accum += buff[0];
		}
		
		Console.WriteLine($"Main:{accum}");
	}



	private static void GenTable(string path, string outputPath)
	{
		Console.WriteLine(path);
		Console.WriteLine(outputPath);
		Console.WriteLine("Sure?");
		Console.ReadLine();


		var rec = new Record();

		using var rdr = new RecordReader(path);
		using var str = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
		using var wtr = new BinaryWriter(str);

		var array = new (int, int)[36_000_000];

		for (var i = 0; i < array.Length; i++)
		{
			if ((i & 65535) == 0) Console.WriteLine($"{i}/{array.Length}:{(double)i / array.Length * 100:f2}%");

			rdr.Fill(rec);
			array[i] = (rec.Id, i);
		}

		Array.Sort(array, (l, r) => l.Item1 - r.Item1);


		Console.WriteLine("Read and sort completed.");
		var idx = 0;

		foreach (var (id, pos) in array)
		{
			if ((++idx & 65535) == 0) Console.WriteLine($"{idx}/{array.Length}:{(double)idx / array.Length * 100:f2}%");

			wtr.Write(id);
			wtr.Write(pos);
		}
	}
}