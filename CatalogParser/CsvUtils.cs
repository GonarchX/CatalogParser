using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static CatalogParser.ProductParser;

namespace CatalogParser
{
    internal class CsvUtils
    {
        public static void WriteProductInfoInFile(string filePath, List<ProductInfo> source, string sep)
        {
            if (source == null || source.Count == 0) return;

            //Adding column names to a file if it doesn't exist
            if (!File.Exists(filePath)) File.AppendAllText(filePath, 
                String.Join(sep ,source[0].GetType().GetFields().Select(x => x.Name)) + Environment.NewLine);

            StringBuilder sb = new(100);
            foreach (var record in source)
            {
                sb.Append(record.ToString(sep));
                sb.Append(Environment.NewLine);
            }
            File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
        }
        public static Dictionary<string, object> DictionaryFromType(object atype)
        {
            if (atype == null) return new Dictionary<string, object>();
            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties();
            Dictionary<string, object> dict = new();
            foreach (PropertyInfo prp in props)
            {
                object value = prp.GetValue(atype, Array.Empty<object>());
                dict.Add(prp.Name, value);
            }
            return dict;
        }
    }
}
