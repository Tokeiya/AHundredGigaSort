using AHundredGigaSort.Common;
using ChainingAssertion;
using static AHundredGigaSort.Common.Record;

namespace AHundredGigaSort.Tests.Common;

public class KeyDecoderTest
{
	[Fact]
	public void KeyDecodeTest()
	{
		using var stream = SampleReader.OpenRead();
		var buff = new byte[TotalRecordSize];
		stream.Read(buff);

		KeyDecoder.Decode(buff).Is(73644369);

		stream.Read(buff);
		KeyDecoder.Decode(buff).Is(91457911);
	}
}