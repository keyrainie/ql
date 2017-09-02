using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;


namespace ECommerce.Entity.Category
{
	/// <summary>
	/// Category information
	/// </summary>
	[Serializable]
	[DataContract]
    public class CategoryInfo : EntityBase
	{
		#region [ Fields ]

		private int id;
		private string name;
		private int sysNo;
		private int parentSysNo;
		private string promotionStatus;
		private CategoryType type;
		private List<CategoryInfo> subCategories;
		private string smartSortStatus;
		private string extensionName;
        private int productCategorySysno;
        private string bottomCategories;
        private int topCategorySysno;
		private string isParentCategoryShow;
		private int priority;

		#endregion

		#region [ Properties ]

		/// <summary>
		/// Gets or sets category's id
		/// </summary>
		[DataMember]
        public int CategoryID
		{
			get { return this.id; }
			set { this.id = value; }
		}

		/// <summary>
		/// Gets or sets category's name
		/// </summary>
		[DataMember]
        public string CategoryName
		{
			get { return this.name; }
			set { this.name = value; }
		}

		/// <summary>
		/// Gets or sets current sub node number
		/// </summary>
		[DataMember]
        public int CurrentSysNo
		{
			get { return this.sysNo; }
			set { this.sysNo = value; }
		}

		/// <summary>
		/// Gets or sets promotionStatus
		/// </summary>
		[DataMember]
		public string PromotionStatus
		{
			get { return this.promotionStatus; }
			set { this.promotionStatus = value; }
		}

		/// <summary>
		/// Gets or sets category's parent id
		/// </summary>
		[DataMember]
        public int ParentSysNo
		{
			get { return this.parentSysNo; }
			set { this.parentSysNo = value; }
		}

		/// <summary>
		/// Gets or sets sub category list
		/// </summary>
		[DataMember]
		public List<CategoryInfo> SubCategories
		{
			get { return this.subCategories; }
			set { this.subCategories = value; }
		}

		/// <summary>
		/// Gets or sets category type
		/// </summary>
		[DataMember]
        public CategoryType CategoryType
		{
			get { return this.type; }
			set { this.type = value; }
		}

		/// <summary>
		/// Gets or sets smartsort status
		/// </summary>
		[DataMember]
        public string SmartSortStatus
		{
			get { return this.smartSortStatus; }
			set { this.smartSortStatus = value; }
		}

		/// <summary>
		/// Gets or sets category's name
		/// </summary>
		[DataMember]
        public string ExtensionName
		{
			get { return this.extensionName; }
			set { this.extensionName = value; }
		}

        /// <summary>
        /// Gets or sets Product's C3Sysno
        /// </summary>
        [DataMember]
        public int C3Sysno
        {
            get { return productCategorySysno; }
            set { productCategorySysno = value; }
        }


        /// <summary>
        /// Gets or sets BottomCategories
        /// </summary>
        [DataMember]
        public string BottomCategories
        {
            get { return bottomCategories; }
            set { bottomCategories = value; }
        }

        /// <summary>
        /// Gets or sets TopCategorySysno
        /// </summary>
        [DataMember]
        public int TopCategorySysno
        {
            get { return topCategorySysno; }
            set { topCategorySysno = value; }
        }

		/// <summary>
		/// Gets or sets IsParentCategoryShow
        /// </summary>
        [DataMember]
		public string IsParentCategoryShow
        {
			get { return isParentCategoryShow; }
			set { isParentCategoryShow = value; }
        }

		/// <summary>
		/// ≈≈–Ú
		/// </summary>
		[DataMember]
		public int Priority
		{
			get { return priority; }
			set { priority = value; }
		}
		#endregion
	}
}
