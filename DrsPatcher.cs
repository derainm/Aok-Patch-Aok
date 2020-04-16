// Decompiled with JetBrains decompiler
// Type: Aok_Patch.patcher_.DrsPatcher
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Aok_Patch.patcher_
{
  internal static class DrsPatcher
  {
    public static void Patch(
      Stream oldDrs,
      Stream newDrs,
      int oldWidth,
      int oldHeight,
      int newWidth,
      int newHeight)
    {
      BinaryReader binaryReader = new BinaryReader(oldDrs);
      BinaryWriter binaryWriter = new BinaryWriter(newDrs);
      bool flag = false;
      while (true)
      {
        byte num = binaryReader.ReadByte();
        binaryWriter.Write(num);
        if (num == (byte) 26)
          flag = true;
        else if (num > (byte) 0 & flag)
          break;
      }
      binaryWriter.Write(binaryReader.ReadBytes(3));
      binaryWriter.Write(binaryReader.ReadBytes(12));
      uint num1 = binaryReader.ReadUInt32();
      binaryWriter.Write(num1);
      uint num2 = binaryReader.ReadUInt32();
      binaryWriter.Write(num2);
      DrsTable[] drsTableArray = new DrsTable[(int) num1];
      for (int index = 0; (long) index < (long) num1; ++index)
        drsTableArray[index] = new DrsTable();
      foreach (DrsTable drsTable in drsTableArray)
      {
        drsTable.Type = binaryReader.ReadUInt32();
        drsTable.Start = binaryReader.ReadUInt32();
        uint num3 = binaryReader.ReadUInt32();
        DrsItem[] drsItemArray = new DrsItem[(int) num3];
        for (int index = 0; (long) index < (long) num3; ++index)
          drsItemArray[index] = new DrsItem();
        drsTable.Items = (IEnumerable<DrsItem>) drsItemArray;
      }
      foreach (DrsTable drsTable in drsTableArray)
      {
        Trace.Assert(oldDrs.Position == (long) drsTable.Start);
        foreach (DrsItem drsItem in drsTable.Items)
        {
          drsItem.Id = binaryReader.ReadUInt32();
          drsItem.Start = binaryReader.ReadUInt32();
          drsItem.Size = binaryReader.ReadUInt32();
        }
      }
      foreach (DrsItem drsItem in ((IEnumerable<DrsTable>) drsTableArray).SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>) (table => table.Items)))
      {
        Trace.Assert(oldDrs.Position == (long) drsItem.Start);
        drsItem.Data = binaryReader.ReadBytes((int) drsItem.Size);
      }
      binaryReader.Close();
      uint num4 = num2;
      List<DrsTable> source = new List<DrsTable>(drsTableArray.Length);
      foreach (DrsTable drsTable1 in drsTableArray)
      {
        List<DrsItem> drsItemList = new List<DrsItem>();
        DrsTable drsTable2 = new DrsTable()
        {
          Start = drsTable1.Start,
          Type = drsTable1.Type,
          Items = (IEnumerable<DrsItem>) drsItemList
        };
        foreach (DrsItem drsItem1 in drsTable1.Items)
        {
          DrsItem drsItem2 = new DrsItem()
          {
            Id = drsItem1.Id,
            Start = num4,
            Data = DrsPatcher.ConvertDrsItem(drsItem1, drsTable1.Type, oldWidth, oldHeight, newWidth, newHeight)
          };
          drsItem2.Size = (uint) drsItem2.Data.Length;
          num4 += drsItem2.Size;
          drsItemList.Add(drsItem2);
        }
        source.Add(drsTable2);
      }
      foreach (DrsTable drsTable in source)
      {
        binaryWriter.Write(drsTable.Type);
        binaryWriter.Write(drsTable.Start);
        binaryWriter.Write(drsTable.Items.Count<DrsItem>());
      }
      foreach (DrsTable drsTable in source)
      {
        Trace.Assert(newDrs.Position == (long) drsTable.Start);
        foreach (DrsItem drsItem in drsTable.Items)
        {
          binaryWriter.Write(drsItem.Id);
          binaryWriter.Write(drsItem.Start);
          binaryWriter.Write(drsItem.Size);
        }
      }
      foreach (DrsItem drsItem in source.SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>) (outTable => outTable.Items)))
      {
        Trace.Assert(newDrs.Position == (long) drsItem.Start);
        binaryWriter.Write(drsItem.Data);
      }
      binaryWriter.Close();
    }

    private static byte[] ConvertDrsItem(
      DrsItem item,
      uint type,
      int oldWidth,
      int oldHeight,
      int newWidth,
      int newHeight)
    {
      switch (type)
      {
        case 1651076705:
          return GuiTablePatcher.UpdateGuiTable(item.Data, item.Id, oldWidth, oldHeight, newWidth, newHeight);
        case 1936486432:
          return SlpStretcher.Enlarge(item.Id, item.Data, oldWidth, oldHeight, newWidth, newHeight);
        default:
          return item.Data;
      }
    }
  }
}
