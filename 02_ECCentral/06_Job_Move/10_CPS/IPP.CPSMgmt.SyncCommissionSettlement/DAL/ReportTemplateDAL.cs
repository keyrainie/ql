using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using IPPOversea.Invoicemgmt.SyncCommissionSettlement.Model;
using System.IO;
using System.Configuration;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.DAL
{
    /// <summary>
    /// 模版数据访问类
    /// </summary>
    public class ReportTemplateDAL
    {
        /// <summary>
        /// 获取邮件数据模版
        /// </summary>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public static FincialPayReportRoot GetReportTemplate(string xmlFileName)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory + Settings.ReportConfigPath + xmlFileName;
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(string.Format("不存在文件：{0}", path));
            }
            XmlSerializer xs = new XmlSerializer(typeof(FincialPayReportRoot));

            using (FileStream stream = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)))
            {
                return (FincialPayReportRoot)xs.Deserialize(stream);
            }
        }
    }
}
