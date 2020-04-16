using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aok_Patch.patcher_
{
    public class slpReader
    {
        public string name;

        public string version = "2.0N";

        public int numframes;

        public string Comment = "ArtDesk 1.00 SLP Writer";

        public int playernumber = 0;

        public int mask = 8;

        public int outline1 = 8;

        public int outline2 = 8;

        internal int shadow = 131;

        public string sample = "50500.bmp";

        internal frame[] frames;

        public void Read(String s)
        {
            try
            {
                FileStream fs = new FileStream(s, FileMode.OpenOrCreate);
                int bytesRead = 0;
                int vlen = this.version.Length;
                byte[] ver = new byte[vlen];
                fs.Read(ver, 0, vlen);
                int nlen = 4;
                byte[] nfram = new byte[nlen];
                fs.Read(nfram, 0, nlen);
                this.numframes = (nfram[3] & 0xFF) << 24 | (
                  nfram[2] & 0xFF) << 16 | (
                  nfram[1] & 0xFF) << 8 | nfram[0] & 0xFF;
                Console.WriteLine("Number of frames: " + this.numframes);
                this.frames = new frame[this.numframes];
                for (int i = 0; i < this.numframes; i++)
                    this.frames[i] = new frame();
                int clen = this.Comment.Length;
                byte[] com = new byte[clen];
                fs.Read(com, 0, clen);
                /*check thos */
                //fs.Read();
                fs.ReadByte();
                bytesRead = 32;
                int j;
                for (j = 0; j < this.numframes; j++)
                {
                    int frame_data_offset_len = 4;
                    byte[] bframedo = new byte[frame_data_offset_len];
                    fs.Read(bframedo, 0, frame_data_offset_len);
                    (this.frames[j]).frame_data_offsets = (bframedo[3] & 0xFF) << 24 | (
                      bframedo[2] & 0xFF) << 16 | (
                      bframedo[1] & 0xFF) << 8 |
                      bframedo[0] &
                      0xFF;
                    int frame_outline_offset_len = 4;
                    byte[] bframeoo = new byte[frame_outline_offset_len];
                    fs.Read(bframeoo, 0, frame_outline_offset_len);
                    (this.frames[j]).frame_outline_offset = (bframeoo[3] & 0xFF) << 24 | (
                      bframeoo[2] & 0xFF) << 16 | (
                      bframeoo[1] & 0xFF) << 8 |
                      bframeoo[0] &
                      0xFF;
                    int palette_offset_len = 4;
                    byte[] bpal = new byte[palette_offset_len];
                    fs.Read(bpal, 0, palette_offset_len);
                    (this.frames[j]).palette_offset = (bpal[3] & 0xFF) << 24 | (
                      bpal[2] & 0xFF) << 16 | (
                      bpal[1] & 0xFF) << 8 | bpal[0] & 0xFF;
                    int properties_len = 4;
                    byte[] bprop = new byte[properties_len];
                    fs.Read(bprop, 0, properties_len);
                    (this.frames[j]).properties = (bprop[3] & 0xFF) << 24 | (
                      bprop[2] & 0xFF) << 16 | (
                      bprop[1] & 0xFF) << 8 | bprop[0] &
                      0xFF;
                    int width_len = 4;
                    byte[] bwidth = new byte[width_len];
                    fs.Read(bwidth, 0, width_len);
                    (this.frames[j]).width = (bwidth[3] & 0xFF) << 24 | (
                      bwidth[2] & 0xFF) << 16 | (
                      bwidth[1] & 0xFF) << 8 | bwidth[0] &
                      0xFF;
                    int height_len = 4;
                    byte[] bheight = new byte[height_len];
                    fs.Read(bheight, 0, height_len);
                    (this.frames[j]).height = (bheight[3] & 0xFF) << 24 | (
                      bheight[2] & 0xFF) << 16 | (
                      bheight[1] & 0xFF) << 8 | bheight[0] &
                      0xFF;
                    int anchorx_len = 4;
                    byte[] banchorx = new byte[anchorx_len];
                    fs.Read(banchorx, 0, anchorx_len);
                    (this.frames[j]).anchorx = (banchorx[3] & 0xFF) << 24 | (
                      banchorx[2] & 0xFF) << 16 | (
                      banchorx[1] & 0xFF) << 8 | banchorx[0] &
                      0xFF;
                    int anchory_len = 4;
                    byte[] banchory = new byte[anchory_len];
                    fs.Read(banchory, 0, anchory_len);
                    (this.frames[j]).anchory = (banchory[3] & 0xFF) << 24 | (
                      banchory[2] & 0xFF) << 16 | (
                      banchory[1] & 0xFF) << 8 | banchory[0] &
                      0xFF;
                    bytesRead += 32;
                }
                for (j = 0; j < this.numframes; j++)
                {
                    this.frames[j].initrowedge((this.frames[j]).height);
                    this.frames[j].initpic((this.frames[j]).width, (this.frames[j]).height);
                    int k;
                    for (k = 0; k < (this.frames[j]).height; k++)
                    {
                        byte[] browedge = new byte[2];
                        fs.Read(browedge, 0, 2);
                        bytesRead += 2;
                        ((this.frames[j]).edges[k]).left = (browedge[1] & 0xFF) << 8 |
                          browedge[0] & 0xFF;
                        if (((this.frames[j]).edges[k]).left > 1000)
                            ((this.frames[j]).edges[k]).left = (this.frames[j]).width;
                        fs.Read(browedge, 0, 2);
                        bytesRead += 2;
                        ((this.frames[j]).edges[k]).right = (browedge[1] & 0xFF) << 8 |
                          browedge[0] & 0xFF;
                        if (((this.frames[j]).edges[k]).right > 1000)
                            ((this.frames[j]).edges[k]).right = (this.frames[j]).width;
                    }
                    for (k = 0; k < (this.frames[j]).height; k++)
                    {
                        byte[] browedge = new byte[4];
                        fs.Read(browedge, 0, 4);
                        bytesRead += 4;
                        (this.frames[j]).commandoff[k] = (browedge[3] & 0xFF) << 24 | (
                          browedge[2] & 0xFF) << 16 | (
                          browedge[1] & 0xFF) << 8 |
                          browedge[0] & 0xFF;
                    }
                    for (k = 0; k < (this.frames[j]).height; k++)
                    {
                        this.frames[j].drawmask(k, ((this.frames[j]).edges[k]).left);
                        if (bytesRead != (this.frames[j]).commandoff[k])
                            Console.WriteLine("--Mismatch in command offset -- Read: " +
                                bytesRead +
                                "-should be " +
                                (this.frames[j]).commandoff[k] + " -");
                        bool flag = true;
                        do
                        {
                            byte[] commandarr = new byte[1];
                            fs.Read(commandarr, 0, 1);
                            bytesRead++;
                            byte commandbyte = commandarr[0];
                            int command = commandbyte & 0xF;
                            if (command == 0 || command == 4 || command == 8 || command == 12)
                            {
                                int length = (commandbyte & 0xFF) >> 2;
                                byte[] data = new byte[length];
                                fs.Read(data, 0, length);
                                bytesRead += length;
                                this.frames[j].drawbytearray(k, data);
                            }
                            else if (command == 1 || command == 5 || command == 9 || command == 13)
                            {
                                int length = (commandbyte & 0xFF) >> 2;
                                this.frames[j].drawmask(k, length);
                            }
                            else if (command == 2)
                            {
                                byte[] next = new byte[1];
                                fs.Read(next, 0, 1);
                                bytesRead++;
                                byte next_byte = next[0];
                                int length = ((commandbyte & 0xF0) << 4) + (next_byte & 0xFF);
                                byte[] data = new byte[length];
                                fs.Read(data, 0, length);
                                bytesRead += length;
                                this.frames[j].drawbytearray(k, data);
                            }
                            else if (command == 3)
                            {
                                byte[] next = new byte[1];
                                fs.Read(next, 0, 1);
                                bytesRead++;
                                byte next_byte = next[0];
                                int length = ((commandbyte & 0xF0) << 4) + (next_byte & 0xFF);
                                this.frames[j].drawmask(k, length);
                            }
                            else if (command == 6)
                            {
                                int length, high4bits = commandbyte & 0xF0;
                                if (high4bits == 0)
                                {
                                    //length = fs.Read();
                                    length = fs.ReadByte();
                                    bytesRead++;
                                }
                                else
                                {
                                    length = (high4bits & 0xFF) >> 4;
                                }
                                byte[] data = new byte[length];
                                fs.Read(data, 0, length);
                                bytesRead += length;
                                this.frames[j].drawplayercolors(k, data, this.playernumber);
                            }
                            else if (command == 7)
                            {
                                int length, high4bits = commandbyte & 0xF0;
                                if (high4bits == 0)
                                {
                                    //length = fs.Read();
                                    length = fs.ReadByte();
                                    bytesRead++;
                                }
                                else
                                {
                                    length = (high4bits & 0xFF) >> 4;
                                }
                                byte[] next = new byte[1];
                                fs.Read(next, 0, 1);
                                bytesRead++;
                                byte next_byte = next[0];
                                int color = next_byte;
                                this.frames[j].fill(k, next, length);
                            }
                            else if (command == 10)
                            {
                                int length, high4bits = commandbyte & 0xF0;
                                if (high4bits == 0)
                                {
                                    //length = fs.Read();
                                    length = fs.ReadByte();
                                    bytesRead++;
                                }
                                else
                                {
                                    length = (high4bits & 0xFF) >> 4;
                                }
                                byte[] next = new byte[1];
                                fs.Read(next, 0, 1);
                                bytesRead++;
                                byte next_byte = next[0];
                                int color = next_byte;
                                this.frames[j].playercolorfill(k, next, length, this.playernumber);
                            }
                            else if (command == 11)
                            {
                                int length, high4bits = commandbyte & 0xF0;
                                if (high4bits == 0)
                                {
                                    //length = fs.Read();
                                    length = fs.ReadByte();
                                    bytesRead++;
                                }
                                else
                                {
                                    length = (high4bits & 0xFF) >> 4;
                                }
                                this.frames[j].drawshadowpixels(k, length);
                            }
                            else if (command == 14)
                            {
                                if (commandbyte == 78 || commandbyte == 110)
                                {
                                    if (commandbyte == 78)
                                    {
                                        this.frames[j].drawselectionmask(k, 1);
                                    }
                                    else
                                    {
                                        this.frames[j].drawselectionmask2(k, 1);
                                    }
                                }
                                else if (commandbyte == 94 || commandbyte == 126)
                                {
                                    byte[] next = new byte[1];
                                    fs.Read(next, 0, 1);
                                    bytesRead++;
                                    byte next_byte = next[0];
                                    int length = next_byte;
                                    if (commandbyte == 94)
                                    {
                                        this.frames[j].drawselectionmask(k, length);
                                    }
                                    else
                                    {
                                        this.frames[j].drawselectionmask2(k, length);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Extended command unrecognized");
                                }
                            }
                            else if (command == 15)
                            {
                                flag = false;
                            }
                        } while (flag);
                        this.frames[j].drawmask(k, ((this.frames[j]).edges[k]).right);
                    }
                }
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught exception in Read slp! ");
                Console.WriteLine(ex);
            }
        }
        public virtual void save(string outdir)
        {
            for (int i = 0; i < this.numframes; i++)
            {
                string filename = outdir.ToString() + this.name + ".bmp";
                this.frames[i].converttobitmap(filename, this.mask, this.outline1, this.outline2, this.shadow, this.sample);
            }
        }

        public virtual void savecsv(string outdir)
        {
            string filename = outdir.ToString() + "Frames" + this.name + "\\" + this.name + ".csv";
            string[] lines = new string[this.numframes];
            for (int i = 0; i < this.numframes; i++)
            {
                lines[i] = ((this.frames[i]).anchorx).ToString() + ", " + (this.frames[i]).anchory;
            }
            try
            {
                StreamWriter @out = new StreamWriter(filename);
                for (int j = 0; j < this.numframes; j++)
                {
                    @out.Write(lines[j].ToString() + "\r\n");
                }
                @out.Close();
            }
            catch (IOException)
            {
            }
        }

        internal virtual void timedelay(double num)
        {
            long now, start = DateTimeHelper.CurrentUnixTimeMillis();
            long wait = (long)(1000.0D * num);
            do
            {
                now = DateTimeHelper.CurrentUnixTimeMillis();
            } while (now - start < wait);
        }

        //Helper class added by Java to C# Converter:

        //---------------------------------------------------------------------------------------------------------
        //	Copyright © 2007 - 2020 Tangible Software Solutions, Inc.
        //	This class can be used by anyone provided that the copyright notice remains intact.
        //
        //	This class is used to replace calls to Java's System.currentTimeMillis with the C# equivalent.
        //	Unix time is defined as the number of seconds that have elapsed since midnight UTC, 1 January 1970.
        //---------------------------------------------------------------------------------------------------------
        internal static class DateTimeHelper
        {
            private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            public static long CurrentUnixTimeMillis()
            {
                return (long)(System.DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
            }
        }

    }

}
