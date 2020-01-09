namespace Utility
{
	public static class XmlHelper
	{
		/// <summary>
        /// 普通序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public static string Serialize<T>(T obj)
		{
			string xml;

            try
            {
                XmlWriterSettings setting = new XmlWriterSettings()
                {
                    Encoding = Encoding.UTF8
                };

                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlWriter writer = XmlWriter.Create(stream, setting))
                    {
                        XmlSerializerNamespaces serializerNS = new XmlSerializerNamespaces();
                        serializerNS.Add("", "");

                        XmlSerializer serializer = new XmlSerializer(obj.GetType());
                        serializer.Serialize(writer, obj, serializerNS);
                    }

                    xml = Encoding.UTF8.GetString(stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                xml = null;
            }
			
			return xml;
		}
		
		/// <summary>
        /// 兼容0x0016进制错误 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public static string SerializeObj<T>(T obj)
		{
			string xml;

            try
            {
                XmlSerializer ser = new XmlSerializer(obj.GetType());

				// StringWriter默认 utf-16
				// 需要utf-8或者其他编码则override Encoding
                using (StringWriter writer = new StringWriterWithEncoding())
                {
                    XmlSerializerNamespaces serializerNS = new XmlSerializerNamespaces();
                    serializerNS.Add("", "");

                    ser.Serialize(writer, obj, serializerNS);
                    xml = writer.ToString();
                }
            }
            catch (Exception ex)
            {
				xml = null;
            }
			
			return xml;
		}
	}
	
	
    public sealed class StringWriterWithEncoding : StringWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}