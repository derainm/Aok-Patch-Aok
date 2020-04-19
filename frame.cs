using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aok_Patch.patcher_
{
    class frame
    {
       public int frame_data_offsets;

        public int frame_outline_offset;

        public int palette_offset;

        public int properties;

        public int width;

        public int height;

        public int anchorx;

        public int anchory;

        public rowedge[] edges;

        public int[] commandoff;

        public commandclass[] commands;

        public int numcommands;

        public int numcommandbytes;

        public int[][] picture;

        public int[] pointer;
        internal virtual void initrowedge(int n)
        {
            this.edges = new rowedge[n];
            for (int i = 0; i < n; i++)
            {
                this.edges[i] = new rowedge();
            }
            this.commandoff = new int[n];
            this.pointer = new int[n];
        }

        internal virtual void initpic(int w, int h)
        {

            this.picture = RectangularArrays.RectangularIntArray(h, w);
        }

        internal virtual void initcommands()
        {
            this.commands = new commandclass[500000];
            this.numcommands = 0;
        }

        internal virtual void drawmask(int row, int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.picture[row][this.pointer[row]] = -1;
                this.pointer[row] = this.pointer[row] + 1;
                //this.pointer[row];
            }
        }

        internal virtual void drawselectionmask(int row, int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.picture[row][this.pointer[row]] = -2;
                this.pointer[row] = this.pointer[row] + 1;
                //this.pointer[row];
            }
        }

        internal virtual void drawselectionmask2(int row, int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.picture[row][this.pointer[row]] = -3;
                this.pointer[row] = this.pointer[row] + 1;
                //this.pointer[row];
            }
        }

        internal virtual void drawshadowpixels(int row, int n)
        {
            for (int i = 0; i < n; i++)
            {
                this.picture[row][this.pointer[row]] = -4;
                this.pointer[row] = this.pointer[row] + 1;
                //this.pointer[row];
            }
        }

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

  internal virtual void drawbytearray(int row, byte[] data)
    {
        int len = data.Length;
        for (int i = 0; i < len; i++)
        {
            int val = data[i];
            if (val < 0)
            {
                val += 256;
            }
            if (this.pointer[row] < this.width)
            {
                this.picture[row][this.pointer[row]] = val;
                this.pointer[row] = this.pointer[row] + 1;
            }
        }
    }

    internal virtual void drawplayercolors(int row, byte[] data, int player)
    {
        int len = data.Length;
        for (int i = 0; i < len; i++)
        {
            int val = data[i];
            if (val < 0)
            {
                val += 256;
            }
            val = player * 16 + 16 + val;
            this.picture[row][this.pointer[row]] = val;
            this.pointer[row] = this.pointer[row] + 1;
            //this.pointer[row];
        }
    }

    internal virtual void fill(int row, byte[] color, int n)
    {
        for (int i = 0; i < n; i++)
        {
            int val = color[0];
            if (val < 0)
            {
                val += 256;
            }
            this.picture[row][this.pointer[row]] = val;
            this.pointer[row] = this.pointer[row] + 1;
            //this.pointer[row];
        }
    }

    internal virtual void playercolorfill(int row, byte[] color, int n, int player)
    {
        for (int i = 0; i < n; i++)
        {
            int val = color[0];
            if (val < 0)
            {
                val += 256;
            }
            val = player * 16 + 16 + val;
            this.picture[row][this.pointer[row]] = val;
            this.pointer[row] = this.pointer[row] + 1;
            //this.pointer[row];
        }
    }

    internal virtual void display()
    {
        for (int i = 0; i < this.height; i++)
        {
            Console.WriteLine("line: " + i);
            for (int j = 0; j < this.width; j++)
            {
                Console.Write(this.picture[i][j].ToString() + " ");
            }
            Console.WriteLine();
        }
    }
    internal virtual void converttobitmap(string filename, int mask, int outline1, int outline2, int shadow, string samp)
    {
        Aokbitmap b = new Aokbitmap(mask, outline1, outline2, shadow);
        b.sample = samp;
        b.Write(filename, this.picture, this.width, this.height);
         
    }

    internal virtual void cmdcolorblock(byte[] array, bool playercolorused)
    {
        int start = 0;
        int Length = 0;
        int playerstart = 0;
        int playerLength = 0;
        bool isprevplayer = false;
        int pos = 0;
        isprevplayer = (array[pos] >= 16 && array[pos] <= 23 && playercolorused);
        if (isprevplayer)
        {
            playerstart = 0;
            playerLength = 1;
        }
        else
        {
            start = 0;
            Length = 1;
        }
        pos = 1;
        while (pos < array.Length)
        {
            bool isplayer = (array[pos] >= 16 && array[pos] <= 23 && playercolorused);
            if (isplayer && !isprevplayer)
            {
                byte[] bytearray = new byte[Length];
                for (int i = 0; i < Length; i++)
                {
                    bytearray[i] = array[start + i];
                }
                cmdcolor(bytearray);
                playerstart = pos;
                playerLength = 1;
                isprevplayer = true;
            }
            else if (!isplayer && isprevplayer)
            {
                byte[] bytearray = new byte[playerLength];
                for (int i = 0; i < playerLength; i++)
                {
                    bytearray[i] = array[playerstart + i];
                }
                cmdplayercolor(bytearray);
                start = pos;
                Length = 1;
                isprevplayer = false;
            }
            else if (isplayer)
            {
                playerLength++;
            }
            else
            {
                Length++;
            }
            pos++;
        }
        if (isprevplayer)
        {
            byte[] bytearray = new byte[playerLength];
            for (int i = 0; i < playerLength; i++)
            {
                bytearray[i] = array[playerstart + i];
            }
            cmdplayercolor(bytearray);
        }
        else
        {
            byte[] bytearray = new byte[Length];
            for (int i = 0; i < Length; i++)
            {
                bytearray[i] = array[start + i];
            }
            cmdcolor(bytearray);
        }
    }

    internal virtual void cmdplayercolor(byte[] array)
    {
        byte command = 6;
        int Length = array.Length;
        byte[] newarray = new byte[Length];
        if (Length >= 256)
        {
            int left, right;
            if (Length % 2 == 0)
            {
                left = Length / 2;
                right = Length / 2;
            }
            else
            {
                left = (Length - 1) / 2;
                right = (Length + 1) / 2;
            }
            byte[] leftarray = new byte[left];
            byte[] rightarray = new byte[right];
            int k;
            for (k = 0; k < left; k++)
            {
                leftarray[k] = array[k];
            }
            for (k = 0; k < right; k++)
            {
                rightarray[k] = array[left + k - 1];
            }
            cmdplayercolor(leftarray);
            cmdplayercolor(rightarray);
            return;
        }
        byte next_byte = (byte)Length;
        for (int i = 0; i < array.Length; i++)
        {
            newarray[i] = unchecked((byte)((array[i] & 0xFF) - 16));
        }
        this.commands[this.numcommands] = new commandclass(command, next_byte, newarray);
        this.numcommands++;
    }

  internal virtual void cmdcolor(byte[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            ;
        }
        int Length = array.Length;
        if (Length >= 64)
        {
            int left, right;
            if (Length % 2 == 0)
            {
                left = Length / 2;
                right = Length / 2;
            }
            else
            {
                left = (Length - 1) / 2;
                right = (Length + 1) / 2;
            }
            byte[] leftarray = new byte[left];
            byte[] rightarray = new byte[right];
            int k;
            for (k = 0; k < left; k++)
            {
                leftarray[k] = array[k];
            }
            for (k = 0; k < right; k++)
            {
                rightarray[k] = array[left + k - 1];
            }
            cmdcolor(leftarray);
            cmdcolor(rightarray);
            return;
        }
        byte command = (byte)(Length << 2);
        int commandnibbleleft = command & 0xC;
        this.commands[this.numcommands] = new commandclass(command, array);
        this.numcommands++;
    }

    internal virtual void cmdskip(int n)
    {
        int Length = n;
        if (Length >= 64)
        {
            int left, right;
            if (n % 2 == 0)
            {
                left = n / 2;
                right = n / 2;
            }
            else
            {
                left = (n - 1) / 2;
                right = (n + 1) / 2;
            }
            cmdskip(left);
            cmdskip(right);
            return;
        }
        byte command = (byte)(Length << 2);
        int commandnibbleleft = command & 0xC;
        if (commandnibbleleft == 0)
        {
            command = unchecked((byte)((command & 0xFF) + 1));
        }
        else if (commandnibbleleft == 4)
        {
            command = unchecked((byte)((command & 0xFF) + 1));
        }
        else if (commandnibbleleft == 8)
        {
            command = unchecked((byte)((command & 0xFF) + 1));
        }
        else if (commandnibbleleft == 12)
        {
            command = unchecked((byte)((command & 0xFF) + 1));
        }
        else
        {
            Console.WriteLine("uh oh");
        }
        this.commands[this.numcommands] = new commandclass(command);
        this.numcommands++;
    }

    internal virtual void cmdoutline1(int n)
    {
        int Length = n;
        if (Length == 1)
        {
            byte command = 78;
            this.commands[this.numcommands] = new commandclass(command);
            this.numcommands++;
        }
        else
        {
            byte command = 94;
            byte next_byte = (byte)Length;
            this.commands[this.numcommands] = new commandclass(command, next_byte);
            this.numcommands++;
        }
    }

    internal virtual void cmdoutline2(int n)
    {
        int Length = n;
        if (Length == 1)
        {
            byte command = 110;
            this.commands[this.numcommands] = new commandclass(command);
            this.numcommands++;
        }
        else
        {
            byte command = 126;
            byte next_byte = (byte)Length;
            this.commands[this.numcommands] = new commandclass(command, next_byte);
            this.numcommands++;
        }
    }

  internal virtual void cmdshadowpix(int n)
    {
        byte command = 11;
        int Length = n;
        if (Length >= 256)
        {
            int left, right;
            if (n % 2 == 0)
            {
                left = n / 2;
                right = n / 2;
            }
            else
            {
                left = (n - 1) / 2;
                right = (n + 1) / 2;
            }
            cmdshadowpix(left);
            cmdshadowpix(right);
            return;
        }
        byte next_byte = (byte)Length;
        this.commands[this.numcommands] = new commandclass(command, next_byte);
        this.numcommands++;
    }

    internal virtual void cmdeol()
    {
        byte command = 15;
        this.commands[this.numcommands] = new commandclass(command);
        this.numcommands++;
    }

    internal virtual void displaycommands()
    {
        int linenum = 0;
        int count = 0;
        Console.WriteLine("----------------------Line 0");
        while (count < this.numcommands)
        {
            this.commands[count].print();
            if ((this.commands[count]).cmdbyte == 15)
            {
                linenum++;
                Console.WriteLine("----------------------Line" + linenum);
            }
            count++;
        }
    }

    internal virtual void convertcvt(int[] array)
    {
        for (int i = 0; i < this.picture.Length; i++)
        {
            for (int j = 0; j < (this.picture[0]).Length; j++)
            {
                for (int k = 0; k < 256; k++)
                {
                    if (this.picture[i][j] == k && array[k] >= 0)
                    {
                        this.picture[i][j] = array[k];
                        if (array[k] == 131)
                        {
                            this.picture[i][j] = -4;
                        }
                    }
                }
            }
        }
    }

    internal virtual string bytestring(byte b)
    {
        int num = b;
        string str = "";
        for (int i = 1; i <= 8; i++)
        {
            int r = num % 2;
            str = r.ToString() + str;
            num /= 2;
        }
        return str;
    }

    internal virtual void computeframeheader()
    {
        this.palette_offset = 0;
        this.properties = 24;
    }

    internal virtual void setanchor(int a, int b)
    {
        this.anchorx = a;
        this.anchory = b;
    }

  internal virtual void computecommandbytes()
    {
        int total = 0;
        for (int i = 0; i < this.numcommands; i++)
        {
            string str = (this.commands[i]).type;
            if (str.Equals("one"))
            {
                total++;
            }
            else if (str.Equals("two Length"))
            {
                total += 2;
            }
            else if (str.Equals("two data"))
            {
                total += 1 + (this.commands[i]).data.Length;
            }
            else if (str.Equals("three"))
            {
                total += 2 + (this.commands[i]).data.Length;
            }
            else
            {
                Console.WriteLine("Whoa, weird command.type in computecommandbytes");
            }
        }
        this.numcommandbytes = total;
    }

    internal virtual int linecommandLength(int n)
    {
        int currentline = 0;
        int currentLength = 0;
        int returnvalue = 0;
        for (int i = 0; i < this.numcommands; i++)
        {
            if ((this.commands[i]).type.Equals("one"))
            {
                currentLength++;
            }
            else if ((this.commands[i]).type.Equals("two Length"))
            {
                currentLength += 2;
            }
            else if ((this.commands[i]).type.Equals("two data"))
            {
                currentLength += 1 + (this.commands[i]).data.Length;
            }
            else if ((this.commands[i]).type.Equals("three"))
            {
                currentLength += 2 + (this.commands[i]).data.Length;
            }
            else
            {
                Console.WriteLine("Whoa, weird command.type in linecommandLength");
            }
            if ((this.commands[i]).cmdbyte == 15)
            {
                if (currentline == n)
                {
                    returnvalue = currentLength;
                }
                currentline++;
                currentLength = 0;
            }
        }
        return returnvalue;
    }

}
}
