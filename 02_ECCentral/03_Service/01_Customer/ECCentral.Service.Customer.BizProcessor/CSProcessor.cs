using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(CSProcessor))]
    public class CSProcessor
    {
        private ICSDA csDA = ObjectFactory<ICSDA>.Instance;

        public virtual CSInfo GetCSListBySysNo(int sysNo)
        {
            return csDA.GetCSBySysNo(sysNo);
        }
        public virtual CSInfo Create(CSInfo entity)
        {
            if (entity.IPPUserSysNo < 1)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CSIsNull"));
            }
            CSInfo cs = csDA.GetCSByIPPUserSysNo(entity.IPPUserSysNo.Value, entity);
            if (cs != null)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CS", "ExistsCS"));
            }
            if (entity.Role == (int)CSRole.CS)
            {
                if (entity.LeaderIPPUserSysNo != 0)
                {
                    if (entity.LeaderIPPUserSysNo == entity.IPPUserSysNo)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CantSerCsSameAsLeader"));
                    }
                    cs = csDA.GetCSByIPPUserSysNo(entity.LeaderIPPUserSysNo.Value, entity);
                    if (cs.SysNo == null)
                    {
                        cs = new CSInfo();
                        cs.Role = (int)CSRole.Leader;//leader
                        cs.IPPUserSysNo = entity.LeaderIPPUserSysNo;
                        cs.UserName = entity.LeaderUserName;
                        cs.CompanyCode = entity.CompanyCode;
                        cs.LeaderSysNo = 0;
                        cs.ManagerSysNo = 0;
                        csDA.InsertCS(cs, false);
                    }
                    if (cs.Role != (int)CSRole.Leader)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CSNotLeader"));
                    }

                    entity.LeaderSysNo = cs.SysNo;
                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.CS", "NeedSetALeader"));
                }
            }

            if (entity.ManagerIPPUserSysNo.HasValue)
            {
                if (entity.ManagerIPPUserSysNo == entity.IPPUserSysNo)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CantSerCsSameAsManager"));
                }
                cs = csDA.GetCSByIPPUserSysNo(entity.ManagerIPPUserSysNo.Value, entity);
                if (cs == null)
                {
                    cs = new CSInfo();
                    cs.Role = (int)CSRole.Manager;//manager

                    cs.UserName = entity.UserName;
                    cs.IPPUserSysNo = entity.ManagerIPPUserSysNo.Value;
                    cs.CompanyCode = entity.CompanyCode;
                    cs.LeaderSysNo = 0;
                    cs.ManagerSysNo = 0;
                    csDA.InsertCS(cs, false);
                }
                if (cs.Role != (int)CSRole.Manager)
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CSNotManager"));
                }
                entity.ManagerSysNo = cs.SysNo;
            }
            entity.UserName = entity.UserName;
            if (!entity.LeaderSysNo.HasValue)
            {
                entity.LeaderSysNo = 0;
            }
            if (!entity.ManagerSysNo.HasValue)
            {
                entity.ManagerSysNo = 0;
            }
            entity = csDA.InsertCS(entity, false);

            if (entity.Role == (int)CSRole.Leader)//addCSLeader
            {
                if (entity.CSIPPUserSysNos != null && entity.CSIPPUserSysNos.Count > 0)
                {
                    for (int i = 0; i < entity.CSIPPUserSysNos.Count; i++)
                    {
                        cs = new CSInfo();
                        cs.Role = (int)CSRole.CS;//manager
                        cs.UserName = entity.CSUserNames[i];
                        cs.IPPUserSysNo = entity.CSIPPUserSysNos[i];
                        cs.CompanyCode = entity.CompanyCode;
                        cs.LeaderSysNo = entity.SysNo;
                        cs.ManagerSysNo = 0;
#warning 请填充下面注释的属性
                        //cs.InUserSysNo = entity.InUserSysNo;
                        //cs.InDate = DateTime.Now;
                        csDA.InsertCS(cs, true);
                    }
                }
            }
            return entity;
        }

        public virtual CSInfo EditCSList(CSInfo entity)
        {
            int result = 0;

            CSInfo cs = csDA.GetCSBySysNo(entity.SysNo.Value);
            if (cs == null)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CS", "NotExistsCS"));
            }
            if (cs.Role != 1 && entity.Role != cs.Role && cs.UnderlingNum != 0)
            {
                throw new BizException(ResouceManager.GetMessageString("Customer.CS", "NeedRemoveUnderLing"));
            }
            if (entity.Role == (int)CSRole.CS)
            {
                if (entity.LeaderIPPUserSysNo.HasValue && entity.LeaderIPPUserSysNo.Value != 0)
                {
                    cs = csDA.GetCSByIPPUserSysNo(entity.LeaderIPPUserSysNo.Value, entity);
                    if (entity.LeaderSysNo == entity.SysNo)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CantSerCsSameAsLeader"));
                    }
                    if (cs == null)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CS", "NotExistsCS"));
                    }
                    if (cs.Role != (int)CSRole.Leader)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CSNotLeader"));
                    }
                    entity.ManagerSysNo = 0;
                    entity.LeaderSysNo = cs.SysNo;
                    result = csDA.UpdateCS(entity);
                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("Customer.CS", "NeedSetALeader"));
                }
                if (entity.ManagerSysNo.HasValue)
                {
                    entity.ManagerSysNo = 0;
                }
            }
            else if (entity.Role == (int)CSRole.Manager)
            {
                if (entity.LeaderSysNo.HasValue)//
                {
                    entity.LeaderSysNo = 0;
                }
                if (entity.ManagerSysNo.HasValue)
                {
                    entity.ManagerSysNo = 0;
                }
            }
            else if (entity.Role == (int)CSRole.Leader)//addCSLeader
            {
                if (entity.ManagerIPPUserSysNo.HasValue && entity.ManagerIPPUserSysNo.Value != 0)
                {
                    cs = csDA.GetCSByIPPUserSysNo(entity.ManagerIPPUserSysNo.Value, entity);
                    if (entity.ManagerSysNo == entity.SysNo)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CantSerCsSameAsManager"));
                    }

                    if (cs == null)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CS", "NotExistsCS"));
                    }
                    if (cs.Role != (int)CSRole.Manager)
                    {
                        throw new BizException(ResouceManager.GetMessageString("Customer.CS", "CSNotManager"));
                    }
                    entity.ManagerSysNo = cs.SysNo;
                }

                if (entity.LeaderSysNo.HasValue)
                {
                    entity.LeaderSysNo = 0;
                }
                List<CSInfo> cslists = csDA.GetCSByLeaderSysNo(entity.SysNo.Value);

                foreach (CSInfo cslist in cslists)
                {
                    cslist.LeaderSysNo = 0;
                    cslist.SysNo = cslist.SysNo;
                    csDA.UpdateCS(cslist);
                }

                if (entity.CSIPPUserSysNos != null && entity.CSIPPUserSysNos.Count > 0)
                {
                    for (int i = 0; i < entity.CSIPPUserSysNos.Count; i++)
                    {
                        cs = new CSInfo();
                        cs.Role = (int)CSRole.CS;
                        cs.LeaderSysNo = entity.SysNo;
                        cs.ManagerSysNo = 0;
                        cs.CompanyCode = entity.CompanyCode;
                        cs.SysNo = entity.CSIPPUserSysNos[i];

                        result = csDA.UpdateCS(cs);
                    }
                }
            }
            if (!entity.LeaderSysNo.HasValue)
            {
                entity.LeaderSysNo = 0;
            }
            if (!entity.ManagerSysNo.HasValue)
            {
                entity.ManagerSysNo = 0;
            }
            result = csDA.UpdateCS(entity);
            return entity;
        }

        public virtual List<CSInfo> GetCSWithDepartmentId(int depid)
        {
            return csDA.GetCSWithDepartmentId(depid);
        }

        public virtual List<CSInfo> GetAllCS(string companyCode)
        {
            return csDA.GetAllCS(companyCode);
        }
        public virtual List<CSInfo> GetCSByLeaderSysNo(int leadersysno)
        {
            return csDA.GetCSByLeaderSysNo(leadersysno);
        }
    }
}
