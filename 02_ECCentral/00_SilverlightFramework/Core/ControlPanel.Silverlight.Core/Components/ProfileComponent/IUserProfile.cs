using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public interface IUserProfile : IComponent
    {
        /// <summary>
        /// 加载UserProfile的数据
        /// 如果独立存储中不存在数据，将从数据库同步数据到独立存储;
        /// </summary>
        void LoadProfileData(Action<object> callback);

        /// <summary>
        /// 同步独立存储的数据到数据库
        /// 由固定的Timer调用
        /// </summary>
        void SyncData();

        /// <summary>
        /// 保存数据到独立存储
        /// </summary>
        /// <param name="key">在独立存储中保存或获取的唯一Key</param>
        /// <param name="data">数据对象</param>
        void Set(string key, object data);

        /// <summary>
        /// 保存数据到独立存储
        /// </summary>
        /// <param name="key">在独立存储中保存或获取的唯一Key</param>
        /// <param name="data">数据对象</param>
        /// <param name="needSync">是否需要同步数据到数据库</param>
        void Set(string key, object data, bool needSync);

        /// <summary>
        /// 通过Key获取独立存储中的数据
        /// </summary>
        /// <typeparam name="T">数据对象的类型</typeparam>
        /// <param name="key">唯一Key</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 同步独立存储数据到数据库的时间差
        /// </summary>
        TimeSpan SyncIntervalTime { get; set; }

        /// <summary>
        /// 获取指定datagrid的guid
        /// </summary>
        /// <param name="gridGuid"></param>
        /// <returns></returns>
        void GetDataGridProfileItems(string gridGuid, Action<List<DataGridProfileItem>> callBack);
    }
}
