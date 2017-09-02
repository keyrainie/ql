using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.MKT.BizProcessor;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(SegmentInfoAppService))]
    public class SegmentInfoAppService
    {
        #region 中文词库

        /// <summary>
        /// 添加中文词库
        /// </summary>
        /// <param name="info"></param>
        public void AddSegmentInfo(SegmentInfo info)
        {
            ObjectFactory<SegmentInfoProcessor>.Instance.AddSegmentInfo(info);
        }

        /// <summary>
        /// 批量设置中文词库有效
        /// </summary>
        /// <param name="item"></param>
        public void SetSegmentInfosValid(List<SegmentInfo> items)
        {
            //var sucessCount = 0;
            //var faileCount = 0;
            //List<Int32> successItems = new List<int>();
            //var errorDesc = string.Empty;
            
            //items.ForEach(item =>
            //    {
            //        if (item.InUser.ToLower().Equals(item.CurrentUser.ToLower()))
            //            faileCount++;
            //        else
            //        {
            //            sucessCount++;
            //            successItems.Add(int.Parse(item.SysNo.ToString()));
            //        }
            //    });
            //if (successItems.Count  > 0)
            //    ObjectFactory<SegmentInfoProcessor>.Instance.SetSegmentInfosValid(successItems);
            //errorDesc += "审核成功共" + sucessCount.ToString() + "条,审核失败共" + faileCount.ToString() + "条;\n失败原因:创建人和审核人不能相同!";
            //if (!String.IsNullOrEmpty(errorDesc))
            //    throw new BizException(errorDesc);
        }

        /// <summary>
        /// 批量设置中文词库无效
        /// </summary>
        /// <param name="item"></param>
        public void SetSegmentInfosInvalid(List<SegmentInfo> items)
        {

            //var sucessCount = 0;
            //var faileCount = 0;
            //List<Int32> successItems = new List<int>();
            //var errorDesc = string.Empty;

            //items.ForEach(item =>
            //{
            //    if (item.InUser.ToLower().Equals(item.CurrentUser.ToLower()))
            //        faileCount++;
            //    else
            //    {
            //        sucessCount++;
            //        successItems.Add(int.Parse(item.SysNo.ToString()));
            //    }
            //});
            //if (successItems.Count > 0)
            //    ObjectFactory<SegmentInfoProcessor>.Instance.SetSegmentInfosInvalid(successItems);
            //errorDesc += "审核成功共" + sucessCount.ToString() + "条,审核失败共" + faileCount.ToString() + "条\n失败原因:创建人和审核人不能相同!";
            //if (!String.IsNullOrEmpty(errorDesc))
            //    throw new BizException(errorDesc);
        }

        /// <summary>
        /// 批量删除中文词库
        /// </summary>
        /// <param name="item"></param>
        public void DeleteSegmentInfos(List<int> items)
        {
            ObjectFactory<SegmentInfoProcessor>.Instance.DeleteSegmentInfos(items);
        }

        /// <summary>
        /// 更新中文词库
        /// </summary>
        /// <param name="item"></param>
        public void UpdateSegmentInfo(SegmentInfo item)
        {
            ObjectFactory<SegmentInfoProcessor>.Instance.UpdateSegmentInfo(item);
        }

        /// <summary>
        /// 加载中文词库
        /// </summary>
        /// <param name="item"></param>
        public SegmentInfo LoadSegmentInfo(int sysNo)
        {
            return ObjectFactory<SegmentInfoProcessor>.Instance.LoadSegmentInfo(sysNo);
        }

        /// <summary>
        /// 上传批量添加中文词库
        /// </summary>
        /// <param name="uploadFileInfo"></param>
        public virtual void BatchImportSegment(string uploadFileInfo)
        {
            ObjectFactory<SegmentInfoProcessor>.Instance.BatchImportSegment(uploadFileInfo);
        }
        #endregion
    }
}
