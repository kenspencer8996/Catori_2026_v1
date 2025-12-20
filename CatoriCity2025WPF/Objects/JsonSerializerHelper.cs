using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace CatoriCity2025WPF.Objects
{
    internal class JsonSerializerHelper
    {
        internal static string Serialize<T>(T obj)
        {
            string result = string.Empty;
            result = JsonSerializer.Serialize(obj);

            return result;
        }
        internal static T DeSerialize<T>(string objString)
        {
            T result;
            result = JsonSerializer.Deserialize<T>(objString);

            return result;
        }
    }
}
