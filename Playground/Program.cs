// See https://aka.ms/new-console-template for more information


using AHundredGigaSort.Common;
using AHundredGigaSort.Common.Logging;

namespace Playground;


static class MainEntry
{
	private const uint Mask = 65535;
	private const uint BuffSize = 1_500_000;


	static void Write(FileStream stream, ReadOnlySpan<byte[]> buffer)
	{
		for (int i = 0; i < buffer.Length; i++)
		{
			stream.Write(buffer[i]);
		}
	}


	public static void Main(string[] args)
	{
		string sourcePath = args.Length == 2 ? args[0] : "D:\\Dummy\\Dummy.txt";
		string outputPath=args.Length == 2 ? args[1] : "H:\\sorted.txt";

		using var reader=new RecordReader(sourcePath);

		using var logWriter = new ElapsedLogger($"{System.Net.Dns.GetHostName()}_{DateTime.Now:yyMMdd_HHmm}_log.txt");

		var array=new (int,int)[reader.RecordSize];

		using var writer = new BufferWriter(outputPath);


		var rec = new Record();
		logWriter.Restart();

		for (int i = 0; i < array.Length; i++)
		{
			if ((i & Mask) == 0)
			{
				Console.WriteLine($"{i:N0}/36,000,000 {i/36000000.0*100:F2}%");
				logWriter.WriteLog("insert",i);
			}
			reader.Fill(rec);
			array[i] = (rec.Id, i);
		}

		logWriter.WriteLog("insert",36000000);
		Console.WriteLine("Insert completed");

		Array.Sort(array, (x, y) => x.Item1 - y.Item1);
		logWriter.WriteLog("sort", 1);

		var buff = new byte[Record.TotalRecordSize];

		for (int i = 0; i < array.Length; i++)
		{
			if ((i & Mask) == 0)
			{
				Console.WriteLine($"{i:N0}/36,000,000 {i / 36000000.0 * 100:F2}%");
				logWriter.WriteLog("write", i);
			}


			reader.Fill(array[i].Item2, buff);
			writer.Write(buff);
		}
	}
}






