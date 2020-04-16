using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aok_Patch.patcher_
{
    internal class rowedge
    {
        internal int left;

        internal int right;

        internal rowedge()
        {
        }

        internal rowedge(int l, int r)
        {
            this.left = l;
            this.right = r;
        }
    }

}
