// Decompiled with JetBrains decompiler
// Type: Aok_Patch.patcher_.Properties.Resources
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Aok_Patch.patcher_.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (Aok_Patch.patcher_.Properties.Resources.resourceMan == null)
          Aok_Patch.patcher_.Properties.Resources.resourceMan = new ResourceManager("Aok_Patch.patcher_.Properties.Resources", typeof (Aok_Patch.patcher_.Properties.Resources).Assembly);
        return Aok_Patch.patcher_.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get
      {
        return Aok_Patch.patcher_.Properties.Resources.resourceCulture;
      }
      set
      {
        Aok_Patch.patcher_.Properties.Resources.resourceCulture = value;
      }
    }

    public static Bitmap aoc
    {
      get
      {
        return (Bitmap) Aok_Patch.patcher_.Properties.Resources.ResourceManager.GetObject(nameof (aoc), Aok_Patch.patcher_.Properties.Resources.resourceCulture);
      }
    }



    public static Bitmap aok
    {
      get
      {
        return (Bitmap) Aok_Patch.patcher_.Properties.Resources.ResourceManager.GetObject(nameof (aok), Aok_Patch.patcher_.Properties.Resources.resourceCulture);
      }
    }
        public static Bitmap res
        {
            get
            {
                return (Bitmap)Aok_Patch.patcher_.Properties.Resources.ResourceManager.GetObject(nameof(res), Aok_Patch.patcher_.Properties.Resources.resourceCulture);
            }
        }


    }
}
