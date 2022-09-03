using AHundredGigaSort.Common;
using ChainingAssertion;
using static AHundredGigaSort.Common.Record;
using static AHundredGigaSort.Tests.Common.SampleReader;
using Record = AHundredGigaSort.Common.Record;

namespace AHundredGigaSort.Tests.Common;

public class RecordReaderTest
{
	private static void Assert(ReadOnlySpan<byte> actual, ReadOnlySpan<byte> expected)
	{
		actual.Length.Is(expected.Length);

		for (int i = 0; i < actual.Length; i++)
		{
			actual[i].Is(expected[i]);
		}
	}

	public RecordReaderTest()
	{
		Read(0);
	}

	[Fact]
	public void InitTest()
	{
		using var target = new RecordReader(SampleDataPath);
		target.RecordSize.Is(100);
		target.Position.Is(0);
	}

	[Fact]
	public void SequentialFillTest()
	{
		var actual = new byte[TotalRecordSize];
		var expected = new byte[TotalRecordSize];

		using var target = new RecordReader(SampleDataPath);


		for (int i = 0; i < 100; i++)
		{
			target.Fill(actual).IsTrue();
			Fill(i, expected);

			Assert(actual, expected);
		}

		target.Fill(actual).IsFalse();
	}

	[Fact]
	public void PositionFillTest()
	{
		var actual = new byte[TotalRecordSize];
		var expected = new byte[TotalRecordSize];

		using var target = new RecordReader(SampleDataPath);

		for (int i = 99; i >= 0; i--)
		{
			target.Fill(i, actual).IsTrue();
			Fill(i, expected);

			Assert(actual, expected);
		}
	}

	[Fact]
	public void SequentialRecordFillTest()
	{
		var record = new Record();

		using var target = new RecordReader(SampleDataPath);

		for (int i = 0; i < 100; i++)
		{
			var ret = target.Fill(record);

			ret.IsTrue();

			RecordTest.Assert(record, Read(i));
		}

		target.Fill(record).IsFalse();
	}


	[Fact]
	public void RandomRecordFillTest()
	{
		var record = new Record();

		using var target = new RecordReader(SampleDataPath);

		for (int i = 99; i >= 0; i--)
		{
			target.Fill(i, record);
			RecordTest.Assert(record, Read(i));
		}
	}


	[Fact]
	public void SequentialReadTest()
	{
		using var target = new RecordReader(SampleDataPath);

		for (int i = 0; i < 100; i++)
		{
			RecordTest.Assert(target.Read()!, Read(i));
		}

		target.Read().IsNull();
	}

	[Fact]
	public void RandomReadTest()
	{
		using var target = new RecordReader(SampleDataPath);

		for (int i = 99; i >= 0; i--)
		{
			RecordTest.Assert(target.Read(i)!, Read(i));
		}

		target.Read(100).IsNull();
	}

	[Fact]
	public void PositionGetTest()
	{
		using var target = new RecordReader(SampleDataPath);

		for (int i = 0; i < 100; i++)
		{
			target.Position.Is(i);
			target.Read().IsNotNull();
		}
	}

	[Fact]
	public void PositionSetTest()
	{
		using var target = new RecordReader(SampleDataPath);

		target.Position = 42;
		RecordTest.Assert(target.Read()!, Read(42));
	}

	[Fact]
	public void DisposeTest()
	{
		using var target = new RecordReader(SampleDataPath);
		target.Dispose();
	

		Xunit.Assert.Throws<ObjectDisposedException>(() => target.Read());
	}
	
	[Fact]
	public void SequentialReadIdTest()
	{
		using var target=new RecordReader(SampleDataPath);

		for (int i = 0; i < 100; i++)
		{
			target.ReadId().Is(Read(i).Id);
		}

		target.ReadId().IsNull();

	}

	[Fact]
	public void PositionReadIdTest()
	{
		using var target = new RecordReader(SampleDataPath);
		
		target.ReadId(42).Is(Read(42).Id);
		
		target.ReadId(100).IsNull();
		
	}





}