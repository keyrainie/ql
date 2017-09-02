using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace ECommerce.Entity.Common
{
    /// <summary>
    /// 最基本的字典配置表，不允许随意修改
    /// </summary>
    [Serializable]
    public class ECConfig
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public int Mode { get; set; }
        public string Memo { get; set; }
        /// <summary>
        /// 以List的方式获取值
        /// </summary>
        /// <returns></returns>
        public List<ECConfig> GetValueList()
        {
            List<ECConfig> list = new List<ECConfig>();
            if (Mode == 1)
            {
                string cv = Value;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ECConfig>));
                using (Stream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(cv)))
                {
                    using (XmlReader xmlReader = XmlReader.Create(xmlStream))
                    {
                        Object obj = xmlSerializer.Deserialize(xmlReader);
                        list = (List<ECConfig>)obj;
                    }
                }
            }
            else
            {
                list.Add(new ECConfig() { Key = Key, Value = Value, Mode = Mode });
            }
            return list;
        }
    }
}
