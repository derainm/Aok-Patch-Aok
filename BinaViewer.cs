using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Aok_Patch.patcher_
{
    public partial class BinaViewer : Form
    {
        public BinaViewer()
        {
            InitializeComponent();
        }
        private string Content;
        private List<DrsTable> lst;
        private uint Id;
        private uint Type;
        public BinaViewer(List<DrsTable> lstDrs,string id,uint type)
        {
            InitializeComponent();
            lst = lstDrs;
            Id = uint.Parse(id);
            Type = type;
            var Data = lstDrs.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id)).First().Data;
            Content = Encoding.UTF8.GetString(Data, 0, Data.Length);
            richTextBox1.Text = Content;
        }
        public void setText(List<DrsTable> lstDrs, string id, uint type)
        {
            lst = lstDrs;
            Id = uint.Parse(id);
            Type = type;
            var Data = lstDrs.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id)).First().Data;
            Content = Encoding.UTF8.GetString(Data, 0, Data.Length);
            richTextBox1.Text = Content;
        }

        private void btn_BinaSave_Click(object sender, EventArgs e)
        {
            var data = Encoding.ASCII.GetBytes(richTextBox1.Text);
            lst.Where(x => x.Type == Type).First().Items.Where(w => w.Id == Id).First().Data = data;
        }
    }
}
