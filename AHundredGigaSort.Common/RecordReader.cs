using System.Buffers;
using static AHundredGigaSort.Common.Record;

namespace AHundredGigaSort.Common;

public class RecordReader : IDisposable
{
	private readonly FileStream _stream;

	public RecordReader(string path)
	{
		_stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, TotalRecordSize);
		RecordSize = (int)(_stream.Length / TotalRecordSize);
	}

	public int RecordSize { get; }

	public int Position
	{
		set => _stream.Position = value * TotalRecordSize;
		get => (int)(_stream.Position / TotalRecordSize);
	}

	public void Dispose()
	{
		_stream.Dispose();
	}

	public bool Fill(Span<byte> buffer)
	{
		return _stream.Read(buffer) == TotalRecordSize;
	}

	public bool Fill(int position, Span<byte> buffer)
	{
		Position = position;
		return Fill(buffer);
	}

	public bool Fill(Record record)
	{
		var buff = ArrayPool<byte>.Shared.Rent(TotalRecordSize);

		try
		{
			if (_stream.Read(buff, 0, TotalRecordSize) != TotalRecordSize) return false;
			record.Reload(buff);

			return true;
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buff);
		}
	}

	public bool Fill(int position, Record record)
	{
		Position = position;
		return Fill(record);
	}


	public Record? Read()
	{
		var buff = ArrayPool<byte>.Shared.Rent(TotalRecordSize);

		try
		{
			if (_stream.Read(buff, 0, TotalRecordSize) != TotalRecordSize) return null;
			return new Record(buff);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buff);
		}
	}

	public Record? Read(int position)
	{
		Position = position;
		return Read();
	}

	public int? ReadId()
	{
		var buff = ArrayPool<byte>.Shared.Rent(KeySize);

		try
		{
			var next = _stream.Position + TotalRecordSize;

			if (_stream.Read(buff, 0, KeySize) != KeySize)
			{
				_stream.Position = next - TotalRecordSize;
				return null;
			}

			_stream.Position = next;
			return KeyDecoder.Decode(buff);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buff);
		}
	}

	public int? ReadId(int position)
	{
		Position = position;
		return ReadId();
	}
}