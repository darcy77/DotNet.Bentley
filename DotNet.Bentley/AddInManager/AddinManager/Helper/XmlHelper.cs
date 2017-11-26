using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace AddinManager.Helper
{
    static class XmlHelper
    {
        public static T Deserialize<T>(string xml)
        {
            try
            {
                using (var reader = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(typeof(T));
                    return (T)xmldes.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                return default(T);
            }
        }

        public static string Serializer<T>(T obj)
        {
            var result = string.Empty;

            using (var stream = new MemoryStream())
            {
                var xml = new XmlSerializer(typeof(T));

                xml.Serialize(stream, obj);
                stream.Position = 0;

                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}
