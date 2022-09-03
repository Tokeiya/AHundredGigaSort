// See https://aka.ms/new-console-template for more information

using System.Text;
using AHundredGigaSort.DataGenerator;

var gen = new Generator(42);


Console.WriteLine("How many records?");
var size = int.Parse(Console.ReadLine());
using var writer = new BinaryWriter(new FileStream($"D:\\{size}_sample.txt", FileMode.Create), Encoding.UTF8);


Console.WriteLine($"Write {size} records.");
gen.WriteData(size, writer);