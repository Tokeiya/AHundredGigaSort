namespace AHundredGigaSort.Common;

public class RecordWriter : IDisposable
{
	private readonly FileStream _stream;


	public RecordWriter(string path)
	{
		_stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, Record.TotalRecordSize);
	}

	public int Position
	{
		get => (int)(_stream.Position / Record.TotalRecordSize);
		set => _stream.Position = value * Record.TotalRecordSize;
	}

	public void Write(Record record)
	{
		_stream.Write(record.Value);
	}

	public void Write(Record record, int position)
	{
		Position = position;
		Write(record);
	}

	public void Dispose()
	{
		_stream.Dispose();
	}
}