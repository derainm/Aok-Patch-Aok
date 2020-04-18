using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aok_Patch.patcher_
{
    public class slpWriter
    {
        int shadow = 131;
        int outline1 = 8;
        int outline2 = 124;
        int outputmask = 0;
        String csvfile = "dunno";
        bool cvtused = false;
        bool plcolorused = false;
        public String maskfile = ""; int numframes;
        frame[] frames;

        public void initframes(int n)
        {
            this.numframes = n;
            this.frames = new frame[n];
        }
        int[] mapping; String version; String comment;
        public void Writemasks(String inputfile, String outputfile, int choice)
        {
            int i3, i2, i1, n, m, j, k;
            imagehandler img = new imagehandler();
            img.loadbitmap(inputfile, 0);
            byte[][] f = img.returnrawpixels();
            int width = (f[0]).Length;
            int height = f.Length;


            int[][] newframe = RectangularArrays.RectangularIntArray(height, width);
            for (int i = 0; i < height; i++)
            {
                for (int i4 = 0; i4 < width; i4++)
                {
                    newframe[i][i4] = f[i][i4] & 0xFF;
                }
            }
            int mask = newframe[height - 1][0];
            switch (choice)
            {
                case 1:
                    for (i3 = 0; i3 < height; i3++)
                    {
                        for (int i4 = 0; i4 < width; i4++)
                        {
                            if (newframe[i3][i4] == mask)
                            {
                                newframe[i3][i4] = this.outputmask;
                            }
                            else
                            {
                                newframe[i3][i4] = 255;
                            }
                        }
                    }
                    break;
                case 2:
                    for (i3 = 0; i3 < height; i3++)
                    {
                        int i4;
                        for (i4 = 0; i4 < width - 1; i4++)
                        {
                            if (newframe[i3][i4] == mask && newframe[i3][i4 + 1] != mask && i4 + 1 != width - 1)
                            {
                                newframe[i3][i4] = -2;
                            }
                        }

                        for (i4 = width - 1; i4 > 0; i4--)
                        {
                            if (newframe[i3][i4] == mask && newframe[i3][i4 - 1] != mask && i4 + 1 != width - 1)
                            {
                                newframe[i3][i4] = -2;
                            }
                        }
                    }
                    for (i2 = 0; i2 < width; i2++)
                    {
                        int i4;
                        for (i4 = 0; i4 <= height - 2; i4++)
                        {
                            if (newframe[i4][i2] == mask && newframe[i4 + 1][i2] != mask && newframe[i4 + 1][i2] != -2)
                            {
                                newframe[i4][i2] = -2;
                            }
                        }
                        for (i4 = height - 1; i4 > 0; i4--)
                        {
                            if (newframe[i4][i2] == mask && newframe[i4 - 1][i2] != mask && newframe[i4 - 1][i2] != -2)
                            {
                                newframe[i4][i2] = -2;
                            }
                        }
                    }
                    for (i1 = 0; i1 < height; i1++)
                    {
                        for (int i4 = 0; i4 < width; i4++)
                        {
                            if (newframe[i1][i4] != -2)
                            {
                                newframe[i1][i4] = this.outputmask;
                            }
                        }
                    }
                    break;
                case 3:
                    for (i1 = 0; i1 < height; i1++)
                    {
                        for (int i4 = 0; i4 < width; i4++)
                        {
                            if (newframe[i1][i4] < 16 || newframe[i1][i4] > 23)
                            {
                                newframe[i1][i4] = this.outputmask;
                            }
                        }
                    }
                    break;
                case 4:
                    for (i1 = 0; i1 < height; i1++)
                    {
                        for (int i4 = 0; i4 < width; i4++)
                        {
                            if (newframe[i1][i4] == 131)
                            {
                                newframe[i1][i4] = 131;
                            }
                            else
                            {
                                newframe[i1][i4] = this.outputmask;
                            }
                        }
                    }
                    break;
                case 5:
                    for (i1 = 0; i1 < height; i1++)
                    {
                        int i4;
                        for (i4 = 0; i4 < width - 1; i4++)
                        {
                            if (newframe[i1][i4] == mask && newframe[i1][i4 + 1] != mask && i4 + 1 != width - 1)
                            {
                                newframe[i1][i4] = -2;
                            }
                        }
                        for (i4 = width - 1; i4 > 0; i4--)
                        {
                            if (newframe[i1][i4] == mask && newframe[i1][i4 - 1] != mask && i4 + 1 != width - 1)
                            {
                                newframe[i1][i4] = -2;
                            }
                        }
                    }
                    for (n = 0; n < width; n++)
                    {
                        int i4;
                        for (i4 = 0; i4 <= height - 2; i4++)
                        {
                            if (newframe[i4][n] == mask && newframe[i4 + 1][n] != mask && newframe[i4 + 1][n] != -2)
                            {
                                newframe[i4][n] = -2;
                            }
                        }

                        for (i4 = height - 2; i4 > 0; i4--)
                        {
                            if (newframe[i4][n] == mask && newframe[i4 - 1][n] != mask && newframe[i4 - 1][n] != -2)
                            {
                                newframe[i4][n] = -2;
                            }
                        }
                    }
                    for (m = 0; m < height; m++)
                    {
                        int i4;
                        for (i4 = 0; i4 < width - 1; i4++)
                        {
                            if (newframe[m][i4] == mask && newframe[m][i4 + 1] != mask && i4 + 1 != width - 1)
                            {
                                newframe[m][i4] = -3;
                            }
                        }
                        for (i4 = width - 1; i4 > 0; i4--)
                        {
                            if (newframe[m][i4] == mask && newframe[m][i4 - 1] != mask && i4 + 1 != width - 1)
                            {
                                newframe[m][i4] = -3;
                            }
                        }
                    }
                    for (j = 0; j < width; j++)
                    {
                        int i4;
                        for (i4 = 0; i4 <= height - 2; i4++)
                        {
                            if (newframe[i4][j] == mask && newframe[i4 + 1][j] != mask && newframe[i4 + 1][j] != -3)
                            {
                                newframe[i4][j] = -3;
                            }
                        }

                        for (i4 = height - 2; i4 > 0; i4--)
                        {
                            if (newframe[i4][j] == mask && newframe[i4 - 1][j] != mask && newframe[i4 - 1][j] != -3)
                            {
                                newframe[i4][j] = -3;
                            }
                        }
                    }
                    for (k = 0; k < height; k++)
                    {
                        for (int i4 = 0; i4 < width; i4++)
                        {
                            if (newframe[k][i4] != -3)
                            {
                                newframe[k][i4] = this.outputmask;
                            }
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Unrecognized command");
                    break;
            }
            Aokbitmap b = new Aokbitmap(this.outputmask, this.outline1, this.outline2, this.shadow);
            b.Write(outputfile, newframe, f[0].Length, f.Length);
        }



        public virtual void getframe(int num, string path, string filename, bool msk, bool o1, bool o2, bool pl, bool sh)
        {
            imagehandler img = new imagehandler();

            img.loadbitmap(path.ToString() + filename, 0);
            byte[][] f = img.returnrawpixels();
            int width = (f[0]).Length;
            int height = f.Length;

            int[][] newframe = RectangularArrays.RectangularIntArray(height, width);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    newframe[i][j] = f[i][j] & 0xFF;
                }
            }

            if (msk)
            {
                string str = this.maskfile;
                if (File.Exists(str))
                    img.loadbitmap(str, 0);
                f = img.returnrawpixels();
                //replace black pixel by transparent pixel
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        int pixel = f[j][k] & 0xFF;
                        if (pixel == 0)
                        {
                            newframe[j][k] = -1;
                        }
                        //var test = newframe[1584][32];
                    }
                }
            }
            if (o1)
            {
                string str = path.ToString() + "U" + filename.Substring(1);
                img.loadbitmap(str, 0);
                f = img.returnrawpixels();

                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        int pixel = f[j][k] & 0xFF;
                        if (pixel != 0)
                        {
                            newframe[j][k] = -2;
                        }
                    }
                }
            }
            if (o2)
            {
                string str = path.ToString() + "O" + filename.Substring(1);
                img.loadbitmap(str, 0);
                f = img.returnrawpixels();

                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        int pixel = f[j][k] & 0xFF;
                        if (pixel != 0)
                        {
                            newframe[j][k] = -3;
                        }
                    }
                }
            }
            if (pl)
            {
                string str = path.ToString() + "P" + filename.Substring(1);
                img.loadbitmap(str, 0);
                f = img.returnrawpixels();

                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        int pixel = f[j][k] & 0xFF;
                        if (pixel != 0)
                        {
                            newframe[j][k] = pixel;
                        }
                    }
                }

                this.plcolorused = true;
            }

            if (sh)
            {
                string str = path.ToString() + "H" + filename.Substring(1);
                img.loadbitmap(str, 0);
                f = img.returnrawpixels();
                Console.WriteLine("Entering shadow stuff");

                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        int pixel = f[j][k] & 0xFF;
                        if (pixel != 0)
                        {
                            newframe[j][k] = -4;
                        }
                    }
                }
            }
            if(this.frames ==null)
                this.frames = new frame[num+1];
            this.frames[num] = new frame();
            (this.frames[num]).width = width;
            (this.frames[num]).height = height;
            (this.frames[num]).picture = newframe;

            if (this.cvtused)
            {
                this.frames[num].convertcvt(this.mapping);
            }
        }

        //Helper class added by Java to C# Converter:

        //----------------------------------------------------------------------------------------
        //	Copyright © 2007 - 2020 Tangible Software Solutions, Inc.
        //	This class can be used by anyone provided that the copyright notice remains intact.
        //
        //	This class includes methods to convert Java rectangular arrays (jagged arrays
        //	with inner arrays of the same Length).
        //----------------------------------------------------------------------------------------
        internal static class RectangularArrays
        {
            public static int[][] RectangularIntArray(int size1, int size2)
            {
                int[][] newArray = new int[size1][];
                for (int array1 = 0; array1 < size1; array1++)
                {
                    newArray[array1] = new int[size2];
                }

                return newArray;
            }
        }

        public virtual void convertcvt(string cvtname)
        {
            try
            {
                this.cvtused = true;
                StreamReader @in = new StreamReader(cvtname);
                this.mapping = new int[256];
                for (int i = 0; i < 256; i++)
                {
                    this.mapping[i] = -11;
                }


                string str = @in.ReadLine();
                if (str.Equals("<MPS PALETTE REMAP>"))
                {
                    Console.WriteLine("Recognized as MPS cvt file");
                }
                else
                {
                    Console.WriteLine("Not recognized as MPS cvt file");
                }

                while (!string.ReferenceEquals((str = @in.ReadLine()), null))
                {
                    int pos = str.IndexOf(" ", StringComparison.Ordinal);
                    int left = int.Parse(str.Substring(0, pos));
                    int right = int.Parse(str.Substring(pos + 1));


                    this.mapping[left] = right;
                }

                @in.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception in applying CVT! ");
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
        }

        public virtual void compress()
        {
            for (int i = 0; i < this.numframes; i++)
            {
                this.frames[i].initrowedge((this.frames[i]).height);
                int j;
                for (j = 0; j < (this.frames[i]).height; j++)
                {
                    int left = 0;
                    for (int k = 0; k < (this.frames[i]).width && (this.frames[i]).picture[j][k] == -1; k++)
                    {

                        left++;
                    }
                    int right = 0;
                    for (int m = (this.frames[i]).width - 1; m >= 0 && (this.frames[i]).picture[j][m] == -1; m--)
                    {

                        right++;
                    }
                    if (left == (this.frames[i]).width)
                    {
                        right = 0;
                    }
                  (this.frames[i]).edges[j] = new rowedge(left, right);
                }

                this.frames[i].initcommands();

                for (j = 0; j < (this.frames[i]).height; j++)
                {
                    int prev = -5;

                    int start = ((this.frames[i]).edges[j]).left;
                    int end = (this.frames[i]).width - ((this.frames[i]).edges[j]).right - 1;

                    int pos = start;
                    int count = 0;
                    string type = "null";
                    int colorLength = -1;
                    int colorstart = -1;

                    while (pos <= end)
                    {
                        int currentpixel = (this.frames[i]).picture[j][pos];

                        if (currentpixel != prev && currentpixel < 0)
                        {
                            if (!type.Equals("null") && !type.Equals("color"))
                            {

                                if (type.Equals("skip"))
                                {
                                    this.frames[i].cmdskip(count);
                                }
                                else if (type.Equals("outline1"))
                                {
                                    this.frames[i].cmdoutline1(count);
                                }
                                else if (type.Equals("outline2"))
                                {
                                    this.frames[i].cmdoutline2(count);
                                }
                                else if (type.Equals("shadowpix"))
                                {
                                    this.frames[i].cmdshadowpix(count);
                                }
                            }

                            if (type.Equals("color"))
                            {
                                byte[] bytearray = new byte[colorLength];
                                for (int k = 0; k < colorLength; k++)
                                {
                                    bytearray[k] = (byte)(this.frames[i]).picture[j][k + colorstart];
                                }

                                this.frames[i].cmdcolorblock(bytearray, this.plcolorused);

                                colorLength = -1;
                                colorstart = -1;
                            }
                            if (currentpixel == -1)
                            {
                                type = "skip";
                            }
                            else if (currentpixel == -2)
                            {
                                type = "outline1";
                            }
                            else if (currentpixel == -3)
                            {
                                type = "outline2";
                            }
                            else if (currentpixel == -4)
                            {
                                type = "shadowpix";
                            }
                            else
                            {
                                Console.WriteLine("Unknown pixel");
                            }

                            prev = currentpixel;
                            count = 1;
                        }
                        else if (currentpixel != prev && prev < 0)
                        {

                            if (!type.Equals("null"))
                            {

                                if (type.Equals("skip"))
                                {
                                    this.frames[i].cmdskip(count);
                                }
                                else if (type.Equals("outline1"))
                                {
                                    this.frames[i].cmdoutline1(count);
                                }
                                else if (type.Equals("outline2"))
                                {
                                    this.frames[i].cmdoutline2(count);
                                }
                                else if (type.Equals("shadowpix"))
                                {
                                    this.frames[i].cmdshadowpix(count);
                                }
                            }

                            type = "color";
                            prev = currentpixel;
                            count = 1;
                            colorstart = pos;
                            colorLength = 1;
                        }
                        else if (type.Equals("color"))
                        {


                            colorLength++;
                            prev = currentpixel;
                            count = 1;
                        }
                        else
                        {
                            count++;
                        }
                        pos++;
                    }
                    if (type.Equals("color"))
                    {

                        byte[] bytearray = new byte[colorLength];
                        for (int k = 0; k < colorLength; k++)
                        {
                            bytearray[k] = (byte)(this.frames[i]).picture[j][k + colorstart];
                        }

                        this.frames[i].cmdcolorblock(bytearray, this.plcolorused);

                        colorLength = -1;
                        colorstart = -1;

                    }
                    else if (type.Equals("skip"))
                    {
                        this.frames[i].cmdskip(count);
                    }
                    else if (type.Equals("outline1"))
                    {
                        this.frames[i].cmdoutline1(count);
                    }
                    else if (type.Equals("outline2"))
                    {
                        this.frames[i].cmdoutline2(count);
                    }
                    else if (type.Equals("shadowpix"))
                    {
                        this.frames[i].cmdshadowpix(count);
                    }



                    this.frames[i].cmdeol();
                }
            }
        }

        public virtual void Write(string filename)
        {
            try
            {
                this.version = "2.0N";
                this.comment = "ArtDesk 1.00 SLP Writer ";


                Console.WriteLine("Number of frames " + this.numframes);
                //var outStream = new MemoryStream();
                FileStream fout = new FileStream(filename, FileMode.Create, FileAccess.Write);
                BinaryWriter writer = new BinaryWriter(fout);
                //fout.CopyTo(outStream);
                
                //var writer = new BinaryWriter(outStream);

                byte[] bv = Encoding.ASCII.GetBytes(this.version);
                writer.Write(bv);
                byte[] bnumfr = bytefromint(this.numframes);
                writer.Write(bnumfr);

                byte[] bcomment = Encoding.ASCII.GetBytes(this.comment);
                writer.Write(bcomment);
                int pointer = 32;
                pointer += 32 * this.numframes;
                int i;
                for (i = 0; i < this.numframes; i++)
                {
                    (this.frames[i]).frame_outline_offset = pointer;
                    this.frames[i].computecommandbytes();




                    pointer += (this.frames[i]).height * 4;
                    (this.frames[i]).frame_data_offsets = pointer;

                    pointer += (this.frames[i]).height * 4;
                    for (int j = 0; j < (this.frames[i]).height; j++)
                    {

                        (this.frames[i]).commandoff[j] = pointer;
                        pointer += this.frames[i].linecommandLength(j);
                    }
                }

                for (i = 0; i < this.numframes; i++)
                {
                    this.frames[i].setanchor(0, 0);
                }


                for (i = 0; i < this.numframes; i++)
                {
                    this.frames[i].computeframeheader();

                    byte[] bframe_data_offset = bytefromint((this.frames[i]).frame_data_offsets);
                    writer.Write(bframe_data_offset);

                    byte[] bframe_outline_offset = bytefromint((this.frames[i]).frame_outline_offset);
                    writer.Write(bframe_outline_offset);

                    byte[] bpalette = bytefromint((this.frames[i]).palette_offset);
                    writer.Write(bpalette);

                    byte[] bproperties = bytefromint((this.frames[i]).properties);
                    writer.Write(bproperties);

                    byte[] bwidth = bytefromint((this.frames[i]).width);
                    writer.Write(bwidth);

                    byte[] bheight = bytefromint((this.frames[i]).height);
                    writer.Write(bheight);

                    byte[] banchorx = bytefromint((this.frames[i]).anchorx);
                    writer.Write(banchorx);

                    byte[] banchory = bytefromint((this.frames[i]).anchory);
                    writer.Write(banchory);
                }

                for (i = 0; i < this.numframes; i++)
                {
                    int j;
                    for (j = 0; j < (this.frames[i]).height; j++)
                    {

                        byte[] bleft = bytefromshort(((this.frames[i]).edges[j]).left);
                        byte[] bright = bytefromshort(((this.frames[i]).edges[j]).right);

                        writer.Write(bleft);
                        writer.Write(bright);
                    }

                    for (j = 0; j < (this.frames[i]).height; j++)
                    {

                        byte[] bcommandoff = bytefromint((this.frames[i]).commandoff[j]);

                        writer.Write(bcommandoff);
                    }

                    for (j = 0; j < (this.frames[i]).numcommands; j++)
                    {
                        string type = ((this.frames[i]).commands[j]).type;
                        if (type.Equals("one"))
                        {
                            byte[] cmd = new byte[] { ((this.frames[i]).commands[j]).cmdbyte };

                            writer.Write(cmd);
                        }
                        else if (type.Equals("two Length"))
                        {
                            byte[] cmdLength = new byte[] { ((this.frames[i]).commands[j]).cmdbyte, ((this.frames[i]).commands[j]).next_byte };

                            writer.Write(cmdLength);
                        }
                        else if (type.Equals("two data"))
                        {
                            byte[] cmd = new byte[] { ((this.frames[i]).commands[j]).cmdbyte };

                            writer.Write(cmd);

                            writer.Write(((this.frames[i]).commands[j]).data);
                        }
                        else if (type.Equals("three"))
                        {
                            byte[] cmdLength = new byte[] { ((this.frames[i]).commands[j]).cmdbyte, ((this.frames[i]).commands[j]).next_byte };

                            writer.Write(cmdLength);

                            writer.Write(((this.frames[i]).commands[j]).data);
                        }
                        else
                        {
                            Console.WriteLine("Whoa, unrecognized type");
                        }
                    }
                }
               
                writer.Close();
                fout.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception in Write slp! ");
                Console.WriteLine(e.ToString());
                Console.Write(e.StackTrace);
            }
        }

        internal virtual byte[] bytefromint(int i)
        {
            return new byte[] { (byte)i, (byte)(i >> 8), (byte)(i >> 16), (byte)(i >> 24) };
        }

        internal virtual byte[] bytefromshort(int i)
        {
            return new byte[] { (byte)i, (byte)(i >> 8) };
        }


    }
}



