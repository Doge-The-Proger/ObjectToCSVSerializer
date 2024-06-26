using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace ObjectToCSVSerializer
{
    public class CustomCsvSerializer<T> where T : class, new()
    {
        /// <summary>
        /// Тип класса, который подвергается сериализации
        /// </summary>
        private Type _type;

        /// <summary>
        /// Набор св-в сериализуемого класса
        /// </summary>
        private List<PropertyInfo> _properties;

        /// <summary>
        /// Символ для разделения
        /// </summary>
        private char _separator;

        /// <summary>
        /// ???
        /// </summary>
        private string _replacement { get; set; }

        public CustomCsvSerializer() : this(';')
        {

        }

        public CustomCsvSerializer(char separator)
        {
            _type = typeof(T);

            _properties = _type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty).ToList();

            _separator = separator;
        }

        /// <summary>
        /// Сериализация списка объектов
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="objList"></param>
        public string Serialize(Stream stream, IList<T> objList)
        {
            var sb = new StringBuilder();
            var csvRow = new List<string>();


            sb.AppendLine(GetHeader());

            foreach (var obj in objList)
            {
                csvRow.Clear();

                foreach (var p in _properties)
                {
                    var objValue = p.GetValue(obj);
                    var csvValue = objValue == null ?
                                "" :
                                objValue.ToString().Replace(_separator.ToString(), _replacement);
                    csvRow.Add(csvValue);
                }
                sb.AppendLine(string.Join(_separator.ToString(), csvRow.ToArray()));
            }

            var serializedStr = sb.ToString().Trim();

            using (var sw = new StreamWriter(stream))
            {
                sw.Write(serializedStr);
            }

            return serializedStr;
        }

        /// <summary>
        /// Сериализация одиночного объекта
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="obj"></param>
        public string Serialize(Stream stream, T obj)
        {
            var objList = new List<T> { obj };
            return Serialize(stream, objList);
        }

        /// <summary>
        /// Преобразует поток данных в массив объектов типа T
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public IList<T> Deserialize(Stream stream)
        {
            string[] columns;
            string[] rows;

            try
            {
                using (var sr = new StreamReader(stream))
                {
                    columns = sr.ReadLine().Split(_separator);
                    rows = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(
                        "Файл не является .CSV или содержит ошибки", ex);
            }

            var data = new List<T>();
            for (int row = 0; row < rows.Length; row++)
            {
                var line = rows[row];
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split(_separator);

                var datum = new T();
                for (int i = 0; i < parts.Length; i++)
                {
                    var value = parts[i];
                    var column = columns[i];

                    //value = value.Replace(_replacement, _separator.ToString());

                    var p = _properties.First(a => a.Name == column);

                    var converter = TypeDescriptor.GetConverter(p.PropertyType);
                    var convertedvalue = converter.ConvertFrom(value);

                    p.SetValue(datum, convertedvalue);
                }
                data.Add(datum);
            }
            return data;
        }

        private string GetHeader()
        {
            var columns = _properties.Select(a => a.Name).ToArray();
            var header = string.Join(_separator.ToString(), columns);
            return header;
        }
    }
}