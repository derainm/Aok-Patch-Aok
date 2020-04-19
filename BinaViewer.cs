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
        public BinaViewer(string content)
        {
            InitializeComponent();
            richTextBox1.Text = content;
        }
        public void setText(string content)
        {
            richTextBox1.Text = content;
        }
    }
}
