using System.Dynamic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CatoriCity2025WPF.Objects
{
    public class GenericSerializer
    {
//        Adding the Namespace


//Generic Json Serialization

    public static string Serialize<T>(T t)
    {
            string json = JsonConvert.SerializeObject(t);

            return json;
    }

    //Generic Json Deserialization

    public static T Deserialize<T>(string jsonString)
    {
        T obj    =   JsonConvert.DeserializeObject<T>(jsonString);
        return obj;
    }
}
}
