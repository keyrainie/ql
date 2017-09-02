using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Core.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Data;

namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    public class UserSetting
    {
        internal static readonly string s_userSetting_Key = "DataGrid_UserSetting";

        private IUserProfile m_userProfile;
        private Data.DataGrid m_dataGrid;

        private Setting m_currentSetting;

        private bool m_enabledStore;

        public UserSetting(Data.DataGrid dataGrid)
        {
            this.m_userProfile = CPApplication.Current.Browser.Profile;
            this.m_dataGrid = dataGrid;
            this.m_enabledStore = CanUserStore();
        }

        #region Columns User Store

        /// <summary>
        /// 加载用户保存的列设置
        /// </summary>
        public void LoadSetting()
        {
            if (!this.m_enabledStore) return;

            var settings = m_userProfile.Get<List<Setting>>(s_userSetting_Key);


            if (settings != null)
            {
                this.m_currentSetting = settings.FirstOrDefault(p => string.Equals(p.Guid, this.m_dataGrid.GridID, StringComparison.OrdinalIgnoreCase));

                if (this.m_currentSetting != null)
                {
                    var storeColumns = this.m_currentSetting.Columns;

                    if (storeColumns.Count != m_dataGrid.Columns.Count)
                    {
                        return;
                    }

                    foreach (var col in storeColumns)
                    {
                        var column = m_dataGrid.Columns[col.Index];

                        column.DisplayIndex = col.DisplayIndex;
                        
                        if (col.ActualWidth > 0)
                        {
                            column.Width = new DataGridLength(col.ActualWidth);
                        }
                    }
                }
                else //如果没有找到保存的信息，则把初始化的Columns信息保存下来
                {
                    this.m_currentSetting = new Setting();
                    this.m_currentSetting.Guid = this.m_dataGrid.GridID;
                    this.m_currentSetting.Columns = new List<StoreColumn>();

                    for (int i = 0; i < this.m_dataGrid.Columns.Count; i++)
                    {
                        var column = this.m_dataGrid.Columns[i];
                        var col = new StoreColumn();

                        col.Name = column.GetColumnName();
                        col.DisplayIndex = column.DisplayIndex;
                        col.Index = i;

                        if (!column.Width.IsAuto)
                        {
                            col.ActualWidth = column.ActualWidth;
                        }

                        this.m_currentSetting.Columns.Add(col);
                    }

                    settings.Add(this.m_currentSetting);

                    this.m_userProfile.Set(s_userSetting_Key, settings);
                }
            }
            else
            {
                this.m_userProfile.Set(s_userSetting_Key, new List<Setting>());
            }
        }

        public void SaveSetting()
        {
            if (!this.m_enabledStore) return; 

            var settings = m_userProfile.Get<List<Setting>>(s_userSetting_Key);

            if (settings != null)
            {
                var setting = new Setting();
                setting.Guid = this.m_dataGrid.GridID;
                setting.Columns = new List<StoreColumn>();

                //1. 保存列的所有状态
                for (int index = 0; index < this.m_dataGrid.Columns.Count; index++)
                {
                    var column = this.m_dataGrid.Columns[index];
                    var col =new StoreColumn();
                    col.Name = column.GetColumnName();
                    col.DisplayIndex = column.DisplayIndex;
                    col.Index = index;
                    if (!column.Width.IsAuto)
                    {
                        col.ActualWidth = column.ActualWidth;
                    }
                    setting.Columns.Add(col);
                }

                //从保存的Settings列表移除当前配置
                var settingInStore = settings.FirstOrDefault(p => string.Equals(p.Guid, setting.Guid, StringComparison.OrdinalIgnoreCase));
                if (settingInStore != null)
                {
                    settings.Remove(settingInStore);
                }

                //添加新的配置到列表中
                settings.Add(setting);
                m_userProfile.Set(s_userSetting_Key, settings);
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// 判断是否启用用户保存
        /// </summary>
        /// <returns></returns>
        private bool CanUserStore()
        {
            if (this.m_dataGrid == null
                || string.IsNullOrEmpty(this.m_dataGrid.GridID)
                || !this.m_dataGrid.IsSaveColumns
                || !this.m_dataGrid.CanUserResizeColumns)
            {
                return false;
            }

            return true;
        }

        #endregion
    }

    public class Setting
    {
        public string Guid { get; set; }

        public List<StoreColumn> Columns { get; set; }
    }

    public class StoreColumn
    {
        public int DisplayIndex { get; set; }

        public double ActualWidth { get; set; }

        public int Index { get; set; }

        public string Name { get; set; }
    }
}
