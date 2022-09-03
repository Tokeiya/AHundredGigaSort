using System.Diagnostics;
using AHundredGigaSort.Common;
using Npgsql;

namespace Playground;

internal class BulkInsert
{
	public static void Insert()
	{
		const int recordSize = 36_000_000;
		const double realSize = recordSize;
		const int mask = 0xFFFF;

		var bld = new NpgsqlConnectionStringBuilder();
		bld.Host = "192.168.100.8";
		bld.Username = "tokeiya3";
		bld.Database = "postgres";

		using var con = new NpgsqlConnection(bld.ToString());

		con.Open();

		using var writer = con.BeginBinaryImport("COPY sorted_data(id,value) FROM STDIN(FORMAT BINARY);");

		using var str = new FileStream("D:\\dummy.txt", FileMode.Open);
		var buff = new byte[3010];


		var chrono = new Stopwatch();

		chrono.Start();

		var ret = new Record(buff);


		for (var i = 0; i < recordSize; i++)
		{
			if ((i & mask) == 0)
			{
				Console.WriteLine(
					$"{i:N0}/{recordSize:N0}\t{i / realSize * 100:N2}%\t{chrono.Elapsed}\t{chrono.ElapsedMilliseconds}");
			}

			str.Read(buff);
			ret.Reload(buff);

			writer.StartRow();
			writer.Write(ret.Id);
			writer.Write(ret.Value);
		}

		Console.WriteLine(
			$"{recordSize:N0}/{recordSize:N0}\t{recordSize / realSize * 100:N2}%\t{chrono.Elapsed}\t{chrono.ElapsedMilliseconds}");


		writer.Complete();

		Console.WriteLine("WriteCompleted");

		chrono.Stop();
		Console.WriteLine(chrono.Elapsed);
		Console.ReadLine();
	}
}