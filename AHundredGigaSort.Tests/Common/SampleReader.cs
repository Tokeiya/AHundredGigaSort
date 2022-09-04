using System.Buffers;
using ChainingAssertion;
using static AHundredGigaSort.Common.Record;
using Record = AHundredGigaSort.Common.Record;

namespace AHundredGigaSort.Tests.Common;

public static class SampleReader
{
	public const string SampleDataPath = "./SampleData/100_sample.txt";

	private static readonly MemoryStream Stream;

	static SampleReader()
	{
		using var fs = OpenRead();
		var buff = new byte[fs.Length];

		fs.Read(buff).Is((int)fs.Length);

		Stream = new MemoryStream(buff, false);
	}

	public static FileStream OpenRead()
	{
		return new FileStream(SampleDataPath, FileMode.Open, FileAccess.Read);
	}

	public static Record Read(int index)
	{
		var buff = ArrayPool<byte>.Shared.Rent(TotalRecordSize);
		try
		{
			var span = buff[..TotalRecordSize];
			Fill(index, span);
			return new Record(span);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buff);
		}
	}

	public static void Fill(int index, Span<byte> buffer)
	{
		Stream.Position = index * TotalRecordSize;
		Stream.Read(buffer);
	}
}