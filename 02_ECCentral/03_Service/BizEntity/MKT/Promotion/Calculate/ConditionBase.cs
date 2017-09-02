using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT 
{
    public class ConditionBase<T> 
    {
        private AndOrType? m_AndOrTypeRelation = AndOrType.Or;
        /// <summary>
        /// 子节点条件之间的关系：Or/And/Not，默认是Or关系；
        /// </summary>
        public AndOrType? AndOrTypeRelation
        {
            get
            {
                return m_AndOrTypeRelation;
            }
            set
            {
                m_AndOrTypeRelation = value;
            }
        }

        public T Data { get; set; }

        /// <summary>
        /// 二叉树左子节点
        /// </summary>
        public ConditionBase<T> LeftChild { get; set; }

        /// <summary>
        /// 二叉树右子节点
        /// </summary>
        public ConditionBase<T> RightChild { get; set; }

    }

    
    



}
