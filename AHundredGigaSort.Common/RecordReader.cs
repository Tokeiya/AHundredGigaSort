using System.Buffers;

namespace AHundredGigaSort.Common;

public class RecordReader : IDisposable
{
	private readonly FileStream _stream;

	public RecordReader(string path)
	{
		_stream = new FileStream(path, FileMode.Open);
		RecordSize = (int)(_stream.Length / Record.TotalRecordSize);
	}

	public int RecordSize { get; }

	public int Position
	{
		set
		{
			//TODO:Implement
#warning NotImplemented
			throw new NotImplementedException();
		}
		get
		{
			//TODO:Implement
#warning NotImplemented
			throw new NotImplementedException();
		}
	}

	public void Dispose()
	{
		//TODO:Implement
#warning NotImplemented
		throw new NotImplementedException();
	}

	public bool Fill(Span<byte> buffer)
	{
		return _stream.Read(buffer) == Record.TotalRecordSize;
	}

	public bool Fill(int position, Span<byte> buffer)
	{
		//TODO:Implement
#warning NotImplemented
		throw new NotImplementedException();
	}

	public bool Fill(Record record)
	{
		var buff = ArrayPool<byte>.Shared.Rent(Record.TotalRecordSize);

		try
		{
			if (_stream.Read(buff, 0, Record.TotalRecordSize) != Record.TotalRecordSize) return false;
			record.Reload(buff);

			return false;
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buff);
		}
	}

	public bool Fill(int position, Record record)
	{
		//TODO:Implement
#warning NotImplemented
		throw new NotImplementedException();
	}


	public Record Read()
	{
		var buff = ArrayPool<byte>.Shared.Rent(Record.TotalRecordSize);

		try
		{
			//TODO:Implement
#warning NotImplemented
			throw new NotImplementedException();
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buff);
		}
	}

	public Record Read(int position)
	{
		//TODO:Implement
#warning NotImplemented
		throw new NotImplementedException();
	}

	public int ReadId()
	{
		//TODO:Implement
#warning NotImplemented
		throw new NotImplementedException();
	}

	public int ReadId(int position)
	{
		//TODO:Implement
#warning NotImplemented
		throw new NotImplementedException();
	}
}