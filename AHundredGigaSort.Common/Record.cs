namespace AHundredGigaSort.Common;

public class Record
{
	public const int KeySize = 8;
	public const int ValueSize = 3000;
	public const int ValueOffset = 9;
	public const int TotalRecordSize = 3010;


	public Record()
	{
		Id=0;
		Value = new byte[ValueSize];
	}

	public Record(ReadOnlySpan<byte> data)
	{
		Id = KeyDecoder.Decode(data[..KeySize]);

		//Due to performance issue.(local array can skip range check)
		var array = new byte[ValueSize];

		var tmp = data[ValueOffset..];

		for (var i = 0; i < ValueSize; i++)
		{
			array[i] = tmp[i];
		}

		Value = array;
	}


	public int Id { get; private set; }
	public byte[] Value { get; }

	public void Reload(ReadOnlySpan<byte> data)
	{
		Id = KeyDecoder.Decode(data[..KeySize]);

		for (var i = 0; i < ValueSize; i++)
		{
			Value[i] = data[i + ValueOffset];
		}
	}
}