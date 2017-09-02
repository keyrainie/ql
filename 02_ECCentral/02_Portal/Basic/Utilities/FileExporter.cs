using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.Basic.Utilities
{
    public static class FileExporter
    {
        private static readonly string s_RelativeUrl = "/UtilityService/ExportFile";

        #region GET

        public static void ExportFile(this RestClient restClient, string relativeUrl, ColumnSet[] columns = null)
        {
            ExportFile(restClient, relativeUrl, null, null, columns);
        }

        public static void ExportFile(this RestClient restClient, string relativeUrl, TextInfo[] textInfoList, ColumnSet[] columns = null)
        {
            ExportFile(restClient, relativeUrl, null, textInfoList, columns);
        }

        public static void ExportFile(this RestClient restClient, string relativeUrl, string exporterName, ColumnSet[] columns = null)
        {
            ExportFile(restClient, relativeUrl, exporterName, null, columns);
        }

        public static void ExportFile(this RestClient restClient, string relativeUrl, string exporterName, TextInfo[] textInfoList, ColumnSet[] columns = null)
        {
            ForwardRequestData data = new ForwardRequestData();
            if (textInfoList != null && textInfoList.Length > 0)
            {
                data.TextInfoList = new List<TextInfo>(textInfoList);
            }
            if (columns != null)
            {
                data.ColumnSetting = new List<List<ColumnData>>(columns.Length);
                foreach (var d in columns)
                {
                    data.ColumnSetting.Add(d.GetColumnDataList());
                }
            }
            data.ExporterName = exporterName;
            if (relativeUrl.ToLower().StartsWith("http"))
            {
                data.Url = relativeUrl;
            }
            else
            {
                Uri u = new Uri(restClient.ServicePath);
                data.Port = u.Port;
                data.Url = u.AbsolutePath.TrimEnd(new char[] { '/', '\\' }) + "/" + relativeUrl.TrimStart(new char[] { '/', '\\' });
                if (u.Query.Length > 0)
                {
                    data.Url = data.Url + u.Query;
                }
            }
            data.HttpContentType = restClient.ContentType;
            data.HttpMethod = Operating.GET;
            string hour = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours.ToString();
            string userSysNo = CPApplication.Current.LoginUser.UserSysNo.GetValueOrDefault().ToString();
            string userAcct = CPApplication.Current.LoginUser.LoginName;
            string sign = restClient.SignParameters(userSysNo, userAcct, hour);
            data.Parameters = new List<CodeNamePair>
            {
                new CodeNamePair{ Code = "Portal_Accept", Name = restClient.ContentType },
                new CodeNamePair{ Code = "Portal_Language",  Name = Thread.CurrentThread.CurrentUICulture.Name },
                new CodeNamePair{ Code = "Portal_UserSysNo", Name = userSysNo },
                new CodeNamePair{ Code = "Portal_UserAcct", Name = userAcct },
                new CodeNamePair{ Code = "Portal_TimeZone", Name = hour },
                new CodeNamePair{ Code = "Portal_Sign", Name = sign }
            };

            restClient.Query<FileExportResult>(s_RelativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                if (args.Result.RestServiceError != null && args.Result.RestServiceError.Trim().Length > 0)
                {
                    ISerializer serializer = SerializerFactory.GetSerializer(restClient.ContentType);
                    if (serializer != null)
                    {
                        RestServiceError error = null;
                        using (MemoryStream me = new MemoryStream(Encoding.UTF8.GetBytes(args.Result.RestServiceError)))
                        {
                            error = serializer.Deserialize(me, typeof(RestServiceError)) as RestServiceError;
                        }
                        args.Error = error;
                    }
                }
                if (args.FaultsHandle())
                {
                    return;
                }
                string url = restClient.ServicePath.TrimEnd(new char[] { '/', '\\' }) + "/" + args.Result.DownloadUrl.TrimStart(new char[] { '/', '\\' });
                UtilityHelper.OpenWebPage(url);
            });
        }

        #endregion

        #region POST

        public static void ExportFile(this RestClient restClient, string relativeUrl, object condition, ColumnSet[] columns = null)
        {
            ExportFile(restClient, relativeUrl, condition, null, null, columns);
        }

        public static void ExportFile(this RestClient restClient, string relativeUrl, object condition, TextInfo[] textInfoList, ColumnSet[] columns = null)
        {
            ExportFile(restClient, relativeUrl, condition, null, textInfoList, columns);
        }

        public static void ExportFile(this RestClient restClient, string relativeUrl, object condition, string exporterName, ColumnSet[] columns = null)
        {
            ExportFile(restClient, relativeUrl, condition, exporterName, null, columns);
        }

        public static void ExportFile(this RestClient restClient, string relativeUrl, object condition, string exporterName, TextInfo[] textInfoList, ColumnSet[] columns = null)
        {
            ForwardRequestData data = new ForwardRequestData();
            if (textInfoList != null && textInfoList.Length > 0)
            {
                data.TextInfoList = new List<TextInfo>(textInfoList);
            }
            if (columns != null)
            {
                data.ColumnSetting = new List<List<ColumnData>>(columns.Length);
                foreach (var d in columns)
                {
                    data.ColumnSetting.Add(d.GetColumnDataList());
                }
            }
            data.ExporterName = exporterName;
            if (relativeUrl.ToLower().StartsWith("http"))
            {
                data.Url = relativeUrl;
            }
            else
            {
                Uri u = new Uri(restClient.ServicePath);
                data.Port = u.Port;
                data.Url = u.AbsolutePath.TrimEnd(new char[] { '/', '\\' }) + "/" + relativeUrl.TrimStart(new char[] { '/', '\\' });
                if (u.Query.Length > 0)
                {
                    data.Url = data.Url + u.Query;
                }
            }
            ISerializer serializer = SerializerFactory.GetSerializer(restClient.ContentType);
            if (serializer != null)
            {
                data.Content = serializer.Serialization(condition, (condition == null ? typeof(object) : condition.GetType()));
            }
            data.HttpContentType = restClient.ContentType;
            data.HttpMethod = Operating.POST;
            string hour = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours.ToString();
            string userSysNo = CPApplication.Current.LoginUser.UserSysNo.GetValueOrDefault().ToString();
            string userAcct = CPApplication.Current.LoginUser.LoginName;
            string sign = restClient.SignParameters(userSysNo, userAcct, hour);
            data.Parameters = new List<CodeNamePair>
            {
                new CodeNamePair{ Code = "X-Accept-Language-Override",  Name = Thread.CurrentThread.CurrentUICulture.Name },
                new CodeNamePair{ Code = "X-User-SysNo", Name = userSysNo },
                new CodeNamePair{ Code = "X-User-Acct", Name = userAcct },
                new CodeNamePair{ Code = "X-Portal-TimeZone", Name = hour },
                new CodeNamePair{ Code = "X-Portal-Sign", Name = sign }
            };

            restClient.Query<FileExportResult>(s_RelativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result.RestServiceError != null && args.Result.RestServiceError.Trim().Length > 0 && serializer != null)
                {
                    RestServiceError error = null;
                    using (MemoryStream me = new MemoryStream(Encoding.UTF8.GetBytes(args.Result.RestServiceError)))
                    {
                        error = serializer.Deserialize(me, typeof(RestServiceError)) as RestServiceError;
                    }
                    args.Error = error;
                }
                if (args.FaultsHandle())
                {
                    return;
                }
                string url = restClient.ServicePath.TrimEnd(new char[] { '/', '\\' }) + "/" + args.Result.DownloadUrl.TrimStart(new char[] { '/', '\\' });
                UtilityHelper.OpenWebPage(url);
            });
        }

        #endregion
    }

    public class ColumnSet : List<ColumnData>
    {
        public ColumnSet()
        {
        }

        public ColumnSet(int capacity)
            : base(capacity)
        {
        }

        public ColumnSet(DataGrid dataGrid)
            : base(dataGrid.Columns.Count)
        {
            AddByGrid(dataGrid);
        }
        /// <summary>
        /// 根据grid列，增加导出ColumnSet
        /// </summary>
        /// <param name="dataGrid">需要导出的grid</param>
        /// <param name="IsVisableControl">true:根据Gird列是否显示和导出标志，控制导出   false:只根据导出标志控制导出</param>
        /// <returns></returns>
        public ColumnSet(DataGrid dataGrid, bool IsVisableControl)
            : base(dataGrid.Columns.Count)
        {
            AddByGrid(dataGrid, IsVisableControl);
        }

        private ColumnData FindColumnByFieldName(string fieldName)
        {
            foreach (ColumnData c in this)
            {
                if (c.FieldName.ToUpper() == fieldName.ToUpper())
                {
                    return c;
                }
            }
            return null;
        }

        #region Set By Field Name

        public ColumnSet SetTitle(string fieldName, string title)
        {
            return Set(fieldName, title, null, null, null, null, null, FooterType.None);
        }

        public ColumnSet SetFormat(string fieldName, string format)
        {
            return Set(fieldName, null, format, null, null, null, null, FooterType.None);
        }

        public ColumnSet SetWidth(string fieldName, int width)
        {
            return Set(fieldName, null, null, null, null, null, width, FooterType.None);
        }

        public ColumnSet SetFooterType(string fieldName, FooterType footerType)
        {
            return Set(fieldName, null, null, null, null, null, null, footerType);
        }

        public ColumnSet SetStyle(string fieldName, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder)
        {
            return Set(fieldName, null, null, horizontalAlignment, verticalAlignment, hasBorder, null, FooterType.None);
        }

        public ColumnSet Set(string fieldName, string format, int width, FooterType footerType = FooterType.None)
        {
            return Set(fieldName, null, format, null, null, null, width);
        }

        public ColumnSet Set(string fieldName, string format, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, FooterType footerType = FooterType.None)
        {
            return Set(fieldName, null, format, horizontalAlignment, verticalAlignment, hasBorder, null);
        }

        public ColumnSet Set(string fieldName, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, int width, FooterType footerType = FooterType.None)
        {
            return Set(fieldName, null, null, new HorizAlignments?(horizontalAlignment),
                new VertiAlignments?(verticalAlignment), new bool?(hasBorder), new int?(width));
        }

        public ColumnSet Set(string fieldName, string format, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, int width, FooterType footerType = FooterType.None)
        {
            return Set(fieldName, null, format, new HorizAlignments?(horizontalAlignment),
                new VertiAlignments?(verticalAlignment), new bool?(hasBorder), new int?(width));
        }

        private ColumnSet Set(string fieldName, string title, string format, HorizAlignments? horizontalAlignment, VertiAlignments? verticalAlignment, bool? hasBorder, int? width, FooterType footerType = FooterType.None)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException(fieldName);
            }
            ColumnData c = this.FindColumnByFieldName(fieldName);
            if (c == null)
            {
                throw new Exception("There is no column with field name '" + fieldName + "'.");
            }
            if (title != null)
            {
                c.Title = title;
            }
            if (format != null)
            {
                c.ValueFormat = format;
            }
            if (horizontalAlignment.HasValue)
            {
                c.HorizontalAlignment = horizontalAlignment.Value;
            }
            if (verticalAlignment.HasValue)
            {
                c.VerticalAlignment = verticalAlignment.Value;
            }
            if (hasBorder.HasValue)
            {
                c.HasBorder = hasBorder.Value;
            }
            if (width.HasValue)
            {
                c.Width = width.Value;
            }
            c.FooterType = footerType;
            return this;
        }

        #endregion Set By Field Name

        #region Add By Field Name

        public ColumnSet Add(string fieldName, string title, FooterType footerType = FooterType.None)
        {
            return Add(fieldName, title, null, footerType);
        }

        public ColumnSet Add(string fieldName, string title, int width, FooterType footerType = FooterType.None)
        {
            return Add(fieldName, title, null, null, null, null, width, footerType);
        }

        public ColumnSet Add(string fieldName, string title, string valueFormat, FooterType footerType = FooterType.None)
        {
            return Add(fieldName, title, valueFormat, null, null, null, null, footerType);
        }

        public ColumnSet Add(string fieldName, string title, string valueFormat, int width, FooterType footerType = FooterType.None)
        {
            return Add(fieldName, title, valueFormat, null, null, null, width, footerType);
        }

        public ColumnSet Add(string fieldName, string title, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, FooterType footerType = FooterType.None)
        {
            return Add(fieldName, title, null, horizontalAlignment, verticalAlignment, hasBorder, null, footerType);
        }

        public ColumnSet Add(string fieldName, string title, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, int width, FooterType footerType = FooterType.None)
        {
            return Add(fieldName, title, null, horizontalAlignment, verticalAlignment, hasBorder, width, footerType);
        }

        public ColumnSet Add(string fieldName, string title, string valueFormat, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, FooterType footerType = FooterType.None)
        {
            return Add(fieldName, title, valueFormat, horizontalAlignment, verticalAlignment, hasBorder, null, footerType);
        }

        public ColumnSet Add(string fieldName, string title, string valueFormat, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, int width, FooterType footerType = FooterType.None)
        {
            return Add(fieldName, title, valueFormat, new HorizAlignments?(horizontalAlignment), new VertiAlignments?(verticalAlignment), new bool?(hasBorder), new int?(width), footerType);
        }

        private ColumnSet Add(string fieldName, string title, string valueFormat, HorizAlignments? horizontalAlignment, VertiAlignments? verticalAlignment, bool? hasBorder, int? width, FooterType footerType = FooterType.None)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException(fieldName);
            }
            foreach (ColumnData c in this)
            {
                if (c.FieldName.ToUpper() == fieldName.ToUpper())
                {
                    throw new Exception(string.Format("Duplicated set the column with field name : {0}.", fieldName));
                }
            }
            ColumnData cols = new ColumnData
            {
                FieldName = fieldName,
                Title = title,
                ValueFormat = valueFormat,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Width = width,
                HasBorder = hasBorder,
                FooterType = footerType
            };
            this.Add(cols);
            return this;
        }

        #endregion Add By Field Name

        #region Insert By Field Name

        public ColumnSet Insert(int index, string fieldName, string title, FooterType footerType = FooterType.None)
        {
            return Insert(index, fieldName, title, null, footerType);
        }

        public ColumnSet Insert(int index, string fieldName, string title, int width, FooterType footerType = FooterType.None)
        {
            return Insert(index, fieldName, title, null, null, null, null, width, footerType);
        }

        public ColumnSet Insert(int index, string fieldName, string title, string valueFormat, FooterType footerType = FooterType.None)
        {
            return Insert(index, fieldName, title, valueFormat, null, null, null, null, footerType);
        }

        public ColumnSet Insert(int index, string fieldName, string title, string valueFormat, int width, FooterType footerType = FooterType.None)
        {
            return Insert(index, fieldName, title, valueFormat, null, null, null, width, footerType);
        }

        public ColumnSet Insert(int index, string fieldName, string title, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, FooterType footerType = FooterType.None)
        {
            return Insert(index, fieldName, title, null, horizontalAlignment, verticalAlignment, hasBorder, null, footerType);
        }

        public ColumnSet Insert(int index, string fieldName, string title, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, int width, FooterType footerType = FooterType.None)
        {
            return Insert(index, fieldName, title, null, horizontalAlignment, verticalAlignment, hasBorder, width, footerType);
        }

        public ColumnSet Insert(int index, string fieldName, string title, string valueFormat, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, FooterType footerType = FooterType.None)
        {
            return Insert(index, fieldName, title, valueFormat, horizontalAlignment, verticalAlignment, hasBorder, null, footerType);
        }

        public ColumnSet Insert(int index, string fieldName, string title, string valueFormat, HorizAlignments horizontalAlignment, VertiAlignments verticalAlignment, bool hasBorder, int width, FooterType footerType = FooterType.None)
        {
            return Insert(index, fieldName, title, valueFormat, new HorizAlignments?(horizontalAlignment), new VertiAlignments?(verticalAlignment), new bool?(hasBorder), new int?(width), footerType);
        }

        private ColumnSet Insert(int index, string fieldName, string title, string valueFormat, HorizAlignments? horizontalAlignment, VertiAlignments? verticalAlignment, bool? hasBorder, int? width, FooterType footerType = FooterType.None)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException(fieldName);
            }
            foreach (ColumnData c in this)
            {
                if (c.FieldName.ToUpper() == fieldName.ToUpper())
                {
                    throw new Exception(string.Format("Duplicated set the column with field name : {0}.", fieldName));
                }
            }
            ColumnData cols = new ColumnData
            {
                FieldName = fieldName,
                Title = title,
                ValueFormat = valueFormat,
                HorizontalAlignment = horizontalAlignment,
                VerticalAlignment = verticalAlignment,
                Width = width,
                HasBorder = hasBorder,
                FooterType = footerType
            };
            this.Insert(index, cols);
            return this;
        }

        #endregion Insert By Field Name

        public ColumnSet AddByGrid(DataGrid dataGrid)
        {
            return AddByGrid(dataGrid, false);
        }

        public ColumnSet AddByGrid(DataGrid dataGrid, bool IsVisableControl)
        {
            foreach (var col in dataGrid.Columns)
            {
                string name, title = string.Empty;
                if (col is DataGridTextColumn)
                {
                    //DataGridTextColumn txtCol = col as DataGridTextColumn;
                    var txtCol = col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTextColumn;
                    if (txtCol.Binding != null &&
                        (IsVisableControl ? col.Visibility == Visibility.Visible && txtCol.NeedExport : txtCol.NeedExport))
                    {
                        name = txtCol.Binding.Path.Path;
                        name = name.Replace("[", "").Replace("]", "");

                        var header = Newegg.Oversea.Silverlight.Controls.Data.DataGridAttached.GetHeader(txtCol);
                        if (header == null)
                        {
                            title = txtCol.Header.ToString();
                        }
                        else
                        {
                            title = header.ToString();
                        }
                        this.Add(name, title);
                    }
                }
                else if (col is DataGridTemplateColumn)
                {
                    var tempCol = col as Newegg.Oversea.Silverlight.Controls.Data.DataGridTemplateColumn;
                    if (tempCol.ExportField != null &&
                        (IsVisableControl ? col.Visibility == Visibility.Visible && tempCol.NeedExport : tempCol.NeedExport))
                    {
                        name = tempCol.ExportField.Replace("[", "").Replace("]", "");
                        var header = Newegg.Oversea.Silverlight.Controls.Data.DataGridAttached.GetHeader(tempCol);
                        if (header == null)
                        {
                            title = tempCol.Header.ToString();
                        }
                        else
                        {
                            title = header.ToString();
                        }
                        this.Add(name, title);
                    }
                }
            }
            return this;
        }

        public ColumnSet Remove(string fieldName)
        {
            int index = -1;
            foreach (ColumnData c in this)
            {
                index++;
                if (c.FieldName.ToUpper() == fieldName.ToUpper())
                {
                    break;
                }
            }
            if (index >= 0 && index < this.Count)
            {
                this.RemoveAt(index);
            }
            return this;
        }

        public ColumnData this[string fieldName]
        {
            get
            {
                foreach (ColumnData c in this)
                {
                    if (c.FieldName.ToUpper() == fieldName.ToUpper())
                    {
                        return c;
                    }
                }
                return null;
            }
        }

        public List<ColumnData> GetColumnDataList()
        {
            return new List<ColumnData>(this);
        }
    }

    #region 数据实体容器

    public class FileExportResult
    {
        public string RestServiceError
        {
            get;
            set;
        }

        public string DownloadUrl
        {
            get;
            set;
        }
    }

    public class ForwardRequestData
    {
        public string Url
        {
            get;
            set;
        }

        public int? Port
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public string HttpMethod
        {
            get;
            set;
        }

        public string HttpContentType
        {
            get;
            set;
        }

        public List<CodeNamePair> Parameters
        {
            get;
            set;
        }

        public string ExporterName
        {
            get;
            set;
        }

        public List<List<ColumnData>> ColumnSetting
        {
            get;
            set;
        }

        public List<TextInfo> TextInfoList
        {
            get;
            set;
        }
    }

    public enum HorizAlignments
    {
        // Summary:
        //     Default - General
        Default = 0,
        //
        // Summary:
        //     General
        General = 0,
        //
        // Summary:
        //     Left
        Left = 1,
        //
        // Summary:
        //     Centered
        Centered = 2,
        //
        // Summary:
        //     Right
        Right = 3,
        //
        // Summary:
        //     Filled
        Filled = 4,
        //
        // Summary:
        //     Justified
        Justified = 5,
        //
        // Summary:
        //     Centered Across the Selection
        CenteredAcrossSelection = 6,
        //
        // Summary:
        //     Distributed
        Distributed = 7
    }

    public enum VertiAlignments
    {
        // Summary:
        //     Top
        Top = 0,
        //
        // Summary:
        //     Centered
        Centered = 1,
        //
        // Summary:
        //     Default - Bottom
        Default = 2,
        //
        // Summary:
        //     Bottom
        Bottom = 2,
        //
        // Summary:
        //     Justified
        Justified = 3,
        //
        // Summary:
        //     Distributed
        Distributed = 4,
    }

    public class ColumnData
    {
        public ColumnData()
        {
            FooterType = Utilities.FooterType.None;
        }

        public int? FieldIndex
        {
            get;
            set;
        }

        public string FieldName
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public HorizAlignments? HorizontalAlignment
        {
            get;
            set;
        }

        public VertiAlignments? VerticalAlignment
        {
            get;
            set;
        }

        public bool? HasBorder
        {
            get;
            set;
        }

        public int? Width
        {
            get;
            set;
        }

        public string ValueFormat
        {
            get;
            set;
        }

        public FooterType FooterType
        {
            get;
            set;
        }
    }

    public enum FooterType
    {
        None = 0,
        Sum = 1,
        Average = 2
    }

    public class TextInfo
    {
        public string Title { get; set; }
        public string Memo { get; set; }
    }

    #endregion 数据实体容器
}