using AHundredGigaSort.Common;
using ChainingAssertion;
using Xunit.Abstractions;
using Record = AHundredGigaSort.Common.Record;

namespace AHundredGigaSort.Tests.Common;

//This test depend on RecordReader
public class RecordWriterTest : IDisposable
{
	private readonly ITestOutputHelper _testOutputHelper;

	public RecordWriterTest(ITestOutputHelper testOutputHelper)
	{
		_testOutputHelper = testOutputHelper;
	}

	public void Dispose()
	{
		foreach (var info in Directory.EnumerateFiles("./Output").Select(x => new FileInfo(x)))
		{
			if (info.Extension == ".dummy") continue;
			info.Delete();
		}
	}

	private static string GetPath()
	{
		return $".\\Output\\{DateTime.Now.Ticks}.txt";
	}

	[Fact]
	public void InitTest()
	{
		var path = GetPath();

		{
			using var target = new RecordWriter(path);
		}

		File.Exists(path).IsTrue();
	}

	[Fact]
	public void PositionTest()
	{
		var path = GetPath();
		var empty = new Record();

		// ReSharper disable once ConvertToUsingDeclaration
		using (var writer = new RecordWriter(path))
		{
			writer.Position.Is(0);
			writer.Write(SampleReader.Read(0));
			writer.Position.Is(1);

			writer.Write(SampleReader.Read(1), 42);
			var pos = writer.Position;

			pos.Is(43);
		}

		using var reader = new RecordReader(path);
		reader.RecordSize.Is(43);

		RecordTest.Assert(reader.Read(0)!, SampleReader.Read(0));

		for (var i = 1; i < 42; i++)
		{
			RecordTest.AreEmpty(reader.Read(i)!);
		}

		RecordTest.Assert(reader.Read(42)!, SampleReader.Read(1));
	}

	[Fact]
	public void SequentialWriteTest()
	{
		var path = GetPath();

		// ReSharper disable once ConvertToUsingDeclaration
		using (var writer = new RecordWriter(path))
		{
			for (var i = 0; i < 20; i++)
			{
				writer.Write(SampleReader.Read(i));
			}

			var pos = writer.Position;
			pos.Is(20);
		}

		using var reader = new RecordReader(path);
		reader.RecordSize.Is(20);

		for (var i = 0; i < 20; i++)
		{
			RecordTest.Assert(reader.Read()!, SampleReader.Read(i));
		}
	}


	[Fact]
	public void RandomWriteTest()
	{
		var path = GetPath();

		using (var writer = new RecordWriter(path))
		{
			for (var i = 0; i < 20; i++)
			{
				writer.Write(SampleReader.Read(i), i * 2);
			}

			var pos = writer.Position;
			pos.Is(39);
		}

		using var reader = new RecordReader(path);
		reader.RecordSize.Is(39);

		for (var i = 0; i < 39; i++)
		{
			if ((i & 1) == 0) RecordTest.Assert(reader.Read(i)!, SampleReader.Read(i / 2));
			else RecordTest.AreEmpty(reader.Read(i)!);
		}

		reader.Read().IsNull();
	}

	[Fact]
	public void DisposeTest()
	{
		var path = GetPath();
		var writer = new RecordWriter(path);

		writer.Dispose();

		Assert.Throws<ObjectDisposedException>(() => writer.Write(SampleReader.Read(1)));
	}
}