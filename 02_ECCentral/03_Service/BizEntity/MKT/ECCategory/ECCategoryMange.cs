using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    public class ECCategoryMange
    {

        public int? Category1SysNo
        {
            get;
            set;
        }
        public int? Category2SysNo
        {
            get;
            set;
        }
        public ECCCategoryManagerType Type
        {
            get;
            set;
        }

        /// <summary>
        /// 类别ID
        /// </summary>
        public string CategoryID
        {
            get;
            set;
        }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 类别状态
        /// </summary>
        public ECCCategoryManagerStatus? Status { get; set; }

        public int? SysNo
        {
            get;
            set;
        }
    }
}
