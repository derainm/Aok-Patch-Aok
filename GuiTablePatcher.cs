// Decompiled with JetBrains decompiler
// Type: Aok_Patch.patcher_.GuiTablePatcher
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aok_Patch.patcher_
{
  internal static class GuiTablePatcher
  {
    public static byte[] UpdateGuiTable(
      byte[] data,
      uint id,
      int oldWidth,
      int oldHeight,
      int newWidth,
      int newHeight)
    {
      int index1;
      int index2;
      int index3;
      int index4;
      switch (oldWidth)
      {
        case 800:
          index1 = 1;
          index2 = 2;
          index3 = 3;
          index4 = 4;
          break;
        case 1024:
          index1 = 5;
          index2 = 6;
          index3 = 7;
          index4 = 8;
          break;
        case 1280:
          index1 = 9;
          index2 = 10;
          index3 = 11;
          index4 = 12;
          break;
        default:
          return data;
      }
      int num1 = newHeight - oldHeight;
      int num2 = newWidth - oldWidth;
      if (data.Length < 10)
        return data;
      try
      {
        string str = Encoding.ASCII.GetString(data, 0, 10);
        if (str == null || !str.Equals("Item Name\t"))
          return data;
      }
      catch
      {
        return data;
      }
      try
      {
        string[][] array = ((IEnumerable<string>) Encoding.ASCII.GetString(data).Split(new char[2]
        {
          '\r',
          '\n'
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string[]>((Func<string, string[]>) (line => line.Split('\t'))).ToArray<string[]>();
        foreach (string[] strArray in array)
        {
          if (strArray.Length >= 13 && !string.IsNullOrEmpty(strArray[0]) && !string.IsNullOrEmpty(strArray[12]))
          {
            int num3 = int.Parse(strArray[index1]);
            int num4 = int.Parse(strArray[index2]);
            int num5 = int.Parse(strArray[index3]);
            int num6 = int.Parse(strArray[index4]);
            if (num4 < oldHeight / 2 && num4 + num6 > oldHeight / 2)
              num6 += num1;
            else if (num4 > oldHeight / 2)
              num4 += num1;
            if (num5 == oldWidth)
              num5 = newWidth;
            strArray[index1] = num3.ToString();
            strArray[index2] = num4.ToString();
            strArray[index3] = num5.ToString();
            strArray[index4] = num6.ToString();
          }
        }
        byte[] bytes = Encoding.ASCII.GetBytes(((IEnumerable<string[]>) array).Select<string[], string>((Func<string[], string>) (tableRow => ((IEnumerable<string>) tableRow).Aggregate<string>((Func<string, string, string>) ((output, col) => output + "\t" + col)))).Aggregate<string>((Func<string, string, string>) ((output, line) => output + "\r\n" + line)));
        UserFeedback.Info("Patched Gui Table #{0}", (object) id);
        return bytes;
      }
      catch (Exception ex)
      {
        UserFeedback.Warning("Failed to patch Gui Table #{0}; leaving it unchanged.", (object) id);
        UserFeedback.Error(ex);
        return data;
      }
    }

    private enum Columns
    {
      Name,
      X800,
      Y800,
      W800,
      H800,
      X1024,
      Y1024,
      W1024,
      H1024,
      X1280,
      Y1280,
      W1280,
      H1280,
      Notes,
    }
  }
}
