using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ECCentral.BizEntity.MKT
{
    public class ECCCategoryManagerModel
    {
        private int? categorySysNo;
        public int? CategorySysNo
        {
            get;
            set;
        }
        private int? category1SysNo;
        public int? Category1SysNo
        {
            get;
            set;
        }
        private int? category2SysNo;
        public int? Category2SysNo
        {
            get;
            set;
        }
        private ECCCategoryManagerType type = ECCCategoryManagerType.ECCCategoryType1;
        public ECCCategoryManagerType Type
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
    }
}
