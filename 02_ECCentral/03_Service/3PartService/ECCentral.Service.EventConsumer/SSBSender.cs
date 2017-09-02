using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.EventConsumer
{
    public static class SSBSender
    {
        private static string GetXmlRootNodeName(string msgBody)
        {
            if (msgBody != null)
            {
                int idxBegin = msgBody.LastIndexOf(">");
                int idxEnd = msgBody.LastIndexOf("</");
                if (idxBegin > -1 && idxEnd > -1)
                {
                    return msgBody.Substring(idxEnd + 2, idxBegin - idxEnd - 2);
                }
            }
            return string.Empty;
        }

        private static string RemoveXmlHeader(string xmlStr)
        {
            int idxEnd = xmlStr.IndexOf("?>");
            if (idxEnd > -1)
            {
                xmlStr = xmlStr.Substring(idxEnd + 2).Trim();
            }
            return xmlStr;
        }

        #region SendV1

        /// <summary>
        /// 使用最早版本的SSB版本来发送消息
        /// </summary>
        /// <typeparam name="T">待发送的消息数据类型，只能为引用类型</typeparam>
        /// <param name="toService">发送到的目标Service的标示</param>
        /// <param name="data">待发送的消息数据对象，其将被Xml序列化后发送</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        /// <param name="spName">负责发送的存储过程的名字</param>
        /// <param name="removeDataRootXmlNode">对于待发送的消息数据对象data，其序列化为Xml文档后，是否需要去掉其根节点，只取根节点下的所有子节点作为消息的主体</param>
        public static void SendV1<T>(string toService, T data, string databaseName, string spName, bool removeDataRootXmlNode = false) where T : class
        {
            SendV1<T>(new string[] { toService }, data, databaseName, spName, removeDataRootXmlNode);
        }

        /// <summary>
        /// 使用最早版本的SSB版本来发送消息
        /// </summary>
        /// <typeparam name="T">待发送的消息数据类型，只能为引用类型</typeparam>
        /// <param name="toServiceArray">发送到的多个或1个目标Service的标示数组</param>
        /// <param name="data">待发送的消息数据对象，其将被Xml序列化后发送</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        /// <param name="spName">负责发送的存储过程的名字</param>
        /// <param name="removeDataRootXmlNode">对于待发送的消息数据对象data，其序列化为Xml文档后，是否需要去掉其根节点，只取根节点下的所有子节点作为消息的主体</param>
        public static void SendV1<T>(string[] toServiceArray, T data, string databaseName, string spName, bool removeDataRootXmlNode = false) where T : class
        {
            string xmlData = data.ToXmlString();
            if (removeDataRootXmlNode)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlData);
                xmlData = doc.LastChild.InnerXml;
            }
            SendV1(toServiceArray, xmlData, databaseName, spName);
        }

        /// <summary>
        /// 使用最早版本的SSB版本来发送消息
        /// </summary>
        /// <param name="toService">发送到的目标Service的标示</param>
        /// <param name="xmlData">消息主体的Xml内容，是发送方和订阅方共同能够理解的数据契约</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        /// <param name="spName">负责发送的存储过程的名字</param>
        public static void SendV1(string toService, string xmlData, string databaseName, string spName)
        {
            SendV1(new string[] { toService }, xmlData, databaseName, spName);
        }

        /// <summary>
        /// 使用最早版本的SSB版本来发送消息
        /// </summary>
        /// <param name="toServiceArray">发送到的多个或1个目标Service的标示数组</param>
        /// <param name="xmlData">消息主体的Xml内容，是发送方和订阅方共同能够理解的数据契约</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        /// <param name="spName">负责发送的存储过程的名字</param>
        public static void SendV1(string[] toServiceArray, string xmlData, string databaseName, string spName)
        {
            string msgFormat = "<ReMsg>{0}<MessageType>{1}</MessageType><Node>{2}</Node></ReMsg>";
            xmlData = RemoveXmlHeader(xmlData);

            string toServiceList = string.Empty;
            toServiceArray.ForEach(delegate(string toService)
            {
                toServiceList += string.Format("<toService>{0}</toService>", toService);
            });
            string ssbMsg = string.Format(msgFormat, toServiceList, GetXmlRootNodeName(xmlData), xmlData);

            DbHelper.ExecuteNonQuery(databaseName, CommandType.Text,
                "EXEC " + spName + " @msgXml", 300, new SqlParameter("@msgXml", ssbMsg));
        }

        #endregion SendV1

        #region SendV2

        /// <summary>
        /// 使用SSB 2.0版本来发送消息
        /// </summary>
        /// <typeparam name="T">待发送的消息数据类型，只能为引用类型</typeparam>
        /// <param name="fromService">发送的Service标示</param>
        /// <param name="toService">发送到的目标Service标示</param>
        /// <param name="subject">消息的主题，SSB 2.0会根据该主题进行消息的路由，将消息发给相应的订阅方</param>
        /// <param name="data">待发送的消息数据对象，其将被Xml序列化后发送</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        /// <param name="removeDataRootXmlNode">对于待发送的消息数据对象data，其序列化为Xml文档后，是否需要去掉其根节点，只取根节点下的所有子节点作为消息的主体</param>
        public static void SendV2<T>(string fromService, string toService, string subject,
            T data, string databaseName, bool removeDataRootXmlNode = false) where T : class
        {
            string xmlData = data.ToXmlString();
            if (removeDataRootXmlNode)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlData);
                xmlData = doc.LastChild.InnerXml;
            }
            SendV2(fromService, toService, subject, xmlData, databaseName);
        }

        /// <summary>
        /// 使用SSB 2.0版本来发送消息
        /// </summary>
        /// <param name="fromService">发送的Service标示</param>
        /// <param name="toService">发送到的目标Service标示</param>
        /// <param name="subject">消息的主题，SSB 2.0会根据该主题进行消息的路由，将消息发给相应的订阅方</param>
        /// <param name="xmlData">消息主体的Xml内容，是发送方和订阅方共同能够理解的数据契约</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        public static void SendV2(string fromService, string toService, string subject, string xmlData, string databaseName)
        {
            xmlData = RemoveXmlHeader(xmlData);
            string ssbMsg = string.Format(@"<Publish xmlns=""http://soa.newegg.com/SOA/CN/InfrastructureService/V10/NeweggCNPubSubService"">
  <Subject>{0}</Subject>
  <FromService>{1}</FromService>
  <ToService>{2}</ToService>
  <Node>
    {3}
  </Node>
</Publish>",
                subject, fromService, toService, xmlData);

            SqlParameter p = new SqlParameter("@msgXml", SqlDbType.Xml);
            p.Value = ssbMsg;
            DbHelper.ExecuteNonQuery(databaseName, CommandType.Text,
                "EXEC [SSB].[NCPubSub].[Up_SendArticle] @msgXml", 300, p);
        }

        #endregion SendV2

        #region SendV3

        /// <summary>
        /// 使用SSB 3.0版本来发送消息
        /// </summary>
        /// <typeparam name="T">待发送的消息数据类型，只能为引用类型</typeparam>
        /// <param name="fromService">发送的Service标示</param>
        /// <param name="toService">发送到的目标Service标示</param>
        /// <param name="articleCategory">消息的Category，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="articleType1">消息的Category的Type1，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="articleType2">消息的Category的Type2，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="data">待发送的消息数据对象，其将被Xml序列化后发送</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        /// <param name="removeDataRootXmlNode">对于待发送的消息数据对象data，其序列化为Xml文档后，是否需要去掉其根节点，只取根节点下的所有子节点作为消息的主体</param>
        public static void SendV3<T>(string fromService, string toService, string articleCategory, string articleType1, string articleType2,
            T data, string databaseName, bool removeDataRootXmlNode = false) where T : class
        {
            SendV3(fromService, new string[] { toService }, articleCategory, articleType1, articleType2, data, databaseName, removeDataRootXmlNode);
        }

        /// <summary>
        /// 使用SSB 3.0版本来发送消息
        /// </summary>
        /// <typeparam name="T">待发送的消息数据类型，只能为引用类型</typeparam>
        /// <param name="fromService">发送的Service标示，可以有多个标示</param>
        /// <param name="toService">发送到的目标Service标示</param>
        /// <param name="articleCategory">消息的Category，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="articleType1">消息的Category的Type1，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="articleType2">消息的Category的Type2，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="data">待发送的消息数据对象，其将被Xml序列化后发送</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        /// <param name="removeDataRootXmlNode">对于待发送的消息数据对象data，其序列化为Xml文档后，是否需要去掉其根节点，只取根节点下的所有子节点作为消息的主体</param>
        public static void SendV3<T>(string fromService, string[] toService, string articleCategory, string articleType1, string articleType2,
            T data, string databaseName, bool removeDataRootXmlNode = false) where T : class
        {
            string xmlData = data.ToXmlString();
            if (removeDataRootXmlNode)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlData);
                xmlData = doc.LastChild.InnerXml;
            }
            SendV3(fromService, toService, articleCategory, articleType1, articleType2, xmlData, databaseName);
        }

        /// <summary>
        /// 使用SSB 3.0版本来发送消息
        /// </summary>
        /// <param name="fromService">发送的Service标示</param>
        /// <param name="toService">发送到的目标Service标示</param>
        /// <param name="articleCategory">消息的Category，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="articleType1">消息的Category的Type1，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="articleType2">消息的Category的Type2，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="xmlData">消息主体的Xml内容，是发送方和订阅方共同能够理解的数据契约</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        public static void SendV3(string fromService, string toService, string articleCategory, string articleType1, string articleType2,
            string xmlData, string databaseName)
        {
            SendV3(fromService, new string[] { toService }, articleCategory, articleType1, articleType2, xmlData, databaseName);
        }

        /// <summary>
        /// 使用SSB 3.0版本来发送消息
        /// </summary>
        /// <param name="fromService">发送的Service标示</param>
        /// <param name="toService">发送到的目标Service标示，可以有多个标示</param>
        /// <param name="articleCategory">消息的Category，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="articleType1">消息的Category的Type1，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="articleType2">消息的Category的Type2，SSB 3.0会根据消息的articleCategory、articleType1、articleType2这三个标示来寻找消息的订阅者</param>
        /// <param name="xmlData">消息主体的Xml内容，是发送方和订阅方共同能够理解的数据契约</param>
        /// <param name="databaseName">负责发送的存储过程所在的DB Server的数据库连接字符串在Database.config文件中的配置名称</param>
        public static void SendV3(string fromService, string[] toService, string articleCategory, string articleType1, string articleType2,
            string xmlData, string databaseName)
        {
            xmlData = RemoveXmlHeader(xmlData);

            string toServiceFormat = "<ToService>{0}</ToService>";
            string toServiceList = "";
            foreach (string str in toService)
            {
                toServiceList = toServiceList + string.Format(toServiceFormat, str);
            }

            string ssbMsg = string.Format(@"<Publish xmlns=""http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService"">
                  <FromService>{0}</FromService>
                  {1}
                  <RouteTable>
                    <Article xmlns=""http://soa.newegg.com/SOA/USA/InfrastructureService/V30/PubSubService"">
                      <ArticleCategory>{2}</ArticleCategory>
                      <ArticleType1>{3}</ArticleType1>
                      <ArticleType2>{4}</ArticleType2>
                    </Article>
                  </RouteTable>
                  <Node>
                    {5}
                  </Node>
             </Publish>", fromService, toServiceList, articleCategory, articleType1, articleType2, xmlData);

            SqlParameter p = new SqlParameter("@msgXml", SqlDbType.Xml);
            p.Value = ssbMsg;
            DbHelper.ExecuteNonQuery(databaseName, CommandType.Text,
                "EXEC [SSB3].[PubSubService].[Up_SendArticle] @msgXml", 300, p);
        }

        #endregion SendV3
    }
}