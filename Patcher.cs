// Decompiled with JetBrains decompiler
// Type: Aok_Patch.patcher_.Patcher
// Assembly: aokWideScreen(for aofset.patcher), Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4C331E86-61C5-4CAD-8718-476DBE126BFB
// Assembly location: C:\Users\Beleive\Desktop\wideScreen\aokWideScreen(for aofset.patcher).exe

using Aok_Patch.patcher_.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Aok_Patch.patcher_
{
    internal static class Patcher
    {
        private static readonly uint[] Resolutions = new uint[11]
        {
      200U,
      320U,
      480U,
      640U,
      600U,
      768U,
      800U,
      1024U,
      1200U,
      1280U,
      1600U
        };

        public static void ListEm(byte[] exe)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < exe.Length - 3; ++index)
            {
                uint num = (uint)((int)exe[index] | (int)exe[index + 1] << 8 | (int)exe[index + 2] << 16 | (int)exe[index + 3] << 24);
                foreach (int resolution in Patcher.Resolutions)
                {
                    if (resolution == (int)num)
                        stringBuilder.AppendLine(string.Format("{0:X8} {1:G4}", (object)index, (object)num));
                }
            }
            File.WriteAllText("..\\Data\\output.txt", stringBuilder.ToString());
        }

        public static Patch TryReadPatch(string patchFile, bool activeOnly)
        {
            try
            {
                UserFeedback.Info("Reading the patch file '{0}'", (object)patchFile);
                return Patcher.ReadPatch(patchFile, activeOnly);
            }
            catch (Exception ex)
            {
                UserFeedback.Error(ex);
                return new Patch() { PatchFilepath = patchFile };
            }
        }

        private static byte[] getPatchFromProperties(string patchFile)
        {
            string str = patchFile;
            if (str == "AoK_20.patch")
                return Aok_Patch.patcher_.Properties.Resource1.AoK_20;
            if (str == "AoK_20a.patch")
                return Aok_Patch.patcher_.Properties.Resource1.AoK_20a;
            if (str == "AoK_20b.patch")
                return Aok_Patch.patcher_.Properties.Resource1.AoK_20b;
            if (str == "AoC_10.patch")
                return Aok_Patch.patcher_.Properties.Resource1.AoC_10;
            if (str == "AoC_10ce.patch")
                return Aok_Patch.patcher_.Properties.Resource1.AoC_10ce;
            if (str == "AoC_10ce.patch")
                return Aok_Patch.patcher_.Properties.Resource1.AoC_10ce;
            return new byte[0];
        }

        private static string[] ReadAllResourceLines(byte[] resourceData)
        {
            using (Stream stream = (Stream)new MemoryStream(resourceData))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                    return Patcher.EnumerateLines((TextReader)streamReader).ToArray<string>();
            }
        }

        private static IEnumerable<string> EnumerateLines(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        public static Patch ReadPatch(string patchFile, bool activeOnly)
        {
            try
            {
                List<Item> objList = new List<Item>(1024);
                string[] strArray1 = Patcher.ReadAllResourceLines(Patcher.getPatchFromProperties(patchFile));
                List<string> stringList = new List<string>(strArray1.Length);
                Patch patch = new Patch()
                {
                    PatchFilepath = patchFile
                };

                foreach (string str in strArray1)
                {
                    if (str.StartsWith("size="))
                        patch.FileSize = int.Parse(str.Substring(5));
                    else if (str.StartsWith("md5="))
                        patch.Md5 = str.Substring(4);
                    else if (str.StartsWith("version="))
                        patch.Version = str.Substring(8);
                    else if (str.StartsWith("drspos="))
                        patch.InterfaceDrsPosition = int.Parse(str.Substring(7), NumberStyles.HexNumber);
                    else if (str.StartsWith("x1drspos="))
                        patch.InterfaceX1DrsPosition = int.Parse(str.Substring(9), NumberStyles.HexNumber);
                    else if (!str.StartsWith("[") && !str.StartsWith("#"))
                    {
                        string[] strArray2 = str.Split(new char[1]
                        {
              '|'
                        }, 2);
                        string[] strArray3 = strArray2[0].Split(new char[2]
                        {
              ' ',
              '\t'
                        }, 4, StringSplitOptions.RemoveEmptyEntries);
                        if (activeOnly)
                        {
                            if (strArray3.Length < 3 || !strArray3[2].Equals("H") && !strArray3[2].Equals("V") && (!strArray3[2].Equals("dH") && !strArray3[2].Equals("dV")) && !strArray3[2].Equals("HV"))
                                continue;
                        }
                        else if (strArray3.Length < 2)
                            continue;
                        Item obj = new Item()
                        {
                            Pos = int.Parse(strArray3[0], NumberStyles.HexNumber),
                            ReferenceValue = int.Parse(strArray3[1]),
                            Type = "",
                            Comments = ""
                        };
                        if (strArray3.Length > 2)
                            obj.Type = strArray3[2];
                        if (strArray3.Length > 3)
                        {
                            string[] strArray4 = strArray3[3].Split(new char[2]
                            {
                ' ',
                '\t'
                            }, 2, StringSplitOptions.RemoveEmptyEntries);
                            obj.Comments = !int.TryParse(strArray4[0], out obj.Parameter) ? strArray3[3].TrimEnd() : (strArray4.Length == 1 ? "" : strArray4[1].TrimEnd());
                        }
                        if (!activeOnly && strArray2.Length > 1)
                            obj.Asm = strArray2[1].Trim();
                        obj.OriginalPos = obj.Pos;
                        objList.Add(obj);
                        stringList.Add(str);
                    }
                }
                patch.Items = (IEnumerable<Item>)objList;
                return patch;
            }
            catch (Exception ex)
            {
                throw new FatalError("Couldn't read or parse patch file");
            }
        }

        public static void PatchDrsRefInExe(byte[] exe, string newInterfaceDrsName, Patch patch)
        {
            int interfaceDrsPosition = patch.InterfaceDrsPosition;
            //if (exe[interfaceDrsPosition] != (byte)105)
            //    throw new FatalError("Didn't find interfac.drs reference at expected location. Wrong exe.");
            foreach (byte num in Encoding.ASCII.GetBytes(newInterfaceDrsName))
            {
                exe[interfaceDrsPosition] = num;
                ++interfaceDrsPosition;
            }
        }

        public static void PatchX1DrsRefInExe(byte[] exe, string newInterfaceDrsName, Patch patch)
        {
            int interfaceX1DrsPosition = patch.InterfaceX1DrsPosition;
            if (exe[interfaceX1DrsPosition] != (byte)105)
                throw new FatalError("Didn't find interfac_x1.drs reference at expected location. Wrong exe.");
            foreach (byte num in Encoding.ASCII.GetBytes(newInterfaceDrsName))
            {
                exe[interfaceX1DrsPosition] = num;
                ++interfaceX1DrsPosition;
            }
        }

        private static string FindAsm(int pos, IDictionary<int, string> asmMap)
        {
            string str = (string)null;
            int num = pos - 10;
            int key = pos;
            while (key > num && !asmMap.TryGetValue(key, out str))
                --key;
            return str;
        }

        public static void AddAsm(Patch patch, IDictionary<int, string> asmMap)
        {
            foreach (Item obj in patch.Items)
                obj.Asm = Patcher.FindAsm(obj.Pos, asmMap);
        }

        public static IDictionary<int, string> ReadAsmMap(string lstPath)
        {
            string[] strArray1 = File.ReadAllLines(lstPath, Encoding.GetEncoding(437));
            Dictionary<int, string> dictionary = new Dictionary<int, string>(strArray1.Length);
            foreach (string str1 in strArray1)
            {
                string[] strArray2 = str1.Split(new char[1] { ';' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (strArray2.Length != 0)
                {
                    string str2 = strArray2[0].TrimEnd();
                    int result;
                    if (str2.Length > 14 && int.TryParse(str2.Substring(6, 8), NumberStyles.HexNumber, (IFormatProvider)null, out result) && result >= 4194304)
                        dictionary[result - 4194304] = str1;
                }
            }
            return (IDictionary<int, string>)dictionary;
        }

        private static IEnumerable<string> StringifyPatch(Patch patch)
        {
            foreach (Item obj in patch.Items)
            {
                Item item = obj;
                string rootPos = item.OriginalPos == 0 || item.OriginalPos == item.Pos ? "" : string.Format("[rootPos: {0:X8}, offset {1,-8}]", (object)item.OriginalPos, (object)(item.Pos - item.OriginalPos));
                string pos = item.Pos > 0 ? item.Pos.ToString("X8") : string.Format("!!!{0:X8}", (object)-item.Pos);
                string asm = string.IsNullOrEmpty(item.Asm) ? "" : " | " + item.Asm;
                string param = item.Parameter != 0 ? item.Parameter.ToString() : "";
                yield return string.Format("{0}  {1,4}  {2,3}  {3,4}  {4,-60}{5}{6}", (object)pos, (object)item.ReferenceValue, (object)item.Type, (object)param, (object)item.Comments, (object)rootPos, (object)asm);
                rootPos = (string)null;
                pos = (string)null;
                asm = (string)null;
                param = (string)null;
                item = (Item)null;
            }
        }

        public static void WritePatch(Patch patch, string filePath)
        {
            List<string> stringList = new List<string>()
      {
        "[header]",
        string.Format("size={0}", (object) patch.FileSize),
        string.Format("md5={0}", (object) patch.Md5),
        string.Format("version={0}", (object) patch.Version),
        string.Format("drspos={0:X8}", (object) patch.InterfaceDrsPosition),
        "[data]"
      };
            IEnumerable<string> collection = Patcher.StringifyPatch(patch);
            stringList.AddRange(collection);
            File.WriteAllLines(filePath, stringList.ToArray());
        }

        public static Patch ConvertPatch(byte[] rootExe, byte[] otherExe, Patch patch)
        {
            return new Patch()
            {
                InterfaceDrsPosition = Patcher.FindComparablePos(rootExe, otherExe, patch.InterfaceDrsPosition),
                Items = (IEnumerable<Item>)patch.Items.Select<Item, Item>((Func<Item, Item>)(item => new Item()
                {
                    Pos = Patcher.FindComparablePos(rootExe, otherExe, item.Pos),
                    Comments = item.Comments,
                    ReferenceValue = item.ReferenceValue,
                    Parameter = item.Parameter,
                    Type = item.Type,
                    OriginalPos = item.Pos
                })).ToArray<Item>()
            };
        }

        private static int FindComparablePos(byte[] correctExe, byte[] otherExe, int pos)
        {
            for (int length = 4; length < 25; length += 4)
            {
                byte[] window = new byte[length];
                int num1 = 0;
                if (num1 + pos < 0)
                    num1 = -pos;
                if (num1 + length >= correctExe.Length)
                    num1 = correctExe.Length - (pos + length);
                for (int index = 0; index < length; ++index)
                    window[index] = correctExe[index + pos + num1];
                int[] array1 = Patcher.FindWindow(otherExe, window).ToArray<int>();
                if (array1.Length == 1)
                    return array1[0] - num1;
                if (array1.Length == 0)
                {
                    if (length == 8 && num1 == 0)
                    {
                        int num2 = -4;
                        for (int index = 0; index < length; ++index)
                            window[index] = correctExe[index + pos + num2];
                        int[] array2 = Patcher.FindWindow(otherExe, window).ToArray<int>();
                        if (array2.Length == 1)
                            return array2[0] - num2;
                    }
                    UserFeedback.Warning(string.Format("Found no matches for block {0:X8} {1}", (object)pos, (object)length));
                    return -pos;
                }
            }
            UserFeedback.Warning("Found too many matches for block {0:X8}", (object)pos);
            return -pos;
        }

        private static IEnumerable<int> FindWindow(byte[] otherExe, byte[] window)
        {
            for (int i = 0; i <= otherExe.Length - window.Length; ++i)
            {
                if (Patcher.Equals(window, otherExe, i))
                    yield return i;
            }
        }

        private static bool Equals(byte[] window, byte[] otherExe, int pos)
        {
            List<int> intList = new List<int>();
            int num = window.Length / 3;
            for (int index = 0; index < window.Length; ++index)
            {
                if ((int)window[index] != (int)otherExe[pos + index])
                {
                    if (index < 5 || intList.Count >= num)
                        return false;
                    intList.Add(index);
                }
            }
            if (intList.Count == 0)
                return true;
            using (List<int>.Enumerator enumerator = intList.GetEnumerator())
            {
            label_19:
                while (enumerator.MoveNext())
                {
                    int current = enumerator.Current;
                    for (int index = 1; current - index >= 4; ++index)
                    {
                        switch (window[current - index])
                        {
                            case 232:
                            case 233:
                                goto label_19;
                            default:
                                if (index == 4)
                                    return false;
                                continue;
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        public static void PatchResolutions(
          byte[] exe,
          int oldWidth,
          int oldHeight,
          int newWidth,
          int newHeight,
          Patch patch,
          Dictionary<int, int> dictHeigth = null,
          Dictionary<int, int> dictWidth = null
          )
        {
            Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
            int[] numArray1 = new int[4] { 800, 1024, 1280, 1600 };
            if (dictHeigth == null)
            {
                int num1 = newWidth;
                foreach (int index in numArray1)
                {
                    if (index == oldWidth)
                        dictionary1[index] = newWidth;
                    else if (index > oldWidth)
                        dictionary1[index] = ++num1;
                }
            }
            else
            {
                dictionary1 = dictHeigth;
            }
            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
            int[] numArray2 = new int[4] { 600, 768, 1024, 1200 };
            if (dictWidth  == null)
            {
                int num2 = newHeight;
                foreach (int index in numArray2)
                {
                    if (index == oldHeight)
                        dictionary2[index] = newHeight;
                    else if (index > oldHeight)
                        dictionary2[index] = ++num2;
                }
            }
            else
            {
                dictionary2 = dictWidth;
            }
            foreach (KeyValuePair<int, int> keyValuePair in dictionary1)
                UserFeedback.Trace(string.Format("Horizontal {0} => {1}", (object)keyValuePair.Key, (object)keyValuePair.Value));
            foreach (KeyValuePair<int, int> keyValuePair in dictionary2)
                UserFeedback.Trace(string.Format("Vertical {0} => {1}", (object)keyValuePair.Key, (object)keyValuePair.Value));
            foreach (Item obj in patch.Items)
            {
                if (obj.Pos >= exe.Length)
                {
                    UserFeedback.Warning("Error in input: Invalid location {0:X8}. [NOT PATCHED]", (object)obj.Pos);
                }
                else
                {
                    int referenceValue = obj.ReferenceValue;
                    bool flag1 = obj.Type.Contains("H");
                    bool flag2 = obj.Type.Contains("V");
                    if (flag1 & flag2)
                    {
                        if (oldWidth == referenceValue)
                            flag2 = false;
                        else
                            flag1 = false;
                    }
                    Trace.Assert(flag1 | flag2);
                    int num3;
                    if (!(flag2 ? dictionary2 : dictionary1).TryGetValue(referenceValue, out num3))
                        num3 = referenceValue;
                    if (obj.Type.Contains("H") && obj.Type.Contains("V"))
                        UserFeedback.Trace(string.Format("{0} HV: Mapping to {1}", (object)referenceValue, (object)num3));
                    int num4 = (int)exe[obj.Pos] | (int)exe[obj.Pos + 1] << 8 | (int)exe[obj.Pos + 2] << 16 | (int)exe[obj.Pos + 3] << 24;
                    if (obj.Type.Equals("dV") || obj.Type.Equals("dH"))
                    {
                        int parameter = obj.Parameter;
                        if (parameter == 0)
                            UserFeedback.Warning(string.Format("{0} action is safer if you mention the expected orgValue. Encountered {1} @ {2:X8}", (object)obj.Type, (object)num4, (object)obj.Pos));
                        else if (parameter != num4)
                        {
                            UserFeedback.Warning(string.Format("{0} action expected value mismatch: {1} expected, {2} encountered @ {3:X8} [NOT PATCHED]", (object)obj.Type, (object)parameter, (object)num4, (object)obj.Pos));
                            continue;
                        }
                        int num5 = num3 - referenceValue;
                        num3 = num4 + num5;
                    }
                    else if (num4 != referenceValue)
                    {
                        UserFeedback.Warning(string.Format("{0} action expected value mismatch: {1} expected, {2} encountered @ {3:X8} [NOT PATCHED]", (object)obj.Type, (object)referenceValue, (object)num4, (object)obj.Pos));
                        continue;
                    }
                    byte num6 = (byte)(num3 & (int)byte.MaxValue);
                    byte num7 = (byte)(num3 >> 8 & (int)byte.MaxValue);
                    byte num8 = (byte)(num3 >> 16 & (int)byte.MaxValue);
                    byte num9 = (byte)(num3 >> 24 & (int)byte.MaxValue);
                    exe[obj.Pos] = num6;
                    exe[obj.Pos + 1] = num7;
                    exe[obj.Pos + 2] = num8;
                    exe[obj.Pos + 3] = num9;
                }
            }
        }
    }
}
