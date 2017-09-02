using System;
using System.Collections.Generic;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.PO;
using System.Text;
using ECCentral.BizEntity;
using System.Data;

namespace ECCentral.Service.ExternalSYS.BizProcessor.VendorPortal
{
    [VersionExport(typeof(UserProcessor))]
    public class UserProcessor
    {
        public UserProcessor()
        {
            m_da = ObjectFactory<IVendorUserDA>.Instance;
        }

        #region Member

        IVendorUserDA m_da;

        #endregion

        #region Method

        /// <summary>
        /// 批量审核通过帐户
        /// </summary>
        /// <param name="sysNos">待通过的账户Id</param>
        public void BatchPass(List<int> sysNos)
        {
            BatchUpdateStatus(sysNos, ECCentral.BizEntity.ExternalSYS.ValidStatus.Active);
        }

        /// <summary>
        /// 批量作废
        /// </summary>
        /// <param name="sysNos">待作废的账户Id</param>
        public void BatchInvaild(List<int> sysNos)
        {
            BatchUpdateStatus(sysNos, ECCentral.BizEntity.ExternalSYS.ValidStatus.DeActive);
        }

        /// <summary>
        /// 批量更新状态
        /// </summary>
        /// <param name="sysNos">编号集合</param>
        /// <param name="status">状态</param>
        private void BatchUpdateStatus(List<int> sysNos, ECCentral.BizEntity.ExternalSYS.ValidStatus status)
        {
            var user = ExternalDomainBroker.GetUserBySysNo(ServiceContext.Current.UserSysNo);
            //原来数据库编辑格式的值为IPPSystemAdmin\bitkoo\IPPSystemAdmin[8601]
            m_da.UpdateVendorUserStatus(sysNos, status, user.UserName);
        }

        /// <summary>
        /// 创建User
        /// </summary>
        /// <param name="entity">请求体</param>
        /// <returns>创建后的实体</returns>
        public VendorUser Create(VendorUser entity)
        {
            if (entity == null
                 || string.IsNullOrEmpty(entity.UserID)
                 )
            {
                BizExceptionHelper.Throw("ExternalSYS_VendorUser_LoginError");
            }
            if (string.IsNullOrEmpty(entity.UserName))
            {
                BizExceptionHelper.Throw("ExternalSYS_VendorUser_LoginNameError");
            }
            if (!entity.VendorSysNo.HasValue)
            {
                BizExceptionHelper.Throw("ExternalSYS_VendorUser_SelectVendorError");
            }
            if (m_da.CountUserID(entity.UserID, 0) > 0)
            {
                BizExceptionHelper.Throw("ExternalSYS_VendorUser_ExistUserError");
            }
            if (string.IsNullOrEmpty(entity.APIKey) && entity.APIStatus == ECCentral.BizEntity.ExternalSYS.ValidStatus.Active)
            {
                entity.APIKey = RandomString(32);
            }

            entity.Pwd = Hash_MD5.GetMD5(AppSettingHelper.Get("VendorUserInitPassword"));

            entity.UserNum = m_da.CountVendorNum(entity.VendorSysNo.Value) + 1;
            entity = m_da.InsertVendorUser(entity);
            if (entity.ManufacturerSysNoList != null && entity.ManufacturerSysNoList.Count > 0)
            {
                foreach (var ManufacturerSysNo in entity.ManufacturerSysNoList)
                {
                    m_da.InsertVendorUserVendorEx(new VendorUserMapping()
                    {
                        UserSysNo = entity.SysNo,
                        ManufacturerSysNo = ManufacturerSysNo,
                        IsAuto = 1,
                        VendorSysNo = entity.VendorSysNo.Value
                    });
                }
            }
            return entity;
        }

        /// <summary>
        /// 更新VendorUser
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update(VendorUser entity)
        {
            if (entity == null
                 || string.IsNullOrEmpty(entity.UserID)
                 )
            {
                BizExceptionHelper.Throw("ExternalSYS_VendorUser_LoginError");
            }
            if (string.IsNullOrEmpty(entity.UserName))
            {
                BizExceptionHelper.Throw("ExternalSYS_VendorUser_LoginNameError");
            }
            if (!entity.VendorSysNo.HasValue)
            {
                BizExceptionHelper.Throw("ExternalSYS_VendorUser_SelectVendorError");
            }
            if (m_da.CountUserID(entity.UserID, entity.SysNo.Value) > 0)
            {
                BizExceptionHelper.Throw("ExternalSYS_VendorUser_ExistUserError");
            }

            if (string.IsNullOrEmpty(entity.APIKey) && entity.APIStatus == ECCentral.BizEntity.ExternalSYS.ValidStatus.Active)
            {
                entity.APIKey = RandomString(32);
            }
            else if (entity.APIStatus == ECCentral.BizEntity.ExternalSYS.ValidStatus.DeActive || entity.APIStatus == null)
            {
                entity.APIKey = null;
            }
            //2015.8.19为重置代码添加 John
            if(entity.Pwd!=null)
            {
                entity.Pwd = Hash_MD5.GetMD5(entity.Pwd);
            }

            bool result = m_da.UpdateVendorUser(entity);
            if (entity.ManufacturerSysNoList != null && entity.ManufacturerSysNoList.Count > 0)
            {
                foreach (var ManufacturerSysNo in entity.ManufacturerSysNoList)
                {
                    m_da.InsertVendorUser_VendorExForUpdate(new VendorUserMapping()
                    {
                        UserSysNo = entity.SysNo,
                        ManufacturerSysNo = ManufacturerSysNo,
                        IsAuto = 0
                    });
                }
            }
            return result;
        }

        /// <summary>
        /// 获取供应商账号信息
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>供应商账号信息</returns>
        public VendorUser GetUserInfo(int sysNo)
        {
            return m_da.GetUserBySysNo(sysNo);
        }

        /// <summary>
        /// 根据供应商编号获取对应的代理信息
        /// </summary>
        /// <param name="vendorInfo"></param>
        /// <returns></returns>
        public List<VendorAgentInfo> GetVendorAgentInfo(VendorInfo vendorInfo)
        {
            return ExternalDomainBroker.GetVendorAgentInfoList(vendorInfo);
        }

        //更新VendorProduct
        public int UpdateVendorProdut(VendorProductList entity)
        {
            if (entity.SetProductSysNoList == null
                && entity.CancelSetProductSysNoList == null && !entity.IsAuto.HasValue)
            {
                throw new BizException("No product is checked");
            }
            if (entity.SetAndCancelAll.HasValue)
            {
                if (entity.SetAndCancelAll.Value)
                {
                    m_da.InsertVendorUser_ProductMappingAll(entity);
                }
                else
                {
                    m_da.DeleteVendorUser_ProductMappingAll(entity);
                }
                return 0;
            }

            if (entity.IsAuto.HasValue)
            {
                m_da.UpdateVendorUser_VendorEx(entity);
                if (entity.IsAuto == 0)
                {
                    m_da.InsertVendorUser_ProductMappingAll(entity);
                }
            }

            if (entity.CancelSetProductSysNoList != null)
            {
                foreach (var item in entity.CancelSetProductSysNoList)
                {
                    m_da.DeleteVendorUser_ProductMapping(
                        new VendorProductList()
                        {
                            UserSysNo = entity.UserSysNo,
                            ProductSysNo = item
                        }
                        );
                }
            }
            if (entity.SetProductSysNoList != null)
            {
                foreach (var item in entity.SetProductSysNoList)
                {
                    m_da.InsertVendorUser_ProductMapping(
                        new VendorProductList()
                        {
                            UserSysNo = entity.UserSysNo,
                            ProductSysNo = item,
                            ManufacturerSysNo = entity.ManufacturerSysNo
                        }
                        );
                }
            }

            return 0;
        }

        #region 生成账户密钥

        private static Random random = new Random((int)DateTime.Now.Ticks);

        private static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            int seed, asc, num;
            for (int i = 0; i < size; i++)
            {
                seed = random.Next(1, 4);
                switch (seed)
                {
                    case 1: asc = 65; num = 26; break;
                    case 2: asc = 97; num = 26; break;
                    case 3: asc = 48; num = 10; break;
                    default: asc = 65; num = 26; break;
                }
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(num * random.NextDouble() + asc)));
                builder.Append(ch);
            }

            return builder.ToString();
        }


        #endregion

        #endregion
    }
}
