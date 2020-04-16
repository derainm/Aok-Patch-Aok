// Decompiled with JetBrains decompiler
// Type: ACT_Reader.ActReader
// Assembly: SLX Studio, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B1DE9AF1-FC33-4D79-BEAE-2919C4AAE382
// Assembly location: C:\Users\Beleive\Desktop\SLX Studio.exe

using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Extension
{
  public class ActReader
  {
    public List<Color> palette { get; private set; }

    public ActReader(string fileName)
    {
      FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      try
      {
        this.palette = new List<Color>();
        byte[] buffer1 = new byte[4];
        int num1 = 0;
        int num2 = 0;
        while (fileStream.Position + 3L <= fileStream.Length && num1 < 256)
        {
          fileStream.Read(buffer1, 0, 3);
          Color color = Color.FromArgb((int) buffer1[0], (int) buffer1[1], (int) buffer1[2]);
          this.palette.Add(color);
          ++num1;
          if (num1 > 1 && color == Color.FromArgb(0, 0, 0) && this.palette[num1 - 2] == Color.FromArgb(0, 0, 0))
            ++num2;
          else if (color != Color.Black && num2 > 0)
            num2 = 0;
        }
        byte[] buffer2 = new byte[2];
        while (fileStream.Position + 2L <= fileStream.Length)
        {
          fileStream.Read(buffer2, 0, 2);
          Color color = this.palette[(int) buffer2[1]];
          this.palette[(int) buffer2[1]] = Color.FromArgb((int) buffer2[0], (int) color.R, (int) color.G, (int) color.B);
        }
        if (num2 <= 0)
          return;
        this.palette.RemoveAt(this.palette.Count - 1);
        for (int index = 0; index < num2; ++index)
          this.palette.RemoveAt(this.palette.Count - 1);
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
