using System;
using System.Collections.Generic;

namespace IPP.OrderMgmt.JobV31
{
  public static class ExtensionsMethods
    {
      public static bool Contains(this String str, List<string> taget)
      {
          bool result = false;
          if (string.IsNullOrEmpty(str))
          {
              return result;
          }                
       
          foreach (string s in taget)
          {
              if (str.Contains(s))
              {
                  result = true;
                  break;
              }
          }
          return result;         
      }

      public static bool Contains(this String str, List<string> taget, string CompareType)
      {
          bool result = false;
          if (string.IsNullOrEmpty(str))
          {
              return result;
          }

          //精确匹配
          if (CompareType == "Accurate")
          {
              foreach (string s in taget)
              {
                  if (str.Trim().Equals(s))
                  {
                      result = true;
                      break;
                  }
              }
          }
          else
          {
              foreach (string s in taget)
              {
                  if (str.Contains(s))
                  {
                      result = true;
                      break;
                  }
              }
          }
       
          return result;         
      }
      
    }
}
