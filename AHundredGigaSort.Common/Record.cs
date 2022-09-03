namespace AHundredGigaSort.Common;

public class Record
{
	public const int KeySize = 8;
	public const int ValueSize = 3000;
	public const int ValueOffset = 9;
	public const int TotalRecordSize = 3010;


	public Record()
	{
		Id = 0;
		Value = new byte[TotalRecordSize];
	}

	public Record(ReadOnlySpan<byte> data)
	{
		Id = KeyDecoder.Decode(data[..KeySize]);

		//Due to performance issue.(local array can skip range check)
		var array = new byte[TotalRecordSize];

		for (var i = 0; i < array.Length; i++)
		{
			array[i] = data[i];
		}

		Value = array;
	}


	public int Id { get; private set; }
	public byte[] Value { get; }

	public void Reload(ReadOnlySpan<byte> data)
	{
		Id = KeyDecoder.Decode(data[..KeySize]);

		//Due to performance issue.(local array can skip range check)
		var array = Value;

		for (var i = 0; i <array.Length; i++)
		{
			array[i] = data[i];
		}
	}
}