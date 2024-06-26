using Newtonsoft.Json;
using System.Diagnostics;

namespace ObjectToCSVSerializer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Мой csv-сериализатор");

            var test = new TestClass
            {
                A1 = 5,
                A2 = "My test string",
                A3 = 1.1,
                A4 = false,
                A5 = DateTime.Now
            };
            var iterations = 100000;

            string resultSerialize = "";
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < iterations; i++)
            {
                using (var stream = new FileStream("test.csv", FileMode.Create, FileAccess.Write))
                {
                    var cs = new CustomCsvSerializer<TestClass>();
                    resultSerialize = cs.Serialize(stream, test);
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Значение строки сериализации:\n" + resultSerialize);
            Console.WriteLine("Время выполнения (мс)- " + stopWatch.ElapsedMilliseconds);

            TestClass resultDeserialize = new();
            stopWatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                using (var stream = new FileStream("test.csv", FileMode.Open, FileAccess.Read))
                {
                    var cs = new CustomCsvSerializer<TestClass>();
                    resultDeserialize = cs.Deserialize(stream).FirstOrDefault() ?? new TestClass();
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Значение строки десериализации:\n" + resultDeserialize.ToString());
            Console.WriteLine("Время выполнения (мс)- " + stopWatch.ElapsedMilliseconds);

            Console.WriteLine("==============================================");

            Console.WriteLine("NewtonsoftJson");
            stopWatch.Restart();
            var serializer = new JsonSerializer();
            for (int i = 0; i < iterations; i++)
            {

                using (var sw = new StreamWriter("test.json"))
                using (var writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, test);
                    resultSerialize = writer.ToString();
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Значение строки сериализации:\n" + resultSerialize);
            Console.WriteLine("Время выполнения (мс)- " + stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                using (var sr = new StreamReader("test.json"))
                using (var read = new JsonTextReader(sr))
                {
                    resultDeserialize = serializer.Deserialize<TestClass>(read);
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Значение строки десериализации:\n" + resultDeserialize?.ToString());
            Console.WriteLine("Время выполнения (мс)- " + stopWatch.ElapsedMilliseconds);
        }
    }
}