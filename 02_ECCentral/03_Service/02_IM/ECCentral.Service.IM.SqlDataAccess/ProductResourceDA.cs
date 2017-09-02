//************************************************************************
// 用户名				泰隆优选
// 系统名				图片管理
// 子系统名		       图片管理业务数据底层接口实现
// 作成者				Tom
// 改版日				2012.6.01
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductResourceDA))]
    public class ProductResourceDA : IProductResourceDA
    {

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="productCommonInfoSysNo"></param>
        /// <param name="resourceSysNo"></param>
        public void DeleteProductResource(int resourceSysNo, int productCommonInfoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("DeleteProductResource");
            cmd.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            cmd.SetParameterValue("@ResourceSysNo", resourceSysNo);
            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// 创建商品图片
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="productCommonInfoSysNo"> </param>
        public void InsertProductResource(ProductResourceForNewegg resource, int productCommonInfoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertProductResource");
            cmd.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            cmd.SetParameterValue("@ResourceUrl", resource.Resource.ResourceURL);
            cmd.SetParameterValue("@Type", resource.Resource.Type);
            cmd.SetParameterValue("@InUser", resource.Resource.OperateUser.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", resource.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", resource.LanguageCode);
            cmd.ExecuteNonQuery();
            var resourceSysNo = (int)cmd.GetParameterValue("@ResourceSysNo");
            resource.Resource.ResourceSysNo = resourceSysNo;
        }

        /// <summary>
        /// 修改商品图片
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="productCommonInfoSysNo"> </param>
        public void UpdateProductResource(ProductResourceForNewegg resource, int productCommonInfoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductResource");
            cmd.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            cmd.SetParameterValue("@Priority", resource.Resource.Priority);
            cmd.SetParameterValue("@ResourceSysNo", resource.Resource.ResourceSysNo);
            cmd.SetParameterValue("@ResourceUrl", resource.Resource.ResourceURL);
            cmd.SetParameterValue("@Type", resource.Resource.Type);
            cmd.ExecuteNonQuery();
        }

        public void UpdateResource(int resourceSysNo, int productGroupSysNo,UserInfo operationUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateResource");
            cmd.SetParameterValue("@ResourceSysNo", resourceSysNo);
            cmd.SetParameterValue("@ProductGroupSysNo", productGroupSysNo);
            cmd.SetParameterValue("@EditUser", operationUser.UserDisplayName);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取商品资源列表(Do Not Delete / Vantal)
        /// </summary>
        /// <param name="productCommonInfoSysNo"></param>
        /// <returns></returns>
        private List<ProductResource> GetProductResourceListByProductCommonInfoSysNo(int productCommonInfoSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetProductResourceListByProductCommonInfoSysNo");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            var sourceEntity = dc.ExecuteEntityList<ProductResource>();
            return sourceEntity.OrderBy(r => r.Priority).ToList();
        }

        /// <summary>
        /// 获取商品资源列表
        /// </summary>
        /// <param name="productCommonInfoSysNo"></param>
        /// <returns></returns>
        public List<ProductResourceForNewegg> GetNeweggProductResourceListByProductCommonInfoSysNo(int productCommonInfoSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetNeweggProductResourceListByProductCommonInfoSysNo");
            dc.SetParameterValue("@ProductCommonInfoSysNo", productCommonInfoSysNo);
            var sourceEntity = dc.ExecuteEntityList<ProductResourceForNewegg>();
            return sourceEntity.OrderBy(r => r.Resource.Priority).ToList();
        }



        /// <summary>
        /// 是否存在相同的名字
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int GetProductGroupInfoImageSysNoByFileName(string fileName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductGroupInfoImageSysNoByFileName");
            cmd.SetParameterValue("@ResourceUrl", fileName);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return -1;
            }
            var count = Convert.ToInt32(value);
            return count;
        }
    }
}
