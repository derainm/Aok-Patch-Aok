using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;


namespace Aok_Patch.patcher_
{
    class imagehandler
    {
        public Image img;

        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: internal byte[][] colortable = new byte[256][4];
        internal byte[][] colortable = RectangularArrays.RectangularbyteArray(256, 4);

        //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
        //ORIGINAL LINE: internal byte[][] aoktable = new byte[256][4];
        public byte[][] aoktable = RectangularArrays.RectangularbyteArray(256, 4);

        public int[] mapping = new int[256];

        public int mask;

        public byte[] rawpixels;

        public byte[] outputpixels;

        public byte[] aokpalette;

        public int aokmask = 255;

        public string sampleused = "50500.bmp";

        public bool aoeused;

        public int imgheight =0;
        public int imgwidth = 0;



        internal int maskoriginal;
        internal static class RectangularArrays
        {
            public static byte[][] RectangularbyteArray(int size1, int size2)
            {
                byte[][] newArray = new byte[size1][];
                for (int array1 = 0; array1 < size1; array1++)
                {
                    newArray[array1] = new byte[size2];
                }

                return newArray;
            }
        }
        public virtual void loadbitmap(string filename, int whichfile)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                    return;
                if (whichfile == 1)
                {
                    filename = this.sampleused;
                }
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                int bflen = 14;
                byte[] bf = new byte[bflen];
                fs.Read(bf, 0, bflen);
                int bilen = 40;
                byte[] bi = new byte[bilen];
                fs.Read(bi, 0, bilen);
                int nsize = (bf[5] & 0xFF) << 24 | (bf[4] & 0xFF) << 16 | (bf[3] & 0xFF) << 8 | bf[2] & 0xFF;
                int nbisize = (bi[3] & 0xFF) << 24 | (bi[2] & 0xFF) << 16 | (bi[1] & 0xFF) << 8 | bi[0] & 0xFF;
                int nwidth = (bi[7] & 0xFF) << 24 | (bi[6] & 0xFF) << 16 | (bi[5] & 0xFF) << 8 | bi[4] & 0xFF;
                int nheight = (bi[11] & 0xFF) << 24 | (bi[10] & 0xFF) << 16 | (bi[9] & 0xFF) << 8 | bi[8] & 0xFF;
                int nplanes = (bi[13] & 0xFF) << 8 | bi[12] & 0xFF;
                int nbitcount = (bi[15] & 0xFF) << 8 | bi[14] & 0xFF;
                int ncompression = bi[19] << 24 | bi[18] << 16 | bi[17] << 8 | bi[16];
                int nsizeimage = (bi[23] & 0xFF) << 24 | (bi[22] & 0xFF) << 16 | (bi[21] & 0xFF) << 8 | bi[20] & 0xFF;
                int nxpm = (bi[27] & 0xFF) << 24 | (bi[26] & 0xFF) << 16 | (bi[25] & 0xFF) << 8 | bi[24] & 0xFF;
                int nypm = (bi[31] & 0xFF) << 24 | (bi[30] & 0xFF) << 16 | (bi[29] & 0xFF) << 8 | bi[28] & 0xFF;
                int nclrused = (bi[35] & 0xFF) << 24 | (bi[34] & 0xFF) << 16 | (bi[33] & 0xFF) << 8 | bi[32] & 0xFF;
                int nclrimp = (bi[39] & 0xFF) << 24 | (bi[38] & 0xFF) << 16 | (bi[37] & 0xFF) << 8 | bi[36] & 0xFF;
                if (nbitcount != 8)
                {
                    Console.WriteLine("Not a 256 color bitmap!");
                }
                else
                {
                    int nNumColors = 0;
                    if (nclrused > 0)
                    {
                        nNumColors = nclrused;
                    }
                    else
                    {
                        nNumColors = 1 << nbitcount;
                    }
                    if (nsizeimage == 0)
                    {
                        nsizeimage =(int) (nwidth * nbitcount + 31 & 0xFFFFFFE0) >> 3;
                        nsizeimage *= nheight;
                    }
                    byte[] bpalette = new byte[nNumColors * 4];
                    fs.Read(bpalette, 0, nNumColors * 4);
                    if (whichfile == 1)
                    {
                        this.aokpalette = new byte[nNumColors * 4];
                        this.aokpalette = bpalette;
                    }
                    int nindex8 = 0;
                    for (int n = 0; n < nNumColors; n++)
                    {
                        for (int j = 0; j <= 3; j++)
                        {
                            if (whichfile == 1)
                            {
                                this.aoktable[n][j] = bpalette[nindex8 + j];
                            }
                            else
                            {
                                this.colortable[n][j] = bpalette[nindex8 + j];
                            }
                        }
                        nindex8 += 4;
                    }
                    int npad8 = nsizeimage / nheight - nwidth;
                    if (whichfile == 0)
                    {
                        this.rawpixels = new byte[(nwidth + npad8) * nheight];
                        fs.Read(this.rawpixels, 0, (nwidth + npad8) * nheight);
                        this.imgheight = nheight;
                        this.imgwidth = nwidth;
                    }
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
                Console.WriteLine("Caught exception in loadbitmap!");
            }
        }
        public virtual void getmapping()
        {
            for (int i = 0; i < 256; i++)
            {
                int min = 200000;
                int index = 0;
                for (int j = 0; j < 256; j++)
                {
                    int cr = this.colortable[i][0];
                    if (cr < 0)
                    {
                        cr += 256;
                    }
                    int cg = this.colortable[i][1];
                    if (cg < 0)
                    {
                        cg += 256;
                    }
                    int cb = this.colortable[i][2];
                    if (cb < 0)
                    {
                        cb += 256;
                    }
                    int aokr = this.aoktable[j][0];
                    if (aokr < 0)
                    {
                        aokr += 256;
                    }
                    int aokg = this.aoktable[j][1];
                    if (aokg < 0)
                    {
                        aokg += 256;
                    }
                    int aokb = this.aoktable[j][2];
                    if (aokb < 0)
                    {
                        aokb += 256;
                    }
                    long dist = ((cr - aokr) * (cr - aokr) + (cg - aokg) * (cg - aokg) + (cb - aokb) * (cb - aokb));
                    if (dist < min)
                    {
                        min = (int)dist;
                        index = j;
                    }
                }
                this.mapping[i] = index;
            }
            byte first = this.rawpixels[0];
            this.mask = first;
            if (this.mask < 0)
            {
                this.mask += 256;
            }
        }

        public virtual void getmask()
        {
            this.maskoriginal = this.mapping[this.mask];
            this.mapping[this.mask] = this.aokmask;
        }

        internal virtual void setmask(int x)
        {
            this.mapping[this.mask] = this.maskoriginal;
            this.mapping[x] = this.aokmask;
        }

        public virtual void setplayercolor()
        {
            int[] bluemapping = new int[256];
            for (int i = 0; i < 256; i++)
            {
                int min = 200000;
                int index = 0;
                int j;
                for (j = 17; j <= 23; j++)
                {
                    int cb = this.aoktable[i][0];
                    if (cb < 0)
                    {
                        cb += 256;
                    }
                    int cg = this.aoktable[i][1];
                    if (cg < 0)
                    {
                        cg += 256;
                    }
                    int cr = this.aoktable[i][2];
                    if (cr < 0)
                    {
                        cr += 256;
                    }
                    int aokb = this.aoktable[j][0];
                    if (aokb < 0)
                    {
                        aokb += 256;
                    }
                    int aokg = this.aoktable[j][1];
                    if (aokg < 0)
                    {
                        aokg += 256;
                    }
                    int aokr = this.aoktable[j][2];
                    if (aokr < 0)
                    {
                        aokr += 256;
                    }
                    long dist = ((cr - aokr) * (cr - aokr) + (cg - aokg) * (cg - aokg) + (cb - aokb) * (cb - aokb));
                    if (dist < min && cb >= 97 && cb > cg + cr && cr < 120 && cg < 120)
                    {
                        min = (int)dist;
                        index = j;
                    }
                }
                if (index != 0)
                {
                    for (j = 0; j < 256; j++)
                    {
                        if (this.mapping[j] == i)
                        {
                            this.mapping[j] = index;
                        }
                    }
                }
            }
            getmask();
        }


        public virtual void savebitmap(string inputfile, string outputfile)
        {
            try
            {
                FileStream fs = new FileStream(inputfile, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outputfile, FileMode.Create, FileAccess.Write);
                int bflen = 14;
                byte[] bf = new byte[bflen];
                fs.Read(bf, 0, bflen);
                fout.Write(bf, 0, bflen);
                int bilen = 40;
                byte[] bi = new byte[bilen];
                fs.Read(bi, 0, bilen);
                fout.Write(bi, 0, bilen);
                int nsize = (bf[5] & 0xFF) << 24 | (bf[4] & 0xFF) << 16 | (bf[3] & 0xFF) << 8 | bf[2] & 0xFF;
                int nbisize = (bi[3] & 0xFF) << 24 | (bi[2] & 0xFF) << 16 | (bi[1] & 0xFF) << 8 | bi[0] & 0xFF;
                int nwidth = (bi[7] & 0xFF) << 24 | (bi[6] & 0xFF) << 16 | (bi[5] & 0xFF) << 8 | bi[4] & 0xFF;
                int nheight = (bi[11] & 0xFF) << 24 | (bi[10] & 0xFF) << 16 | (bi[9] & 0xFF) << 8 | bi[8] & 0xFF;
                int nplanes = (bi[13] & 0xFF) << 8 | bi[12] & 0xFF;
                int nbitcount = (bi[15] & 0xFF) << 8 | bi[14] & 0xFF;
                int ncompression = bi[19] << 24 | bi[18] << 16 | bi[17] << 8 | bi[16];
                int nsizeimage = (bi[23] & 0xFF) << 24 | (bi[22] & 0xFF) << 16 | (bi[21] & 0xFF) << 8 | bi[20] & 0xFF;
                int nxpm = (bi[27] & 0xFF) << 24 | (bi[26] & 0xFF) << 16 | (bi[25] & 0xFF) << 8 | bi[24] & 0xFF;
                int nypm = (bi[31] & 0xFF) << 24 | (bi[30] & 0xFF) << 16 | (bi[29] & 0xFF) << 8 | bi[28] & 0xFF;
                int nclrused = (bi[35] & 0xFF) << 24 | (bi[34] & 0xFF) << 16 | (bi[33] & 0xFF) << 8 | bi[32] & 0xFF;
                int nclrimp = (bi[39] & 0xFF) << 24 | (bi[38] & 0xFF) << 16 | (bi[37] & 0xFF) << 8 | bi[36] & 0xFF;
                if (nbitcount != 8)
                {
                    Console.WriteLine("Not a 256 color bitmap!");
                }
                else
                {
                    int nNumColors = 0;
                    if (nclrused > 0)
                    {
                        nNumColors = nclrused;
                    }
                    else
                    {
                        nNumColors = 1 << nbitcount;
                    }
                    if (nsizeimage == 0)
                    {
                        nsizeimage = (int)(nwidth * nbitcount + 31 & 0xFFFFFFE0) >> 3;
                        nsizeimage *= nheight;
                    }
                    byte[] bpalette = new byte[nNumColors * 4];
                    fs.Read(bpalette, 0, nNumColors * 4);
                    fout.Write(this.aokpalette, 0, nNumColors * 4);
                    int npad8 = nsizeimage / nheight - nwidth;
                    this.rawpixels = new byte[(nwidth + npad8) * nheight];
                    fs.Read(this.rawpixels, 0, (nwidth + npad8) * nheight);
                    this.outputpixels = new byte[(nwidth + npad8) * nheight];
                    for (int i = 0; i < this.outputpixels.Length; i++)
                    {
                        byte b = this.rawpixels[i];
                        int index = b;
                        if (index < 0)
                        {
                            index += 256;
                        }
                        this.outputpixels[i] = (byte)this.mapping[index];
                    }
                    fout.Write(this.outputpixels, 0, (nwidth + npad8) * nheight);
                    fs.Close();
                    fout.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception in savebitmap!" + e);
            }
        }

        internal virtual byte[] returnaokpalette()
        {
            return this.aokpalette;
        }

        internal virtual byte[][] returnaoktable()
        {
            return this.aoktable;
        }

        internal virtual byte[][] returnrawpixels()
        {
            int w = this.rawpixels.Length / this.imgheight;
            //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            //ORIGINAL LINE: byte[][] frame = new byte[this.imgheight][this.imgwidth];
            byte[][] frame = RectangularArrays.RectangularbyteArray(this.imgheight, this.imgwidth);
            for (int i = 0; i < this.imgheight; i++)
            {
                for (int j = 0; j < this.imgwidth; j++)
                {
                    frame[this.imgheight - i - 1][j] = this.rawpixels[i * w + j];
                }
            }
            return frame;
        }
        /**********************wrong*******************************/
        public virtual void copybitmap(string infilename, string outfilename)
        {
            try
            {
                FileStream fin = new FileStream(infilename, FileMode.Open, FileAccess.Read);
                FileStream fout = new FileStream(outfilename, FileMode.Create, FileAccess.Write);
                int n =(int) fin.Length;//fin.available();
                byte[] b = new byte[n];
                fin.Read(b, 0, n);
                fout.Write(b, 0, n);
                fin.Close();
                fout.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception in copybitmap!" + e);
            }
        }

    }
}
