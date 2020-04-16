// Decompiled with JetBrains decompiler
// Type: PAL_Reader.PalReader
// Assembly: SLX Studio, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B1DE9AF1-FC33-4D79-BEAE-2919C4AAE382
// Assembly location: C:\Users\Beleive\Desktop\SLX Studio.exe

using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Extension
{
  public class PalReader
  {
    public List<Color> palette { get; private set; }

    public PalReader(string fileName)
    {
      FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      try
      {
        int count = 24;
        byte[] buffer1 = new byte[count];
        fileStream.Read(buffer1, 0, count);
        this.palette = new List<Color>();
        byte[] buffer2 = new byte[4];
        while (fileStream.Position + 4L <= fileStream.Length)
        {
          fileStream.Read(buffer2, 0, 4);
          this.palette.Add(Color.FromArgb((int) byte.MaxValue - (int) buffer2[3], (int) buffer2[0], (int) buffer2[1], (int) buffer2[2]));
        }
      }
      finally
      {
        fileStream.Close();
      }
    }

    public Color[] PaletteColors()
    {
      if (this.palette != null)
        return this.palette.ToArray();
      return (Color[]) null;
    }
  }
}
