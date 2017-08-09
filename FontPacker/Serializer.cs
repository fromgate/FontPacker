using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace FontPacker
{
    internal static class Serializer
    {
        internal static void Serialize<T>(this T arg, string fileName)
        {
            string res = JsonConvert.SerializeObject(arg, Formatting.Indented);
            File.WriteAllText(fileName,res);
        }

        internal static T Deserialize<T>(string fileName)
        {
            string json = File.ReadAllText(fileName);
            T res = JsonConvert.DeserializeObject<T>(json);
            return res;
        }
        
        
        internal static void Serialize<T>(this T arg, Stream stream)
        {
            string res = JsonConvert.SerializeObject(arg, Formatting.Indented);
            byte[] byteArray = Encoding.UTF8.GetBytes(res);
            stream.Write(byteArray,0, byteArray.Length);
        }
        
    }

}