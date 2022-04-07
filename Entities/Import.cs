using System.Xml.Serialization;
using System.IO;
using ServiceLib;

namespace Entities
{
    public class Import
    {
        public static Post[] Deserialize(string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Post[]));
            StreamReader reader = new StreamReader(filePath);
            Post[] value = (Post[])ser.Deserialize(reader);
            reader.Close();
            return value;
        }
    }
}