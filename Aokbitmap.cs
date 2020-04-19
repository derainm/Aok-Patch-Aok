using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aok_Patch.patcher_
{
    class Aokbitmap
    {

        public const int BITMAPFILEHEADER_SIZE = 14;

        public const int BITMAPINFOHEADER_SIZE = 40;

        public byte[] bitmapFileHeader = new byte[14];

        public byte[] bfType = new byte[] { 66, 77 };

        public int bfSize = 0;

        public int bfReserved1 = 0;

        public int bfReserved2 = 0;

        public int bfOffset = 54;

        public byte[] bitmapInfoHeader = new byte[40];

        public int biSize = 40;

        public int biWidth = 0;

        public int biHeight = 0;

        public int biPlanes = 1;

        public int biBitCount = 8;

        public int biCompression = 0;

        public int biSizeImage = 196608;

        public int biXPelsPerMeter = 0;

        public int biYPelsPerMeter = 0;

        public int biClrUsed = 0;

        public int biClrImportant = 0;

        public byte[] colortable;

        public byte[] bitmap;

        public FileStream fo;

        public int mask;

        public int outline1;

        public int outline2;

        public int shadow;

        public string sample;



        internal Aokbitmap()
        {
            this.mask = 244;
            this.outline1 = this.outline2 = 251;
            this.shadow = 13;
            this.sample = "50500.bmp";
        }


        internal Aokbitmap(int m, int o1, int o2, int sh)
        {
            this.mask = m;
            this.outline1 = o1;
            this.outline2 = o2;
            this.shadow = sh;
            this.sample = "50500.bmp";
        }

        internal virtual void Write(string outputfile, int[][] picture, int width, int height)
        {
            try
            {
                this.fo = new FileStream(outputfile, FileMode.Create, FileAccess.Write);
                convertimage(picture, width, height);
                //WriteBitmapFileHeader();
                BinaryWriter writer = new BinaryWriter(this.fo);
                writer.Write(this.bfType);
                writer.Write(intToDWord(this.bfSize));
                writer.Write(intToWord(this.bfReserved1));
                writer.Write(intToWord(this.bfReserved2));
                writer.Write(intToDWord(this.bfOffset));
                //WriteBitmapInfoHeader();
                writer.Write(intToDWord(this.biSize));
                writer.Write(intToDWord(this.biWidth));
                writer.Write(intToDWord(this.biHeight));
                writer.Write(intToWord(this.biPlanes));
                writer.Write(intToWord(this.biBitCount));
                writer.Write(intToDWord(this.biCompression));
                writer.Write(intToDWord(this.biSizeImage));
                writer.Write(intToDWord(this.biXPelsPerMeter));
                writer.Write(intToDWord(this.biYPelsPerMeter));
                writer.Write(intToDWord(this.biClrUsed));
                writer.Write(intToDWord(this.biClrImportant));
                //Writecolortable();
                writer.Write(this.colortable);
                //WriteBitmap();
                writer.Write(this.bitmap);
                writer.Close();
                writer.Dispose();
                this.fo.Close();
                this.fo.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception in saving bitmap!" + e);
            }
        }

        internal virtual void convertimage(int[][] picture, int width, int height)
        {
            this.bfOffset = 1078;
            int pad = (4 - width % 4) * height;
            if (4 - width % 4 == 4)
            {
                pad = 0;
            }
            this.biSizeImage = width * height + pad;
            this.bfSize = this.biSizeImage + 14 + 40 + 1024;
            this.biWidth = width;
            this.biHeight = height;
            imagehandler img = new imagehandler();
            img.sampleused = this.sample;
            img.loadbitmap(this.sample, 1);
            this.colortable = img.returnaokpalette();
            int pad_line = 4 - width % 4;
            if (pad_line == 4)
            {
                pad_line = 0;
            }
            this.bitmap = new byte[(width + pad_line) * height];
            int count = 0;
            for (int i = height - 1; i >= 0; i--)
            {
                int j;
                for (j = 0; j < width; j++)
                {
                    int x = picture[i][j];
                    if (x == -1)
                    {
                        x = this.mask;
                    }
                    if (x == -2)
                    {
                        x = this.outline1;
                    }
                    if (x == -3)
                    {
                        x = this.outline2;
                    }
                    if (x == -4)
                    {
                        x = this.shadow;
                    }
                    this.bitmap[count] = (byte)x;
                    count++;
                }
                for (j = 0; j < pad_line; j++)
                {
                    this.bitmap[count] = 0;
                    count++;
                }
            }
        }

    public void WriteBitmapFileHeader()
    {
        try
        {

            //writer.Close();
        }
        catch (Exception wbfh)
        {
            Console.WriteLine(wbfh.ToString());
            Console.Write(wbfh.StackTrace);
        }
    }

    public void WriteBitmapInfoHeader()
    {
        try
        {
            BinaryWriter writer = new BinaryWriter(this.fo);

        }
        catch (Exception wbih)
        {
            Console.WriteLine(wbih.ToString());
            Console.Write(wbih.StackTrace);
        }
    }

    public void Writecolortable()
    {
        try
        {
            BinaryWriter writer = new BinaryWriter(this.fo);
            
            //writer.Close();
        }
        catch (Exception wbih)
        {
            Console.WriteLine(wbih.ToString());
            Console.Write(wbih.StackTrace);
        }
    }

    public void WriteBitmap()
    {
        try
        {
            BinaryWriter writer = new BinaryWriter(this.fo);

        }
        catch (Exception wbih)
        {
            Console.WriteLine(wbih.ToString());
            Console.Write(wbih.StackTrace);
        }
    }

    public byte[] intToWord(int parValue)
    {
        byte[] retValue = new byte[2];
        retValue[0] = unchecked((byte)(parValue & 0xFF));
        retValue[1] = unchecked((byte)(parValue >> 8 & 0xFF));
        return retValue;
    }

    public byte[] intToDWord(int parValue)
    {
        byte[] retValue = new byte[4];
        retValue[0] = unchecked((byte)(parValue & 0xFF));
        retValue[1] = unchecked((byte)(parValue >> 8 & 0xFF));
        retValue[2] = unchecked((byte)(parValue >> 16 & 0xFF));
        retValue[3] = unchecked((byte)(parValue >> 24 & 0xFF));
        return retValue;
    }


}
}
