using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;

using Newegg.Oversea.Silverlight.CommonDomain.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Rest;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;

namespace Newegg.Oversea.Silverlight.CommonDomain.Utilities
{
    public class MenuExporter
    {
        private RestClient m_restServiceClient;
        private PageBase m_page;

        public MenuExporter(PageBase page)
        {
            this.m_page = page;
            this.m_restServiceClient = new RestClient("/Service/Framework/V50/MenuRestService.svc", m_page);
        }

        /// <summary>
        /// 菜单导出
        /// 支持部分导出和全部菜单导出
        /// </summary>
        /// <param name="sourceItem"></param>
        public void Export(MenuItemModel sourceItem)
        {
            try
            {
                var sfd = new SaveFileDialog
                {
                    DefaultExt = "xml",
                    Filter = "Xml File (*.xml)|*.xml",
                    FilterIndex = 1
                };

                if (sfd.ShowDialog() == true)
                {
                    m_restServiceClient.Query<ObservableCollection<MenuItemModel>>("", (target, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }

                        var collection = args.Result;
                        ObservableCollection<MenuItemData> dataSource = null;

                        if (sourceItem != null)
                        {
                            var dataItem = collection.SingleOrDefault(item => item.MenuId == sourceItem.MenuId);

                            if (dataItem != null)
                            {
                                var list = new ObservableCollection<MenuItemModel>();

                                if (dataItem.Type == "L")
                                {
                                    dataItem = BuildLinkItem(dataItem, collection, ref list);
                                }

                                if (dataItem.Type == "C")
                                { 
                                    GetChilden(dataItem, collection, ref list);
                                }
                                GetParents(dataItem, collection, ref list);

                                if (!list.Contains(dataItem))
                                    list.Add(dataItem);

                                dataSource = list.ToXmlEntity();
                            }
                        }
                        else
                        {
                            var list = new ObservableCollection<MenuItemModel>();

                            foreach (var dataItem in collection)
                            {
                                MenuItemModel item;
                                if (dataItem.Type == "L")
                                {
                                    item = BuildLinkItem(dataItem, collection, ref list);
                                }
                                else
                                {
                                    item = dataItem;
                                }

                                if(!list.Contains(item))
                                    list.Add(item);
                            }

                            dataSource = list.ToXmlEntity();
                        }

                        if (dataSource != null && dataSource.Count > 0)
                        {
                            SaveAs(dataSource, sfd);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                m_page.Window.MessageBox.Show(ex.Message, MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 菜单导入
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public void Import(MenuItemModel source, Action<ObservableCollection<MenuItemModel>> callback)
        {
            try
            {
                var xml = string.Empty;
                var ofd = new OpenFileDialog
                {
                    Multiselect = false,
                    Filter = "Xml File (.xml)|*.xml",
                    FilterIndex = 1
                };

                if (ofd.ShowDialog() == true)
                {
                    using (var fs = ofd.File.OpenRead())
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            xml = sr.ReadToEnd();

                            sr.Close();
                        }
                        fs.Close();
                    }

                    var dataSource = UtilityHelper.XmlDeserialize<ObservableCollection<MenuItemData>>(xml);

                    if (Validate(dataSource))
                    {
                        m_restServiceClient.Create<ObservableCollection<MenuItemModel>>("ImportMenu", dataSource.ToEntity(), (target, args) =>
                        {
                            if (args.FaultsHandle())
                            {
                                return;
                            }

                            if (callback != null)
                            {
                                callback(args.Result);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                m_page.Window.MessageBox.Show(ex.Message, MessageBoxType.Error);
            }
        }

        #region Private Methods

        /// <summary>
        /// 对导入的数据源数据进行验证
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        private bool Validate(ObservableCollection<MenuItemData> dataSource)
        {
            var isValid = true;
            var msgBuilder = new StringBuilder();

            if (dataSource == null || dataSource.Count == 0)
            {
                isValid = false;
                msgBuilder.Append("Data source is null. \r\n");
            }

            var menuIdCount = 0;
            var nameCount = 0;
            var menuTypeCount = 0;
            var appCount = 0;
            var linkCount = 0;
            var inValidMenuTypeCount = 0;

            foreach (var item in dataSource)
            {
                if (item.MenuId.IsNullOrEmpty())
                {
                    menuIdCount++;
                }

                if (item.Name.IsNullOrEmpty())
                {
                    nameCount++;
                }
                if (item.MenuType.IsNullOrEmpty())
                {
                    menuTypeCount++;
                }
                else if (item.MenuType != "P" && item.MenuType != "C" && item.MenuType != "L")
                {
                    inValidMenuTypeCount++;
                }

                if (item.ApplicationName.IsNullOrEmpty())
                {
                    appCount++;
                }

                if ((item.MenuType == "P" || item.MenuType == "L") && item.LinkPath.IsNullOrEmpty())
                {
                    linkCount++;
                }
            }


            if (menuIdCount > 0)
            {
                msgBuilder.Append(string.Format("{0} item(s) lack of the property:MenuId. \r\n", menuIdCount));
            }
            if (nameCount > 0)
            {
                msgBuilder.Append(string.Format("{0} item(s) lack of the property:Name. \r\n", nameCount));
            }
            if (menuTypeCount > 0)
            {
                msgBuilder.Append(string.Format("{0} item(s) lack of the property:MenuType. \r\n", menuTypeCount));
            }
            if (appCount > 0)
            {
                msgBuilder.Append(string.Format("{0} item(s) lack of the property:ApplicationName. \r\n", appCount));
            }
            if (linkCount > 0)
            {
                msgBuilder.Append(string.Format("{0} item(s) lack of the property:LinkPath. \r\n", linkCount));
            }
            if (inValidMenuTypeCount > 0)
            {
                msgBuilder.Append(string.Format("{0} item(s)' MenuType is invalid,valid MenuType Value is: [P], [C] or [L]\r\n",inValidMenuTypeCount));
            }

            if (menuIdCount > 0 || nameCount > 0 || menuTypeCount > 0 || appCount > 0 || linkCount > 0)
            {
                isValid = false;
            }


            if (!isValid)
            {
                m_page.Window.MessageBox.Show(msgBuilder.ToString(), MessageBoxType.Warning);
            }
            return isValid;
        }

        /// <summary>
        /// 构建用于导入的LinkItem的数据
        /// LinkPath = 对应页面的MenuId + @ +对应页面的LinkPath
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        private MenuItemModel BuildLinkItem(MenuItemModel dataItem, ObservableCollection<MenuItemModel> collection,ref ObservableCollection<MenuItemModel> list)
        {
            var sourcePage = collection.SingleOrDefault(item => item.MenuId.ToString() == dataItem.LinkPath);

            if (sourcePage != null)
            {
                dataItem = new MenuItemModel
                {
                    DisplayName = dataItem.DisplayName,
                    MenuId = dataItem.MenuId,
                    LinkPath = sourcePage.MenuId + "@" + sourcePage.LinkPath,
                    Type = dataItem.Type,
                    ParentMenuId = dataItem.ParentMenuId,
                    ApplicationId = dataItem.ApplicationId
                };

                if (!list.Contains(sourcePage))
                {
                    list.Add(sourcePage);
                    GetParents(sourcePage, collection, ref list);
                }
            }

            return dataItem;
        }

        /// <summary>
        /// 获取所有的子节点
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="collection"></param>
        /// <param name="list"></param>
        private void GetChilden(MenuItemModel dataItem, ObservableCollection<MenuItemModel> collection, ref ObservableCollection<MenuItemModel> list)
        {
            var childen = (from item in collection where item.ParentMenuId == dataItem.MenuId select item).ToList();

            foreach (var child in childen)
            {
                MenuItemModel item;
                if (child.Type == "L")
                {
                    item = BuildLinkItem(child, collection, ref list);
                }
                else
                {
                    item = child;
                }

                list.Add(item);

                if (item.Type == "C")
                {
                    GetChilden(item, collection, ref list);
                }
            }
        }

        /// <summary>
        /// 获取所有的父节点
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="collection"></param>
        /// <param name="list"></param>
        private void GetParents(MenuItemModel dataItem, ObservableCollection<MenuItemModel> collection, ref ObservableCollection<MenuItemModel> list)
        {
            if (dataItem.ParentMenuId != null)
            {
                var parent = collection.SingleOrDefault(item => item.MenuId == dataItem.ParentMenuId);
                if (parent != null)
                {
                    if (!list.Contains(parent))
                    {
                        list.Add(parent);
                        GetParents(parent, collection, ref list);
                    }
                }
            }
        }

        /// <summary>
        /// 保存数据源到Xml文件
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="sfd"></param>
        private void SaveAs(ObservableCollection<MenuItemData> dataSource, SaveFileDialog sfd)
        {
            try
            {
                var xmlString = UtilityHelper.XmlSerialize(dataSource);

                using (var stream = sfd.OpenFile())
                {
                    using (var sw = new StreamWriter(stream, Encoding.Unicode))
                    {
                        sw.Write(xmlString);
                    }
                }
                m_page.Window.MessageBox.Show(CommonResource.Info_ExportSuccessfully, MessageBoxType.Success);
            }
            catch (Exception ex)
            {
                m_page.Window.MessageBox.Show(ex.Message, MessageBoxType.Error);
            }
        }

        #endregion
    }
}
