using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SendMKTPointEmail
{
 

   public class AccountPointNoticeMailEntity :ConfigurationElement
   {
       // Test flag.
       private static bool _displayIt = false;

       public AccountPointNoticeMailEntity(string newPointAccount,
           int newPointLimt, string newMailTo)
       {
           Account = newPointAccount;
           PointLimt = newPointLimt;
           MailTo = newMailTo;

       }

       public AccountPointNoticeMailEntity()
       {

       }

       public AccountPointNoticeMailEntity(string elementName)
       {
           Account = elementName;
       }

       [ConfigurationProperty("account",
           DefaultValue = "PointAccount",
           IsRequired = true,
           IsKey = true)]
       public string Account
       {
           get
           {
               return (string)this["account"];
           }
           set
           {
               this["account"] = value;
           }
       }

       [ConfigurationProperty("pointLimt",
           DefaultValue = "10000",
           IsRequired = true)]
       [IntegerValidator(MinValue = 1000,
          MaxValue = int.MaxValue, ExcludeRange = false)]
      
       public int PointLimt
       {
           get
           {
               return (int)this["pointLimt"];
           }
           set
           {
               this["pointLimt"] = value;
           }
       }

       [ConfigurationProperty("mailTo",
           DefaultValue = "Tom.D.Zhou@newegg.com;Cropland.J.Tian@newegg.com",
           IsRequired = false)]
       public string MailTo
       {
           get
           {
               return (string)this["mailTo"];
           }
           set
           {
               this["mailTo"] = value;
           }
       }

       protected override void DeserializeElement(
          System.Xml.XmlReader reader,
           bool serializeCollectionKey)
       {
           base.DeserializeElement(reader,
               serializeCollectionKey);

           // Enter your custom processing code here.
           if (_displayIt)
           {
               Console.WriteLine(
                  "AccountPointNoticeMailEntity.DeserializeElement({0}, {1}) called",
                  (reader == null) ? "null" : reader.ToString(),
                  serializeCollectionKey.ToString());
           }
       }


       protected override bool SerializeElement(
           System.Xml.XmlWriter writer,
           bool serializeCollectionKey)
       {
           bool ret = base.SerializeElement(writer,
               serializeCollectionKey);

           // Enter your custom processing code here.

           if (_displayIt)
           {
               Console.WriteLine(
                   "AccountPointNoticeMailEntity.SerializeElement({0}, {1}) called = {2}",
                   (writer == null) ? "null" : writer.ToString(),
                   serializeCollectionKey.ToString(), ret.ToString());
           }
           return ret;

       }


       protected override bool IsModified()
       {
           bool ret = base.IsModified();

           // Enter your custom processing code here.

           Console.WriteLine("AccountPointNoticeMailEntity.IsModified() called.");

           return ret;
       }


   }
}
