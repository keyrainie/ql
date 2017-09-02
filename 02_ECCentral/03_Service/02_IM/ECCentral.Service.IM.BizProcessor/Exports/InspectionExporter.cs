
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using org.in2bits.MyXls;
using System.Data;
using System.Threading;
using System.IO;
using System.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using System.Net;
using System.Drawing;

namespace ECCentral.Service.IM.BizProcessor.Exports
{
    public class InspectionExporter : IFileExport
    {
        protected HSSFWorkbook m_Document;

        public byte[] CreateFile(List<System.Data.DataTable> data, List<List<ColumnData>> columnList, List<TextInfo> textInfoList, out string fileName, string FileTitle)
        {
            int index = 0;

            string path = ConfigurationManager.AppSettings["ExporterTemplateFilePath"];
            if (path == null || path.Trim().Length <= 0) // 没有配置，使用默认路径
            {
                path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Configuration/ExporterTemplates");
            }
            else if (path.IndexOf(':') < 0) // 配置的不是绝对路径
            {
                path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, path.TrimStart('~', '/', '\\'));
            }

            string templateFileName = Path.Combine(path, "IM/Inspection." + Thread.CurrentThread.CurrentCulture.Name + ".xls");
            FileStream file = new FileStream(templateFileName, FileMode.Open, FileAccess.Read); 

            this.m_Document = new HSSFWorkbook(file);
             file.Close();
        
            foreach (DataTable table in data)
            {
                if (table != null && table.Rows != null
                    && table.Rows.Count > MaxDataTableRowCountLimit
                    && ThrowExceptionWhenDataTableRowCountExceedLimit)
                {
                    string msg = string.Format(ResouceManager.GetMessageString("IM.Product", "DataExport_DataTableRowCountExceedLimit"), MaxDataTableRowCountLimit);
                    throw BuildBizException(msg, table.Rows.Count);
                }
                Sheet worksheet = this.m_Document.GetSheetAt(0);
                if (table != null)
                {

                    HSSFPatriarch patriarch = (HSSFPatriarch)(worksheet.CreateDrawingPatriarch());

                    BuildSheet(worksheet, index, table, patriarch);
                    worksheet.ForceFormulaRecalculation = true;
                }
                index++;
            }
            

            fileName = SetFileName(data, textInfoList);
            MemoryStream mStream = new MemoryStream();
            m_Document.Write(mStream);
            byte[] array =  mStream.GetBuffer();
            mStream.Close();
            return array;
        }


        protected virtual void BuildSheet(Sheet worksheet, int sheetIndex, DataTable data, HSSFPatriarch patriarch)
        {

            int currentRowIndex = 5;
            int currentColIndex = 0;


            foreach (DataRow row in data.Rows)
            {
                currentColIndex = 0;

                if (worksheet.GetRow(currentRowIndex) == null)
                {
                    //MyInsertRow((HSSFSheet)worksheet, currentRowIndex, 1, (HSSFRow)(worksheet.GetRow(currentRowIndex)));
                    worksheet.CreateRow(currentRowIndex);
                }
                if (currentRowIndex != 5)
                {
                    SetRowStyle((HSSFSheet)worksheet, currentRowIndex);
                }
                for (currentColIndex = 0; currentColIndex < 34; currentColIndex++)
                {
                    SetCellValue(worksheet, currentRowIndex, currentColIndex, row, patriarch,data);
                }

                    ++currentRowIndex;
            }

        }

        public void SetRowStyle(Sheet sheet, int rowIndex)
        {
            HSSFRow targetRow = null;
            HSSFCell sourceCell = null;
            HSSFCell targetCell = null;
            HSSFRow sourceRow = null;
            sourceRow = (HSSFRow)(sheet.GetRow(rowIndex-1));
            targetRow = (HSSFRow)(sheet.GetRow(rowIndex));

            for (int m = sourceRow.FirstCellNum; m < sourceRow.LastCellNum; m++)
            {
                sourceCell = (HSSFCell)(sourceRow.GetCell(m));
                if (sourceCell == null)
                    continue;

                if (targetRow.GetCell(m) == null)
                {
                    targetRow.CreateCell(m);
                }

                targetCell = (HSSFCell)(targetRow.GetCell(m));

                targetCell.Encoding = sourceCell.Encoding;

                CellStyle cellstyle = sourceCell.CellStyle;
                 if (cellstyle == null)
                 {
                     cellstyle = sheet.Workbook.CreateCellStyle();
                 }
                 cellstyle.CloneStyleFrom(sourceCell.CellStyle);
                 targetCell.CellStyle = cellstyle;
                if (!string.IsNullOrWhiteSpace(sourceCell.CellFormula))
                {
                    targetCell.CellFormula = sourceCell.CellFormula.Replace(rowIndex.ToString(), (rowIndex + 1).ToString());
                }
                if (sourceCell.CellType == CellType.NUMERIC
                    || sourceCell.CellType == CellType.BOOLEAN 
                    || sourceCell.CellType == CellType.STRING )
                {
                    targetCell.SetCellType(sourceCell.CellType);
                }
            }
        }

        public void SetCellValue(Sheet worksheet, int currentRowIndex, int currentColIndex, DataRow row, HSSFPatriarch patriarch,DataTable dt)
        {

            if (worksheet.GetRow(currentRowIndex).GetCell(currentColIndex) == null)
            {
                worksheet.GetRow(currentRowIndex).CreateCell(currentColIndex);
            }
           
            string columnName = string.Empty;
            switch (currentColIndex)
            {
                case 0://UPCCode
                    {
                        columnName = "UPCCode";
                        break;
                    }
                case 1://商品是否需效期
                    {
                        columnName = "NeedValid";
                        break;
                    }
                case 2://是否需黏贴中文标签
                    {
                        columnName = "NeedLabel";
                        break;
                    }
                case 3://编号
                    {
                        columnName = "ProductID";
                        break;
                    }
                case 4://品牌
                    {
                        columnName = "BrandNameCH";
                        break;
                    }
                case 5://商品名称（中文）
                    {
                        columnName = "ProductName";
                        break;
                    }
                case 6://商品名称（英文）
                    {
                        columnName = "ProductName_EN";
                        break;
                    }
                case 7://其他名称
                    {
                        columnName = "ProductOthterName";
                        break;
                    }
                case 8://型号
                    {
                        columnName = "ProductMode";
                        break;
                    }
                case 9://规格
                    {
                        columnName = "Specifications";
                        break;
                    }
                case 10://产地
                    {
                        columnName = "OriginCountryName";
                        break;
                    }
                case 11://功能
                    {
                        columnName = "Functions";
                        break;
                    }
                case 12://用途
                    {
                        columnName = "Purpose";
                        break;
                    }
                case 13://成份
                    {
                        columnName = "Component";
                        break;
                    }
                case 14://出厂日期
                    {
                        columnName = "ManufactureDate";
                        break;
                    }
                case 15://其他备注
                    {
                        columnName = "Note";
                        break;
                    }
                case 16://商品图片（100*100）
                    {
                        columnName = "DefaultImage";
                        break;
                    }
                case 17://商品单位（计税单位）
                    {
                        columnName = "TaxUnit";
                        break;
                    }
                case 18://商品数量
                    {
                        columnName = "TaxQty";
                        break;
                    }
                case 19://商品单价（人民币）
                    {
                        columnName = "CurrentPrice";
                        break;
                    }
                case 20://业务类型
                    {
                        columnName = "BizType";
                        break;
                    }
                case 21://申报关区
                    {
                        columnName = "ApplyDistrict";
                        break;
                    }
                case 22://货号
                    {
                        columnName = "Product_SKUNO";
                        break;
                    }
                case 23://物资序号
                    {
                        columnName = "Supplies_Serial_No";
                        break;
                    }
                case 24://申报单位
                    {
                        columnName = "ApplyUnit";
                        break;
                    }
                case 25://申报数量
                    {
                        columnName = "ApplyQty";
                        break;
                    }
                case 26://毛重KG
                    {
                        columnName = "GrossWeight";
                        break;
                    }
                case 27://净重KG
                    {
                        columnName = "SuttleWeight";
                        break;
                    }
                case 28://进口检疫审批许可证确认（自贸）
                    {
                        columnName = "Remark1";
                        break;
                    }
                case 29://输出国或地区官方出具检疫证书确认（自贸）
                    {
                        columnName = "Remark2";
                        break;
                    }
                case 30://原产地证明确认（自贸）
                    {
                        columnName = "Remark3";
                        break;
                    }
                case 31://品牌方授权确认（直邮）
                    {
                        columnName = "Remark4";
                        break;
                    }
                case 32://税则号
                    {
                        columnName = "TariffCode";
                        break;
                    }
                case 33://税率
                    {
                        columnName = "";
                        break;
                    }
                case 34://完税价格
                    {
                        columnName = "";
                        break;
                    }
                case 35://预估关税
                    {
                        columnName = "";
                        break;
                    }
                case 36://商品含税价
                    {
                        columnName = "";
                        break;
                    }
                case 37:
                    {
                        columnName = "";
                        break;
                    }
            }
            string dataValue = string.Empty;
            if (!string.IsNullOrWhiteSpace(columnName))
            {
                if (dt.Columns.Contains(columnName))
                {
                    dataValue = GetDataValue(row, columnName);
                }
                
            }
            if (string.IsNullOrWhiteSpace(dataValue))
            {
                if (currentColIndex != 32)
                {

                    dataValue = "-";
                }
            }
            else
            {
                switch (currentColIndex)
                {
                    case 1:
                        {
                            if (dataValue == "1")
                            {
                                dataValue = "是";
                            }
                            else if (dataValue == "0")
                            {
                                dataValue = "否";
                            }
                            break;
                        }
                    case 2:
                        {
                            if (dataValue == "1")
                            {
                                dataValue = "是";
                            }
                            else if (dataValue == "0")
                            {
                                dataValue = "否";
                            }
                            break;
                        }
                    case 14:
                        {
                            DateTime dateManufactureDate;
                            if (DateTime.TryParse(dataValue, out dateManufactureDate))
                            {
                                dataValue = dateManufactureDate.ToShortDateString();
                            }
                            break;
                        }
                    case 20:
                        {
                            if (dataValue == "0")
                            {
                                dataValue = "一般进口";
                            }
                            if (dataValue == "1")
                            {
                                dataValue = "保税进口";
                            }
                            break;
                        }
                }
            }

            if (currentColIndex == 16 && (!string.IsNullOrWhiteSpace(dataValue)))
            {
                string fileName = GetSaveFilePath(dataValue,"P100");
                if (File.Exists(fileName))
                {
                    if (worksheet.GetRow(currentRowIndex).HeightInPoints < 100)
                    {
                        worksheet.GetRow(currentRowIndex).HeightInPoints = 100;
                    }
                    worksheet.SetColumnWidth(currentColIndex, 16 * 256);

                    Image image = Image.FromFile(fileName);
                    Image ihumbnailImage = image.GetThumbnailImage(100, 100, null, System.IntPtr.Zero);
                    MemoryStream stream = new MemoryStream();
                    ihumbnailImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                    byte[] bytes = stream.GetBuffer();
                    stream.Close();
                    image.Dispose();
                    ihumbnailImage.Dispose();
                    int pictureIdx = worksheet.Workbook.AddPicture(bytes, PictureType.JPEG);

                   // HSSFClientAnchor anchor = new HSSFClientAnchor(190, 30, 1054, 228, currentColIndex, currentRowIndex, currentColIndex, currentRowIndex);

                    HSSFClientAnchor anchor = new HSSFClientAnchor(100, 30, 900, 222, currentColIndex, currentRowIndex, currentColIndex, currentRowIndex);
                    anchor.AnchorType = 2;
                    //patriarch.SetCoordinates(50, 50, 100, 100);
                    HSSFPicture pict = (HSSFPicture)patriarch.CreatePicture(anchor, pictureIdx);
                    
                    //pict.Resize(1.0);
                }
            }
            else if (currentColIndex < 33)
            {
                worksheet.GetRow(currentRowIndex).GetCell(currentColIndex).SetCellValue(dataValue); 
            }
        }

        public string GetDataValue(DataRow row, string columnName)
        {
            if (row[columnName] == null || row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }
            else
            {
                return row[columnName].ToString();
            }
        }

        /// <summary>
        /// 保存文件地址
        /// </summary>
        private string[] LocalPathList = AppSettingManager.GetSetting("IM", "ProductImage_UploadFilePath").Split(Char.Parse("|"));

        private string FileGroup = AppSettingManager.GetSetting("IM", "ProductImage_DFISFileGroup");

        private  string GetSaveFilePath(string fileName, string midFilepath)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(FileGroup)
               || string.IsNullOrWhiteSpace(midFilepath)) return "";
            if (LocalPathList == null || LocalPathList.Count() == 0) return "";
            var diretory = FilePathHelp.GetSubFolderName(fileName).Replace("/", "\\");
            var saveDirectory = String.Format(@"{0}{1}\{2}\{3}", LocalPathList[0], FileGroup, midFilepath, diretory);

            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            var filePath = saveDirectory + fileName;
            return filePath;
        }

        protected virtual string SetFileName(List<DataTable> data, List<TextInfo> textInfoList)
        {
            if (textInfoList == null || textInfoList.Count <= 0)
            {
                return DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_ffff") + ".xls";
            }
            string t = null;
            foreach (var text in textInfoList)
            {
                if (text.Title != null && text.Title.Trim().Length > 0)
                {
                    t = text.Title.Trim();
                    break;
                }
            }
            if (t == null)
            {
                return DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_ffff") + ".xls";
            }
            return t + DateTime.Now.ToString("_yyyy-MM-dd_HH_mm_ss_ffff") + ".xls";
        }


        protected virtual int MaxDataTableRowCountLimit
        {
            get
            {
                return 10000;
            }
        }

        protected virtual bool ThrowExceptionWhenDataTableRowCountExceedLimit
        {
            get
            {
                return true;
            }
        }

        protected virtual Exception BuildBizException(string msg, int queryResultRowCount)
        {
            Type type = Type.GetType("ECCentral.BizEntity.BizException, ECCentral.BizEntity");
            return (Exception)Activator.CreateInstance(type, msg);
        }
    }
}
