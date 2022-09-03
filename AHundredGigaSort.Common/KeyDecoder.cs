namespace AHundredGigaSort.Common;

public static class KeyDecoder
{
	private const int Offset = 0x30;

	public static int Decode(ReadOnlySpan<byte> data)
	{
		var ret = 0;

		for (var i = 0; i < 8; i++)
		{
			ret *= 10;
			ret += data[i] - Offset;
		}

		return ret;
	}
}