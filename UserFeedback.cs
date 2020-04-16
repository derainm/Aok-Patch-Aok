// Decompiled with JetBrains decompiler
// Type: Aok_Patch.patcher_.UserFeedback
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using System;
using System.Threading;
using System.Windows.Forms;

namespace Aok_Patch.patcher_
{
  internal class UserFeedback
  {
    public static void Error(Exception e)
    {
      ConsoleColor foregroundColor = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("An error occurred:");
      Console.WriteLine(e.Message);
      if (!(e is FatalError))
      {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("Exception Details:");
        Console.WriteLine(e.ToString());
      }
      Console.ForegroundColor = foregroundColor;
    }

    public static void Info(string msg, object param1)
    {
      UserFeedback.Info(string.Format(msg, param1));
    }

    public static void Info(string msg)
    {
      Console.WriteLine(msg);
    }

    public static void Trace(string msg, object param1)
    {
      UserFeedback.Trace(string.Format(msg, param1));
    }

    public static void Trace(string msg)
    {
      ConsoleColor foregroundColor = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(msg);
      Console.ForegroundColor = foregroundColor;
    }

    public static void Warning(string msg, object param1)
    {
      UserFeedback.Warning(string.Format(msg, param1));
    }

    public static void Warning(string msg)
    {
      ConsoleColor foregroundColor = Console.ForegroundColor;
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.Write("Warning: ");
      Console.ForegroundColor = ConsoleColor.Yellow;
      Console.WriteLine(msg);
      Console.ForegroundColor = foregroundColor;
    }

    internal static void Close()
    {
      try
      {
        if (!Application.ExecutablePath.Equals(Console.Title, StringComparison.InvariantCultureIgnoreCase))
          return;
        UserFeedback.Info("Press any key to quit");
        while (!Console.KeyAvailable)
          Thread.Sleep(50);
      }
      catch
      {
      }
    }
  }
}
