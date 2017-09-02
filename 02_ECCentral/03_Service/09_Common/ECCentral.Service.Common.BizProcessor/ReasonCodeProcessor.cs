using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.BizProcessor
{
    [VersionExport(typeof(ReasonCodeProcessor))]
    public class ReasonCodeProcessor
    {
        private IReasonCodeDA DA = ObjectFactory<IReasonCodeDA>.Instance;
        public virtual ReasonCodeEntity Create(ReasonCodeEntity entity)
        {
            return DA.InsertReasonCode(entity);
        }

        public virtual void Update(ReasonCodeEntity entity)
        {
            DA.UpdateReasonCode(entity);
        }

        public virtual void UpdateReasonStatusList(List<ReasonCodeEntity> list)
        {

            //string editUser=
            int[] voidList=list.Where(x=>(x.Status==0 && x.SysNo.HasValue)).Select(x=>(x.SysNo.Value)).ToArray();
            int[] avaList=list.Where(x=>(x.Status==1 && x.SysNo.HasValue)).Select(x=>(x.SysNo.Value)).ToArray();

            string editUser=list[0].EditUser;

            string voidStr=string.Format("where SysNo IN ({0})",string.Join(",",voidList));
            string avaStr=string.Format("where SysNo IN ({0})",string.Join(",",avaList));

            if (voidList!=null && voidList.Length>0 )
                DA.UpdateReasonStatusList(voidStr, 0, editUser);

            if (avaList != null && avaList.Length > 0)
                DA.UpdateReasonStatusList(avaStr,1,editUser);
        }

        public virtual ReasonCodeEntity GetReasonCodeBySysNo(int SysNo)
        {
            return DA.GetReasonCodeBySysNo(SysNo);
        }

        public virtual List<ReasonCodeEntity> GetChildrenReasonCode(int parentSysNo)
        {
            return DA.GetChildrenReasonCode(parentSysNo);
        }

        public virtual List<ReasonCodeEntity> GetReasonCodeByNodeLevel(int level, string companyCode)
        {
            return DA.GetReasonCodeByNodeLevel(level, companyCode);
        }
        public virtual string GetReasonCodePath(int codeSysNo, string companyCode)
        {
            string result = string.Empty;
            List<ReasonCodeEntity> allList = DA.GetAllReasonCodeByCompany(companyCode);
            ReasonCodeEntity nodeData = allList.SingleOrDefault(p => p.SysNo == codeSysNo);
            if (nodeData == null) { return ""; }
            Stack<string> stackPath = new Stack<string>();

            stackPath.Push(nodeData.Description);
            while (nodeData.ParentNodeSysNo != 1)
            {
                nodeData = allList.Single(p => p.SysNo == nodeData.ParentNodeSysNo);
                stackPath.Push(nodeData.Description);
            }

            StringBuilder sb = new StringBuilder();

            while (stackPath.Count > 0)
            {
                sb.Append(stackPath.Pop());
                sb.Append(">");
            }
            result = sb.ToString();
            if (result.EndsWith(">"))
            {
                result = result.Remove(sb.ToString().Length - 1, 1);
            }

            return result;
        }
    }
}
