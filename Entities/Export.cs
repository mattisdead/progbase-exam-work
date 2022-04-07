using System.Xml.Serialization;
using ServiceLib;

namespace Entities
{
    public class Export
    {
        public static void Serialize(Post[] posts, string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Post[]));
            System.IO.File.WriteAllText(filePath, "");
            System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath);
            ser.Serialize(writer, posts);

            writer.Close();
        }
    }
}