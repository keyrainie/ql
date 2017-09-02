using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility.DataAccess.SearchEngine;

namespace ECommerce.Entity.SolrSearch
{
    public class ProductSearchCondition : SearchCondition
    {
        #region Field
        private string withInKeyWord;
        private List<string> nValueList;
        private string barcode;
        private string endecaId;
        private string nFilter;
        #endregion

        #region Property
        /// <summary>
        /// 范围内搜索
        /// </summary>
        public string WithInKeyWord
        {
            get { return withInKeyWord; }
            set { withInKeyWord = value; }
        }

        /// <summary>
        /// N值
        /// </summary>
        public List<string> NValueList
        {
            get { return nValueList; }
            set { nValueList = value; }
        }

        /// <summary>
        /// 商品二维码 
        /// </summary>
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }

        /// <summary>
        /// enid
        /// </summary>
        public string EndecaId
        {
            get { return endecaId; }
            set { endecaId = value; }
        }

        /// <summary>
        /// filter
        /// </summary>
        public string NFilter
        {
            get { return nFilter; }
            set { nFilter = value; }
        }
        #endregion
    }

}
