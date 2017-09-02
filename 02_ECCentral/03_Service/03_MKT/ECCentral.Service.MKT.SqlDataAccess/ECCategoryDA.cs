using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility;

namespace ECCentral.Service.PO.SqlDataAccess
{
    [VersionExport(typeof(IECCategoryDA))]
    public class ECCategoryDA : IECCategoryDA
    {
        #region IECCategoryDA Members

        //通过前台3级类别找到对应的后台3级类别，
        //在把与后台3级类别对用的所有前台3级类别找出来
        public List<ECCategory> GetRelatedECCategory3SysNo(int c3SysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ECCategory_GetC3SysNo");
            dc.SetParameterValue("@SysNo", c3SysNo);

            return dc.ExecuteEntityList<ECCategory>();
        }

        /// <summary>
        /// 插入前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        public void Insert(ECCategory entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_Insert");
            cmd.SetParameterValue(entity);

            cmd.ExecuteNonQuery();
            entity.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
        }

        /// <summary>
        /// 更新前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        public void Update(ECCategory entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_Update");
            cmd.SetParameterValue(entity);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除前台显示分类
        /// </summary>
        /// <param name="sysNo">前台显示分类系统编号</param>
        public void Delete(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_Delete");
            cmd.SetParameterValue("@SysNo", sysNo);

            cmd.ExecuteNonQuery();
        }

        //验证同级分类名称是否重复
        public bool CheckNameDuplicate(string name, int excludeSysNo, ECCategoryLevel level, string companyCode, string channelID, string parentSysnoList)
        {
            //DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_CheckNameDuplicate");
            //cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            //cmd.SetParameterValue("@CategoryName", name);
            //cmd.SetParameterValue("@Level", level);
            //cmd.SetParameterValue("@CompanyCode", companyCode);
            ////TODO:添加渠道参数
            //cmd.SetParameterValue("@parentSysnoList", parentSysnoList);
            //return cmd.ExecuteScalar<int>() > 0;
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("ECCategory_CheckNameDuplicate");
            string expendStr=string.Format("AND cr.ParentSysno in ({0})",parentSysnoList);
            if (level != ECCategoryLevel.Category1)
            {
                cmd.CommandText = cmd.CommandText.Replace("#parentSysnoList", expendStr);
            }
            else
            {
                cmd.CommandText = cmd.CommandText.Replace("#parentSysnoList", "");
            }
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@CategoryName", name);
            cmd.SetParameterValue("@Level", level);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO:添加渠道参数
            return cmd.ExecuteScalar<int>() > 0;

        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>ECCategory对象</returns>
        public ECCategory Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_Load");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<ECCategory>();
        }

        #endregion

        /// <summary>
        /// 判断层级关系是否存在
        /// </summary>
        /// <param name="ecCategorySysNo">前台分类系统编号</param>
        /// <param name="parentSysNo">父级关系系统编号</param>
        public bool ExistsRelation(int ecCategorySysNo, int parentSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_ExistsRelation");
            cmd.SetParameterValue("@ECCategorySysNo", ecCategorySysNo);
            cmd.SetParameterValue("@ParentSysNo", parentSysNo);

            return cmd.ExecuteScalar<int>() > 0;
        }

        /// <summary>
        /// 插入前台分类的层级关系
        /// </summary>
        /// <param name="relation">前台分类的层级关系</param>
        public void InsertRelation(ECCategoryRelation relation)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_InsertRelation");
            cmd.SetParameterValue(relation);

            cmd.ExecuteNonQuery();
            relation.SysNo = Convert.ToInt32(cmd.GetParameterValue("@SysNo"));
        }

        /// <summary>
        /// 删除前台分类的层级关系
        /// </summary>
        /// <param name="ecCategorySysNo">前台分类系统编号</param>
        public void DeleteRelation(int ecCategorySysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_DeleteRelation");
            cmd.SetParameterValue("@ECCategorySysNo", ecCategorySysNo);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 在维护前台分类父类时，如果移除了一些父类，相应的关系也要移除
        /// </summary>
        /// <param name="ecCategorySysNo">前台分类系统编号</param>
        public void DeleteOldRelation(int ecCategorySysNo, List<int> rParentSysNoList)
        {
            if (rParentSysNoList != null && rParentSysNoList.Count > 0)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("ECCategory_MaintainCategoryRelation");
                cmd.SetParameterValue("@ECCategorySysNo", ecCategorySysNo);
                cmd.ReplaceParameterValue("#CurrentParentRelationSysNo#", rParentSysNoList.Join(","));

                cmd.ExecuteNonQuery();
            }
        }

        public void InsertCategoryProductMapping(int ecCategorySysNo, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("MKT_InsertECCategoryProductMapping");
            cmd.SetParameterValue("@ECCategorySysNo", ecCategorySysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@InUser");
            cmd.ExecuteNonQuery();
        }

        public bool DeleteCategoryProductMapping(int ecCategorySysNo, int productSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("MKT_DeleteECCategoryProductMapping");

            cmd.SetParameterValue("@ECCategorySysNo", ecCategorySysNo);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@EditUser");

            return cmd.ExecuteNonQuery() > 0;
        }

        /// <summary>
        /// 检测商品是否已经添加到某个分类中
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public string IsProductMapped(int productSysNo)
        {
            var cmd = DataCommandManager.GetDataCommand("MKT_IsProductMapped");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            return cmd.ExecuteScalar<string>();
        }
    }
}
