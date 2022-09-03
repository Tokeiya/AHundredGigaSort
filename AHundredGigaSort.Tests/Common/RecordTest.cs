using ChainingAssertion;
using static AHundredGigaSort.Common.Record;
using Record = AHundredGigaSort.Common.Record;

namespace AHundredGigaSort.Tests.Common;

public class RecordTest
{
	public static void Assert(Record actual, ReadOnlySpan<byte> expected)
	{
		var expectedId = 0;

		for (var i = 0; i < KeySize; i++)
		{
			expectedId *= 10;
			expectedId += expected[i] - 0x30;
		}

		var tmp = expected[ValueOffset..];
		for (var i = 0; i < ValueSize; i++)
		{
			actual.Value[i].Is(tmp[i]);
		}
	}

	public static void Assert(Record actual, Record expected)
	{
		actual.Id.Is(expected.Id);

		actual.Value.Length.Is(ValueSize);
		expected.Value.Length.Is(ValueSize);


		for (var i = 0; i < ValueSize; i++)
		{
			actual.Value[i].Is(expected.Value[i]);
		}
	}

	[Fact]
	public void ConstTest()
	{
		KeySize.Is(8);
		ValueSize.Is(3000);
		ValueOffset.Is(9);
		TotalRecordSize.Is(3010);
	}

	[Fact]
	public void InitTest()
	{
		using var stream = SampleReader.OpenRead();
		var buff = new byte[TotalRecordSize];

		stream.Read(buff, 0, TotalRecordSize).Is(TotalRecordSize);
		var actual = new Record(buff);

		var expected = SampleReader.Read(0);

		Assert(actual, expected);
	}

	[Fact]
	public void DefaultInitTest()
	{
		var actual = new Record();
		actual.Id.Is(0);
		actual.Value.Length.Is(ValueSize);


		for (int i = 0; i < actual.Value.Length; i++)
		{
			actual.Value[i].Is((byte)0);
		}
	}

	[Fact]
	public void ReloadTest()
	{
		using var stream = SampleReader.OpenRead();
		var buff = new byte[TotalRecordSize];

		stream.Read(buff, 0, TotalRecordSize).Is(TotalRecordSize);
		var record = new Record(buff);

		Assert(record, SampleReader.Read(0));

		stream.Read(buff, 0, TotalRecordSize).Is(TotalRecordSize);
		record.Reload(buff);

		Assert(record, SampleReader.Read(1));
	}
}