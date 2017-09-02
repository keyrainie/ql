using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums.Promotion;

namespace ECommerce.Entity.Promotion
{
    public class GroupBuyingInfo : EntityBase
    {
        /// <summary>
        /// ϵͳ���
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        public string GroupBuyingTitle { get; set; }
        /// <summary>
        /// �Ź�����
        /// </summary>
        public string GroupBuyingDesc { get; set; }
        /// <summary>
        /// �Ź���ϸ����
        /// </summary>
        public string GroupBuyingDescLong { get; set; }

        /// <summary>
        /// �Ź���������
        /// </summary>
        public string GroupBuyingRules { get; set; }

        /// <summary>
        /// ��Ʒϵͳ���
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// �Ź��״̬
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// ʱ����������ʼʱ��
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// ʱ������������ʱ��
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// �Ź���Ʒ����ͼ,��ͼ,��Ҫ������
        /// </summary>
        public string GroupBuyingPicUrl { get; set; }

        /// <summary>
        /// �Ź���Ʒ����ͼ,��ͼ,��Ҫ������
        /// </summary>
        public string GroupBuyingMiddlePicUrl { get; set; }

        /// <summary>
        /// �Ź���Ʒ����ͼ,Сͼ,��Ҫ������
        /// </summary>
        public string GroupBuyingSmallPicUrl { get; set; }

        /// <summary>
        /// ��ʾ���ȼ�
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// ÿ���Ź��������ɹ������Ʒ����
        /// </summary>
        public int MaxPerOrder { get; set; }
        /// <summary>
        /// �޹�����(ÿ��Customer ID)
        /// </summary>
        public int LimitOrderCount { get; set; }

        /// <summary>
        /// �Ź���Ʒ����������
        /// </summary>
        public int AvailableSaleQty { get; set; }

        /// <summary>
        /// ��Ʒ�Ź��۸�
        /// </summary>
        public decimal GroupBuyPrice { get; set; }

        /// <summary>
        /// ��Ʒ���
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// ��Ʒ����
        /// </summary>
        public decimal CurrentPrice { get; set; }
    }
}
