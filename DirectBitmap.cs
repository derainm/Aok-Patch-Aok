// Decompiled with JetBrains decompiler
// Type: Direct_Bitmap.DirectBitmap
// Assembly: SLX Studio, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B1DE9AF1-FC33-4D79-BEAE-2919C4AAE382
// Assembly location: C:\Users\Beleive\Desktop\SLX Studio.exe

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Extension
{
  public class DirectBitmap : IDisposable
  {
    public Bitmap Bitmap { get; private set; }

    public int[] Bits { get; private set; }

    public bool Disposed { get; private set; }

    public int Height { get; private set; }

    public int Width { get; private set; }

    protected GCHandle BitsHandle { get; private set; }

    public DirectBitmap(int width, int height)
    {
      this.Width = width;
      this.Height = height;
      this.Bits = new int[width * height];
      this.BitsHandle = GCHandle.Alloc((object) this.Bits, GCHandleType.Pinned);
      this.Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, this.BitsHandle.AddrOfPinnedObject());
    }

    public DirectBitmap(Image img)
    {
      this.Width = img.Width;
      this.Height = img.Height;
      this.Bits = new int[img.Width * img.Height];
      this.BitsHandle = GCHandle.Alloc((object) this.Bits, GCHandleType.Pinned);
      this.Bitmap = new Bitmap(img.Width, img.Height, img.Width * 4, PixelFormat.Format32bppPArgb, this.BitsHandle.AddrOfPinnedObject());
      using (Graphics graphics = Graphics.FromImage((Image) this.Bitmap))
        graphics.DrawImage(img, 0, 0, this.Bitmap.Width, this.Bitmap.Height);
    }

    public DirectBitmap(Bitmap bmp)
    {
      this.Width = bmp.Width;
      this.Height = bmp.Height;
      this.Bits = new int[bmp.Width * bmp.Height];
      this.BitsHandle = GCHandle.Alloc((object) this.Bits, GCHandleType.Pinned);
      this.Bitmap = new Bitmap(bmp.Width, bmp.Height, bmp.Width * 4, PixelFormat.Format32bppPArgb, this.BitsHandle.AddrOfPinnedObject());
      using (Graphics graphics = Graphics.FromImage((Image) this.Bitmap))
        graphics.DrawImage((Image) bmp, 0, 0, this.Bitmap.Width, this.Bitmap.Height);
    }

    public void SetPixel(int x, int y, Color colour)
    {
      this.Bits[x + y * this.Width] = colour.ToArgb();
    }

    public Color GetPixel(int x, int y)
    {
      return Color.FromArgb(this.Bits[x + y * this.Width]);
    }

    public void Dispose()
    {
      if (this.Disposed)
        return;
      this.Disposed = true;
      this.Bitmap.Dispose();
      this.BitsHandle.Free();
    }

    public void GarbageCollection()
    {
      GC.Collect();
    }
  }
}
