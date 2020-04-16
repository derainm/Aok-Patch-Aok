// Decompiled with JetBrains decompiler
// Type: ConsoleApplication1.Program
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
  internal class Program
  {
    private static string[] GetExports(string ModuleFileName)
    {
      SafeFileHandle file = NativeMethods.CreateFile(ModuleFileName, NativeMethods.EFileAccess.GenericRead, NativeMethods.EFileShare.Read, IntPtr.Zero, NativeMethods.ECreationDisposition.OpenExisting, NativeMethods.EFileAttributes.Normal, IntPtr.Zero);
      if (file.IsInvalid)
        throw new Win32Exception();
      try
      {
        SafeFileHandle fileMapping = NativeMethods.CreateFileMapping(file, IntPtr.Zero, NativeMethods.FileMapProtection.PageReadonly, 0U, 0U, IntPtr.Zero);
        if (fileMapping.IsInvalid)
          throw new Win32Exception();
        try
        {
          IntPtr num1 = NativeMethods.MapViewOfFile(fileMapping, NativeMethods.FileMapAccess.FileMapRead, 0U, 0U, UIntPtr.Zero);
          if (num1 == IntPtr.Zero)
            throw new Win32Exception();
          try
          {
            IntPtr num2 = NativeMethods.ImageNtHeader(num1);
            if (num2 == IntPtr.Zero)
              throw new Win32Exception();
            NativeMethods.IMAGE_NT_HEADERS structure1 = (NativeMethods.IMAGE_NT_HEADERS) Marshal.PtrToStructure(num2, typeof (NativeMethods.IMAGE_NT_HEADERS));
            if (structure1.Signature != 17744U)
              throw new Exception(ModuleFileName + " is not a valid PE file");
            IntPtr va1 = NativeMethods.ImageRvaToVa(num2, num1, structure1.OptionalHeader.DataDirectory[0].VirtualAddress, IntPtr.Zero);
            if (va1 == IntPtr.Zero)
              throw new Win32Exception();
            NativeMethods.IMAGE_EXPORT_DIRECTORY structure2 = (NativeMethods.IMAGE_EXPORT_DIRECTORY) Marshal.PtrToStructure(va1, typeof (NativeMethods.IMAGE_EXPORT_DIRECTORY));
            IntPtr va2 = NativeMethods.ImageRvaToVa(num2, num1, structure2.AddressOfNames, IntPtr.Zero);
            if (va2 == IntPtr.Zero)
              throw new Win32Exception();
            IntPtr va3 = NativeMethods.ImageRvaToVa(num2, num1, (uint) Marshal.ReadInt32(va2), IntPtr.Zero);
            if (va3 == IntPtr.Zero)
              throw new Win32Exception();
            string[] strArray = new string[(int) structure2.NumberOfNames];
            for (int index = 0; index < strArray.Length; ++index)
            {
              strArray[index] = Marshal.PtrToStringAnsi(va3);
              va3 += strArray[index].Length + 1;
            }
            return strArray;
          }
          finally
          {
            if (!NativeMethods.UnmapViewOfFile(num1))
              throw new Win32Exception();
          }
        }
        finally
        {
          fileMapping.Close();
        }
      }
      finally
      {
        file.Close();
      }
    }
  }
}
