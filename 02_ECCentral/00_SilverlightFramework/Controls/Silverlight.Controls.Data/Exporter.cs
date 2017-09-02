using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Newegg.Oversea.Silverlight.Controls.Data
{
    public static class Exporter
    {
        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields, string strFormat)
        {
            switch (strFormat)
            {
                case "XLS":
                    strBuilder.AppendLine("<Row>");
                    strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
                    strBuilder.AppendLine("</Row>");
                    break;
                case "CSV":
                    strBuilder.AppendLine(String.Join(",", lstFields.ToArray()));
                    break;
            }
        }

        private static void FormatCellWidth(StringBuilder strBuilder, List<double> colsWidth, string strFormat)
        {
            var index = 1;
            colsWidth.ForEach(p =>
            {
                switch (strFormat)
                {
                    case "XLS":
                        strBuilder.AppendLine(string.Format(@"<Column ss:Index='{0}' ss:AutoFitWidth='0' ss:Width='{1}'/>", index, colsWidth[index - 1]));
                        break;
                }
                index++;
            });
        }

        private static string FormatField(string data, string format)
        {
            //替换< , >否则，导出的Excel打不开
            data = data.Replace("<", "	&lt;").Replace(">", "&gt;");
            switch (format)
            {
                case "XLS":
                    return String.Format("<Cell><Data ss:Type=\"String" +
                       "\">{0}</Data></Cell>", data);
                case "CSV":
                    return String.Format("\"{0}\"",
                      data.Replace("\"", "\"\"\"").Replace("\n",
                      "").Replace("\r", ""));
            }
            return data;
        }

        internal static void ExportDataGrid(DataGrid dGrid)
        {
            if (dGrid.ItemsSource == null)
            {
                return;
            }
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = "xls",
                Filter = "Excel (*.xls)|*.xls",
                FilterIndex = 1
            };
            if (objSFD.ShowDialog() == true)
            {
                string strFormat = objSFD.SafeFileName.Substring(objSFD.SafeFileName.LastIndexOf('.') + 1).ToUpper();
                StringBuilder strBuilder = new StringBuilder();
                List<string> lstFields = new List<string>();

                List<double> colsWidth = new List<double>();
                if (dGrid.HeadersVisibility == DataGridHeadersVisibility.Column ||
                    dGrid.HeadersVisibility == DataGridHeadersVisibility.All)
                {
                    foreach (DataGridColumn dgcol in dGrid.Columns)
                    {
                        var needExport = true;

                        if (dgcol is DataGridTextColumn)
                        {
                            needExport = (dgcol as DataGridTextColumn).NeedExport;
                        }
                        else if (dgcol is DataGridTemplateColumn)
                        {
                            needExport = (dgcol as DataGridTemplateColumn).NeedExport;
                        }

                        if (dgcol.Visibility == Visibility.Visible && needExport)
                        {
                            if (dgcol.Header == null)
                            {
                                lstFields.Add(FormatField("", strFormat));
                            }
                            else
                            {
                                lstFields.Add(FormatField(dgcol.Header.ToString(), strFormat));
                            }
                            colsWidth.Add(dgcol.ActualWidth);
                        }
                    }

                    FormatCellWidth(strBuilder, colsWidth, strFormat);

                    BuildStringOfRow(strBuilder, lstFields, strFormat);

                }
                foreach (object data in dGrid.ItemsSource)
                {
                    lstFields.Clear();
                    foreach (DataGridColumn col in dGrid.Columns)
                    {
                        var needExport = true;

                        if (col is DataGridTextColumn)
                        {
                            needExport = (col as DataGridTextColumn).NeedExport;
                        }
                        else if (col is DataGridTemplateColumn)
                        {
                            needExport = (col as DataGridTemplateColumn).NeedExport;
                        }

                        if (col.Visibility == Visibility.Visible && needExport)
                        {
                            object objValue = null;
                            Binding objBinding = null;
                            string templateExportField = null;
                            if (col is DataGridBoundColumn)
                            {
                                objBinding = (col as DataGridBoundColumn).Binding;
                            }
                            if (col is DataGridTemplateColumn)
                            {
                                //如果是模板列，根据设置的ExportField属性获取导出数据；
                                var templateColumn = col as DataGridTemplateColumn;
                                if (templateColumn != null && !string.IsNullOrWhiteSpace(templateColumn.ExportField))
                                {
                                    templateExportField = templateColumn.ExportField;
                                }
                            }
                            if (templateExportField != null)
                            {
                                GetPropertyValueByField(data,ref objValue, templateExportField);
                            }

                            if (objBinding != null)
                            {
                                if (objBinding.Path.Path != "")
                                {
                                    var field = objBinding.Path.Path.Replace("[", "").Replace("]", "");

                                    GetPropertyValueByField(data, ref objValue, field);
                                }
                                if (objBinding.Converter != null)
                                {
                                    if (objValue != null)
                                    {
                                        objValue = objBinding.Converter.Convert(objValue,
                                          typeof(string), objBinding.ConverterParameter,
                                          objBinding.ConverterCulture).ToString();
                                    }
                                }
                            }
                            lstFields.Add(FormatField(objValue != null ? objValue.ToString() : string.Empty, strFormat));
                        }
                    }
                    BuildStringOfRow(strBuilder, lstFields, strFormat);
                }
                Stream stream = null;
                try
                {
                    stream = objSFD.OpenFile();
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format("Export failed！Reason：{0}", ex.Message));
                }
                StreamWriter sw = new StreamWriter(stream);
                if (strFormat == "XLS")
                {
                    //Let us write the headers for the Excel XML
                    sw.WriteLine("<?xml version=\"1.0\" " +
                                 "encoding=\"utf-8\"?>");
                    sw.WriteLine("<?mso-application progid" +
                                 "=\"Excel.Sheet\"?>");
                    sw.WriteLine("<Workbook xmlns=\"urn:" +
                                 "schemas-microsoft-com:office:spreadsheet\">");
                    sw.WriteLine("<DocumentProperties " +
                                 "xmlns=\"urn:schemas-microsoft-com:" +
                                 "office:office\">");
                    sw.WriteLine("<Author>Arasu Elango</Author>");
                    sw.WriteLine("<Created>" +
                                 DateTime.Now.ToLocalTime().ToLongDateString() +
                                 "</Created>");
                    sw.WriteLine("<LastSaved>" +
                                 DateTime.Now.ToLocalTime().ToLongDateString() +
                                 "</LastSaved>");
                    sw.WriteLine("<Company>Atom8 IT Solutions (P) " +
                                 "Ltd.,</Company>");
                    sw.WriteLine("<Version>12.00</Version>");
                    sw.WriteLine("</DocumentProperties>");
                    sw.WriteLine("<Worksheet ss:Name=\"Export Data\" " +
                       "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                    sw.WriteLine("<Table>");
                }

                sw.Write(strBuilder.ToString());
                if (strFormat == "XLS")
                {
                    sw.WriteLine("</Table>");
                    sw.WriteLine("</Worksheet>");
                    sw.WriteLine("</Workbook>");
                }
                sw.Close();
            }
        }

        private static void GetPropertyValueByField(object data, ref object objValue, string field)
        {
            var properties = field.Split('.').ToList();

            if (properties.Count == 1)
            {
                if (data.GetType().Name == "DynamicXml")
                {
                    objValue=((dynamic)data)[field];
                }
                else
                {
                    PropertyInfo pi = data.GetType().GetProperty(field);
                    if (pi != null)
                    {
                        var value = pi.GetValue(data, null);
                        objValue = value;
                    }
                }
            }
            else if (properties.Count > 1)
            {
                GetValue(data, ref objValue, properties);
            }
        }

        private static void GetValue(object data, ref object objValue, List<string> properties)
        {
            Queue<string> queue = new Queue<string>();
            foreach (var p in properties)
            {
                queue.Enqueue(p);
            }
            var value = GetPropertyValue(data, queue);
            if (value != null)
            {
                objValue = value;
            }
        }

        private static object GetPropertyValue(object data, Queue<string> queue)
        {
            var p = queue.Dequeue();
            if (data.GetType().Name == "DynamicXml")
            {
                return ((dynamic)data)[p];
            }
            else
            {
                PropertyInfo pi = data.GetType().GetProperty(p);
                if (pi != null)
                {
                    var value = pi.GetValue(data, null);
                    if (queue.Count > 0)
                    {
                        return GetPropertyValue(value, queue);
                    }
                    else
                    {
                        return value;
                    }
                }
            }
            return null;
        }

        internal static void ExportAllData(byte[] bytes, Stream stream)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
