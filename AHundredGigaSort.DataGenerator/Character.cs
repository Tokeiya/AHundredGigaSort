using System.Text;

namespace AHundredGigaSort.DataGenerator;

public readonly struct Character
{
	public byte First { get; }
	public byte Second { get; }
	public byte Third { get; }

	public Character(char value)
	{
		var r = new Rune(value);
		Span<byte> buff = stackalloc byte[4];

		var len = r.EncodeToUtf8(buff);

		if (len != 3) throw new ArgumentException($"{value} is unexpected character.");

		First = buff[0];
		Second = buff[1];
		Third = buff[2];
	}


	public void Write(BinaryWriter writer)
	{
		Span<byte> buff = stackalloc byte[3];
		buff[0] = First;
		buff[1] = Second;
		buff[2] = Third;

		writer.Write(buff);
	}
}