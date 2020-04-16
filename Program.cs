// Decompiled with JetBrains decompiler
// Type: Aok_Patch.patcher_.Program
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using System;
using System.Windows.Forms;

namespace Aok_Patch.patcher_
{
  internal class Program
  {
    [STAThread]
    private static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run((Form) new AokPatch());
    }
  }
}
