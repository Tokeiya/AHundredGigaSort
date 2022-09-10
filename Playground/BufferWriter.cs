using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AHundredGigaSort.Common;

namespace Playground
{
	internal class BufferWriter:IDisposable
	{
		public const int BufferLength = 700_000;
		public const int BufferSize = BufferLength * Record.TotalRecordSize;

		private readonly byte[] _buffer = new byte[BufferSize];

		private int _length=0;
		private readonly FileStream _stream;

		public BufferWriter(string outputPath)
		{
			_stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 3010);
			_stream.Position = 0;
		}

		private void Flush()
		{
			_stream.Write(_buffer,0,_length*Record.TotalRecordSize);
			_stream.Flush();
		}

		public void Write(ReadOnlySpan<byte> datum)
		{
			if (_length == BufferLength)
			{
				Flush();
				_length = 0;
			}

			var offset = _length * Record.TotalRecordSize;

			for (int i = 0; i < datum.Length; i++)
			{
				_buffer[i + offset] = datum[i];
			}

			++_length;
		}



		public void Dispose()
		{
			Flush();
			_stream.Dispose();
		}
	}
}
