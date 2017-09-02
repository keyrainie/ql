using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;

namespace IPPOversea.Invoicemgmt.DAL
{
    class CommandManager
    {
        private static string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB_Config\Commands.xml");
        private static string DatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB_Config\Database.xml");
        public static void Make()
        {
            //CheckPath();
            //XDocument CommandsXml = new XDocument(
            //    new XDeclaration("1.0", "UTF-8", "yes")
            //    , new XElement("Commands"
            //        , new XElement("Command", new XAttribute("Key", "GetData")
            //        , new XElement("CommandText", "select * from ss where<>0"))
            //        , new XElement("Command", new XAttribute("Key", "GetData2")
            //        , new XElement("CommandText", "select * from customer where>0"))
            //    ));
            //CommandsXml.Save(configPath);
        }

        public static string GetCommandText(string key)
        {
            CheckPath();
            XDocument Commands = XDocument.Load(ConfigPath);
            var result =(from c in Commands.Descendants("Command")
                         where c.Attribute("Key").Value == key
                         select c.Element("CommandText").Value);
            if (result.Count()>0)
            {
                return result.First();
            }
            else
            {
                return string.Empty;
            }           
        }
        public static SqlCommand GetCommand(string key)
        {
            CheckPath();
            XDocument Commands = XDocument.Load(ConfigPath);
            var result = (from c in Commands.Descendants("Command")
                          where c.Attribute("Key").Value == key
                          select c);
            if (result.Count() > 0)
            {
                XElement element = result.First();
                string Database = element.Attribute("Database").Value;
                string conString = GetConString(Database);
                string sqlText = element.Element("CommandText").Value;
                SqlConnection cnn = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(sqlText, cnn);
                return cmd;
            }
            else
            {
                return null;
            }
        }
        private static string GetConString(string Database)
        {
            XDocument Databases = XDocument.Load(DatabasePath); 
            var result = (from c in Databases.Descendants("database")
                          where c.Attribute("name").Value == Database
                          select c.Element("connectionString").Value);
            if (result.Count() > 0)
            {
                return result.First();
            }
            else
            {
                return null;
            }
        }
        private static void CheckPath()
        {
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB_Config")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DB_Config"));
            }
        }
    }
}
