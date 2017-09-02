using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;
using ECCentral.Service.MKT.IDataAccess;
using System.IO;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(SegmentInfoProcessor))]
    public class SegmentInfoProcessor
    { 
        #region `中文词库
        
        private ISegmentInfoDA keywordDA = ObjectFactory<ISegmentInfoDA>.Instance;

        /// <summary>
        /// 添加中文词库
        /// </summary>
        /// <param name="item"></param>
        public virtual void AddSegmentInfo(SegmentInfo item)
        {
            if (keywordDA.CheckSegmentInfo(item))
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheKeywords"));
            else
                keywordDA.AddSegmentInfo(item);
        }

        /// <summary>
        /// 更新中文词库
        /// </summary>
        /// <param name="item"></param>
        public virtual void UpdateSegmentInfo(SegmentInfo item)
        {
            if (!keywordDA.CheckSegmentInfo(item))
            {
                SegmentInfo s = LoadSegmentInfo(item.SysNo.Value);
                if (s == null)
                    throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_DonnotExsitTheKeywords"));
                else if (s.Keywords.Content != item.Keywords.Content)
                {
                    item.Status = KeywordsStatus.Waiting;
                    keywordDA.UpdateSegmentInfo(item);
                }
            }
            else
            {
                //throw new BizException("已存在的关键字,无法修改!");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ExsitTheKeywords"));
            }
        }

        /// <summary>
        /// 加载中文词库
        /// </summary>
        /// <param name="item"></param>
        public virtual SegmentInfo LoadSegmentInfo(int sysNo)
        {
            return keywordDA.LoadSegmentInfo(sysNo);
        }

        /// <summary>
        /// 批量删除中文词库
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeleteSegmentInfos(List<int> items)
        {
            keywordDA.DeleteSegmentInfos(items);
        }

        /// <summary>
        /// 批量设置中文词库无效
        /// </summary>
        /// <param name="item"></param>
        public virtual void SetSegmentInfosInvalid(List<int> items)
        {
            keywordDA.SetSegmentInfosInvalid(items);
        }

        /// <summary>
        /// 批量设置中文词库有效
        /// </summary>
        /// <param name="item"></param>
        public virtual void SetSegmentInfosValid(List<int> items)
        {
            keywordDA.SetSegmentInfosValid(items);
        }
        /// <summary>
        /// 上传批量添加中文词库
        /// </summary>
        /// <param name="uploadFileInfo"></param>
        public virtual void BatchImportSegment(string uploadFileInfo)
        {
            //segmentInfoAppService.BatchImportSegment(uploadFileInfo);
            if (FileUploadManager.FileExists(uploadFileInfo))
            {

                string configPath = AppSettingManager.GetSetting("MKT", "PostSegmentInfoFilesPath");
                if (!Path.IsPathRooted(configPath))
                {
                    configPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, configPath);
                }
                string destinationPath = Path.Combine(configPath, uploadFileInfo);
                string folder = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                FileUploadManager.MoveFile(uploadFileInfo, destinationPath);

                using (var reader = new StreamReader(destinationPath, Encoding.Default))
                {
                    var lines = new HashSet<string>();
                    while (!reader.EndOfStream)
                    {
                        var value = reader.ReadLine().Trim().Replace("\t", " ");
                        if (!string.IsNullOrEmpty(value))
                        {
                            lines.Add(value);
                        }
                    }

                    if (!lines.Any())
                        //throw new BizException("导入的txt文件没有任何内容");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Keywords","Keywords_HasNotActiveDataInTxt"));

                    if (lines.Count > 2000)
                        //throw new BizException("导入的关键字不能超过2000个");
                        throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_KeywordsMoreThan2000"));
                    string companyCode = "8601";    //[Mark][Alan.X.Luo 硬编码]

                    int sameSegment = 0;
                    int failedSegment = 0;
                    int seccussfulSegment = 0;
                    lines.Where(e => !string.IsNullOrEmpty(e)).ForEach(e =>
                    {
                        var keywords = e.Trim();
                        SegmentInfo item = new SegmentInfo();
                        item.Keywords = new LanguageContent();
                        item.Keywords.Content = e;
                        item.Keywords.LanguageCode = "zh-CN";
                        item.CompanyCode = "8601";  //[Mark][Alan.X.Luo 硬编码]
                        if (keywordDA.CheckSegmentInfo(item))
                            sameSegment++;
                        else if (e.Length > 50)
                            failedSegment++; //also can add a property to counting the case  
                        else
                        {
                            item.Status = KeywordsStatus.Waiting;
                            item.CompanyCode = companyCode;
                            keywordDA.AddSegmentInfo(item);

                            seccussfulSegment++;
                        }
                    });
                   
                    StringBuilder message = new StringBuilder();
                    if (failedSegment > 0)
                        //message.AppendLine(failedSegment.ToString() + "条数据导入失败！");
                        message.AppendLine(failedSegment.ToString() + ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ImportFailed"));

                    if (sameSegment > 0)
                        //message.AppendLine(sameSegment.ToString() + "条数据已经存在数据库！");
                        message.AppendLine(sameSegment.ToString() + ResouceManager.GetMessageString("MKT.Keywords", "Keywords_AlreadyInDataBase"));
                    if (seccussfulSegment > 0)
                        //message.AppendLine(seccussfulSegment.ToString() + "条数据导入成功！");
                        message.AppendLine(seccussfulSegment.ToString() + ResouceManager.GetMessageString("MKT.Keywords", "Keywords_ImportSuccess"));

                    if (!string.IsNullOrEmpty(message.ToString()))
                        throw new BizException(message.ToString());
                }
            }
            else
                //throw new BizException("上传文件丢失！");
                throw new BizException(ResouceManager.GetMessageString("MKT.Keywords", "Keywords_UploadFileLost"));
        }
        #endregion
    }
}
