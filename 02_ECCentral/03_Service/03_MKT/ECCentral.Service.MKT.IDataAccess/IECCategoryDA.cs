using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IECCategoryDA
    {
        //通过前台3级类别找到对应的后台3级类别，
        //在把与后台3级类别对用的所有前台3级类别找出来
        List<ECCategory> GetRelatedECCategory3SysNo(int c3SysNo);

        /// <summary>
        /// 插入前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        void Insert(ECCategory entity);

        /// <summary>
        /// 更新前台显示分类
        /// </summary>
        /// <param name="entity">前台显示分类</param>
        void Update(ECCategory entity);

        /// <summary>
        /// 删除前台显示分类
        /// </summary>
        /// <param name="sysNo">前台显示分类系统编号</param>
        void Delete(int sysNo);

        //验证分类名称是否重复
        bool CheckNameDuplicate(string name, int excludeSysNo, ECCategoryLevel level, string companyCode, string channelID,string ParentList);

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>ECCategory对象</returns>
        ECCategory Load(int sysNo);

        /// <summary>
        /// 判断层级关系是否存在
        /// </summary>
        /// <param name="ecCategorySysNo">前台分类系统编号</param>
        /// <param name="parentSysNo">父级关系系统编号</param>
        bool ExistsRelation(int ecCategorySysNo, int parentSysNo);

        /// <summary>
        /// 插入前台分类的层级关系
        /// </summary>
        /// <param name="relation">前台分类的层级关系</param>
        void InsertRelation(ECCategoryRelation relation);

        /// <summary>
        /// 删除前台分类的层级关系
        /// </summary>
        /// <param name="ecCategorySysNo">前台分类系统编号</param>
        void DeleteRelation(int ecCategorySysNo);

        /// <summary>
        /// 在维护前台分类父类时，如果移除了一些父类，相应的关系也要移除
        /// </summary>
        void DeleteOldRelation(int ecCategorySysNo, List<int> rParentSysNoList);

        void InsertCategoryProductMapping(int ecCategorySysNo, int productSysNo);

        /// <summary>
        /// 检测商品是否已经添加到某个分类中
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        string IsProductMapped(int productSysNo);

        bool DeleteCategoryProductMapping(int ecCategorySysNo, int productSysNo);

        
    }
}
