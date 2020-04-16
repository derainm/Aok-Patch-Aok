using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aok_Patch.patcher_
{
    class commandclass
    {

        public byte cmdbyte;

        public byte next_byte;

        public byte[] data;

        public string type;


        internal commandclass(byte b)
        {
            this.cmdbyte = b;
            this.type = "one";
        }


        internal commandclass(byte b, byte n)
        {
            this.cmdbyte = b;
            this.next_byte = n;
            this.type = "two length";
        }


        internal commandclass(byte b, byte[] d)
        {
            this.cmdbyte = b;
            this.data = d;
            this.type = "two data";
        }


        internal commandclass(byte b, byte nb, byte[] d)
        {
            this.cmdbyte = b;
            this.next_byte = nb;
            this.data = d;
            this.type = "three";
        }

        internal virtual void print()
        {
            if (this.type.Equals("one"))
            {
                Console.WriteLine("command: " + byteToHex(this.cmdbyte));
            }
            else if (this.type.Equals("two length"))
            {
                Console.WriteLine("command: " + byteToHex(this.cmdbyte) + " next byte " + byteToHex(this.next_byte));
            }
            else if (this.type.Equals("two data"))
            {
                Console.Write("command: " + byteToHex(this.cmdbyte) + " data ");
                for (int i = 0; i < this.data.Length; i++)
                {
                    Console.Write((this.data[i] & 0xFF).ToString() + " ");
                }
                Console.WriteLine();
            }
            else if (this.type.Equals("three"))
            {
                Console.WriteLine("command: " + byteToHex(this.cmdbyte) + " next byte " + byteToHex(this.next_byte) + " data " + " length " + this.data.Length);
                for (int i = 0; i < this.data.Length; i++)
                {
                    Console.Write((this.data[i] & 0xFF).ToString() + " ");
                }
                Console.WriteLine();
            }
        }

        internal virtual int commandlength()
        {
            if (this.type.Equals("one"))
            {
                return 1;
            }
            if (this.type.Equals("two length"))
            {
                return 2;
            }
            if (this.type.Equals("two data"))
            {
                return 1 + this.data.Length;
            }
            if (this.type.Equals("three"))
            {
                return 2 + this.data.Length;
            }
            Console.WriteLine("Whoa, weird type of command");
            return 0;
        }

        public virtual string byteToHex(byte d)
        {
            return (d & 0xFF).ToString("x");
        }

    }
}
