/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Sanford Ma ("Sanford.Y.Ma@Newegg.com)
 *  Date:    2009-06-03 14:02:33
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;

namespace IPP.Oversea.CN.ContentManagement.BizProcess.Common
{
    public class ListSort<T> : IComparer<T>
    {
        Type type = null;
        string sortExpression = string.Empty;
        SortHelperType sortDirection = SortHelperType.DESC;

        public ListSort(string sortExpression, SortHelperType sortType)
        {
            this.type = typeof(T);
            this.sortExpression = sortExpression;
            this.sortDirection = sortType == SortHelperType.ASC ? SortHelperType.ASC : SortHelperType.DESC;
        }

        void Swap(ref object x, ref object y)
        {
            object temp = null;
            temp = x;
            x = y;
            y = temp;
        }

        int IComparer<T>.Compare(T x, T y)
        {
            object x1 = this.type.InvokeMember(this.sortExpression, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, x, null);
            object y1 = this.type.InvokeMember(this.sortExpression, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, y, null);

            if (sortDirection == SortHelperType.DESC)
            {
                Swap(ref x1, ref y1);
            }

            return ((new CaseInsensitiveComparer()).Compare(x1, y1));
        }
    }
    public enum SortHelperType
    {
        ASC = 0,
        DESC = 1,
    }

}
