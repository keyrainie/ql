using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.IDataAccess.Keywords;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.BizProcessor.Keywords
{
    [VersionExport(typeof(AdvancedKeywordsProcessor))]
    public class AdvancedKeywordsProcessor
    {
        private IAdvancedKeywordsDA keywordDA = ObjectFactory<IAdvancedKeywordsDA>.Instance;

        public bool ValidateAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            if (item == null)
                //throw new BizException("对象不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords","Keywords_ObjectNotNull"));
            else if (string.IsNullOrEmpty(item.Keywords.Content))
                //throw new BizException("关键字不能为空！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NeedTheKeywordsValue"));
            else if (string.IsNullOrEmpty(item.ShowName.Content))
                //throw new BizException("必须指定显示名称！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NeedDispalyName"));
            else if (item.EndDate < item.BeginDate)
                //throw new BizException("开始日期不能大于结束日期！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_StartDateLessEndDate"));
            else if (string.IsNullOrEmpty(item.LinkUrl))
                //throw new BizException("必须指定链接地址！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NeedUrlPath"));
            else
                return true;
        }

        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            if(ValidateAdvancedKeywords(item))
            {
                var keywordsLines = item.Keywords.Content.SplitByLine();
                var hashSet = new HashSet<string>();
                for (int i = 0; i < keywordsLines.Length; i++)
                {
                    var line = keywordsLines[i].Trim();
                    if (string.IsNullOrEmpty(line.ToString()))
                        continue;

                    if (line.IndexOf(' ') != -1)  //if the line has any space
                        //throw new BizException("失败！每行关键字中不能有空格");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_HaveSpace"));

                    hashSet.Add(line);
                }
                item.Keywords.Content = hashSet.Join("\n");

                if (keywordDA.CheckSameAdvancedKeywords(item))
                    //throw new BizException("Keywords 在数据库中已存在!");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsistKeywords"));
                else
                {
                    int sysNO = keywordDA.AddAdvancedKeywords(item);
                    AdvancedKeywordsLog log = new AdvancedKeywordsLog();
                    log.CompanyCode = item.CompanyCode;
                    log.Operation = "Create";
                    log.AdvancedKeywordsInfoSysNo = sysNO;
                    log.Description = "";
                    log.LanguageCode = item.Keywords.LanguageCode;
                    keywordDA.CreateAdvancedKeywordsLog(log);
                }
            }
        }

        /// <summary>
        /// 编辑跳转关键字
        /// </summary>
        /// <param name="item"></param>
        public virtual void EditAdvancedKeywords(AdvancedKeywordsInfo item)
        {
            if (ValidateAdvancedKeywords(item))
            {
                AdvancedKeywordsInfo words = keywordDA.LoadAdvancedKeywordsInfo(item.SysNo.Value);
                if (words == null)
                    //throw new BizException("数据库中不存在该关键字！");
                    throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_NotExsistKeywords"));
                else
                {
                    keywordDA.EditAdvancedKeywords(item);
                    AdvancedKeywordsLog log = new AdvancedKeywordsLog();
                    log.CompanyCode = item.CompanyCode;
                    log.Operation = "Update";
                    log.AdvancedKeywordsInfoSysNo = item.SysNo.Value;
                    log.Description = "";
                    log.LanguageCode = item.Keywords.LanguageCode;
                    keywordDA.CreateAdvancedKeywordsLog(log);
                }
            }
        }

        /// <summary>
        /// 加载跳转关键字
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual AdvancedKeywordsInfo LoadAdvancedKeywordsInfo(int sysNo)
        {
            return keywordDA.LoadAdvancedKeywordsInfo(sysNo);
        }
    }
}
