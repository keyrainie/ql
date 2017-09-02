using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExcelToCategoryForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = this.openFileDialog1.FileName;

                DataTable dt = FileSvr.GetExcelDatatable(this.textBox1.Text, this.openFileDialog1.SafeFileName);
                List<CategoryEntity> list = new List<CategoryEntity>();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CategoryEntity ca = new CategoryEntity();
                        ca.Category1 = dr[0].ToString().Trim();
                        ca.Category2 = dr[1].ToString().Trim();
                        ca.Category3 = dr[2].ToString().Trim();
                        list.Add(ca);
                    }
                }
                foreach (var item in list)
                {
                    if (!string.IsNullOrEmpty(item.Category1) && !string.IsNullOrEmpty(item.Category2) && !string.IsNullOrEmpty(item.Category3))
                    {
                        EC_Category ecca1 = ExcelToCategoryFormsDA.VCategory1(item.Category1);
                        if (ecca1 == null)
                        {
                            CategoryEntity entity1 = ExcelToCategoryFormsDA.AddCategory1(item);
                            CategoryEntity entity2 = ExcelToCategoryFormsDA.AddCategory2(entity1);
                            CategoryEntity entity3 = ExcelToCategoryFormsDA.AddCategory3(entity2);
                        }
                        else
                        {
                            EC_Category ecca2 = ExcelToCategoryFormsDA.VCategory2(item.Category2);
                            if (ecca2 == null)
                            {
                                EC_Category EC_CategoryRelationSysNo = ExcelToCategoryFormsDA.GetEC_CategoryRelationSysNo(item.Category1);
                                item.ParentCategorySysNo = ecca1.SysNo;
                                item.ParentSysNo = EC_CategoryRelationSysNo.SysNo;
                                CategoryEntity entity2 = ExcelToCategoryFormsDA.AddCategory2(item);
                                CategoryEntity entity3 = ExcelToCategoryFormsDA.AddCategory3(entity2);
                            }
                            else 
                            {
                                EC_Category ecca3 = ExcelToCategoryFormsDA.VCategory3(item.Category3);
                                if (ecca3 == null)
                                {
                                    EC_Category EC_CategoryRelationSysNo = ExcelToCategoryFormsDA.GetEC_CategoryRelationSysNo(item.Category2);
                                    item.ParentCategorySysNo = ecca2.SysNo;
                                    item.ParentSysNo = EC_CategoryRelationSysNo.SysNo;
                                    CategoryEntity entity3 = ExcelToCategoryFormsDA.AddCategory3(item);
                                }
                            }
                        }
                    }
                }
                this.label2.Text = "上传完成，谢谢使用";
            }  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<EC_Category> list = ExcelToCategoryFormsDA.GetEC_CategorySysNoList();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    List<EC_Category> TopSysNolist = ExcelToCategoryFormsDA.GetEC_CategoryTopSysNo(item.SysNo);
                    List<EC_Category> BottomSysNolist = ExcelToCategoryFormsDA.GetEC_CategoryBottomSysNoList(139510);
                    string Bottom = null;
                    if (BottomSysNolist.Count > 0)
                    {
                        foreach (var SysNo in BottomSysNolist)
                        {
                            Bottom += SysNo.SysNo.ToString() + ',';
                        }
                    }
                    Bottom = Bottom.Trim().TrimEnd(',');
                    //ExcelToCategoryFormsDA.UpdateEC_CategoryRelationTopAndBottom(TopSysNolist.Count > 0 ? TopSysNolist[0].SysNo.ToString() : null, Bottom, item.SysNo);
                }
            }
            this.label2.Text = "更新完成，谢谢使用";
        }

        #region Excel批量导入
        public static class FileSvr
        {
            /// <summary>
            /// Excel第一个sheet数据导入Datable
            /// </summary>
            /// <param name="fileUrl">Excel文件绝对路径</param>
            /// <param name="table">Excel名称</param>
            /// <returns></returns>
            public static System.Data.DataTable GetExcelDatatable(string fileUrl, string excelName)
            {
                //office2007之前 仅支持.xls
                //const string cmdText = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1';";
                //支持.xls和.xlsx，即包括office2010等版本的   HDR=Yes代表第一行是标题，不是数据；
                const string cmdText = "Provider=Microsoft.Ace.OleDb.12.0;Data Source={0};Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";

                System.Data.DataTable dt = null;
                //建立连接
                OleDbConnection conn = new OleDbConnection(string.Format(cmdText, fileUrl));
                try
                {
                    //打开连接
                    if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }


                    System.Data.DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    //获取Excel的第一个Sheet名称
                    string sheetName = schemaTable.Rows[0]["TABLE_NAME"].ToString().Trim();

                    //查询sheet中的数据
                    string strSql = "select * from [" + sheetName + "]";
                    OleDbDataAdapter da = new OleDbDataAdapter(strSql, conn);
                    DataSet ds = new DataSet();
                    da.Fill(ds, excelName);
                    dt = ds.Tables[0];

                    return dt;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }

            }
        }
        #endregion
    }
}
