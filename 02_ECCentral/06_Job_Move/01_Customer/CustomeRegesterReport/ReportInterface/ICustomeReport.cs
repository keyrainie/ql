using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReportInterface
{
    public delegate void ShowMsg(string info);
    public interface ICustomeReport 
    {
        void SendCustomeReport(); 
        ShowMsg ShowInfo { get; set; }
    }
}
