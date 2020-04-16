// Decompiled with JetBrains decompiler
// Type: Aok_Patch.patcher_.FatalError
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using System;

namespace Aok_Patch.patcher_
{
  internal class FatalError : Exception
  {
    public FatalError(string msg)
      : base(msg)
    {
    }
  }
}
