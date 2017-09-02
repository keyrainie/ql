using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SendMKTPointEmail
{
    public class AccountPointNoticeMailEntityCollection :
     ConfigurationElementCollection
    {
        public AccountPointNoticeMailEntityCollection()
        {}

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return
                    ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new AccountPointNoticeMailEntity();
        }

        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new AccountPointNoticeMailEntity(elementName);
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((AccountPointNoticeMailEntity)element).Account;
        }


        public new string AddElementName
        {
            get
            { return base.AddElementName; }

            set
            { base.AddElementName = value; }

        }

        public new string ClearElementName
        {
            get
            { return base.ClearElementName; }

            set
            { base.AddElementName = value; }

        }

        public new string RemoveElementName
        {
            get
            { return base.RemoveElementName; }


        }

        public new int Count
        {

            get { return base.Count; }

        }


        public AccountPointNoticeMailEntity this[int index]
        {
            get
            {
                return (AccountPointNoticeMailEntity)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public AccountPointNoticeMailEntity this[string Account]
        {
            get
            {
                return (AccountPointNoticeMailEntity)BaseGet(Account);
            }
        }

        public int IndexOf(AccountPointNoticeMailEntity entity)
        {
            return BaseIndexOf(entity);
        }

        public void Add(AccountPointNoticeMailEntity entity)
        {
            BaseAdd(entity);

            // Add custom code here.
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
            // Add custom code here.
        }

        public void Remove(AccountPointNoticeMailEntity entity)
        {
            if (BaseIndexOf(entity) >= 0)
                BaseRemove(entity.Account);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
            // Add custom code here.
        }
    }
}
