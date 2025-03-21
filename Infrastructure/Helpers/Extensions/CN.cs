﻿using System;

namespace Infrastructure.Helpers;
public static class CNHelpers
{
   static string[] cnNumbers = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

   public static string ToCNNumber(this int val)
   {
      string strVal = val.ToString();
      int length = strVal.Length;
      string intStr = strVal.Substring(length - 1, 1);

      if (length == 1) return cnNumbers[intStr.ToInt()];

      intStr = intStr.ToInt() == 0 ? "" : cnNumbers[intStr.ToInt()];

      string ten = strVal.Substring(length - 2, 1);
      string tenStr = cnNumbers[ten.ToInt()];
      if (length == 2)
      {
         if (String.IsNullOrEmpty(intStr))
         {
            return $"{tenStr}十{intStr}";
         }
         else
         {
            if (ten.ToInt() > 1) return $"{tenStr}十{intStr}";
            else return $"十{intStr}";
         }

      }

      string hundred = strVal.Substring(length - 3, 1);
      string hundredStr = cnNumbers[hundred.ToInt()];
      if (length == 3)
      {
         if (String.IsNullOrEmpty(intStr))
         {
            return $"{hundredStr}百{tenStr}十";
         }
         else
         {
            return $"{hundredStr}百{tenStr}十${intStr}";

         }
      }

      return "";

   }
   public static int ToROCYear(this int val)
      => val - 1911;

   public static int ROCYearToBC(this int val)
      => val + 1911;

   public static bool IsValidRocDate(this int val)
   {
      string str_val = val.ToString();
      return str_val.Length == 6 || str_val.Length == 7;
   }

   public static string ToRocDateText(this int val)
   {
      string strVal = val.ToString();
      if (strVal.Length == 6)
      {
         return $"{strVal.Substring(0, 2)}-{strVal.Substring(2, 2)}-{strVal.Substring(4, 2)}";
      }
      if (strVal.Length == 7)
      {
         return $"{strVal.Substring(0, 3)}-{strVal.Substring(3, 2)}-{strVal.Substring(5, 2)}";
      }
      return "";

   }
}
