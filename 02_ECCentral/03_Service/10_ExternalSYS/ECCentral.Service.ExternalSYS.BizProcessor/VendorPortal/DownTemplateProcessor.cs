using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ECCentral.BizEntity.IM;
using ECCentral.Service.Utility;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace ECCentral.Service.ExternalSYS.BizProcessor.VendorPortal
{
    [VersionExport(typeof(DownTemplateProcessor))]
    public class DownTemplateProcessor
    {
        #region Member
        
        DataFeedFile m_dataFeed = new DataFeedFile();

        string m_companyCode;

        #endregion

        #region Method
        
        public string Download(int c3SysNo,string companyCode)
        {
            m_companyCode = companyCode;

            string c3Name = ExternalDomainBroker.GetCategory3Info(c3SysNo).CategoryName.Content;
            string fileName = BulidResponseFileName(c3Name);
            //生成模版
            DataFeedTemplate template = BulidTemplate(c3SysNo, c3Name);
            //输出模版
            return GetTemplateUrl(template, fileName);
        }

        string BulidResponseFileName(string c3name)
        {
            string fileName = string.Empty;
            fileName = string.Format("{0}_批量新品创建_{1}.xls"
                , c3name
                , DateTime.Now.ToString("yyyyMMdd_hh_mm_ss"));

            return fileName.Replace(@"/","_").Replace(@"\","_");
        }

        DataFeedTemplate BulidTemplate(int c3SysNo, string c3Name)
        {
            string configPath = Path.Combine(
                                            AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                                            , AppSettingHelper.Get("DataFeedFilesConfigPath")
                                        );
            m_dataFeed = SerializationUtility.LoadFromXml<DataFeedFileMapping>(configPath).Mappings[0];
            DataFeedTemplate template = new DataFeedTemplate();
            template.C3Name = c3Name;
            template.C3SysNo = c3SysNo;
            template.C3RowIndex = int.Parse(m_dataFeed.C3RowIndex);
            //填充列分组信息
            template.HeaderGroups = new List<HeaderGroup>();
            foreach (var item in m_dataFeed.HeaderGroups)
            {
                template.HeaderGroups.Add(item);
            }
            template.BasicColumns = BuildBasicColumns(c3SysNo);
            var list = ExternalDomainBroker.GetCategoryPropertyByCategorySysNo(c3SysNo);

            if (list != null)
            {
                template.Properties = new List<DataFeedColumn>();
                var activeList = list.Where(p => p.Property.Status == PropertyStatus.Active);
                if (activeList.Count() > 0)
                {
                    var propertySysNoList = activeList.Select(p => p.Property.SysNo.Value).ToList();
                    var propertyValues = ExternalDomainBroker.GetPropertyValueInfoByPropertySysNoList(propertySysNoList);
                    foreach (var item in activeList)
                    {
                        DataFeedColumn column = new DataFeedColumn();
                        column.Number = item.Property.SysNo.Value;
                        // item.PropertyType == "D"? 没有这个属性？
                        if (item.PropertyType == PropertyType.Grouping)
                        {
                            column.Name = string.Format("{0}_列表", item.Property.PropertyName.Content);
                        }
                        else
                        {
                            column.Name = item.Property.PropertyName.Content;
                        }
                        column.Type = DataType.LIST;
                        column.Width = column.Name.Length;

                        if (item.IsMustInput == CategoryPropertyStatus.Yes)
                        {
                            column.IsMustInput = true;
                        }
                        else
                        {
                            column.IsMustInput = false;
                        }

                        if (item.IsInAdvSearch == CategoryPropertyStatus.Yes)
                        {
                            column.IsInAdvSearch = true;
                        }

                        column.List = new List<string>();
                        var propertyList = propertyValues.FirstOrDefault(p=>p.Key == item.Property.SysNo.Value).Value;
                        if (propertyList != null)
                        {
                            foreach (var value in propertyList.Where(p => p.PropertyInfo.Status == PropertyStatus.Active))
                            {
                                column.List.Add(value.ValueDescription.Content);
                            }
                        }

                        template.Properties.Add(column);

                        //分组属性需要多加一列
                        // item.PropertyType == "D"? 没有这个属性？
                        if (item.PropertyType == PropertyType.Grouping)
                        {
                            column = new DataFeedColumn();
                            column.Name = string.Format("{0}_自定义", item.Property.PropertyName.Content);
                            column.Width = column.Name.Length;
                            column.Type = DataType.TEXT_LENGTH;

                            template.Properties.Add(column);

                        }

                        //HACK:Bob.H.Li 如果Item.Type==G，那么添加一列“是否分组属性”
                        if (item.PropertyType == PropertyType.Grouping)
                        {
                            column = new DataFeedColumn();

                            column.Number = template.Properties.Count;

                            column.Name = DataFeedConstString.EXCEL_ADDSELECTGROUPSTRING;
                            column.Width = column.Name.Length;
                            column.Type = DataType.LIST;

                            column.List = new List<string>() { 
                         GroupPropertyType.OnlyGroupText,
                         GroupPropertyType.OnlyGroupPicture ,
                         GroupPropertyType.GroupAggregationText,
                         GroupPropertyType.GroupAggregationPicture
                        };
                            template.Properties.Add(column);
                        }
                    }
                }
            }

            return template;
        }

        string GetTemplateUrl(DataFeedTemplate template, string fileName)
        {
            string path = Path.Combine(
                                        AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                                        , AppSettingHelper.Get("DataFeedFilesTemplatePath")
                                    );
            template.LoadWorkbook(path);
            template.InitWorkbook();

            byte[] buf = template.GetBytes();

            return FileUploadManager.UploadFileAndGetUrl(fileName, buf);
        }

        List<DataFeedColumn> BuildBasicColumns(int c3SysNo)
        {
            List<DataFeedColumn> list = new List<DataFeedColumn>();
            if (m_dataFeed != null)
            {
                int number = 1;
                foreach (var header in m_dataFeed.Headers)
                {
                    DataFeedColumn column = new DataFeedColumn();
                    column.Number = number;
                    column.Name = header.Name;
                    column.Width = header.Width;
                    column.IsMustInput = header.IsMustInput;
                    column.Type = DataType.ANY;
                    column.HeaderGroupIndex = header.HeaderGroupIndex;
                    if (header.Type == "List")
                    {
                        column.Type = DataType.LIST;
                        column.List = new List<string>();

                        if (header.Name == "生产商")
                        {
                            List<ManufacturerInfo> manufactures = ExternalDomainBroker.GetManufacturerList(m_companyCode);
                            foreach (var item in manufactures)
                            {
                                column.List.Add(!string.IsNullOrEmpty(item.ManufacturerNameLocal.Content)
                                                ? item.ManufacturerNameLocal.Content
                                                : item.ManufacturerNameGlobal);
                            }
                        }
                        else if (header.Name == "品牌")
                        {
                            var brands = ExternalDomainBroker.GetBrandList(m_companyCode);
                            var brandsList = brands.Select(p => !string.IsNullOrEmpty(p.BrandNameLocal.Content)
                                                ? p.BrandNameLocal.Content
                                                : p.BrandNameGlobal);
                            column.List.AddRange(brandsList);
                        }
                        else if (header.Name == "PM")
                        {
                            var pmList = ExternalDomainBroker.GetAllValidPMList(m_companyCode);
                            foreach (var item in pmList)
                            {
                                column.List.Add(item.UserInfo.UserName);
                            }
                        }

                        else if (header.Name == "版本")
                        {
                            column.List.Add("零售版");
                            column.List.Add("OEM版");
                        }
                        else if (header.Name == "航空禁运")
                        {
                            column.List.Add("是");
                            column.List.Add("否");
                        }
                        //是否拍照
                        else if (header.Name == DataFeedConstString.EXCEL_COLUMNNAME_ITEM_HASPHOTO)
                        {
                            column.List.Add("是");
                            column.List.Add("否");
                        }
                        //是否代销
                        else if (header.Name == DataFeedConstString.EXCEL_COLUMNNAME_ITEM_SALEBYPROXY)
                        {
                            column.List.Add("代销");
                            column.List.Add("非代销");
                            column.List.Add("临时代销");
                        }
                        //是否虚库
                        else if (header.Name == DataFeedConstString.EXCEL_COLUMNNAME_ITEM_ISVIRTUALINVENTORY)
                        {
                            column.List.Add("是");
                            column.List.Add("否");
                        }

                        //商品类型
                        else if (header.Name == DataFeedConstString.EXCEL_COLUMNNAME_ITEM_PRODUCTTYPE)
                        {
                            column.List.Add("正常品");
                            column.List.Add("二手品");
                            column.List.Add("坏品");
                        }
                    }
                    list.Add(column);

                    number++;
                }
            }
            return list;
        }

        #endregion
    }

    public interface IKeyedObject
    {
        string Key { get; }
    }

    public abstract class ExporterBase<T>
    {
        protected HSSFWorkbook WorkBook;
        protected Sheet worksheet;

        public ExporterBase()
        {
            WorkBook = new HSSFWorkbook();

        }
        public void Create(string worksheetName, T dataList)
        {
            worksheet = WorkBook.CreateSheet(worksheetName);
            this.InitSheet(worksheet, dataList);
        }

        public void InitSheet(Sheet worksheet, T data)
        {
            this.SetDataHeader(worksheet, data);
            this.SetExcelColumns(worksheet, data);
        }

        public void LoadWorkbook(string path)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            WorkBook = new HSSFWorkbook(file);
        }

        //设置列宽
        protected abstract void SetExcelColumns(Sheet worksheet, T data);

        //设置列头
        protected abstract void SetDataHeader(Sheet worksheet, T data);


        public void WriteToFile(string filaName)
        {
            //Write the stream data of workbook to the root directory
            FileStream file = new FileStream(filaName, FileMode.Create);
            WorkBook.Write(file);
            file.Close();
        }

        public Byte[] GetBytes()
        {
            //WorkBook.GetBytes()有问题   
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                this.WorkBook.Write(ms);
                buffer = ms.GetBuffer();
            }
            return buffer;
        }

        protected void SetExcelColumns(Sheet worksheet, int width, params ushort[] colIndexs)
        {
            for (int i = 0; i < colIndexs.Length; i++)
            {
                worksheet.SetColumnWidth(colIndexs[i], width);
            }
        }
    }

    public class DataFeedTemplate : ExporterBase<DataFeedTemplate>
    {
        private const string DETAILSHEET = "商品详细信息_属性值";
        private const string DATASHEET = "资料提供";
        private const string ITEMDECLARE = "商品基本信息_定义";

        /// <summary>
        /// 标题列的行号
        /// </summary>
        private const int HeaderNameRowIndex = 2;

        /// <summary>
        /// 必填标识列的行号
        /// </summary>
        private const int MustInputSymbolRowIndex = 0;

        /// <summary>
        /// 和并列的行号
        /// </summary>
        private const int MergeHeaderRowIndex = 1;

        //基本数据列
        public List<DataFeedColumn> BasicColumns { get; set; }

        //基本属性1、基本属性2、... ...、
        public List<DataFeedColumn> Properties { get; set; }

        public string C3Name { get; set; }
        public int C3SysNo { get; set; }

        //基本列定义中的C3所在的行号(比excel 的行号少1)
        public int C3RowIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 列分组信息
        /// </summary>
        public List<HeaderGroup> HeaderGroups
        {
            get;
            set;
        }

        /// <summary>
        /// 必填标志
        /// </summary>
        private const string MustInputSymbol = "●";
        /// <summary>
        /// 高级搜素标志
        /// </summary>
        private const string AdvSearchSymbol = "●";

        /// <summary>
        /// 不需要填写标志
        /// </summary>
        private const string NotInputSymbol = "○";

        private const string SystemFillSymbol = "系统回填";

        private const string HeaderGroupName = "特有属性";

        //private const string NotInputHeaderName_TakePhoto = "代销属性";

        private const string NotInputHeaderName_Item = "#Item";

        private const short NotInputCellColor = 36;

        protected override void SetDataHeader(Sheet worksheet, DataFeedTemplate data)
        {
            int colindex = 0;
            Row headerNameRow = worksheet.CreateRow(HeaderNameRowIndex);
            headerNameRow.HeightInPoints = 30;
            Row headerSymbolRow = worksheet.CreateRow(MustInputSymbolRowIndex);
            //headerSymbolRow.HeightInPoints = 30;
            Row headerMergeRow = worksheet.CreateRow(MergeHeaderRowIndex);
            headerMergeRow.HeightInPoints = 30;

            //int mustInputColIndex = 0;

            //分组字典
            Dictionary<string, List<int>> dicGroupList = new Dictionary<string, List<int>>();

            foreach (var item in data.HeaderGroups)
            {
                dicGroupList.Add(item.Key, new List<int>());
            }
            //基本列
            foreach (var item in data.BasicColumns)
            {
                HeaderGroup currentHeaderGroup = data.HeaderGroups.SingleOrDefault(p => p.Index == item.HeaderGroupIndex);

                #region[处理列必填标志]
                Cell cellSymbol = headerSymbolRow.CreateCell(colindex);
                float cellHeight = cellSymbol.Row.HeightInPoints;
                if (item.IsMustInput)
                {

                    cellSymbol.SetCellValue(MustInputSymbol);
                    //mustInputColIndex++;

                }

                CellStyle style = cellSymbol.GetCommonCellStyle();
                Font font = cellSymbol.Sheet.Workbook.CreateFont();
                font.FontHeightInPoints = 18;
                font.Boldweight = (short)FontBoldWeight.BOLD;
                font.Color = HSSFColor.RED.index;

                style.BorderLeft = CellBorderType.THIN;
                style.BorderRight = CellBorderType.THIN;

                if (item.Name == NotInputHeaderName_Item)
                {
                    cellSymbol.SetCellValue(SystemFillSymbol);
                    font.FontHeightInPoints = 10;
                    font.Color = HSSFColor.BLACK.index;
                }
                style.SetFont(font);
                cellSymbol.CellStyle = style;
                cellSymbol.Row.HeightInPoints = cellHeight;

                #endregion

                #region [写列名称]

                Cell cellHeaderName = headerNameRow.CreateCell(colindex);
                cellHeaderName.SetCellType(CellType.STRING);
                style = cellHeaderName.GetCommonCellStyle();
                style.FillPattern = FillPatternType.SOLID_FOREGROUND;
                style.FillForegroundColor = short.Parse(currentHeaderGroup.Color);
                if (item.Name == NotInputHeaderName_Item)
                {
                    Font fontHeaderName = cellHeaderName.Sheet.Workbook.CreateFont();
                    style.FillForegroundColor = NotInputCellColor;
                    fontHeaderName.Color = HSSFColor.WHITE.index;
                    fontHeaderName.Boldweight = (short)FontBoldWeight.BOLD;

                    style.SetFont(fontHeaderName);
                }
                style.BorderLeft = CellBorderType.THIN;
                style.BorderRight = CellBorderType.THIN;
                cellHeaderName.CellStyle = style;
                cellHeaderName.SetCellValue(item.Name);
                #endregion

                //填充分组字典

                if (currentHeaderGroup != null)
                {
                    dicGroupList[currentHeaderGroup.Key].Add(colindex);
                }
                colindex = colindex + 1;
            }


            //获取颜色
            HeaderGroup currentDynamicHeader = data.HeaderGroups.SingleOrDefault(p => p.Name == HeaderGroupName);
            if (data.Properties != null && data.Properties.Count > 0)
            {

                Font font = headerNameRow.Sheet.Workbook.CreateFont();
                foreach (var item in data.Properties)
                {
                    Cell cellHeaderNameDynamic = headerNameRow.CreateCell(colindex);
                    Cell cellSymbol = headerSymbolRow.CreateCell(colindex);
                    cellHeaderNameDynamic.SetCellValue(item.Name);
                    float cellHeight = cellSymbol.Row.HeightInPoints;

                    //if (item.IsMustInput)
                    //{
                    //    font.Color = HSSFColor.RED.index;
                    //    cellSymbol.SetCellValue(MustInputSymbol);
                    //}

                    if (item.IsInAdvSearch)
                    {
                        cellSymbol.SetCellValue(AdvSearchSymbol);
                    } 

                    CellStyle styleHeaderNameDynamic = cellHeaderNameDynamic.GetCommonCellStyle();
                    CellStyle styleSymbol = cellSymbol.GetCommonCellStyle();

                    font.FontHeightInPoints = 18;
                    font.Boldweight = (short)FontBoldWeight.BOLD;
                    font.Color = HSSFColor.GREEN.index;


                    styleSymbol.SetFont(font);
                    cellSymbol.Row.HeightInPoints = cellHeight;

                    styleHeaderNameDynamic.BorderLeft = CellBorderType.THIN;
                    styleHeaderNameDynamic.BorderTop = CellBorderType.THIN;
                    styleHeaderNameDynamic.BorderRight = CellBorderType.THIN;

                    styleSymbol.BorderLeft = CellBorderType.THIN;
                    styleSymbol.BorderRight = CellBorderType.THIN;
                    //背景色
                    styleHeaderNameDynamic.FillPattern = FillPatternType.SOLID_FOREGROUND;
                    styleHeaderNameDynamic.FillForegroundColor = short.Parse(currentDynamicHeader.Color);

                    cellHeaderNameDynamic.CellStyle = styleHeaderNameDynamic;
                    cellSymbol.CellStyle = styleSymbol;

                    dicGroupList[HeaderGroupName].Add(colindex);
                    colindex = colindex + 1;
                }
            }

            #region[处理列分类信息]

            foreach (var item in dicGroupList)
            {
                if (item.Value != null && item.Value.Count > 0)
                {
                    CellRangeAddress mergeArea = new CellRangeAddress(MergeHeaderRowIndex, MergeHeaderRowIndex, item.Value.First(), item.Value.Last());
                    worksheet.AddMergedRegion(mergeArea);

                    ((HSSFSheet)worksheet).SetEnclosedBorderOfRegion(mergeArea, CellBorderType.MEDIUM, NPOI.HSSF.Util.HSSFColor.BLACK.index);
                    headerMergeRow.HeightInPoints = 30;
                    Cell currentGroupCell = headerMergeRow.GetCell(item.Value.First());
                    if (currentGroupCell == null)
                    {
                        currentGroupCell = headerMergeRow.CreateCell(item.Value.First());
                    }
                    if (item.Key == HeaderGroupName)
                    {
                        currentGroupCell.SetCellValue(C3Name + item.Key);

                    }
                    else
                    {
                        currentGroupCell.SetCellValue(item.Key);
                    }
                    CellStyle style = currentGroupCell.GetCommonCellStyle();
                    currentGroupCell.CellStyle = style;
                }

            }

            #endregion

        }

        protected override void SetExcelColumns(Sheet worksheet, DataFeedTemplate data)
        {

            ushort colindex = 0;
            ushort colindex2 = 0;
            HSSFSheet sheet = ((HSSFSheet)worksheet);
            CellStyle style = sheet.Workbook.CreateCellStyle();
            style.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");

            for (int i = 0; i < data.BasicColumns.Count; i++)
            {
                var item = data.BasicColumns[i];

                if (item.Type == DataType.LIST)
                {
                    if (item.Type == DataType.LIST && item.List != null && item.List.Count > 0)
                    {
                        CellRangeAddressList regions = new CellRangeAddressList(HeaderNameRowIndex + 1, 65535, colindex2, colindex2);

                        Name range = WorkBook.CreateName();
                        HSSFSheet sheetDetails = (HSSFSheet)WorkBook.GetSheet(DETAILSHEET);

                        string celName = GetExcelColumnIndex(colindex);
                        range.RefersToFormula = DETAILSHEET + "!$" + celName + "$2" + ":$" + celName + "$" + (item.List.Count + 1);
                        range.NameName = string.Format("sheet{0}ranges{1}",
                             worksheet.Workbook.GetSheetIndex(worksheet),
                            item.Number);

                        DVConstraint constraint = DVConstraint.CreateFormulaListConstraint(range.NameName);
                        HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
                        sheet.AddValidationData(dataValidate);

                        sheet.SetDefaultColumnStyle(colindex2, style);
                    }
                    colindex++;
                }
                this.SetExcelColumns(worksheet, item.Width * 600, colindex2);
                colindex2++;
            }

            if (data.Properties != null && data.Properties.Count > 0)
            {
                for (int i = 0; i < data.Properties.Count; i++)
                {
                    var item = data.Properties[i];

                    if (item.Type == DataType.LIST && item.List != null && item.List.Count > 0)
                    {
                        CellRangeAddressList regions = new CellRangeAddressList(HeaderNameRowIndex + 1, 65535, colindex2, colindex2);

                        Name range = WorkBook.CreateName();
                        HSSFSheet sheetDetails = (HSSFSheet)WorkBook.GetSheet(DETAILSHEET);

                        string celName = GetExcelColumnIndex(colindex);
                        range.RefersToFormula = DETAILSHEET + "!$" + celName + "$2" + ":$" + celName + "$" + (item.List.Count + 1);
                        range.NameName = BuildRangeName(
                            item.Number,
                            worksheet.Workbook.GetSheetIndex(worksheet)
                            );

                        DVConstraint constraint = DVConstraint.CreateFormulaListConstraint(range.NameName);
                        HSSFDataValidation dataValidate = new HSSFDataValidation(regions, constraint);
                        sheet.AddValidationData(dataValidate);
                        sheet.SetDefaultColumnStyle(colindex2, style);
                    }
                    this.SetExcelColumns(worksheet, item.Width * 600, colindex2);
                    colindex++;
                    colindex2++;
                }
            }
        }

        public string GetExcelColumnIndex(int celNum)
        {
            int num = celNum + 1;//celNum是从0算起   
            String tem = "";
            while (num > 0)
            {
                int lo = (num - 1) % 26;//取余，A到Z是26进制，   
                tem = (char)(lo + 'A') + tem;
                num = (num - 1) / 26;//取模   
            }
            return tem;
        }

        public string BuildRangeName(int name, int sheetIndex)
        {
            String tem = string.Format("sheet{0}range{1}", name, sheetIndex);
            return tem;
        }

        public void InitWorkbook()
        {
            HSSFSheet sheet = (HSSFSheet)WorkBook.GetSheet(DETAILSHEET);
            this.InitDetailSheet(sheet);

            sheet = (HSSFSheet)WorkBook.GetSheet(DATASHEET);
            this.InitSheet(sheet, this);

            sheet = (HSSFSheet)WorkBook.GetSheet(ITEMDECLARE);
            this.InitExtendInfoSheet(sheet);
        }

        private void InitExtendInfoSheet(HSSFSheet sheet)
        {
            Row row = sheet.GetRow(C3RowIndex);
            Cell cell = row.GetCell(1);
            cell.SetCellValue(this.C3SysNo + "(类别编号)");
        }


        protected void InitDetailSheet(HSSFSheet sheet)
        {
            ushort colindex = 0;
            Row header = sheet.CreateRow(0);

            List<DataFeedColumn> listBasicColums = this.BasicColumns
                .FindAll(item => item.Type == DataType.LIST);

            for (int i = 0; i < listBasicColums.Count; i++)
            {
                DataFeedColumn item = listBasicColums[i];
                header.CreateCell(colindex).SetHeaderCell().SetCellValue(item.Name);
                this.SetExcelColumns(sheet, item.Width * 600, colindex);
                if (item.Type == DataType.LIST && item.List != null)
                {
                    for (int j = 0; j < item.List.Count; j++)
                    {
                        Row row = sheet.GetRow(j + 1);
                        if (row == null)
                        {
                            row = sheet.CreateRow(j + 1);
                        }
                        row.CreateCell(colindex).SetCellValue(item.List[j]);
                    }
                }
                colindex++;
            }
            if (this.Properties != null && this.Properties.Count > 0)
            {
                for (int i = 0; i < this.Properties.Count; i++)
                {
                    DataFeedColumn item = this.Properties[i];
                    header.CreateCell(colindex).SetHeaderCell().SetCellValue(item.Name);
                    this.SetExcelColumns(sheet, item.Width * 600, colindex);
                    if (item.Type == DataType.LIST && item.List != null)
                    {
                        for (int j = 0; j < item.List.Count; j++)
                        {
                            Row row = sheet.GetRow(j + 1);
                            if (row == null)
                            {
                                row = sheet.CreateRow(j + 1);
                            }
                            row.CreateCell(colindex).SetCellValue(item.List[j]);
                        }
                    }
                    colindex++;
                }
            }
        }

    }

    public class DataFeedColumn
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public DataType Type { get; set; }

        public int Width { get; set; }

        public List<string> List { get; set; }

        public bool IsMustInput { get; set; }

        public bool IsInAdvSearch { get; set; }
        /// <summary>
        /// 分组信息
        /// </summary>
        public string HeaderGroupIndex
        {
            get;
            set;
        }
    }

    public enum DataType
    {
        ANY = 0,
        INTEGER = 1,
        DECIMAL = 2,
        LIST = 3,
        DATE = 4,
        TIME = 5,
        TEXT_LENGTH = 6,
        FORMULA = 7,
    }

    /// <summary>
    /// 分组属性类型
    /// </summary>
    public class GroupPropertyType
    {
        public const string OnlyGroupText = "仅分组--文本";
        public const string OnlyGroupPicture = "仅分组--图片";
        public const string GroupAggregationText = "分组+聚合--图片";
        public const string GroupAggregationPicture = "分组+聚合--文本";
    }

    /// <summary>
    ///Datafeed 常量
    /// </summary>
    public class DataFeedConstString
    {
        /// <summary>
        /// 是否拍照
        /// </summary>
        public const string EXCEL_COLUMNNAME_ITEM_HASPHOTO = "是否拍照";

        /// <summary>
        /// 代销属性
        /// </summary>
        public const string EXCEL_COLUMNNAME_ITEM_SALEBYPROXY = "代销属性";


        /// <summary>
        /// 是否虚库
        /// </summary>
        public const string EXCEL_COLUMNNAME_ITEM_ISVIRTUALINVENTORY = "是否虚库";

        /// <summary>
        /// 商品类型
        /// </summary>
        public const string EXCEL_COLUMNNAME_ITEM_PRODUCTTYPE = "商品类型";

        /// <summary>
        /// 是否分组/聚合属性
        /// </summary>
        public const string EXCEL_ADDSELECTGROUPSTRING = "是否分组/聚合属性";
    }

    /// <summary>
    /// 列分组信息
    /// </summary>
    public class HeaderGroup : IKeyedObject
    {
        #region IKeyedObject Members

        public string Key
        {
            get { return Name; }
        }
        #endregion
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Index")]
        public string Index { get; set; }

        [XmlAttribute("Color")]
        public string Color { get; set; }
    }

    [Serializable]
    [XmlRoot("DatafeedFiles", Namespace = "http://IPP/web")]
    public class DataFeedFileMapping
    {
        private KeyedCollection<DataFeedFile> m_Mappings;

        [XmlElement("DatafeedFile")]
        public KeyedCollection<DataFeedFile> Mappings
        {
            get { return m_Mappings; }
            set { m_Mappings = value; }
        }
    }
    [Serializable]
    public class DataFeedFile : IKeyedObject
    {
        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("sourceType")]
        public string SourceType
        {
            get;
            set;
        }

        [XmlAttribute("c3RowIndex")]
        public string C3RowIndex
        {
            get;
            set;
        }

        [XmlArray("headers")]
        [XmlArrayItem("header")]
        public KeyedCollection<DataFeedHeader> Headers
        {
            get;
            set;
        }

        [XmlArray("HeaderGroups")]
        [XmlArrayItem("HeaderGroup")]
        public KeyedCollection<HeaderGroup> HeaderGroups
        {
            get;
            set;
        }

        #region IKeyedObject Members

        public string Key
        {
            get { return Name; }
        }

        #endregion
    }

    public class DataFeedHeader : IKeyedObject
    {
        #region IKeyedObject Members

        public string Key
        {
            get { return Name; }
        }
        #endregion
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("IsMustInput")]
        public bool IsMustInput { get; set; }

        [XmlAttribute("Width")]
        public int Width { get; set; }

        [XmlAttribute("Type")]
        public string Type { get; set; }

        [XmlAttribute("HeaderGroupIndex")]
        public string HeaderGroupIndex { get; set; }
    }

    /// <summary>
    /// 提供集合键嵌入在实现 <see cref="IKeyedObject"/> 集合子项中的<b>Key</b>属性的集合类。 
    /// </summary>
    /// <typeparam name="T">带有集合中的键属性的集合子项。</typeparam>
    public class KeyedCollection<T> : KeyedCollection<string, T>
        where T : IKeyedObject
    {

        /// <summary>
        /// 初始化使用默认相等比较器的 <b>KeyedCollection</b> 类的新实例。 
        /// </summary>
        /// <remarks>字符串键值不区分大小写。</remarks>
        public KeyedCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// 初始化使用指定字符串相等比较器的 <b>KeyedCollection</b> 类的新实例。 
        /// </summary>
        /// <param name="comparer">比较键时要使用的 <see cref="StringComparer"/>。</param>
        public KeyedCollection(StringComparer comparer)
            : base(comparer ?? StringComparer.InvariantCultureIgnoreCase)
        {
        }

        /// <summary>
        /// 从指定元素提取键。
        /// </summary>
        /// <param name="item">从中提取键的元素。</param>
        /// <returns>指定元素的键。</returns>
        protected override string GetKeyForItem(T item)
        {
            return item.Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
            if (this.Contains(item.Key))
                throw new Exception("error");//DuplicateKeyException(item.Key);

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Adds the query string.
        /// If the specified name already exists, the previous value will be replaced.
        /// </summary>
        /// <param name="item"></param>
        public void AddAndReplace(T item)
        {
            if (item == null)
                return;

            if (base.Contains(item.Key))
                base.Remove(item.Key);

            base.Add(item);
        }

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new T this[string key]
        {
            get
            {
                T item = default(T);
                if (Contains(key))
                    item = base[key];

                return item;
            }
            set
            {
                AddAndReplace(value);
            }
        }
    }

    public static class StyleManager
    {
        public static Cell SetLeftStringCell(this Cell cell)
        {
            cell.SetCellType(CellType.STRING);
            CellStyle style = cell.Sheet.Workbook.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.LEFT;
            cell.CellStyle = style;
            return cell;
        }
        public static Cell SetRightStringCell(this Cell cell)
        {
            cell.SetCellType(CellType.STRING);
            CellStyle style = cell.Sheet.Workbook.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.RIGHT;
            cell.CellStyle = style;
            return cell;
        }
        public static Cell SetCenterStringCell(this Cell cell)
        {
            cell.SetCellType(CellType.STRING);
            CellStyle style = cell.Sheet.Workbook.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            cell.CellStyle = style;
            return cell;
        }

        [Obsolete("多种单元格样式设置会相互冲突")]
        public static Cell SetHeaderCell(this Cell cell)
        {
            cell.SetCellType(CellType.STRING);
            CellStyle style = cell.Sheet.Workbook.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            style.VerticalAlignment = VerticalAlignment.CENTER;
            style.BorderBottom = CellBorderType.THIN;

            Font font = cell.Sheet.Workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.Boldweight = (short)FontBoldWeight.BOLD;
            style.SetFont(font);

            cell.CellStyle = style;
            return cell;
        }

        /// <summary>
        /// 获取Cell统一样式
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public static CellStyle GetCommonCellStyle(this Cell cell)
        {

            CellStyle style = cell.Sheet.Workbook.CreateCellStyle();
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.CENTER;
            style.VerticalAlignment = VerticalAlignment.CENTER;
            style.BorderBottom = CellBorderType.THIN;

            Font font = cell.Sheet.Workbook.CreateFont();
            font.FontHeightInPoints = 10;
            font.Boldweight = (short)FontBoldWeight.BOLD;
            style.SetFont(font);
            return style;
        }
    }

}
