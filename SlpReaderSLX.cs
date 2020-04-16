// Decompiled with JetBrains decompiler
// Type: SLP_Reader.SlpReader
// Assembly: SLX Studio, Version=1.4.1.0, Culture=neutral, PublicKeyToken=null
// MVID: B1DE9AF1-FC33-4D79-BEAE-2919C4AAE382
// Assembly location: C:\Users\Beleive\Desktop\SLX Studio.exe


using Extension;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLP_Reader
{
  internal class SlpReader
  {
    //private FormProgressBar fpBar = new FormProgressBar();
    public string fpVerb = "Extracting ";
    public List<string> safeSLPFilesList = new List<string>();
    private List<string> slp4X = new List<string>()
    {
      "4.0X",
      "4.1X"
    };
    public string slpPalName = "";
    public List<string> slxLines = new List<string>();
    public List<int[]> cmdTableOffsets = new List<int[]>();
        public Color slpMaskColor = Global.sxpMaskColor;
        public Color slpShadowColor = Global.sxpShadowColor;
        public Color slpOutline1Color = Global.sxpOutline1Color;
        public Color slpOutline2Color = Global.sxpOutline2Color;
        public int slpPlayerIndex = Global.sxpPlayerIndex;
        public int slpPalIndex = Global.sxpPalIndex;
        public bool slpEmbedPalChecked = Global.slpEmbedPal;
        public Color[] slpPal = Global.sxpPal;
        public Color[] slpPlayerColorPal = Global.sxpPlayerColorPal;
        //private Form1 form1;
        private BackgroundWorker SlpImporter;
    private BackgroundWorker BatchSlxCreator;
    public int percentage;
    private int numFramesTotal;
    private int frameCount;
    public string slpOpenFileName;
    public string itemName;
    public string destination;
    public string slxFileName;
    public string dataName;
    private bool multipleFiles;
    public bool is32Bit;
    public bool isTerrain;
    public bool readPalette;
    public bool slpImportCompleted;
    public bool slpLoadCompleted;
    public bool slpSuccessful;
    public bool slpRenderCompleted;
    public bool slpDecodeCompleted;
    public bool batchSlxConversionCompleted;
    public bool slpCancelLoad;
    public bool createSLXFile;
    public bool slpIndicesChanged;
    public FileStream fs;
    public string version;
    public int numFrames;
    public int type;
    public int numDirections;
    public int framesPerDirection;
    public int paletteID;
    public int frameInfoOffset;
    public int layerInfoOffset;
    public string comment;
    public int[] frameDataOffsets;
    public int[] frameOutlineOffset;
    public int[] framePaletteOffset;
    public int[] framePaletteID;
    public int[] frameWidth;
    public int[] frameHeight;
    public int[] frameAnchorX;
    public int[] frameAnchorY;
    public int[] layerDataOffsets;
    public int[] layerOutlineOffset;
    public int[] layerProperties;
    public int[] layerWidth;
    public int[] layerHeight;
    public int[] layerAnchorX;
    public int[] layerAnchorY;
    public int numRows;
    public int numColumns;
    private long lastPos;
    private long filePos;
    public int decayTime;
    private MethodInvoker mi;
    public bool invalidLength;

    public SlpReader()
    {
    }



        public void LoadSLP(string fileName)
    {
      this.slpLoadCompleted = false;
      this.fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      if (this.fs.ReadUInt32() == 3203339063U)
      {
        if (MessageBox.Show("This SLP is encoded by Voobly. Would you like to decode it into a readable SLP file?", "Voobly Encoded SLP", MessageBoxButtons.OKCancel) == DialogResult.OK)
        {
          SaveFileDialog saveFileDialog = new SaveFileDialog();
          if (false)
            saveFileDialog.AutoUpgradeEnabled = false;
          saveFileDialog.Title = "Decode Voobly SLP";
          saveFileDialog.FileName = Path.GetFileName(fileName);
          saveFileDialog.InitialDirectory = Path.GetDirectoryName(fileName);
          saveFileDialog.Filter = "SLP File (*.slp)|*.slp";
          if (saveFileDialog.ShowDialog() == DialogResult.OK)
          {
            bool overwrite = false;
            if (fileName == saveFileDialog.FileName)
            {
              saveFileDialog.FileName += ".temp";
              overwrite = true;
            }
            this.DecodeVooblySLP(this.fs, fileName, saveFileDialog.FileName, overwrite, true);
            return;
          }
        }
        else
        {
          this.fs.Close();
          return;
        }
      }
      this.slpOpenFileName = fileName;
      this.fs.Position -= 4L;
      this.version = this.fs.ReadASCII(4);
      if (!this.version.Substring(0, 1).IsNumeric() || this.version.Substring(1, 1) != "." || !this.version.Substring(2, 1).IsNumeric())
      {
        switch (MessageBox.Show("This SLP file may be unreadable.", "Error", MessageBoxButtons.AbortRetryIgnore))
        {
          case DialogResult.Abort:
            this.fs.Close();
            this.slpLoadCompleted = true;
            this.slpCancelLoad = true;
            return;
          case DialogResult.Retry:
            this.LoadSLP(fileName);
            return;
        }
      }
      this.ReadSLP(this.fs, this.fs.Position);
    }

    public void LoadSLP(FileStream drsStream, long position)
    {
      this.fs = drsStream;
      this.filePos = position;
      this.fs.Position = this.filePos;
      this.version = this.fs.ReadASCII(4);
      if (!this.version.Substring(0, 1).IsNumeric() || this.version.Substring(1, 1) != "." || !this.version.Substring(2, 1).IsNumeric())
      {
        switch (MessageBox.Show("This SLP file may be unreadable.", "Error", MessageBoxButtons.AbortRetryIgnore))
        {
          case DialogResult.Abort:
            this.fs.Close();
            this.slpLoadCompleted = true;
            this.slpCancelLoad = true;
            return;
          case DialogResult.Retry:
            this.LoadSLP(drsStream, position);
            return;
        }
      }
      this.ReadSLP(this.fs, this.fs.Position);
    }

    private void ReadSLP(FileStream stream, long position)
    {
      this.fs = stream;
      this.fs.Position = position;
      this.invalidLength = false;
      if (this.fs.Length - this.fs.Position < 64L)
      {
        int num = (int) MessageBox.Show("Unable to read SLP file. Size is invalid.", "Error");
        this.slpLoadCompleted = true;
      }
      else
      {
        this.numFrames = (int) this.fs.ReadUInt16();
        this.type = (int) this.fs.ReadUInt16();
        if (this.version == "2.0N" || this.version.Substring(0, 3) == "3.0")
        {
          this.comment = this.fs.ReadASCII(24);
          if (this.comment.Contains("TrnSLP"))
          {
            this.fs.Position -= 24L;
            this.numRows = (int) this.fs.ReadUInt8();
            this.numColumns = (int) this.fs.ReadUInt8();
            this.fs.Position += 22L;
            if (this.numRows == 0 && this.numColumns == 0)
            {
              int num = Math.Ceiling(Math.Sqrt((double) this.numFrames)).ToInt();
              this.numRows = num;
              this.numColumns = num;
            }
            this.isTerrain = true;
          }
          else if (this.comment.Contains("TRN") || this.comment.Contains("Terrain") || this.comment.Contains("UP "))
          {
            int num = Math.Ceiling(Math.Sqrt((double) this.numFrames)).ToInt();
            this.numRows = num;
            this.numColumns = num;
            if (this.numFrames == 96)
            {
              this.numRows = 8;
              this.numColumns = 12;
            }
            this.isTerrain = true;
          }
        }
        else if (this.version.Substring(0, 2) == "4." && this.version.Substring(3) == "X")
        {
          this.numDirections = (int) this.fs.ReadUInt16();
          this.framesPerDirection = (int) this.fs.ReadUInt16();
          this.paletteID = this.fs.ReadInt32();
          this.frameInfoOffset = this.fs.ReadInt32();
          this.layerInfoOffset = this.fs.ReadInt32();
          this.comment = this.fs.ReadASCII(this.frameInfoOffset - 24);
        }
        if (this.slp4X.Contains(this.version))
          this.fs.Position = this.filePos + (long) this.frameInfoOffset;
        else
          this.fs.Position = this.filePos + 32L;
        this.UpdateAutoPalette();
        this.frameDataOffsets = new int[this.numFrames];
        this.frameOutlineOffset = new int[this.numFrames];
        this.framePaletteOffset = new int[this.numFrames];
        this.framePaletteID = new int[this.numFrames];
        this.frameWidth = new int[this.numFrames];
        this.frameHeight = new int[this.numFrames];
        this.frameAnchorX = new int[this.numFrames];
        this.frameAnchorY = new int[this.numFrames];
        try
        {
          for (int index = 0; index < this.numFrames; ++index)
          {
            this.frameDataOffsets[index] = this.fs.ReadInt32();
            this.frameOutlineOffset[index] = this.fs.ReadInt32();
            this.framePaletteOffset[index] = this.fs.ReadInt32();
            this.framePaletteID[index] = this.fs.ReadInt32();
            if ((this.framePaletteID[index] & 7) == 7)
              this.is32Bit = true;
            this.frameWidth[index] = this.fs.ReadInt32();
            this.frameHeight[index] = this.fs.ReadInt32();
            long num1 = (long) this.fs.ReadInt32();
            if (num1 >= 2147483000L)
              num1 -= 2147483000L;
            else if (num1 <= -2147483000L)
              num1 += 2147483000L;
            this.frameAnchorX[index] = Convert.ToInt32(num1);
            long num2 = (long) this.fs.ReadInt32();
            if (num2 >= 2147483000L)
              num2 -= 2147483000L;
            else if (num2 <= -2147483000L)
              num2 += 2147483000L;
            this.frameAnchorY[index] = Convert.ToInt32(num2);
          }
        }
        catch
        {
          if (this.fs.Position > this.fs.Length)
          {
            int num1 = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
          }
          else
          {
            int num2 = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
          }
          this.slpLoadCompleted = true;
          return;
        }
        if (!this.isTerrain && this.frameWidth.IsAllEqual() && (this.frameHeight.IsAllEqual() && this.numFrames > 1) && (this.frameHeight[0] == (this.frameWidth[0] - 1) / 2 + 1 && this.numRows == 0 && this.numColumns == 0))
        {
          int num = Math.Ceiling(Math.Sqrt((double) this.numFrames)).ToInt();
          this.numRows = num;
          this.numColumns = num;
          this.isTerrain = true;
        }
        if (this.slp4X.Contains(this.version) && this.layerInfoOffset > 0)
        {
          this.layerDataOffsets = new int[this.numFrames];
          this.layerOutlineOffset = new int[this.numFrames];
          this.layerProperties = new int[this.numFrames];
          this.layerWidth = new int[this.numFrames];
          this.layerHeight = new int[this.numFrames];
          this.layerAnchorX = new int[this.numFrames];
          this.layerAnchorY = new int[this.numFrames];
          this.fs.Position = this.filePos + (long) this.layerInfoOffset;
          try
          {
            for (int index = 0; index < this.numFrames; ++index)
            {
              this.layerDataOffsets[index] = this.fs.ReadInt32();
              this.layerOutlineOffset[index] = this.fs.ReadInt32();
              this.fs.Position += 7L;
              this.layerProperties[index] = (int) this.fs.ReadUInt8();
              this.layerWidth[index] = this.fs.ReadInt32();
              this.layerHeight[index] = this.fs.ReadInt32();
              long num1 = (long) this.fs.ReadInt32();
              if (num1 >= 2147483000L)
                num1 -= 2147483000L;
              else if (num1 <= -2147483000L)
                num1 += 2147483000L;
              this.layerAnchorX[index] = Convert.ToInt32(num1);
              long num2 = (long) this.fs.ReadInt32();
              if (num2 >= 2147483000L)
                num2 -= 2147483000L;
              else if (num2 <= -2147483000L)
                num2 += 2147483000L;
              this.layerAnchorY[index] = Convert.ToInt32(num2);
            }
          }
          catch
          {
            if (this.fs.Position > this.fs.Length)
            {
              int num1 = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
            }
            else
            {
              int num2 = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
            }
            this.slpLoadCompleted = true;
            return;
          }
        }
        this.slpLoadCompleted = true;
      }
    }

    public void BatchConvertToSLX(string[] slpFiles)
    {
      this.batchSlxConversionCompleted = false;
      this.BatchSlxCreator = new BackgroundWorker();
      //this.BatchSlxCreator.DoWork += new DoWorkEventHandler(this.BatchSlxCreator_DoWork);
      //this.BatchSlxCreator.ProgressChanged += new ProgressChangedEventHandler(this.BatchSlxCreator_ProgressChanged);
      //this.BatchSlxCreator.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BatchSlxCreator_RunWorkerCompleted);
      this.BatchSlxCreator.WorkerReportsProgress = true;
      this.createSLXFile = true;
      this.safeSLPFilesList = new List<string>();
      this.numFramesTotal = 0;
      foreach (string slpFile in slpFiles)
      {
        try
        {
          this.fs = File.Open(slpFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
          bool flag1 = false;
          bool flag2 = false;
          string str = this.fs.ReadASCII(4);
          if (str != "2.0N" && str.Substring(0, 3) != "3.0" && (str != "4.0X" && str != "4.1X"))
          {
            flag1 = true;
            this.fs.Position = 0L;
            if (this.fs.ReadUInt32() == 3203339063U)
              flag2 = true;
          }
          if (!flag1 && !flag2)
          {
            this.fs.Position = 4L;
            int num = (int) this.fs.ReadUInt16();
            if (num > 0)
            {
              this.numFramesTotal += num;
              this.safeSLPFilesList.Add(slpFile);
            }
            else
            {
              //TextBox logBox = this.form1.logBox;
              //logBox.Text = logBox.Text + Environment.NewLine + "[Batch SLP Convert]\tFile skipped: " + Path.GetFileName(slpFile) + " contains invalid data.";
            }
          }
          else if (flag2)
          {
            //TextBox logBox = this.form1.logBox;
            //logBox.Text = logBox.Text + Environment.NewLine + "[Batch SLP Convert]\tFile skipped: " + Path.GetFileName(slpFile) + " is a Voobly encoded SLP.";
          }
          else
          {
            //TextBox logBox = this.form1.logBox;
            //logBox.Text = logBox.Text + Environment.NewLine + "[Batch SLP Convert]\tFile skipped: " + Path.GetFileName(slpFile) + " is unrecognizable.";
          }
          this.fs.Close();
        }
        catch
        {
          //TextBox logBox = this.form1.logBox;
          //logBox.Text = log/*B*/ox.Text + Environment.NewLine + "[Batch SLP Convert]\tFile skipped: Unable to read " + Path.GetFileName(slpFile);
        }
      }
      if (this.safeSLPFilesList.Count == 0)
      {
        //TextBox logBox = this.form1.logBox;
        //logBox.Text = logBox.Text + Environment.NewLine + "[Batch SLP Convert]\tNo files converted to SLX projects.";
      }
      else
      {
        //this.fpBar = new FormProgressBar();
        //this.fpBar.Owner = (Form) this.form1;
        //this.fpBar.Show();
        //this.fpBar.progressBar.Minimum = 0;
        //this.fpBar.progressBar.Maximum = 100;
        //this.fpBar.progressBar.Value = 0;
        //this.fpBar.progressBar.Step = 1;
        //this.BatchSlxCreator.RunWorkerAsync();
      }
    }

    public void SlpImport(string fileName, string path, bool createSlxFile = true)
    {
      this.slpImportCompleted = false;
      this.SlpImporter = new BackgroundWorker();
      this.SlpImporter.DoWork += new DoWorkEventHandler(this.SlpImporter_DoWork);
      this.SlpImporter.ProgressChanged += new ProgressChangedEventHandler(this.SlpImporter_ProgressChanged);
      //this.SlpImporter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.SlpImporter_RunWorkerCompleted);
      this.SlpImporter.WorkerReportsProgress = true;
      string str = path;
      if (str.Substring(str.Length - 1) != "\\")
        path += "\\";
      this.slpOpenFileName = fileName;
      this.itemName = Path.GetFileNameWithoutExtension(this.slpOpenFileName);
      this.destination = path;
      this.createSLXFile = createSlxFile;
      //this.dataName = this.form1.dataBox.Text;
      //this.fpBar = new FormProgressBar();
      //this.fpBar.Owner = (Form) this.form1;
      //this.fpBar.Show();
      //this.fpBar.progressBar.Minimum = 0;
      //this.fpBar.progressBar.Maximum = 100;
      //this.fpBar.progressBar.Value = 0;
      //this.fpBar.progressBar.Step = 1;
      this.SlpImporter.RunWorkerAsync();
    }

    public void SaveSLPAnchors(long position, int[] newAnchorX, int[] newAnchorY)
    {
      if (position >= 0L)
      {
        this.fs.Position = position + 24L;
        Array.Copy((Array) newAnchorX, (Array) this.frameAnchorX, newAnchorX.Length);
        Array.Copy((Array) newAnchorY, (Array) this.frameAnchorY, newAnchorY.Length);
        try
        {
          for (int index = 0; index < this.numFrames; ++index)
          {
            byte[] byteArray = (this.frameAnchorX[index].ToString("X8").ReverseHexString() + this.frameAnchorY[index].ToString("X8").ReverseHexString()).HexStringToByteArray();
            this.SetFileAccess(FileAccess.ReadWrite, this.fs.Position);
            this.fs.Write(byteArray, 0, byteArray.Length);
            this.SetFileAccess(FileAccess.Read, this.fs.Position);
            this.fs.Position += 24L;
          }
        }
        catch
        {
          if (MessageBox.Show("The file may be in use by another program.", "File Save Error", MessageBoxButtons.RetryCancel) != DialogResult.Retry)
            return;
          this.SaveSLPAnchors(position, newAnchorX, newAnchorY);
        }
      }
      else
      {
        int num = (int) MessageBox.Show("Invalid file position.", "Error");
      }
    }

    public void SavePaletteOffsets(long position, int[] newPalOffsets)
    {
      if (position >= 0L)
      {
        this.fs.Position = position + 8L;
        Array.Copy((Array) newPalOffsets, (Array) this.framePaletteOffset, newPalOffsets.Length);
        try
        {
          for (int index = 0; index < this.numFrames; ++index)
          {
            byte[] byteArray = this.framePaletteOffset[index].ToString("X8").ReverseHexString().HexStringToByteArray();
            this.SetFileAccess(FileAccess.ReadWrite, this.fs.Position);
            this.fs.Write(byteArray, 0, byteArray.Length);
            this.SetFileAccess(FileAccess.Read, this.fs.Position);
            this.fs.Position += 28L;
          }
        }
        catch
        {
          if (MessageBox.Show("The file may be in use by another program.", "File Save Error", MessageBoxButtons.RetryCancel) != DialogResult.Retry)
            return;
          this.SavePaletteOffsets(position, newPalOffsets);
        }
      }
      else
      {
        int num = (int) MessageBox.Show("Invalid file position.", "Error");
      }
    }

    public void SetFileAccess(FileAccess access, long position)
    {
      this.fs = File.Open(this.slpOpenFileName, FileMode.Open, access, FileShare.ReadWrite);
      this.fs.Position = position;
    }

    public void ChangeSLPIndices(
      MemoryStream ms,
      List<int> oldIndex,
      List<int> newIndex,
      bool onlyPC = false)
    {
      this.slpIndicesChanged = false;
      try
      {
        for (int index1 = 0; index1 < this.numFrames; ++index1)
        {
          ms.Position = (long) this.frameDataOffsets[index1];
          int[] numArray = new int[this.frameHeight[index1]];
          for (int index2 = 0; index2 < this.frameHeight[index1]; ++index2)
            numArray[index2] = ms.ReadInt32();
          int num1 = 0;
          foreach (int num2 in numArray)
          {
            ms.Position = this.filePos + (long) num2;
            int num3 = -1;
            while (num3 != 15)
            {
              num3 = (int) ms.ReadUInt8();
              if (num3 != 15)
              {
                int num4 = num3 & 15;
                long num5;
                switch (num4)
                {
                  case 0:
                  case 4:
                  case 8:
                  case 12:
                    int num6 = num3 >> 2;
                    for (int index2 = 0; index2 < num6; ++index2)
                    {
                      int num7 = (int) ms.ReadUInt8();
                      if (!onlyPC && oldIndex.Contains(num7))
                      {
                        --ms.Position;
                        byte num8 = newIndex[oldIndex.IndexOf(num7)].ToByte();
                        ms.WriteByte(num8);
                        this.slpIndicesChanged = true;
                      }
                    }
                    continue;
                  case 1:
                  case 5:
                  case 9:
                  case 13:
                    continue;
                  case 2:
                    int num9 = ((num3 & 240) << 4) + (int) ms.ReadUInt8();
                    for (int index2 = 0; index2 < num9; ++index2)
                    {
                      int num7 = (int) ms.ReadUInt8();
                      if (!onlyPC && oldIndex.Contains(num7))
                      {
                        --ms.Position;
                        byte num8 = newIndex[oldIndex.IndexOf(num7)].ToByte();
                        ms.WriteByte(num8);
                        this.slpIndicesChanged = true;
                      }
                    }
                    continue;
                  case 3:
                    int num10 = (int) ms.ReadUInt8();
                    continue;
                  case 6:
                    int num11 = num3 >> 4;
                    if (num11 == 0)
                      num11 = (int) ms.ReadUInt8();
                    for (int index2 = 0; index2 < num11; ++index2)
                    {
                      int num7 = (int) ms.ReadUInt8();
                      if (onlyPC && oldIndex.Contains(num7))
                      {
                        --ms.Position;
                        byte num8 = newIndex[oldIndex.IndexOf(num7)].ToByte();
                        ms.WriteByte(num8);
                        this.slpIndicesChanged = true;
                      }
                    }
                    continue;
                  case 7:
                    if (num3 >> 4 == 0)
                    {
                      int num12 = (int) ms.ReadUInt8();
                    }
                    int num13 = (int) ms.ReadUInt8();
                    if (!onlyPC && oldIndex.Contains(num13))
                    {
                      --ms.Position;
                      byte num7 = newIndex[oldIndex.IndexOf(num13)].ToByte();
                      ms.WriteByte(num7);
                      this.slpIndicesChanged = true;
                      continue;
                    }
                    continue;
                  case 10:
                    if (num3 >> 4 == 0)
                    {
                      int num14 = (int) ms.ReadUInt8();
                    }
                    int num15 = (int) ms.ReadUInt8();
                    if (onlyPC && oldIndex.Contains(num15))
                    {
                      --ms.Position;
                      byte num7 = newIndex[oldIndex.IndexOf(num15)].ToByte();
                      ms.WriteByte(num7);
                      this.slpIndicesChanged = true;
                      continue;
                    }
                    continue;
                  case 11:
                    if (num3 >> 4 == 0)
                    {
                      int num7 = (int) ms.ReadUInt8();
                      continue;
                    }
                    continue;
                  case 14:
                    switch (num3)
                    {
                      case 14:
                      case 30:
                      case 46:
                      case 62:
                      case 78:
                      case 110:
                        continue;
                      case 94:
                        int num16 = (int) ms.ReadUInt8();
                        continue;
                      case 126:
                        int num17 = (int) ms.ReadUInt8();
                        continue;
                      case 158:
                        byte num18 = ms.ReadUInt8();
                        for (int index2 = 0; index2 < (int) num18; ++index2)
                          Color.FromArgb(ms.ReadInt32());
                        continue;
                      //default:
                      //  TextBox logBox1 = this.form1.logBox;
                      //  TextBox textBox1 = logBox1;
                        //string[] strArray1 = new string[6]
                        //{
                        //  //logBox1.Text,
                        //  Environment.NewLine,
                        //  "[SLP Error]\t\tUknown extended command ",
                        //  num3.ToString("X"),
                        //  " at position: ",
                        //  null
                        //};
                        num5 = ms.Position - 1L;
                        //strArray1[5] = num5.ToString("X");
                        //string str1 = string.Concat(strArray1);
                        //textBox1.Text = str1;
                        continue;
                    }
                                        break;
                  case 15:
                    goto label_46;
                  default:
                    //TextBox logBox2 = this.form1.logBox;
                    //TextBox textBox2 = logBox2;
                    //string[] strArray2 = new string[6]
                    //{
                    //  //logBox2.Text,
                    //  //Environment.NewLine,
                    //  "[SLP Error]\t\tUknown command case ",
                    //  num4.ToString("X"),
                    //  " at position: ",
                    //  null
                    //};
                    num5 = ms.Position - 1L;
                    //strArray2[5] = num5.ToString("X");
                    //string str2 = string.Concat(strArray2);
                    //textBox2.Text = str2;
                    continue;
                }
              }
              else
                break;
            }
label_46:
            ++num1;
          }
        }
      }
      catch
      {
        if (MessageBox.Show("The file may be in use by another program.", "File Save Error", MessageBoxButtons.RetryCancel) != DialogResult.Retry)
          return;
        this.ChangeSLPIndices(ms, oldIndex, newIndex, false);
      }
    }

    public void UpdateAutoPalette()
    {
      if (!Global.slpAutoColorPalette)
        return;
      if (this.version == "2.0N" && this.comment.Length >= 8)
      {
        if (this.comment.Substring(0, 8) == "RGE RLE " || this.comment.Substring(0, 8) == "AGE1 SLP")
        {
          //////this.slpPal = ColorTables.aoe_GraphicsPal.CopyColors(true);
          //this.slpPalName = "AoE_Graphics";
          if (!Global.viewerMode)
            return;
          Global.tileWidth = 65;
          Global.tileHeight = 33;
          Global.tileMidX = 32;
          Global.tileMidY = 16;
        }
        else if (this.comment.Substring(0, 8) == "ArtDesk ")
        {
          this.readPalette = true;
          if (!Global.viewerMode)
            return;
          Global.tileWidth = 97;
          Global.tileHeight = 49;
          Global.tileMidX = 48;
          Global.tileMidY = 24;
        }
        else if (this.comment.Contains("SLX Studio"))
        {
          this.readPalette = true;
        }
        else
        {
          ////this.slpPal = ColorTables.graphicsPal.CopyColors(true);
          //this.slpPalName = "Standard_Graphics";
          if (!Global.viewerMode)
            return;
          Global.tileWidth = 97;
          Global.tileHeight = 49;
          Global.tileMidX = 48;
          Global.tileMidY = 24;
        }
      }
      else if (this.version.Substring(0, 3) == "3.0")
      {
        ////this.slpPal = ColorTables.aoe_GraphicsPal.CopyColors(true);
        //this.slpPalName = "AoE_Graphics";
        if (!Global.viewerMode)
          return;
        Global.tileWidth = 65;
        Global.tileHeight = 33;
        Global.tileMidX = 32;
        Global.tileMidY = 16;
      }
      else
      {
        if (!(this.version.Substring(0, 2) == "4.") || !(this.version.Substring(3) == "X"))
          return;
        this.readPalette = true;
      }
    }

    public void UpdateAutoPalette(int properties, int offset = 0)
    {
      if (!Global.slpAutoColorPalette)
        return;
      bool flag = false;
      //this.slpPal = ColorTables.graphicsPal.CopyColors(true);
      //this.slpPalName = ColorTables.palList[0];
      if (this.comment.Contains("SLX Studio") || this.comment.Contains("ArtDesk") || this.comment.Contains("Ykkrosh"))
      {
        switch (offset)
        {
          case 65536:
            //this.slpPal = ColorTables.ef_GraphicsPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[1];
            flag = true;
            break;
          case 131072:
            //this.slpPal = ColorTables.gb_MenuPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[2];
            flag = true;
            break;
          case 196608:
            //this.slpPal = ColorTables.gb_GamePal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[3];
            flag = true;
            break;
          case 262144:
            //this.slpPal = ColorTables.gb_DatabankPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[4];
            flag = true;
            break;
          case 327680:
            //this.slpPal = ColorTables.gb_LoadScreenPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[5];
            flag = true;
            break;
          case 393216:
            //this.slpPal = ColorTables.gb_CpnBkgPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[6];
            flag = true;
            break;
          case 458752:
            //this.slpPal = ColorTables.aok_MenuPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[7];
            flag = true;
            break;
          case 524288:
            //this.slpPal = ColorTables.aok_MenuTCPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[8];
            flag = true;
            break;
          case 589824:
            //this.slpPal = ColorTables.aok_HistoryPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[9];
            flag = true;
            break;
          case 655360:
            //this.slpPal = ColorTables.aok_LoadScreenPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[10];
            flag = true;
            break;
          case 720896:
            ////this.slpPal = ColorTables.aoe_GraphicsPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[11];
            flag = true;
            break;
          case 786432:
            //this.slpPal = ColorTables.aoeDE_01_units.CopyColors(true);
            //this.slpPalName = ColorTables.palList[12];
            flag = true;
            break;
          case 851968:
            //this.slpPal = ColorTables.aoeDE_02_nature.CopyColors(true);
            //this.slpPalName = ColorTables.palList[13];
            flag = true;
            break;
          case 917504:
            //this.slpPal = ColorTables.aoeDE_03_bld_stonetool.CopyColors(true);
            //this.slpPalName = ColorTables.palList[14];
            flag = true;
            break;
          case 983040:
            //this.slpPal = ColorTables.aoeDE_04_bld_greek.CopyColors(true);
            //this.slpPalName = ColorTables.palList[15];
            flag = true;
            break;
          case 1048576:
            //this.slpPal = ColorTables.aoeDE_05_bld_babylon.CopyColors(true);
            //this.slpPalName = ColorTables.palList[16];
            flag = true;
            break;
          case 1114112:
            //this.slpPal = ColorTables.aoeDE_06_bld_roman.CopyColors(true);
            //this.slpPalName = ColorTables.palList[17];
            flag = true;
            break;
          case 1179648:
            //this.slpPal = ColorTables.aoeDE_07_tree_conifers.CopyColors(true);
            //this.slpPalName = ColorTables.palList[18];
            flag = true;
            break;
          case 1245184:
            //this.slpPal = ColorTables.aoeDE_08_tree_palms.CopyColors(true);
            //this.slpPalName = ColorTables.palList[19];
            flag = true;
            break;
          case 1310720:
            //this.slpPal = ColorTables.aoeDE_09_bld_asian.CopyColors(true);
            //this.slpPalName = ColorTables.palList[20];
            flag = true;
            break;
          case 1376256:
            //this.slpPal = ColorTables.aoeDE_10_bld_egypt.CopyColors(true);
            //this.slpPalName = ColorTables.palList[21];
            flag = true;
            break;
          case 1441792:
            //this.slpPal = ColorTables.aoeDE_effects.CopyColors(true);
            //this.slpPalName = ColorTables.palList[22];
            flag = true;
            break;
          case 1507328:
            //this.slpPal = ColorTables.aokHD_cliff.CopyColors(true);
            //this.slpPalName = ColorTables.palList[23];
            flag = true;
            break;
          case 1572864:
            //this.slpPal = ColorTables.aokHD_2_oaktree.CopyColors(true);
            //this.slpPalName = ColorTables.palList[24];
            flag = true;
            break;
          case 1638400:
            //this.slpPal = ColorTables.aokHD_3_palmtree.CopyColors(true);
            //this.slpPalName = ColorTables.palList[25];
            flag = true;
            break;
          case 1703936:
            //this.slpPal = ColorTables.aokHD_4_pinetree.CopyColors(true);
            //this.slpPalName = ColorTables.palList[26];
            flag = true;
            break;
          case 1769472:
            //this.slpPal = ColorTables.aokHD_5_snowpinetree.CopyColors(true);
            //this.slpPalName = ColorTables.palList[27];
            flag = true;
            break;
          case 1835008:
            //this.slpPal = ColorTables.aokHD_6_fire.CopyColors(true);
            //this.slpPalName = ColorTables.palList[28];
            flag = true;
            break;
          case 1900544:
            //this.slpPal = ColorTables.aokDE_16_bld_dark.CopyColors(true);
            //this.slpPalName = ColorTables.palList[29];
            flag = true;
            break;
          case 1966080:
            //this.slpPal = ColorTables.aokDE_17_bld_arabian.CopyColors(true);
            //this.slpPalName = ColorTables.palList[30];
            flag = true;
            break;
          case 2031616:
            //this.slpPal = ColorTables.aokDE_18_bld_seasian.CopyColors(true);
            //this.slpPalName = ColorTables.palList[30];
            flag = true;
            break;
          case 2097152:
            //this.slpPal = ColorTables.aokDE_19_bld_ceasian.CopyColors(true);
            //this.slpPalName = ColorTables.palList[31];
            flag = true;
            break;
          case 2162688:
            //this.slpPal = ColorTables.aokDE_20_bld_easteuro.CopyColors(true);
            //this.slpPalName = ColorTables.palList[32];
            flag = true;
            break;
          case 2228224:
            //this.slpPal = ColorTables.aokDE_21_bld_westeuro.CopyColors(true);
            //this.slpPalName = ColorTables.palList[33];
            flag = true;
            break;
          case 2293760:
            //this.slpPal = ColorTables.aokDE_22_bld_eastasian.CopyColors(true);
            //this.slpPalName = ColorTables.palList[34];
            flag = true;
            break;
          case 2359296:
            //this.slpPal = ColorTables.aokDE_23_bld_meso.CopyColors(true);
            //this.slpPalName = ColorTables.palList[35];
            flag = true;
            break;
          case 2424832:
            //this.slpPal = ColorTables.aokDE_24_bld_slavic.CopyColors(true);
            //this.slpPalName = ColorTables.palList[36];
            flag = true;
            break;
          case 2490368:
            //this.slpPal = ColorTables.aokDE_25_bld_african.CopyColors(true);
            //this.slpPalName = ColorTables.palList[37];
            flag = true;
            break;
          case 2555904:
            //this.slpPal = ColorTables.aokDE_26_bld_indian.CopyColors(true);
            //this.slpPalName = ColorTables.palList[38];
            flag = true;
            break;
          case 2621440:
            //this.slpPal = ColorTables.aokDE_27_bld_medi.CopyColors(true);
            //this.slpPalName = ColorTables.palList[39];
            flag = true;
            break;
          case 2686976:
            //this.slpPal = ColorTables.aokDE_28_scenario.CopyColors(true);
            //this.slpPalName = ColorTables.palList[40];
            flag = true;
            break;
          case 2752512:
            //this.slpPal = ColorTables.aokDE_trees.CopyColors(true);
            //this.slpPalName = ColorTables.palList[41];
            flag = true;
            break;
          case 2818048:
            //this.slpPal = ColorTables.aokDE_effects.CopyColors(true);
            //this.slpPalName = ColorTables.palList[42];
            flag = true;
            break;
        }
      }
      if (this.version == "2.0N" && !flag)
      {
        switch (properties)
        {
          case 65536:
            //this.slpPal = ColorTables.aokHD_cliff.CopyColors(true);
            //this.slpPalName = ColorTables.palList[24];
            break;
          case 131072:
            //this.slpPal = ColorTables.aokHD_2_oaktree.CopyColors(true);
            //this.slpPalName = ColorTables.palList[25];
            break;
          case 196608:
            //this.slpPal = ColorTables.aokHD_3_palmtree.CopyColors(true);
            //this.slpPalName = ColorTables.palList[26];
            break;
          case 262144:
            //this.slpPal = ColorTables.aokHD_4_pinetree.CopyColors(true);
            //this.slpPalName = ColorTables.palList[27];
            break;
          case 327680:
            //this.slpPal = ColorTables.aokHD_5_snowpinetree.CopyColors(true);
            //this.slpPalName = ColorTables.palList[28];
            break;
          case 393216:
            //this.slpPal = ColorTables.aokHD_6_fire.CopyColors(true);
            //this.slpPalName = ColorTables.palList[29];
            break;
          default:
            //this.slpPal = ColorTables.graphicsPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[0];
            break;
        }
        if (this.comment.Length != 24 || !this.comment.Contains("SLX Studio") || !(this.comment.Substring(8, 5) == "v1.1a") && !(this.comment.Substring(8, 4) == "v1.0") && !(this.comment.Substring(8, 2) == "v0"))
          return;
        switch (properties)
        {
          case 65536:
            //this.slpPal = ColorTables.ef_GraphicsPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[1];
            break;
          case 131072:
            //this.slpPal = ColorTables.gb_MenuPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[2];
            break;
          case 196608:
            //this.slpPal = ColorTables.gb_GamePal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[3];
            break;
          case 262144:
            //this.slpPal = ColorTables.gb_DatabankPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[4];
            break;
          case 327680:
            //this.slpPal = ColorTables.gb_LoadScreenPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[5];
            break;
          case 393216:
            //this.slpPal = ColorTables.gb_CpnBkgPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[6];
            break;
          case 458752:
            //this.slpPal = ColorTables.aok_MenuPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[7];
            break;
          case 524288:
            //this.slpPal = ColorTables.aok_MenuTCPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[8];
            break;
          case 589824:
            //this.slpPal = ColorTables.aok_HistoryPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[9];
            break;
          case 655360:
            //this.slpPal = ColorTables.aok_LoadScreenPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[10];
            break;
          case 720896:
            ////this.slpPal = ColorTables.aoe_GraphicsPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[11];
            break;
        }
      }
      else
      {
        if (!(this.version.Substring(0, 2) == "4.") || !(this.version.Substring(3) == "X") || flag)
          return;
        switch (properties)
        {
          case 0:
            ////this.slpPal = ColorTables.aoe_GraphicsPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[11];
            break;
          case 65536:
            //this.slpPal = ColorTables.aoeDE_01_units.CopyColors(true);
            //this.slpPalName = ColorTables.palList[12];
            break;
          case 131072:
            //this.slpPal = ColorTables.aoeDE_02_nature.CopyColors(true);
            //this.slpPalName = ColorTables.palList[13];
            break;
          case 196608:
            //this.slpPal = ColorTables.aoeDE_03_bld_stonetool.CopyColors(true);
            //this.slpPalName = ColorTables.palList[14];
            break;
          case 262144:
            //this.slpPal = ColorTables.aoeDE_04_bld_greek.CopyColors(true);
            //this.slpPalName = ColorTables.palList[15];
            break;
          case 327680:
            //this.slpPal = ColorTables.aoeDE_05_bld_babylon.CopyColors(true);
            //this.slpPalName = ColorTables.palList[16];
            break;
          case 393216:
            //this.slpPal = ColorTables.aoeDE_06_bld_roman.CopyColors(true);
            //this.slpPalName = ColorTables.palList[17];
            break;
          case 458752:
            //this.slpPal = ColorTables.aoeDE_07_tree_conifers.CopyColors(true);
            //this.slpPalName = ColorTables.palList[18];
            break;
          case 524288:
            //this.slpPal = ColorTables.aoeDE_08_tree_palms.CopyColors(true);
            //this.slpPalName = ColorTables.palList[19];
            break;
          case 589824:
            //this.slpPal = ColorTables.aoeDE_09_bld_asian.CopyColors(true);
            //this.slpPalName = ColorTables.palList[20];
            break;
          case 655360:
            //this.slpPal = ColorTables.aoeDE_10_bld_egypt.CopyColors(true);
            //this.slpPalName = ColorTables.palList[21];
            break;
          case 3538944:
          case 134217728:
            //this.slpPal = ColorTables.aoeDE_effects;
            //this.slpPalName = ColorTables.palList[22];
            break;
          default:
            ////this.slpPal = ColorTables.aoe_GraphicsPal.CopyColors(true);
            //this.slpPalName = ColorTables.palList[11];
            break;
        }
        if (!Global.viewerMode)
          return;
        Global.tileWidth = 81;
        Global.tileHeight = 41;
        Global.tileMidX = 40;
        Global.tileMidY = 20;
      }
    }

    public void UpdateSetPalette(int offset)
    {
      bool flag1 = false;
      bool flag2;
      switch (offset)
      {
        case 65536:
          //this.slpPal = ColorTables.ef_GraphicsPal;
          //this.slpPalName = ColorTables.palList[1];
          flag2 = true;
          break;
        case 131072:
          //this.slpPal = ColorTables.gb_MenuPal;
          //this.slpPalName = ColorTables.palList[2];
          flag2 = true;
          break;
        case 196608:
          //this.slpPal = ColorTables.gb_GamePal;
          //this.slpPalName = ColorTables.palList[3];
          flag2 = true;
          break;
        case 262144:
          //this.slpPal = ColorTables.gb_DatabankPal;
          //this.slpPalName = ColorTables.palList[4];
          flag2 = true;
          break;
        case 327680:
          //this.slpPal = ColorTables.gb_LoadScreenPal;
          //this.slpPalName = ColorTables.palList[5];
          flag2 = true;
          break;
        case 393216:
          //this.slpPal = ColorTables.gb_CpnBkgPal;
          //this.slpPalName = ColorTables.palList[6];
          flag2 = true;
          break;
        case 458752:
          //this.slpPal = ColorTables.aok_MenuPal;
          //this.slpPalName = ColorTables.palList[7];
          flag2 = true;
          break;
        case 524288:
          //this.slpPal = ColorTables.aok_MenuTCPal;
          //this.slpPalName = ColorTables.palList[8];
          flag2 = true;
          break;
        case 589824:
          //this.slpPal = ColorTables.aok_HistoryPal;
          //this.slpPalName = ColorTables.palList[9];
          flag2 = true;
          break;
        case 655360:
          //this.slpPal = ColorTables.aok_LoadScreenPal;
          //this.slpPalName = ColorTables.palList[10];
          flag2 = true;
          break;
        case 720896:
          //this.slpPal = ColorTables.aoe_GraphicsPal;
          //this.slpPalName = ColorTables.palList[11];
          flag2 = true;
          break;
        case 786432:
          //this.slpPal = ColorTables.aoeDE_01_units;
          //this.slpPalName = ColorTables.palList[12];
          flag2 = true;
          break;
        case 851968:
          //this.slpPal = ColorTables.aoeDE_02_nature;
          //this.slpPalName = ColorTables.palList[13];
          flag2 = true;
          break;
        case 917504:
          //this.slpPal = ColorTables.aoeDE_03_bld_stonetool;
          //this.slpPalName = ColorTables.palList[14];
          flag2 = true;
          break;
        case 983040:
          //this.slpPal = ColorTables.aoeDE_04_bld_greek;
          //this.slpPalName = ColorTables.palList[15];
          flag2 = true;
          break;
        case 1048576:
          //this.slpPal = ColorTables.aoeDE_05_bld_babylon;
          //this.slpPalName = ColorTables.palList[16];
          flag2 = true;
          break;
        case 1114112:
          //this.slpPal = ColorTables.aoeDE_06_bld_roman;
          //this.slpPalName = ColorTables.palList[17];
          flag2 = true;
          break;
        case 1179648:
          //this.slpPal = ColorTables.aoeDE_07_tree_conifers;
          //this.slpPalName = ColorTables.palList[18];
          flag2 = true;
          break;
        case 1245184:
          //this.slpPal = ColorTables.aoeDE_08_tree_palms;
          //this.slpPalName = ColorTables.palList[19];
          flag2 = true;
          break;
        case 1310720:
          //this.slpPal = ColorTables.aoeDE_09_bld_asian;
          //this.slpPalName = ColorTables.palList[20];
          flag2 = true;
          break;
        case 1376256:
          //this.slpPal = ColorTables.aoeDE_10_bld_egypt;
          //this.slpPalName = ColorTables.palList[21];
          flag2 = true;
          break;
        case 1441792:
          //this.slpPal = ColorTables.aoeDE_effects;
          //this.slpPalName = ColorTables.palList[22];
          flag2 = true;
          break;
        case 1507328:
          //this.slpPal = ColorTables.aokHD_cliff;
          //this.slpPalName = ColorTables.palList[23];
          flag2 = true;
          break;
        case 1572864:
          //this.slpPal = ColorTables.aokHD_2_oaktree;
          //this.slpPalName = ColorTables.palList[24];
          flag2 = true;
          break;
        case 1638400:
          //this.slpPal = ColorTables.aokHD_3_palmtree;
          //this.slpPalName = ColorTables.palList[25];
          flag2 = true;
          break;
        case 1703936:
          //this.slpPal = ColorTables.aokHD_4_pinetree;
          //this.slpPalName = ColorTables.palList[26];
          flag2 = true;
          break;
        case 1769472:
          //this.slpPal = ColorTables.aokHD_5_snowpinetree;
          //this.slpPalName = ColorTables.palList[27];
          flag2 = true;
          break;
        case 1835008:
          //this.slpPal = ColorTables.aokHD_6_fire;
          //this.slpPalName = ColorTables.palList[28];
          flag2 = true;
          break;
        case 1900544:
          //this.slpPal = ColorTables.aokDE_16_bld_dark;
          //this.slpPalName = ColorTables.palList[29];
          flag2 = true;
          break;
        case 1966080:
          //this.slpPal = ColorTables.aokDE_17_bld_arabian;
          //this.slpPalName = ColorTables.palList[30];
          flag2 = true;
          break;
        case 2031616:
          //this.slpPal = ColorTables.aokDE_18_bld_seasian;
          //this.slpPalName = ColorTables.palList[30];
          flag2 = true;
          break;
        case 2097152:
          //this.slpPal = ColorTables.aokDE_19_bld_ceasian;
          //this.slpPalName = ColorTables.palList[31];
          flag2 = true;
          break;
        case 2162688:
          //this.slpPal = ColorTables.aokDE_20_bld_easteuro;
          //this.slpPalName = ColorTables.palList[32];
          flag2 = true;
          break;
        case 2228224:
          //this.slpPal = ColorTables.aokDE_21_bld_westeuro;
          //this.slpPalName = ColorTables.palList[33];
          flag2 = true;
          break;
        case 2293760:
          //this.slpPal = ColorTables.aokDE_22_bld_eastasian;
          //this.slpPalName = ColorTables.palList[34];
          flag2 = true;
          break;
        case 2359296:
          //this.slpPal = ColorTables.aokDE_23_bld_meso;
          //this.slpPalName = ColorTables.palList[35];
          flag2 = true;
          break;
        case 2424832:
          //this.slpPal = ColorTables.aokDE_24_bld_slavic;
          //this.slpPalName = ColorTables.palList[36];
          flag2 = true;
          break;
        case 2490368:
          //this.slpPal = ColorTables.aokDE_25_bld_african;
          //this.slpPalName = ColorTables.palList[37];
          flag2 = true;
          break;
        case 2555904:
          //this.slpPal = ColorTables.aokDE_26_bld_indian;
          //this.slpPalName = ColorTables.palList[38];
          flag2 = true;
          break;
        case 2621440:
          //this.slpPal = ColorTables.aokDE_27_bld_medi;
          //this.slpPalName = ColorTables.palList[39];
          flag2 = true;
          break;
        case 2686976:
          //this.slpPal = ColorTables.aokDE_28_scenario;
          //this.slpPalName = ColorTables.palList[40];
          flag2 = true;
          break;
        case 2752512:
          //this.slpPal = ColorTables.aokDE_trees;
          //this.slpPalName = ColorTables.palList[41];
          flag2 = true;
          break;
        case 2818048:
          //this.slpPal = ColorTables.aokDE_effects;
          //this.slpPalName = ColorTables.palList[42];
          flag2 = true;
          break;
        default:
          if (flag1)
            break;
          //this.slpPal = ColorTables.graphicsPal;
          //this.slpPalName = ColorTables.palList[0];
          break;
      }
    }

    public void CloseFS()
    {
      this.fs.Close();
    }

    public void UpdateFS(string newFileName)
    {
      this.fs = File.Open(newFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    public void ClearSLP()
    {
      this.version = (string) null;
      this.numFrames = 0;
      this.type = 0;
      this.numDirections = 0;
      this.framesPerDirection = 0;
      this.paletteID = 0;
      this.frameInfoOffset = 0;
      this.layerInfoOffset = 0;
      this.comment = (string) null;
      this.frameDataOffsets = (int[]) null;
      this.frameOutlineOffset = (int[]) null;
      this.framePaletteOffset = (int[]) null;
      this.framePaletteID = (int[]) null;
      this.frameWidth = (int[]) null;
      this.frameHeight = (int[]) null;
      this.frameAnchorX = (int[]) null;
      this.frameAnchorY = (int[]) null;
      this.layerDataOffsets = (int[]) null;
      this.layerOutlineOffset = (int[]) null;
      this.layerProperties = (int[]) null;
      this.layerWidth = (int[]) null;
      this.layerHeight = (int[]) null;
      this.layerAnchorX = (int[]) null;
      this.layerAnchorY = (int[]) null;
      this.numRows = 0;
      this.numColumns = 0;
    }

    public void CloseSLP()
    {
      if (this.fs != null)
        this.fs.Close();
      this.ClearSLP();
    }

    public void DecodeVooblySLP(
      FileStream fs,
      string originalFile,
      string newSLPFile,
      bool overwrite,
      bool reloadSLP = false)
    {
      this.slpDecodeCompleted = false;
      FileStream fileStream = File.Open(newSLPFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
      int int32 = Convert.ToInt32(fs.Length - 4L);
      byte[] buffer = new byte[int32];
      fs.Read(buffer, 0, int32);
      for (int index = 0; index < int32; ++index)
        buffer[index] = (32 * ((int) buffer[index] - 17 ^ 35) | (int) (byte) ((int) buffer[index] - 17 ^ 35) >> 3).ToByte();
      fileStream.Write(buffer, 0, int32);
      fileStream.SetLength((long) int32);
      fs.Close();
      fileStream.Close();
      if (overwrite)
      {
        if (File.Exists(originalFile))
          File.Delete(originalFile);
        if (File.Exists(newSLPFile))
          File.Move(newSLPFile, originalFile);
        if (reloadSLP)
          this.LoadSLP(originalFile);
      }
      else if (reloadSLP)
        this.LoadSLP(newSLPFile);
      this.slpDecodeCompleted = true;
    }

//    public Bitmap DrawFrame(
//      int frameNum,
//      bool outlines,
//      bool shadows,
//      bool mask,
//      bool data = false)
//    {
//      if (this.fs == null)
//      {
//        this.slpRenderCompleted = true;
//        return new Bitmap(1, 1);
//      }
//      if (this.frameWidth[frameNum] <= 0 || this.frameHeight[frameNum] <= 0)
//      {
//        this.slpRenderCompleted = true;
//        return new Bitmap(1, 1);
//      }
//      this.is32Bit = (this.framePaletteID[frameNum] & 7) == 7;
//      if (this.readPalette)
//        this.UpdateAutoPalette(this.framePaletteID[frameNum], this.framePaletteOffset[frameNum]);
//      this.fs.Position = this.filePos + (long) this.frameOutlineOffset[frameNum];
//      if (this.fs.Position >= this.fs.Length)
//      {
//        int num = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
//        return new Bitmap(1, 1);
//      }
//      int[] numArray1 = new int[this.frameHeight[frameNum]];
//      int[] numArray2 = new int[this.frameHeight[frameNum]];
//      try
//      {
//        for (int index = 0; index < this.frameHeight[frameNum]; ++index)
//        {
//          numArray1[index] = Convert.ToInt32(this.fs.ReadUInt16());
//          numArray2[index] = Convert.ToInt32(this.fs.ReadUInt16());
//          if (numArray1[index] == 32768 && numArray2[index] == 32768)
//          {
//            numArray2[index] = 0;
//          }
//          else
//          {
//            if (numArray1[index] < 0 || numArray1[index] > this.frameWidth[frameNum])
//              numArray1[index] = this.frameWidth[frameNum];
//            if (numArray2[index] < 0)
//              numArray2[index] = 0;
//            if (numArray2[index] > this.frameWidth[frameNum])
//              numArray2[index] = this.frameWidth[frameNum];
//          }
//        }
//      }
//      catch
//      {
//        int num = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
//        return new Bitmap(1, 1);
//      }
//      this.fs.Position = this.filePos + (long) this.frameDataOffsets[frameNum];
//      if (this.fs.Position >= this.fs.Length)
//      {
//        int num = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
//        return new Bitmap(1, 1);
//      }
//      int[] numArray3 = new int[this.frameHeight[frameNum]];
//      for (int index = 0; index < this.frameHeight[frameNum]; ++index)
//        numArray3[index] = this.fs.ReadInt32();
//      Bitmap frame = new Bitmap(this.frameWidth[frameNum], this.frameHeight[frameNum], PixelFormat.Format32bppArgb);
//      SolidBrush solidBrush = new SolidBrush(Global.sxpMaskColor);
//      if (this.is32Bit && !mask)
//        solidBrush = new SolidBrush(Color.FromArgb((int) byte.MaxValue, 0, 0, 0));
//      if (data)
//        solidBrush = new SolidBrush(Color.FromArgb((int) byte.MaxValue, 0, (int) byte.MaxValue));
//      using (Graphics graphics = Graphics.FromImage((Image) frame))
//        graphics.FillRectangle((Brush) solidBrush, new Rectangle(0, 0, frame.Width, frame.Height));
//      if (!mask)
//        frame.MakeTransparent();
//      int lineNum = 0;
//      foreach (int num1 in numArray3)
//      {
//        if (num1 < 0)
//        {
//          int num2 = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
//          return frame;
//        }
//        this.fs.Position = this.filePos + (long) num1;
//        if (this.fs.Position >= this.fs.Length)
//        {
//          int num2 = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
//          return frame;
//        }
//        int linePos = 0;
//        int num3 = -1;
//        if (numArray1[lineNum] > 0 || numArray1[lineNum] <= frame.Width)
//          linePos += numArray1[lineNum];
//        else if (numArray1[lineNum] == 32768)
//        {
//          linePos = frame.Width;
//          num3 = 15;
//        }
//        try
//        {
//          while (num3 != 15)
//          {
//            if (linePos < frame.Width - numArray2[lineNum])
//            {
//              num3 = (int) this.fs.ReadUInt8();
//              if (num3 != 15)
//              {
//                int num2 = num3 & 15;
//                switch (num2)
//                {
//                  case 0:
//                  case 4:
//                  case 8:
//                  case 12:
//                    int num4 = num3 >> 2;
//                    for (int index = 0; index < num4; ++index)
//                    {
//                      Color color;
//                      if (this.is32Bit)
//                      {
//                        color = Color.FromArgb(this.fs.ReadInt32());
//                      }
//                      else
//                      {
//                        int num5 = 0;
//                        if (this.type == 32)
//                          num5 = (int) this.fs.ReadUInt8();
//                        //color = this.slpPal[(int) this.fs.ReadUInt8()];
//                        if (this.type == 32 && this.decayTime > num5)
//                          color = Color.FromArgb(0, 0, 0, 0);
//                      }
//                      frame = data ? this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue)) : this.DrawPixels(frame, lineNum, linePos, 1, color);
//                      ++linePos;
//                    }
//                    continue;
//                  case 1:
//                  case 5:
//                  case 9:
//                  case 13:
//                    int num6 = num3 >> 2;
//                    linePos += num6;
//                    continue;
//                  case 2:
//                    int num7 = ((num3 & 240) << 4) + (int) this.fs.ReadUInt8();
//                    for (int index = 0; index < num7; ++index)
//                    {
//                      Color color;
//                      if (this.is32Bit)
//                      {
//                        color = Color.FromArgb(this.fs.ReadInt32());
//                      }
//                      else
//                      {
//                        int num5 = 0;
//                        if (this.type == 32)
//                          num5 = (int) this.fs.ReadUInt8();
//                        color = //this.slpPal[(int) this.fs.ReadUInt8()];
//                        if (this.type == 32 && this.decayTime > num5)
//                          color = Color.FromArgb(0, 0, 0, 0);
//                      }
//                      frame = data ? this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue)) : this.DrawPixels(frame, lineNum, linePos, 1, color);
//                      ++linePos;
//                    }
//                    continue;
//                  case 3:
//                    int num8 = ((num3 & 240) << 4) + (int) this.fs.ReadUInt8();
//                    linePos += num8;
//                    continue;
//                  case 6:
//                    int num9 = num3 >> 4;
//                    if (num9 == 0)
//                      num9 = (int) this.fs.ReadUInt8();
//                    for (int index1 = 0; index1 < num9; ++index1)
//                    {
//                      int num5 = 0;
//                      if (this.type == 32)
//                        num5 = (int) this.fs.ReadUInt8();
//                      byte num10 = this.fs.ReadUInt8();
//                      int num11 = num10.ToInt();
//                      int index2 = Global.slpPlayerColorStartIndex + num11;
//                      Color color;
//                      if (this.version.Substring(0, 2) == "4." && this.version.Substring(3) == "X")
//                      {
//                        if (num10 > (byte) 127)
//                          num10 = (byte) 15;
//                        //color = this.slpPlayerColorPal[(int) num10];
//                      }
//                      else
//                        //color = !this.is32Bit ? /his.slpPal[index2] : ColorTables.graphicsPal[index2];
//                      if (this.type == 32 && this.decayTime > num5)
//                        color = Color.FromArgb(0, 0, 0, 0);
//                      frame = data ? this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int) byte.MaxValue, 0, (int) byte.MaxValue, 0)) : this.DrawPixels(frame, lineNum, linePos, 1, color);
//                      ++linePos;
//                    }
//                    continue;
//                  case 7:
//                    int length1 = num3 >> 4;
//                    if (length1 == 0)
//                      length1 = (int) this.fs.ReadUInt8();
//                    Color color1;
//                    if (this.is32Bit)
//                    {
//                      color1 = Color.FromArgb(this.fs.ReadInt32());
//                    }
//                    else
//                    {
//                      int num5 = 0;
//                      if (this.type == 32)
//                        num5 = (int) this.fs.ReadUInt8();
//                      //color1 = //this.slpPal[(int) this.fs.ReadUInt8()];
//                      if (this.type == 32 && this.decayTime > num5)
//                        color1 = Color.FromArgb(0, 0, 0, 0);
//                    }
//                    frame = data ? this.DrawPixels(frame, lineNum, linePos, length1, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue)) : this.DrawPixels(frame, lineNum, linePos, length1, color1);
//                    linePos += length1;
//                    continue;
//                  case 10:
//                    int length2 = num3 >> 4;
//                    if (length2 == 0)
//                      length2 = (int) this.fs.ReadUInt8();
//                    int num12 = 0;
//                    if (this.type == 32)
//                      num12 = (int) this.fs.ReadUInt8();
//                    byte num13 = this.fs.ReadUInt8();
//                    int num14 = num13.ToInt();
//                    int index3 = Global.slpPlayerColorStartIndex + num14;
//                    Color color2;
//                    if (this.version.Substring(0, 2) == "4." && this.version.Substring(3) == "X")
//                    {
//                      if (num13 > (byte) 127)
//                        num13 = (byte) 15;
//                      //color2 = this.slpPlayerColorPal[(int) num13];
//                    }
//                    else
//                      //color2 = !this.is32Bit ? //this.slpPal[index3] : ColorTables.graphicsPal[index3];
//                    if (this.type == 32 && this.decayTime > num12)
//                      color2 = Color.FromArgb(0, 0, 0, 0);
//                    frame = data ? this.DrawPixels(frame, lineNum, linePos, length2, Color.FromArgb((int) byte.MaxValue, 0, (int) byte.MaxValue, 0)) : this.DrawPixels(frame, lineNum, linePos, length2, color2);
//                    linePos += length2;
//                    continue;
//                  case 11:
//                    int length3 = num3 >> 4;
//                    if (length3 == 0)
//                      length3 = (int) this.fs.ReadUInt8();
//                    if (!data)
//                    {
//                      if (shadows)
//                        frame = this.DrawPixels(frame, lineNum, linePos, length3, this.slpShadowColor);
//                    }
//                    else
//                      frame = this.DrawPixels(frame, lineNum, linePos, length3, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, 0, 0));
//                    linePos += length3;
//                    continue;
//                  case 14:
//                    switch (num3)
//                    {
//                      case 14:
//                      case 46:
//                        continue;
//                      case 30:
//                        TextBox logBox1 = this.form1.logBox;
//                        logBox1.Text = logBox1.Text + Environment.NewLine + "[SLP Error]\t\tObsolete extended command " + num3.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
//                        return frame;
//                      case 62:
//                        TextBox logBox2 = this.form1.logBox;
//                        logBox2.Text = logBox2.Text + Environment.NewLine + "[SLP Error]\t\tObsolete extended command " + num3.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
//                        return frame;
//                      case 78:
//                        if (!data)
//                        {
//                          if (outlines)
//                            frame = this.DrawPixels(frame, lineNum, linePos, 1, this.slpOutline1Color);
//                        }
//                        else
//                          frame = this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int) byte.MaxValue, 0, 0, (int) byte.MaxValue));
//                        ++linePos;
//                        continue;
//                      case 94:
//                        byte num15 = this.fs.ReadUInt8();
//                        if (!data)
//                        {
//                          if (outlines)
//                            frame = this.DrawPixels(frame, lineNum, linePos, (int) num15, this.slpOutline1Color);
//                        }
//                        else
//                          frame = this.DrawPixels(frame, lineNum, linePos, (int) num15, Color.FromArgb((int) byte.MaxValue, 0, 0, (int) byte.MaxValue));
//                        linePos += (int) num15;
//                        continue;
//                      case 110:
//                        if (!data)
//                        {
//                          if (outlines)
//                            frame = this.DrawPixels(frame, lineNum, linePos, 1, this.slpOutline2Color);
//                        }
//                        else
//                          frame = this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 0));
//                        ++linePos;
//                        continue;
//                      case 126:
//                        byte num16 = this.fs.ReadUInt8();
//                        if (!data)
//                        {
//                          if (outlines)
//                            frame = this.DrawPixels(frame, lineNum, linePos, (int) num16, this.slpOutline2Color);
//                        }
//                        else
//                          frame = this.DrawPixels(frame, lineNum, linePos, (int) num16, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, 0));
//                        linePos += (int) num16;
//                        continue;
//                      case 158:
//                      case 174:
//                        byte num17 = this.fs.ReadUInt8();
//                        for (int index1 = 0; index1 < (int) num17; ++index1)
//                        {
//                          Color color3 = Color.FromArgb(this.fs.ReadInt32());
//                          color3 = Color.FromArgb((int) byte.MaxValue - (int) color3.A, (int) color3.R, (int) color3.G, (int) color3.B);
//                          if (!data)
//                            this.DrawPixels(frame, lineNum, linePos, 1, color3);
//                          else
//                            frame = this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue, (int) byte.MaxValue));
//                          ++linePos;
//                        }
//                        continue;
//                      default:
//                        TextBox logBox3 = this.form1.logBox;
//                        logBox3.Text = logBox3.Text + Environment.NewLine + "[SLP Error]\t\tUknown extended command " + num3.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
//                        return frame;
//                    }
//                  case 15:
//                    goto label_146;
//                  default:
//                    TextBox logBox4 = this.form1.logBox;
//                    logBox4.Text = logBox4.Text + Environment.NewLine + "[SLP Error]\t\tUknown command case " + num2.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
//                    return frame;
//                }
//              }
//              else
//                break;
//            }
//            else
//              break;
//          }
//        }
//        catch
//        {
//          int num2 = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
//          return frame;
//        }
//label_146:
//        if (numArray2[lineNum] != 0 && numArray2[lineNum] <= frame.Width && numArray1[lineNum] < frame.Width)
//        {
//          int num18 = linePos + numArray2[lineNum];
//        }
//        ++lineNum;
//      }
//      this.slpRenderCompleted = true;
//      return frame;
//    }

    public Bitmap DrawFrameAlphas(Bitmap frame, int frameNum, bool mask = false)
    {
      if (this.fs == null)
      {
        this.slpRenderCompleted = true;
        return frame;
      }
      if (this.layerWidth[frameNum] != frame.Width || this.layerHeight[frameNum] != frame.Height)
      {
        this.slpRenderCompleted = true;
        return frame;
      }
      this.fs.Position = this.filePos + (long) this.layerOutlineOffset[frameNum];
      if (this.fs.Position >= this.fs.Length)
      {
        int num = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
        return frame;
      }
      int[] numArray1 = new int[this.layerHeight[frameNum]];
      int[] numArray2 = new int[this.layerHeight[frameNum]];
      try
      {
        for (int index = 0; index < this.layerHeight[frameNum]; ++index)
        {
          numArray1[index] = Convert.ToInt32(this.fs.ReadUInt16());
          numArray2[index] = Convert.ToInt32(this.fs.ReadUInt16());
          if (numArray1[index] == 32768 && numArray2[index] == 32768)
          {
            numArray2[index] = 0;
          }
          else
          {
            if (numArray1[index] < 0 || numArray1[index] > this.layerWidth[frameNum])
              numArray1[index] = this.layerWidth[frameNum];
            if (numArray2[index] < 0)
              numArray2[index] = 0;
            if (numArray2[index] > this.layerWidth[frameNum])
              numArray2[index] = this.layerWidth[frameNum];
          }
        }
      }
      catch
      {
        int num = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
        return frame;
      }
      this.fs.Position = this.filePos + (long) this.layerDataOffsets[frameNum];
      if (this.fs.Position >= this.fs.Length)
      {
        int num = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
        return frame;
      }
      int[] numArray3 = new int[this.layerHeight[frameNum]];
      try
      {
        for (int index = 0; index < this.layerHeight[frameNum]; ++index)
          numArray3[index] = this.fs.ReadInt32();
      }
      catch
      {
        int num = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
        return frame;
      }
      DirectBitmap directBitmap = new DirectBitmap(frame);
      Bitmap frame1 = new Bitmap(this.layerWidth[frameNum], this.layerHeight[frameNum], PixelFormat.Format32bppArgb);
      if (mask)
      {
        SolidBrush solidBrush = new SolidBrush(Global.sxpMaskColor);
        using (Graphics graphics = Graphics.FromImage((Image) frame1))
          graphics.FillRectangle((Brush) solidBrush, new Rectangle(0, 0, frame1.Width, frame1.Height));
      }
      int index1 = 0;
      foreach (int num1 in numArray3)
      {
        if (num1 < 0)
        {
          int num2 = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
          return frame;
        }
        this.fs.Position = this.filePos + (long) num1;
        if (this.fs.Position >= this.fs.Length)
        {
          int num2 = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
          return frame;
        }
        int num3 = 0;
        int num4 = -1;
        if (numArray1[index1] > 0 || numArray1[index1] <= frame.Width)
          num3 += numArray1[index1];
        else if (numArray1[index1] == 32768)
        {
          num3 = frame.Width;
          num4 = 15;
        }
        try
        {
          while (num4 != 15)
          {
            if (num3 < frame.Width - numArray2[index1])
            {
              num4 = (int) this.fs.ReadUInt8();
              if (num4 != 15)
              {
                int num2 = num4 & 15;
                switch (num2)
                {
                  case 0:
                  case 4:
                  case 8:
                  case 12:
                    int num5 = num4 >> 2;
                    for (int index2 = 0; index2 < num5; ++index2)
                    {
                      int alpha = (int) this.fs.ReadUInt8();
                      Color pixel = directBitmap.GetPixel(num3, index1);
                      int r = (int) pixel.R;
                      int g = (int) pixel.G;
                      int b = (int) pixel.B;
                      Color color = Color.FromArgb(alpha, r, g, b);
                      frame1 = this.DrawPixels(frame1, index1, num3, 1, color);
                      ++num3;
                    }
                    continue;
                  case 1:
                  case 5:
                  case 9:
                  case 13:
                    int num6 = num4 >> 2;
                    num3 += num6;
                    continue;
                  case 2:
                    int num7 = ((num4 & 240) << 4) + (int) this.fs.ReadUInt8();
                    for (int index2 = 0; index2 < num7; ++index2)
                    {
                      int alpha = (int) this.fs.ReadUInt8();
                      Color pixel = directBitmap.GetPixel(num3, index1);
                      int r = (int) pixel.R;
                      int g = (int) pixel.G;
                      int b = (int) pixel.B;
                      Color color = Color.FromArgb(alpha, r, g, b);
                      frame1 = this.DrawPixels(frame1, index1, num3, 1, color);
                      ++num3;
                    }
                    continue;
                  case 3:
                    int num8 = ((num4 & 240) << 4) + (int) this.fs.ReadUInt8();
                    num3 += num8;
                    continue;
                  case 7:
                    int num9 = num4 >> 4;
                    if (num9 == 0)
                      num9 = (int) this.fs.ReadUInt8();
                    int alpha1 = (int) this.fs.ReadUInt8();
                    for (int index2 = 0; index2 < num9; ++index2)
                    {
                      Color color = directBitmap.GetPixel(num3, index1);
                      color = Color.FromArgb(alpha1, (int) color.R, (int) color.G, (int) color.B);
                      frame1 = this.DrawPixels(frame1, index1, num3, 1, color);
                      ++num3;
                    }
                    continue;
                  case 15:
                    goto label_62;
                  default:
                    //TextBox logBox = this.form1.logBox;
                    //logBox.Text = logBox.Text + Environment.NewLine + "[SLP Error]\t\tUknown command case " + num2.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
                    frame = new Bitmap((Image) directBitmap.Bitmap);
                    return frame;
                }
              }
              else
                break;
            }
            else
              break;
          }
        }
        catch
        {
          int num2 = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
          return frame;
        }
label_62:
        if (numArray2[index1] != 0 && numArray2[index1] <= frame.Width && numArray1[index1] < frame.Width)
        {
          int num10 = num3 + numArray2[index1];
        }
        ++index1;
      }
      frame.Dispose();
      directBitmap.Dispose();
      GC.Collect();
      this.slpRenderCompleted = true;
      return frame1;
    }

    public Bitmap DrawShadowFrame(int frameNum, bool data = false)
    {
      if (this.fs == null)
      {
        this.slpRenderCompleted = true;
        return new Bitmap(1, 1);
      }
      if (this.layerWidth[frameNum] <= 0 || this.layerHeight[frameNum] <= 0)
      {
        this.slpRenderCompleted = true;
        return new Bitmap(1, 1);
      }
      if (this.type == 32 && this.decayTime > 0)
        return new Bitmap(1, 1);
      this.fs.Position = this.filePos + (long) this.layerOutlineOffset[frameNum];
      int[] numArray1 = new int[this.layerHeight[frameNum]];
      int[] numArray2 = new int[this.layerHeight[frameNum]];
      try
      {
        for (int index = 0; index < this.layerHeight[frameNum]; ++index)
        {
          numArray1[index] = Convert.ToInt32(this.fs.ReadUInt16());
          numArray2[index] = Convert.ToInt32(this.fs.ReadUInt16());
          if (numArray1[index] == 32768 && numArray2[index] == 32768)
          {
            numArray2[index] = 0;
          }
          else
          {
            if (numArray1[index] < 0 || numArray1[index] > this.layerWidth[frameNum])
              numArray1[index] = this.layerWidth[frameNum];
            if (numArray2[index] < 0)
              numArray2[index] = 0;
            if (numArray2[index] > this.layerWidth[frameNum])
              numArray2[index] = this.layerWidth[frameNum];
          }
        }
      }
      catch
      {
        int num = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
        return new Bitmap(1, 1);
      }
      this.fs.Position = this.filePos + (long) this.layerDataOffsets[frameNum];
      int[] numArray3 = new int[this.layerHeight[frameNum]];
      try
      {
        for (int index = 0; index < this.layerHeight[frameNum]; ++index)
          numArray3[index] = this.fs.ReadInt32();
      }
      catch
      {
        int num = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
        return new Bitmap(1, 1);
      }
      Bitmap frame = new Bitmap(this.layerWidth[frameNum], this.layerHeight[frameNum], PixelFormat.Format32bppArgb);
      int lineNum = 0;
      foreach (int num1 in numArray3)
      {
        if (num1 < 0)
        {
          int num2 = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
          return frame;
        }
        this.fs.Position = this.filePos + (long) num1;
        if (this.fs.Position >= this.fs.Length)
        {
          int num2 = (int) MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
          return frame;
        }
        int linePos = 0;
        int num3 = -1;
        if (numArray1[lineNum] > 0 || numArray1[lineNum] <= frame.Width)
          linePos += numArray1[lineNum];
        else if (numArray1[lineNum] == 32768)
        {
          linePos = frame.Width;
          num3 = 15;
        }
        try
        {
          while (num3 != 15)
          {
            if (linePos < frame.Width - numArray2[lineNum])
            {
              num3 = (int) this.fs.ReadUInt8();
              if (num3 != 15)
              {
                int num2 = num3 & 15;
                switch (num2)
                {
                  case 0:
                  case 4:
                  case 8:
                  case 12:
                    int num4 = num3 >> 2;
                    for (int index = 0; index < num4; ++index)
                    {
                      int num5 = (int) this.fs.ReadUInt8();
                      if (num5 >= 64)
                        num5 = 63;
                      Color color = Color.FromArgb((int) byte.MaxValue - num5 * 4, 0, 0, 0);
                      frame = data ? this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, 0, 0)) : this.DrawPixels(frame, lineNum, linePos, 1, color);
                      ++linePos;
                    }
                    continue;
                  case 1:
                  case 5:
                  case 9:
                  case 13:
                    int num6 = num3 >> 2;
                    linePos += num6;
                    continue;
                  case 2:
                    int num7 = ((num3 & 240) << 4) + (int) this.fs.ReadUInt8();
                    for (int index = 0; index < num7; ++index)
                    {
                      int num5 = (int) this.fs.ReadUInt8();
                      if (num5 >= 64)
                        num5 = 63;
                      Color color = Color.FromArgb((int) byte.MaxValue - num5 * 4, 0, 0, 0);
                      frame = data ? this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, 0, 0)) : this.DrawPixels(frame, lineNum, linePos, 1, color);
                      ++linePos;
                    }
                    continue;
                  case 3:
                    int num8 = ((num3 & 240) << 4) + (int) this.fs.ReadUInt8();
                    linePos += num8;
                    continue;
                  case 7:
                    int length = num3 >> 4;
                    if (length == 0)
                      length = (int) this.fs.ReadUInt8();
                    int num9 = (int) this.fs.ReadUInt8();
                    if (num9 >= 64)
                      num9 = 63;
                    Color color1 = Color.FromArgb((int) byte.MaxValue - num9 * 4, 0, 0, 0);
                    frame = data ? this.DrawPixels(frame, lineNum, linePos, length, Color.FromArgb((int) byte.MaxValue, (int) byte.MaxValue, 0, 0)) : this.DrawPixels(frame, lineNum, linePos, length, color1);
                    linePos += length;
                    continue;
                  case 15:
                    goto label_58;
                  default:
                    //TextBox logBox = this.form1.logBox;
                    //logBox.Text = logBox.Text + Environment.NewLine + "[SLP Error]\t\tUknown command case " + num2.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
                    return frame;
                }
              }
              else
                break;
            }
            else
              break;
          }
        }
        catch
        {
          int num2 = (int) MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
          return frame;
        }
label_58:
        if (numArray2[lineNum] != 0 && numArray2[lineNum] <= frame.Width && numArray1[lineNum] < frame.Width)
        {
          int num10 = linePos + numArray2[lineNum];
        }
        ++lineNum;
      }
      this.slpRenderCompleted = true;
      return frame;
    }

    private Bitmap DrawPixels(
      Bitmap frame,
      int lineNum,
      int linePos,
      int length,
      Color color)
    {
      using (Graphics graphics = Graphics.FromImage((Image) frame))
      {
        SolidBrush solidBrush = new SolidBrush(color);
        graphics.FillRectangle((Brush) solidBrush, new Rectangle(linePos, lineNum, length, 1));
      }
      return frame;
    }

    private void SlpImporter_DoWork(object sender, DoWorkEventArgs e)
    {
      this.percentage = 0;
      this.multipleFiles = false;
      this.frameCount = 0;
      this.CreateSLX(this.itemName, this.dataName, this.slpOpenFileName, this.destination);
      if (Global.cancelProgress)
      {
        e.Cancel = true;
      }
      else
      {
        while (this.percentage < 100)
        {
          ++this.percentage;
          this.SlpImporter.ReportProgress(this.percentage);
        }
      }
    }

    private void SlpImporter_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      //this.mi = (MethodInvoker) (() => this.fpBar.progressBar.Value = e.ProgressPercentage);
      //if (this.fpBar.InvokeRequired)
      //  this.fpBar.Invoke((Delegate) this.mi);
      //else
      //  this.mi();
      //this.mi = (MethodInvoker) (() => this.fpBar.percentLabel.Text = "(" + this.fpBar.progressBar.Value.ToString() + "%)");
      //if (this.fpBar.InvokeRequired)
      //  this.fpBar.Invoke((Delegate) this.mi);
      //else
      //  this.mi();
    }

        //private async void SlpImporter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //  if (e.Cancelled)
        //  {
        //    this.mi = (MethodInvoker) (() => this.fpBar.progressLabel.Text = "Cancelling process... ");
        //    if (this.fpBar.InvokeRequired)
        //      this.fpBar.Invoke((Delegate) this.mi);
        //    else
        //      this.mi();
        //    this.fs.Close();
        //  }
        //  else
        //  {
        //    this.mi = (MethodInvoker) (() => this.fpBar.ProgressLabel.Text = "Task complete...");
        //    if (this.fpBar.InvokeRequired)
        //      this.fpBar.Invoke((Delegate) this.mi);
        //    else
        //      this.mi();
        //    this.mi = (MethodInvoker) (() => this.fpBar.percentLabel.Text = "(100%)");
        //    if (this.fpBar.InvokeRequired)
        //      this.fpBar.Invoke((Delegate) this.mi);
        //    else
        //      this.mi();
        //    this.slpSuccessful = true;
        //  }
        //  this.fs.Dispose();
        //  GC.Collect();
        //  if (File.Exists(this.slpOpenFileName + ".voobly"))
        //    File.Delete(this.slpOpenFileName + ".voobly");
        //  await Task.Delay(1000);
        //  this.slpImportCompleted = true;
        //  this.fpBar.Close();
        //  Global.cancelProgress = false;
        //}

        //private void BatchSlxCreator_DoWork(object sender, DoWorkEventArgs e)
        //{
        //  this.percentage = 0;
        //  this.multipleFiles = true;
        //  this.frameCount = 0;
        //  foreach (string safeSlpFiles in this.safeSLPFilesList)
        //  {
        //    this.destination = Path.GetDirectoryName(safeSlpFiles) + "\\" + Path.GetFileNameWithoutExtension(safeSlpFiles) + "\\";
        //    this.slpOpenFileName = safeSlpFiles;
        //    this.itemName = Path.GetFileNameWithoutExtension(safeSlpFiles);
        //    this.dataName = string.IsNullOrEmpty(this.form1.dataBox.Text) ? "d" : this.form1.dataBox.Text;
        //    this.slpSuccessful = false;
        //    this.CreateSLX(this.itemName, this.dataName, this.slpOpenFileName, this.destination);
        //    if (File.Exists(safeSlpFiles + ".voobly"))
        //      File.Delete(safeSlpFiles + ".voobly");
        //    if (Global.cancelProgress)
        //    {
        //      e.Cancel = true;
        //      return;
        //    }
        //  }
        //  if (Global.cancelProgress)
        //  {
        //    e.Cancel = true;
        //  }
        //  else
        //  {
        //    while (this.percentage < 100)
        //    {
        //      ++this.percentage;
        //      this.BatchSlxCreator.ReportProgress(this.percentage);
        //    }
        //  }
        //}

        //private void BatchSlxCreator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //  this.mi = (MethodInvoker) (() => this.fpBar.progressBar.Value = e.ProgressPercentage);
        //  if (this.fpBar.InvokeRequired)
        //    this.fpBar.Invoke((Delegate) this.mi);
        //  else
        //    this.mi();
        //  this.mi = (MethodInvoker) (() => this.fpBar.percentLabel.Text = "(" + this.fpBar.progressBar.Value.ToString() + "%)");
        //  if (this.fpBar.InvokeRequired)
        //    this.fpBar.Invoke((Delegate) this.mi);
        //  else
        //    this.mi();
        //}

        //private async void BatchSlxCreator_RunWorkerCompleted(
        //  object sender,
        //  RunWorkerCompletedEventArgs e)
        //{
        //  if (e.Cancelled)
        //  {
        //    this.mi = (MethodInvoker) (() => this.fpBar.progressLabel.Text = "Cancelling process... ");
        //    if (this.fpBar.InvokeRequired)
        //      this.fpBar.Invoke((Delegate) this.mi);
        //    else
        //      this.mi();
        //    this.fs.Close();
        //  }
        //  else
        //  {
        //    this.mi = (MethodInvoker) (() => this.fpBar.ProgressLabel.Text = "Task complete...");
        //    if (this.fpBar.InvokeRequired)
        //      this.fpBar.Invoke((Delegate) this.mi);
        //    else
        //      this.mi();
        //    this.mi = (MethodInvoker) (() => this.fpBar.percentLabel.Text = "(100%)");
        //    if (this.fpBar.InvokeRequired)
        //      this.fpBar.Invoke((Delegate) this.mi);
        //    else
        //      this.mi();
        //    this.slpSuccessful = true;
        //  }
        //  this.fs.Dispose();
        //  GC.Collect();
        //  await Task.Delay(1000);
        //  this.batchSlxConversionCompleted = true;
        //  this.fpBar.Close();
        //  Global.cancelProgress = false;
        //}
        //public Color slpOutline1Color = Global.sxpOutline1Color;
        //public Color slpShadowColor = Global.sxpShadowColor;
        public Bitmap DrawFrame(
 int frameNum,
 bool outlines,
 bool shadows,
 bool mask,
 bool data = false)
        {
            if (this.fs == null)
            {
                this.slpRenderCompleted = true;
                return new Bitmap(1, 1);
            }
            if (this.frameWidth[frameNum] <= 0 || this.frameHeight[frameNum] <= 0)
            {
                this.slpRenderCompleted = true;
                return new Bitmap(1, 1);
            }
            this.is32Bit = (this.framePaletteID[frameNum] & 7) == 7;
            if (this.readPalette)
                this.UpdateAutoPalette(this.framePaletteID[frameNum], this.framePaletteOffset[frameNum]);
            this.fs.Position = this.filePos + (long)this.frameOutlineOffset[frameNum];
            if (this.fs.Position >= this.fs.Length)
            {
                int num = (int)MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
                return new Bitmap(1, 1);
            }
            int[] numArray1 = new int[this.frameHeight[frameNum]];
            int[] numArray2 = new int[this.frameHeight[frameNum]];
            try
            {
                for (int index = 0; index < this.frameHeight[frameNum]; ++index)
                {
                    numArray1[index] = Convert.ToInt32(this.fs.ReadUInt16());
                    numArray2[index] = Convert.ToInt32(this.fs.ReadUInt16());
                    if (numArray1[index] == 32768 && numArray2[index] == 32768)
                    {
                        numArray2[index] = 0;
                    }
                    else
                    {
                        if (numArray1[index] < 0 || numArray1[index] > this.frameWidth[frameNum])
                            numArray1[index] = this.frameWidth[frameNum];
                        if (numArray2[index] < 0)
                            numArray2[index] = 0;
                        if (numArray2[index] > this.frameWidth[frameNum])
                            numArray2[index] = this.frameWidth[frameNum];
                    }
                }
            }
            catch
            {
                int num = (int)MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
                return new Bitmap(1, 1);
            }
            this.fs.Position = this.filePos + (long)this.frameDataOffsets[frameNum];
            if (this.fs.Position >= this.fs.Length)
            {
                int num = (int)MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
                return new Bitmap(1, 1);
            }
            int[] numArray3 = new int[this.frameHeight[frameNum]];
            for (int index = 0; index < this.frameHeight[frameNum]; ++index)
                numArray3[index] = this.fs.ReadInt32();
            Bitmap frame = new Bitmap(this.frameWidth[frameNum], this.frameHeight[frameNum], PixelFormat.Format32bppArgb);
            SolidBrush solidBrush = new SolidBrush(Global.sxpMaskColor);
            if (this.is32Bit && !mask)
                solidBrush = new SolidBrush(Color.FromArgb((int)byte.MaxValue, 0, 0, 0));
            if (data)
                solidBrush = new SolidBrush(Color.FromArgb((int)byte.MaxValue, 0, (int)byte.MaxValue));
            using (Graphics graphics = Graphics.FromImage((Image)frame))
                graphics.FillRectangle((Brush)solidBrush, new Rectangle(0, 0, frame.Width, frame.Height));
            if (!mask)
                frame.MakeTransparent();
            int lineNum = 0;
            foreach (int num1 in numArray3)
            {
                if (num1 < 0)
                {
                    int num2 = (int)MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
                    return frame;
                }
                this.fs.Position = this.filePos + (long)num1;
                if (this.fs.Position >= this.fs.Length)
                {
                    int num2 = (int)MessageBox.Show("Attempted to read past the end of stream. SLP may be corrupted.", "Error");
                    return frame;
                }
                int linePos = 0;
                int num3 = -1;
                if (numArray1[lineNum] > 0 || numArray1[lineNum] <= frame.Width)
                    linePos += numArray1[lineNum];
                else if (numArray1[lineNum] == 32768)
                {
                    linePos = frame.Width;
                    num3 = 15;
                }
                try
                {
                    while (num3 != 15)
                    {
                        if (linePos < frame.Width - numArray2[lineNum])
                        {
                            num3 = (int)this.fs.ReadUInt8();
                            if (num3 != 15)
                            {
                                int num2 = num3 & 15;
                                switch (num2)
                                {
                                    case 0:
                                    case 4:
                                    case 8:
                                    case 12:
                                        int num4 = num3 >> 2;
                                        for (int index = 0; index < num4; ++index)
                                        {
                                            Color color;
                                            if (this.is32Bit)
                                            {
                                                color = Color.FromArgb(this.fs.ReadInt32());
                                            }
                                            else
                                            {
                                                int num5 = 0;
                                                if (this.type == 32)
                                                    num5 = (int)this.fs.ReadUInt8();
                                                color = this.slpPal[(int)this.fs.ReadUInt8()];
                                                if (this.type == 32 && this.decayTime > num5)
                                                    color = Color.FromArgb(0, 0, 0, 0);
                                            }
                                            frame = data ? this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue)) : this.DrawPixels(frame, lineNum, linePos, 1, color);
                                            ++linePos;
                                        }
                                        continue;
                                    case 1:
                                    case 5:
                                    case 9:
                                    case 13:
                                        int num6 = num3 >> 2;
                                        linePos += num6;
                                        continue;
                                    case 2:
                                        int num7 = ((num3 & 240) << 4) + (int)this.fs.ReadUInt8();
                                        for (int index = 0; index < num7; ++index)
                                        {
                                            Color color;
                                            if (this.is32Bit)
                                            {
                                                color = Color.FromArgb(this.fs.ReadInt32());
                                            }
                                            else
                                            {
                                                int num5 = 0;
                                                if (this.type == 32)
                                                    num5 = (int)this.fs.ReadUInt8();
                                                color = this.slpPal[(int)this.fs.ReadUInt8()];
                                                if (this.type == 32 && this.decayTime > num5)
                                                    color = Color.FromArgb(0, 0, 0, 0);
                                            }
                                            frame = data ? this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue)) : this.DrawPixels(frame, lineNum, linePos, 1, color);
                                            ++linePos;
                                        }
                                        continue;
                                    case 3:
                                        int num8 = ((num3 & 240) << 4) + (int)this.fs.ReadUInt8();
                                        linePos += num8;
                                        continue;
                                    case 6:
                                        int num9 = num3 >> 4;
                                        if (num9 == 0)
                                            num9 = (int)this.fs.ReadUInt8();
                                        for (int index1 = 0; index1 < num9; ++index1)
                                        {
                                            int num5 = 0;
                                            if (this.type == 32)
                                                num5 = (int)this.fs.ReadUInt8();
                                            byte num10 = this.fs.ReadUInt8();
                                            int num11 = num10.ToInt();
                                            int index2 = Global.slpPlayerColorStartIndex + num11;
                                            Color color;
                                            if (this.version.Substring(0, 2) == "4." && this.version.Substring(3) == "X")
                                            {
                                                if (num10 > (byte)127)
                                                    num10 = (byte)15;
                                                color = this.slpPlayerColorPal[(int)num10];
                                            }
                                            else
                                                color = !this.is32Bit ? this.slpPal[index2] : ColorTables.graphicsPal[index2];
                                            if (this.type == 32 && this.decayTime > num5)
                                                color = Color.FromArgb(0, 0, 0, 0);
                                            frame = data ? this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int)byte.MaxValue, 0, (int)byte.MaxValue, 0)) : this.DrawPixels(frame, lineNum, linePos, 1, color);
                                            ++linePos;
                                        }
                                        continue;
                                    case 7:
                                        int length1 = num3 >> 4;
                                        if (length1 == 0)
                                            length1 = (int)this.fs.ReadUInt8();
                                        Color color1;
                                        if (this.is32Bit)
                                        {
                                            color1 = Color.FromArgb(this.fs.ReadInt32());
                                        }
                                        else
                                        {
                                            int num5 = 0;
                                            if (this.type == 32)
                                                num5 = (int)this.fs.ReadUInt8();
                                            color1 = this.slpPal[(int)this.fs.ReadUInt8()];
                                            if (this.type == 32 && this.decayTime > num5)
                                                color1 = Color.FromArgb(0, 0, 0, 0);
                                        }
                                        frame = data ? this.DrawPixels(frame, lineNum, linePos, length1, Color.FromArgb((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue)) : this.DrawPixels(frame, lineNum, linePos, length1, color1);
                                        linePos += length1;
                                        continue;
                                    case 10:
                                        int length2 = num3 >> 4;
                                        if (length2 == 0)
                                            length2 = (int)this.fs.ReadUInt8();
                                        int num12 = 0;
                                        if (this.type == 32)
                                            num12 = (int)this.fs.ReadUInt8();
                                        byte num13 = this.fs.ReadUInt8();
                                        int num14 = num13.ToInt();
                                        int index3 = Global.slpPlayerColorStartIndex + num14;
                                        Color color2;
                                        if (this.version.Substring(0, 2) == "4." && this.version.Substring(3) == "X")
                                        {
                                            if (num13 > (byte)127)
                                                num13 = (byte)15;
                                            color2 = this.slpPlayerColorPal[(int)num13];
                                        }
                                        else
                                            color2 = !this.is32Bit ? this.slpPal[index3] : ColorTables.graphicsPal[index3];
                                        if (this.type == 32 && this.decayTime > num12)
                                            color2 = Color.FromArgb(0, 0, 0, 0);
                                        frame = data ? this.DrawPixels(frame, lineNum, linePos, length2, Color.FromArgb((int)byte.MaxValue, 0, (int)byte.MaxValue, 0)) : this.DrawPixels(frame, lineNum, linePos, length2, color2);
                                        linePos += length2;
                                        continue;
                                    case 11:
                                        int length3 = num3 >> 4;
                                        if (length3 == 0)
                                            length3 = (int)this.fs.ReadUInt8();
                                        if (!data)
                                        {
                                            if (shadows)
                                                frame = this.DrawPixels(frame, lineNum, linePos, length3, this.slpShadowColor);
                                        }
                                        else
                                            frame = this.DrawPixels(frame, lineNum, linePos, length3, Color.FromArgb((int)byte.MaxValue, (int)byte.MaxValue, 0, 0));
                                        linePos += length3;
                                        continue;
                                    case 14:
                                        switch (num3)
                                        {
                                            case 14:
                                            case 46:
                                                continue;
                                            case 30:
                                                 
                                                //logBox1.Text = logBox1.Text + Environment.NewLine + "[SLP Error]\t\tObsolete extended command " + num3.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
                                                return frame;
                                            case 62:
                                                 
                                                //logBox2.Text =   Environment.NewLine + "[SLP Error]\t\tObsolete extended command " + num3.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
                                                return frame;
                                            case 78:
                                                if (!data)
                                                {
                                                    if (outlines)
                                                        frame = this.DrawPixels(frame, lineNum, linePos, 1, this.slpOutline1Color);
                                                }
                                                else
                                                    frame = this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int)byte.MaxValue, 0, 0, (int)byte.MaxValue));
                                                ++linePos;
                                                continue;
                                            case 94:
                                                byte num15 = this.fs.ReadUInt8();
                                                if (!data)
                                                {
                                                    if (outlines)
                                                        frame = this.DrawPixels(frame, lineNum, linePos, (int)num15, this.slpOutline1Color);
                                                }
                                                else
                                                    frame = this.DrawPixels(frame, lineNum, linePos, (int)num15, Color.FromArgb((int)byte.MaxValue, 0, 0, (int)byte.MaxValue));
                                                linePos += (int)num15;
                                                continue;
                                            case 110:
                                                if (!data)
                                                {
                                                    if (outlines)
                                                        frame = this.DrawPixels(frame, lineNum, linePos, 1, this.slpOutline2Color);
                                                }
                                                else
                                                    frame = this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0));
                                                ++linePos;
                                                continue;
                                            case 126:
                                                byte num16 = this.fs.ReadUInt8();
                                                if (!data)
                                                {
                                                    if (outlines)
                                                        frame = this.DrawPixels(frame, lineNum, linePos, (int)num16, this.slpOutline2Color);
                                                }
                                                else
                                                    frame = this.DrawPixels(frame, lineNum, linePos, (int)num16, Color.FromArgb((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, 0));
                                                linePos += (int)num16;
                                                continue;
                                            case 158:
                                            case 174:
                                                byte num17 = this.fs.ReadUInt8();
                                                for (int index1 = 0; index1 < (int)num17; ++index1)
                                                {
                                                    Color color3 = Color.FromArgb(this.fs.ReadInt32());
                                                    color3 = Color.FromArgb((int)byte.MaxValue - (int)color3.A, (int)color3.R, (int)color3.G, (int)color3.B);
                                                    if (!data)
                                                        this.DrawPixels(frame, lineNum, linePos, 1, color3);
                                                    else
                                                        frame = this.DrawPixels(frame, lineNum, linePos, 1, Color.FromArgb((int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue));
                                                    ++linePos;
                                                }
                                                continue;
                                            default:
                                                //TextBox logBox3 = this.form1.logBox;
                                                //logBox3.Text = logBox3.Text + Environment.NewLine + "[SLP Error]\t\tUknown extended command " + num3.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
                                                return frame;
                                        }
                                    case 15:
                                        goto label_146;
                                    default:
                                        //TextBox logBox4 = this.form1.logBox;
                                        //logBox4.Text = logBox4.Text + Environment.NewLine + "[SLP Error]\t\tUknown command case " + num2.ToString("X") + " at position: " + (this.fs.Position - 1L).ToString("X");
                                        return frame;
                                }
                            }
                            else
                                break;
                        }
                        else
                            break;
                    }
                }
                catch
                {
                    int num2 = (int)MessageBox.Show("Unable to read data. SLP may be corrupted.", "Error");
                    return frame;
                }
            label_146:
                if (numArray2[lineNum] != 0 && numArray2[lineNum] <= frame.Width && numArray1[lineNum] < frame.Width)
                {
                    int num18 = linePos + numArray2[lineNum];
                }
                ++lineNum;
            }
            this.slpRenderCompleted = true;
            return frame;
        }

        //public Color[] slpPal = Global.sxpPal;
        //public Color[] slpPlayerColorPal = Global.sxpPlayerColorPal;
        public void CreateSLX(
      string itemName,
      string dataName,
      string slpFileName,
      string destination)
    {
      //this.mi = (MethodInvoker) (() => this.fpBar.ProgressLabel.Text = "Importing " + Path.GetFileName(slpFileName) + "...");
      //if (this.fpBar.InvokeRequired)
      //  this.fpBar.Invoke((Delegate) this.mi);
      //else
      //  this.mi();
      if (File.Exists(slpFileName) && Methods.IsFileLocked(slpFileName))
      {
        if (MessageBox.Show(Path.GetFileName(slpFileName) + " is locked or in use.", "File Open Error", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
          this.CreateSLX(itemName, dataName, slpFileName, destination);
        else
          Global.cancelProgress = true;
      }
      else
      {
        this.fs = File.Open(slpFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (this.fs.ReadUInt32() == 3203339063U)
        {
          FileStream fileStream = File.Open(slpFileName + ".voobly", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
          int int32 = Convert.ToInt32(this.fs.Length - 4L);
          byte[] buffer = new byte[int32];
          this.fs.Read(buffer, 0, int32);
          for (int index = 0; index < int32; ++index)
            buffer[index] = (32 * ((int) buffer[index] - 17 ^ 35) | (int) (byte) ((int) buffer[index] - 17 ^ 35) >> 3).ToByte();
          fileStream.Write(buffer, 0, int32);
          fileStream.SetLength((long) int32);
          fileStream.Close();
          this.fs = File.Open(slpFileName + ".voobly", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        this.fs.Position -= 4L;
        this.version = this.fs.ReadASCII(4);
        if (!this.version.Substring(0, 1).IsNumeric() || this.version.Substring(1, 1) != "." || !this.version.Substring(2, 1).IsNumeric())
        {
          int num = (int) MessageBox.Show("Unable to read the SLP file.", "Error");
          this.fs.Close();
          this.slpLoadCompleted = true;
          this.slpCancelLoad = true;
        }
        else
        {
          this.numFrames = (int) this.fs.ReadUInt16();
          this.type = (int) this.fs.ReadUInt16();
          if (!this.multipleFiles)
            this.numFramesTotal = this.numFrames;
                    //ColorTables.SetGlobalPlayerColorPalette(Global.sxpPlayerIndex);
                    //this.slpPlayerColorPal = Global.sxpPlayerColorPal.CopyColors(true);
                    if (this.version == "2.0N" || this.version.Substring(0, 3) == "3.0")
            this.comment = this.fs.ReadASCII(24);
          else if (this.slp4X.Contains(this.version))
          {
            this.numDirections = (int) this.fs.ReadUInt16();
            this.framesPerDirection = (int) this.fs.ReadUInt16();
            this.paletteID = this.fs.ReadInt32();
            this.frameInfoOffset = this.fs.ReadInt32();
            this.layerInfoOffset = this.fs.ReadInt32();
            this.comment = this.fs.ReadASCII(this.frameInfoOffset - 24);
          }
          if (this.slp4X.Contains(this.version) && this.layerInfoOffset > 0 && !this.multipleFiles)
            this.numFramesTotal += this.numFrames;
          if (this.slp4X.Contains(this.version))
            this.fs.Position = (long) this.frameInfoOffset;
          else
            this.fs.Position = 32L;
          this.frameDataOffsets = new int[this.numFrames];
          this.frameOutlineOffset = new int[this.numFrames];
          this.framePaletteOffset = new int[this.numFrames];
          this.framePaletteID = new int[this.numFrames];
          this.frameWidth = new int[this.numFrames];
          this.frameHeight = new int[this.numFrames];
          this.frameAnchorX = new int[this.numFrames];
          this.frameAnchorY = new int[this.numFrames];
          ImageFormat format = ImageFormat.Png;
          bool mask = true;
          List<Bitmap> bitmapList1 = new List<Bitmap>();
          List<Bitmap> bitmapList2 = new List<Bitmap>();
          List<string> stringList = new List<string>();
          if (this.slp4X.Contains(this.version) && this.type == 32)
            this.decayTime = 0;
          for (int frameNum = 0; frameNum < this.numFrames; ++frameNum)
          {
            bool flag = false;
            this.frameDataOffsets[frameNum] = this.fs.ReadInt32();
            this.frameOutlineOffset[frameNum] = this.fs.ReadInt32();
            this.framePaletteOffset[frameNum] = this.fs.ReadInt32();
            this.framePaletteID[frameNum] = this.fs.ReadInt32();
            if ((this.framePaletteID[frameNum] & 7) == 7)
              this.is32Bit = true;
            this.frameWidth[frameNum] = this.fs.ReadInt32();
            this.frameHeight[frameNum] = this.fs.ReadInt32();
            if (this.frameWidth[frameNum] <= 0 || this.frameHeight[frameNum] <= 0)
              flag = true;
            long num1 = (long) this.fs.ReadInt32();
            if (num1 >= 2147483000L)
              num1 -= 2147483000L;
            else if (num1 <= -2147483000L)
              num1 += 2147483000L;
            this.frameAnchorX[frameNum] = Convert.ToInt32(num1);
            long num2 = (long) this.fs.ReadInt32();
            if (num2 >= 2147483000L)
              num2 -= 2147483000L;
            else if (num2 <= -2147483000L)
              num2 += 2147483000L;
            this.frameAnchorY[frameNum] = Convert.ToInt32(num2);
            this.lastPos = this.fs.Position;
            if (this.is32Bit || this.type == 8)
            {
              Global.currentFormat = ".png";
              Global.slxVersion = "SLX 1.0";
              format = ImageFormat.Png;
              mask = false;
            }
            else if (Global.imageFormat == ".bmp")
            {
              Global.currentFormat = ".bmp";
              Global.slxVersion = "SLX 0.5";
              format = ImageFormat.Bmp;
            }
            else
            {
              Global.currentFormat = ".png";
              Global.slxVersion = "SLX 1.0";
              format = ImageFormat.Png;
            }
            string frameName = Methods.AddItemPadding(Path.GetFileNameWithoutExtension(slpFileName) + "_", this.numFrames.ToString().Length + 1, frameNum + 1);
            stringList.Add(frameName);
            if (this.frameDataOffsets[frameNum] > 0 && !flag)
            {
              if (!this.is32Bit)
              {
                if (Global.sxpForcePalette)
                    this.slpPal = Global.sxpPal.CopyColors(true);
                else
                    this.UpdateAutoPalette(this.framePaletteID[frameNum], this.framePaletteOffset[frameNum]);
              }
                    if (this.slpPal == null)
                        this.slpPal = ColorTables.graphicsPal;
                    Bitmap bitmap1 = this.DrawFrame(frameNum, false, true, mask, false);
                    Bitmap bitmap2 = this.DrawFrame(frameNum, true, true, true, true);
                    bitmapList1.Add(new Bitmap((Image)bitmap1));
                    bitmapList2.Add(new Bitmap((Image)bitmap2));
                    bitmap1.Dispose();
                    bitmap2.Dispose();
                        }
            this.fs.Position = this.lastPos;
            ++this.frameCount;
            //this.FileProgress(frameName, this.frameCount);
            if (Global.cancelProgress)
              return;
          }
          if (this.version.Substring(0, 2) == "4." && this.version.Substring(3) == "X" && this.layerInfoOffset > 0)
          {
            this.layerDataOffsets = new int[this.numFrames];
            this.layerOutlineOffset = new int[this.numFrames];
            this.layerProperties = new int[this.numFrames];
            this.layerWidth = new int[this.numFrames];
            this.layerHeight = new int[this.numFrames];
            this.layerAnchorX = new int[this.numFrames];
            this.layerAnchorY = new int[this.numFrames];
            this.fs.Position = (long) this.layerInfoOffset;
            if (!Directory.Exists(destination))
              Directory.CreateDirectory(destination);
            for (int frameNum = 0; frameNum < this.numFrames; ++frameNum)
            {
              bool flag = false;
              this.layerDataOffsets[frameNum] = this.fs.ReadInt32();
              this.layerOutlineOffset[frameNum] = this.fs.ReadInt32();
              this.fs.Position += 7L;
              this.layerProperties[frameNum] = (int) this.fs.ReadUInt8();
              this.layerWidth[frameNum] = this.fs.ReadInt32();
              this.layerHeight[frameNum] = this.fs.ReadInt32();
              if (this.layerWidth[frameNum] <= 0 || this.layerHeight[frameNum] <= 0)
                flag = true;
              long num1 = (long) this.fs.ReadInt32();
              if (num1 >= 2147483000L)
                num1 -= 2147483000L;
              else if (num1 <= -2147483000L)
                num1 += 2147483000L;
              this.layerAnchorX[frameNum] = Convert.ToInt32(num1);
              long num2 = (long) this.fs.ReadInt32();
              if (num2 >= 2147483000L)
                num2 -= 2147483000L;
              else if (num2 <= -2147483000L)
                num2 += 2147483000L;
              this.layerAnchorY[frameNum] = Convert.ToInt32(num2);
              this.lastPos = this.fs.Position;
              Color sxpMaskColor = Global.sxpMaskColor;
              if (this.layerProperties[frameNum] == 2)
              {
                if (!Directory.Exists(destination))
                  Directory.CreateDirectory(destination);
                if (this.layerDataOffsets[frameNum] > 0 && !flag)
                {
                  Bitmap overlay1 = this.DrawShadowFrame(frameNum, false);
                  Bitmap overlay2 = this.DrawShadowFrame(frameNum, true);
                  string frameName = Methods.AddItemPadding(Path.GetFileNameWithoutExtension(slpFileName) + "_", this.numFrames.ToString().Length + 1, frameNum + 1);
                  bitmapList1[frameNum] = new Bitmap((Image) bitmapList1[frameNum].MergeGraphics(this.frameAnchorX[frameNum], this.frameAnchorY[frameNum], sxpMaskColor, overlay1, this.layerAnchorX[frameNum], this.layerAnchorY[frameNum], false));
                  bitmapList2[frameNum] = new Bitmap((Image) bitmapList2[frameNum].MergeGraphics(this.frameAnchorX[frameNum], this.frameAnchorY[frameNum], Color.FromArgb((int) byte.MaxValue, 0, (int) byte.MaxValue), overlay2, this.layerAnchorX[frameNum], this.layerAnchorY[frameNum], false));
                  int num3 = (this.frameAnchorX[frameNum] - this.layerAnchorX[frameNum]) * -1;
                  if (num3 > 0)
                    this.frameAnchorX[frameNum] += num3;
                  int num4 = (this.frameAnchorY[frameNum] - this.layerAnchorY[frameNum]) * -1;
                  if (num4 > 0)
                    this.frameAnchorY[frameNum] += num4;
                  overlay1.Dispose();
                  overlay2.Dispose();
                  this.fpVerb = "Drawing shadows for ";
                  //this.FileProgress(frameName, this.frameCount + 1);
                }
              }
              if (this.layerProperties[frameNum] == 8)
              {
                bitmapList1[frameNum] = new Bitmap((Image) this.DrawFrameAlphas(bitmapList1[frameNum], frameNum, false));
                string frameName = Methods.AddItemPadding(Path.GetFileNameWithoutExtension(slpFileName) + "_", this.numFrames.ToString().Length + 1, frameNum + 1);
                if (!Directory.Exists(destination))
                  Directory.CreateDirectory(destination);
                this.fpVerb = "Drawing alphas for ";
                //this.FileProgress(frameName, this.frameCount + 1);
              }
              this.fs.Position = this.lastPos;
              ++this.frameCount;
              if (Global.cancelProgress)
                return;
            }
          }
          if (!Directory.Exists(destination))
            Directory.CreateDirectory(destination);
          for (int index = 0; index < bitmapList1.Count; ++index)
          {
            bitmapList1[index].Save(destination + stringList[index] + Global.currentFormat, format);
                        var dest = destination + stringList[index] + DateTime.Now.ToString("yyyyMMddsss")+ Global.currentFormat;
            bitmapList2[index].Save(dest, format);
          }
          bitmapList1.Clear();
          bitmapList2.Clear();
          stringList.Clear();
          GC.Collect();
          GC.WaitForPendingFinalizers();
          int paddingSize = this.numFrames.ToString().Length + 1;
          this.slxLines = new List<string>();
          this.slxLines.Add(Global.slxVersion);
          this.slxLines.Add(this.numFrames.ToString());
          for (int index = 0; index < this.numFrames; ++index)
          {
            this.slxLines.Add(this.frameAnchorX[index].ToString() + "," + this.frameAnchorY[index].ToString());
            this.slxLines.Add(Methods.AddItemPadding(itemName + "_", paddingSize, index + 1) + dataName + Global.currentFormat);
            this.slxLines.Add(Methods.AddItemPadding(itemName + "_", paddingSize, index + 1) + Global.currentFormat);
          }
          this.slxFileName = destination + itemName + ".slx";
          if (!this.createSLXFile)
            return;
          if (!Directory.Exists(destination))
            Directory.CreateDirectory(destination);
          File.WriteAllLines(this.slxFileName, this.slxLines.ToArray());
        }
      }
    }

    //private void FileProgress(string frameName, int frameCount)
    //{
    //  int num = frameCount * 200 / (this.numFramesTotal * 2);
    //  if (!this.multipleFiles)
    //  {
    //    this.mi = (MethodInvoker) (() => this.fpBar.progressLabel.Text = this.fpVerb + frameName);
    //    if (this.fpBar.InvokeRequired)
    //      this.fpBar.Invoke((Delegate) this.mi);
    //    else
    //      this.mi();
    //    while (this.percentage < num - 1)
    //    {
    //      ++this.percentage;
    //      this.SlpImporter.ReportProgress(this.percentage);
    //    }
    //  }
    //  else
    //  {
    //    if (!this.multipleFiles)
    //      return;
    //    this.mi = (MethodInvoker) (() => this.fpBar.progressLabel.Text = "Extracting " + frameName);
    //    if (this.fpBar.InvokeRequired)
    //      this.fpBar.Invoke((Delegate) this.mi);
    //    else
    //      this.mi();
    //    while (this.percentage < num - 1)
    //    {
    //      ++this.percentage;
    //      this.BatchSlxCreator.ReportProgress(this.percentage);
    //    }
    //  }
    //}
  }
}
