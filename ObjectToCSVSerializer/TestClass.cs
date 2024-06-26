using System.Text;

namespace ObjectToCSVSerializer
{
    public class TestClass
    {
        public int A1 { get; set; }

        public string? A2 { get; set; }

        public double A3 { get; set; }

        public bool A4 { get; set; }

        public DateTime A5 { get; set; }

        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append("A1 = ").Append(A1);
            str.Append("\tA2 = ").Append(A2);
            str.Append("\tA3 = ").Append(A3);
            str.Append("\tA4 = ").Append(A4);
            str.Append("\tA5 = ").Append(A5);
            return str.ToString();
        }
    }
}