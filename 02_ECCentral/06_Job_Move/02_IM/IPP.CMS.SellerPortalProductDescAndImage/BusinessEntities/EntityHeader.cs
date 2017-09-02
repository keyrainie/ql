/*****************************************************************
 * Copyright (C) Newegg Corporation. All rights reserved.
 * 
 * Author:           Phoebe Zhang(Phoebe.F.Zhang@newegg.com)
 * Create Date:  2009-03-06
 * Usage:  
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/

using System;

namespace IPP.Oversea.CN.ContentManagement.BusinessEntities.Common
{
    /// <summary>
    /// ��Ϣͷ
    /// </summary>
    [Serializable]
    public class EntityHeader
    {
        /// <summary>
        /// �����ռ�,�������Լ���WCF Policy������2.4��
        /// </summary>
        public string NameSpace
        {
            get;
            set;
        }
        /// <summary>
        /// ����,���磺Void,Create,Hold,Update,Batch,UnHold,Dispatch,Verify,UnVerify
        /// </summary>
        public string Action
        {
            get;
            set;
        }
        /// <summary>
        /// �汾
        /// </summary>
        public string Version
        {
            get;
            set;
        }
        /// <summary>
        /// ����
        /// </summary>
        public string Type
        {
            get;
            set;
        }
        /// <summary>
        /// ������
        /// </summary>
        public string Sender
        {
            get;
            set;
        }
        /// <summary>
        /// ��˾���
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// �ؼ���
        /// </summary>
        public string Tag
        {
            get;
            set;
        }
        /// <summary>
        /// ����,�磺ENU��CH
        /// </summary>
        public string Language
        {
            get;
            set;
        }
        /// <summary>
        /// ʱ��
        /// </summary>
        public string TimeZone
        {
            get;
            set;
        }
        /// <summary>
        /// ����
        /// </summary>
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// ԭʼ������
        /// </summary>
        public string OriginalSender
        {
            get;
            set;
        }
        /// <summary>
        /// ԭʼ��Ϣ��
        /// </summary>
        public string OriginalGUID
        {
            get;
            set;
        }
        /// <summary>
        /// ���ص�ַ
        /// </summary>
        public string CallbackAddress
        {
            get;
            set;
        }

        public string From
        {
            get;
            set;
        }

        public string To
        {
            get;
            set;
        }

        public string FromSystem
        {
            get;
            set;
        }

        public string ToSystem
        {
            get;
            set;
        }

        public string CountryCode
        {
            get;
            set;
        }

        public GlobalBusinessType GlobalBusinessType
        {
            get;
            set;
        }

        /// <summary>
        /// Store Company Code
        /// </summary>
        public string StoreCompanyCode
        {
            get;
            set;
        }

        public string TransactionCode
        {
            get;
            set;
        }

        public OperationUserEntity OperationUser 
        { 
            get; 
            set; 
        }
    }

    /// <summary>
    /// GlobalBusinessType of Enum type
    /// </summary>
    [Serializable]
    public enum GlobalBusinessType
    {
        VF,
        Normal,
        Listing
    }

    public class OperationUserEntity
    {
        public string CompanyCode { get; set; }
        public string FullName { get; set; }
        public string LogUserName { get; set; }
        public string SourceDirectoryKey { get; set; }
        public string SourceUserName { get; set; }
        public string UniqueUserName { get; set; }
    }
}