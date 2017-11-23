using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nesoft.Utility.DataAccess;

namespace ExcelToCategoryForms
{
    public class ExcelToCategoryFormsDA
    {
        public static CategoryEntity AddCategory1(CategoryEntity entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertAddCategory1");
            dc.SetParameterValue("@CategoryName", entity.Category1);
            dc.ExecuteNonQuery();
            entity.ParentCategorySysNo = 0;
            entity.ParentSysNo = 0;
            entity.ParentCategorySysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            entity.ParentSysNo = Convert.ToInt32(dc.GetParameterValue("@EC_CategoryRelationSysNo"));

            return entity;
        }
        public static CategoryEntity AddCategory2(CategoryEntity entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertAddCategory2");
            dc.SetParameterValue("@CategoryName", entity.Category2);
            dc.SetParameterValue("@ParentCategorySysNo", entity.ParentCategorySysNo);
            dc.SetParameterValue("@ParentSysNo", entity.ParentSysNo);
            dc.ExecuteNonQuery();
            entity.ParentCategorySysNo = 0;
            entity.ParentSysNo = 0;
            entity.ParentCategorySysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));
            entity.ParentSysNo = Convert.ToInt32(dc.GetParameterValue("@EC_CategoryRelationSysNo"));
            return entity;
        }
        public static CategoryEntity AddCategory3(CategoryEntity entity)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("InsertAddCategory3");
            dc.SetParameterValue("@CategoryName", entity.Category3);
            dc.SetParameterValue("@ParentCategorySysNo", entity.ParentCategorySysNo);
            dc.SetParameterValue("@ParentSysNo", entity.ParentSysNo);
            dc.ExecuteNonQuery();
            entity.ParentCategorySysNo = 0;
            entity.ParentCategorySysNo = Convert.ToInt32(dc.GetParameterValue("@SysNo"));

            return entity;
        }

        public static EC_Category VCategory1(string Name)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("VCategory1");
            dc.SetParameterValue("@CategoryName", Name);

            return dc.ExecuteEntity<EC_Category>();
        }
        public static EC_Category VCategory2(string Name)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("VCategory2");
            dc.SetParameterValue("@CategoryName", Name);

            return dc.ExecuteEntity<EC_Category>();
        }
        public static EC_Category VCategory3(string Name)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("VCategory3");
            dc.SetParameterValue("@CategoryName", Name);

            return dc.ExecuteEntity<EC_Category>();
        }
        public static EC_Category GetEC_CategoryRelationSysNo(string Name)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetEC_CategoryRelationSysNo");
            dc.SetParameterValue("@CategoryName", Name);

            return dc.ExecuteEntity<EC_Category>();
        }


        public static List<EC_Category> GetEC_CategorySysNoList()
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetEC_CategorySysNoList");
            return dc.ExecuteEntityList<EC_Category>();
        }
        public static List<EC_Category> GetEC_CategoryTopSysNo(int SysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetEC_CategoryTopSysNo");
            dc.SetParameterValue("@EC_CategorySysNo", SysNo);

            return dc.ExecuteEntityList<EC_Category>();
        }
        public static List<EC_Category> GetEC_CategoryBottomSysNoList(int SysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("GetEC_CategoryBottomSysNoList");
            dc.SetParameterValue("@EC_CategorySysNo", SysNo);

            return dc.ExecuteEntityList<EC_Category>();
        }
        public static void UpdateEC_CategoryRelationTopAndBottom(string TopCategorySysno, string BottomCategories, int EC_CategorySysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdateEC_CategoryRelationTopAndBottom");
            dc.SetParameterValue("@TopCategorySysno", TopCategorySysno);
            dc.SetParameterValue("@BottomCategories", BottomCategories);
            dc.SetParameterValue("@EC_CategorySysNo", EC_CategorySysNo);
            dc.ExecuteNonQuery();
        }

    }


    public class CategoryEntity
    {
        public string Category1 { get; set; }

        public string Category2 { get; set; }

        public string Category3 { get; set; }

        public int ParentSysNo { get; set; }

        public int ParentCategorySysNo { get; set; }
    }

    public class EC_Category
    {

        public int SysNo { get; set; }
    }
}
