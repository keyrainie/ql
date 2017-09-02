//************************************************************************
// 用户名				泰隆优选
// 系统名				PM组管理
// 子系统名		        PM组管理业务接口
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductManagerGroupDA
    {
        /// <summary>
        /// 根据SysNO获取PM组信息
        /// </summary>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        ProductManagerGroupInfo GetProductManagerGroupInfoBySysNo(int productManagerGroupInfoSysNo);

        /// <summary>
        ///  根据用户编号获取PM组信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        ProductManagerGroupInfo GetPMListByUserSysNo(int userSysNo);

        /// <summary>
        /// 创建PM组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductManagerGroupInfo CreateProductManagerGroupInfo(ProductManagerGroupInfo entity);

        /// <summary>
        /// 修改PM组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ProductManagerGroupInfo UpdateProductManagerGroupInfo(ProductManagerGroupInfo entity);

        /// <summary>
        /// 除本身之外是否存在某个组名
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        bool IsExistPMGroupName(string groupName, int productManagerGroupInfoSysNo);

        /// <summary>
        /// 是否存在某个组名
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        bool IsExistPMGroupName(string groupName);

        /// <summary>
        /// 更新PM隶属于哪个PM组
        /// </summary>
        /// <param name="PMUserSysNo"></param>
        /// <param name="pmGroupSysNo"></param>
        /// <param name="companyCode"></param>
        void UpdatePMMasterGroupSysNo(int PMUserSysNo, int pmGroupSysNo);

        /// <summary>
        /// 清空PM对应的PM组
        /// </summary>
        /// <param name="pmGroupSysNo"></param>
        void ClearPMMasterGroupSysNo(int pmGroupSysNo);

    }
}
