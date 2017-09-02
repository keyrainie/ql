using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 using ECCentral.BizEntity;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 配送方式-地区（非）信息
    /// </summary>
    public  class ShipTypeAreaUnInfo:IIdentity,ICompany
    {
        /// <summary>
        /// 配送方式-地区(非)ID
        /// </summary>
        /// </summary> 
        public int? SysNo
        {
            get;
            set;

        }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;

        }
        /// <summary>
        ///配送方式编号
        /// </summary>
        public int? ShipTypeSysNo
        {
            get;
            set;
        }
        /// <summary>
        ///配送方式名称
        /// </summary>
        public string ShipTypeName
        {
            get;
            set;
        }
        /// <summary>
        ///地区编号
        /// </summary>
        public int? AreaSysNo
        {
            get;
            set;
        }
        /// <summary>
        ///地区名称
        /// </summary>
        public string AreaName
        {
            get;
            set;
        }
        /// <summary>
        ///地区
        /// </summary>
        public List<int?> AreaSysNoList
        {
            get;
            set;
        }
    }
   /// <summary>
   /// 返回的提示信息
   /// </summary>
    public class ErroDetail
    {
        
        public ErroDetail()
        {
            this.ErroList = new List<ShipTypeAreaUnInfo>();
            this.SucceedList = new List<ShipTypeAreaUnInfo>();
        }
        /// <summary>
        /// 错误信息列表
        /// </summary>
        public List<ShipTypeAreaUnInfo> ErroList
        {
            get;
            set;
        }
        /// <summary>
        /// 正确信息列表
        /// </summary>
        public List<ShipTypeAreaUnInfo> SucceedList
        {
            get;
            set;
        }
    }
}
