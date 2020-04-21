using Aok_Patch.patcher_.Properties;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using KGySoft.Drawing;
using System.Text.RegularExpressions;
using System.Data;
using NAudio;

namespace Aok_Patch.patcher_
{

    public partial class AokPatch : Form
    {

        public AokPatch()
        {
            InitializeComponent();
            
        }
        private class FatalError : Exception
        {
            public FatalError(string msg)
              : base(msg)
            {
            }
        }
        private static readonly bool IsVistaOrHigher = Environment.OSVersion.Version.Major >= 6;
        //private IContainer components = (IContainer)null;
        private static string _orgDrsPath;
        private static string _orgX1DrsPath;
        private static string _orgExePath;
        private static string _gameDirectory;
        private const bool skipExistingFiles = false;
        private static string[] patchFiles;
        private static Patch[] allPatches_;
        private string gameExe;
        private string gamePath;
        private string gameData;
        private TabControlHelper tabhelper;


        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int num = (int)MessageBox.Show("This projet was made By Katsuie and FreePalestine.");
        }

        private string getPatchVersion()
        {
            string str = string.Empty;
            if (this.radioButton__20.Checked)
                str = "AoK_20.patch";
            if (this.radioButton_20a.Checked)
                str = "AoK_20a.patch";
            if (this.radioButton_20b.Checked)
                str = "AoK_20b.patch";
            if (this.radioButton_10.Checked)
                str = "AoC_10.patch";
            if (this.radioButton_10c.Checked)
                str = "AoC_10ce.patch";
            if (this.radioButton_10e.Checked)
                str = "AoC_10ce.patch";
            if (string.IsNullOrEmpty(str))
                throw new Exception("No version Game selected !!!!");
            return str;
        }

        public string gethVersionFromUser()
        {
            string str = string.Empty;
            if (this.radioButton__20.Checked)
                str = "2.0";
            if (this.radioButton_20a.Checked)
                str = "2.0a";
            if (this.radioButton_20b.Checked)
                str = "2.0b";
            if (this.radioButton_10.Checked)
                str = "1.0";
            if (this.radioButton_10c.Checked)
                str = "1.0c";
            if (this.radioButton_10e.Checked)
                str = "1.0e";
            if (string.IsNullOrEmpty(str))
                throw new Exception("No version Game selected !!!!");
            return str;
        }

        private string getFileName(string version)
        {
            int num1 = 0;
            int num2 = 0;
            foreach (Screen allScreen in Screen.AllScreens)
            {
                Rectangle bounds = allScreen.Bounds;
                num1 = bounds.Width;
                bounds = allScreen.Bounds;
                num2 = bounds.Height;
            }
            string str = string.Empty;

                str = Path.Combine(_gameDirectory, string.Format("{3}_{2}_{0}x{1}.exe", (object)num1, (object)num2, (object)version, gameExe));
            return str;
        }

        private string GetGameOriginalGameName()
        {
            var split = gameExe.Split('\\');
            string exe = split.LastOrDefault();
            return exe;
        }

        private void btnPatch_Click(object sender, EventArgs e)
        {
            try
            {
  
                //string currentDirector = Directory.GetCurrentDirectory();
                //radioButton__20.Checked = true;
                //this.gameExe = @"C:\Users\Beleive\Documents\Age of Empires II\empires2.exe";
                //this.gamePath = new FileInfo(this.gameExe).Directory.FullName;
                //this.gameData = this.gamePath + "\\data";
                //FileVersionInfo.GetVersionInfo(Path.Combine(Environment.SystemDirectory, this.gameExe));
                //FileVersionInfo.GetVersionInfo(this.gameExe);
                // Set cursor as hourglass
                if (string.IsNullOrEmpty(this.gameExe))
                {
                    MessageBox.Show("Browser Game exe !!");
                    return;
                }
                Cursor.Current = Cursors.WaitCursor;
                labelWideScreen.Text = "Please wait this can take some time ...";

                if (!string.IsNullOrEmpty(this.gameExe) && (this.radioButton__20.Checked || this.radioButton_20a.Checked || (this.radioButton_20b.Checked || this.radioButton_10.Checked) || this.radioButton_10c.Checked || this.radioButton_10e.Checked))
                {
                    int num1 = 0;
                    int num2 = 0;
                    foreach (Screen allScreen in Screen.AllScreens)
                    {
                        Rectangle bounds = allScreen.Bounds;
                        num1 = bounds.Width;
                        bounds = allScreen.Bounds;
                        num2 = bounds.Height;
                    }
                    string version = this.gethVersionFromUser();
                    string originalGameName = this.GetGameOriginalGameName();
                    _gameDirectory = FindGameDirectory();
                    string fileName = this.getFileName(version);
                    string str1 = num1.ToString() + (object)num2 + ".drs";
                    if (Directory.Exists("Data"))
                        Directory.Delete(_gameDirectory + "\\Data", true);
                    foreach (string file in Directory.GetFiles(".", "*.exe", SearchOption.AllDirectories))
                    {
                        if (!file.Contains("Aok Patch"))
                            File.Delete(file);
                    }
                    if (!Directory.Exists("Data"))
                        Directory.CreateDirectory("Data");
                    if (!File.Exists("Data\\interfac.drs"))
                        File.Copy(this.gameData.Replace("Age2_x1", "") + "\\interfac.drs", "Data\\interfac.drs");
                    if (!File.Exists("Data\\" + str1))
                        File.Delete(this.gameData.Replace("Age2_x1", "") + "\\" + str1);

                    if (!File.Exists(originalGameName))
                        File.Copy(this.gameExe, originalGameName.Replace("\\", ""));
                    try
                    {
                        int x = 0;
                        int y = 0;
                        if (cb_SetManuelReso.Checked && (comboBox800.SelectedItem.ToString() == "Auto") &&
                            (comboBox1024.SelectedItem.ToString() == "Auto") && (comboBox1280.SelectedItem.ToString() != "Auto"))
                        {
                            var resSplit = comboBox1280.SelectedItem.ToString().Split('x');
                            x = int.Parse(resSplit[0]);
                            y = int.Parse(resSplit[1]);
                            Go(new string[] { "800", "600", "1024","768",x.ToString(),y.ToString() }, this.getPatchVersion());
                        }
                        else if (cb_SetManuelReso.Checked && (comboBox800.SelectedItem.ToString() == "Auto") &&
                            (comboBox1024.SelectedItem.ToString() != "Auto") && (comboBox1280.SelectedItem.ToString() != "Auto")
                            )
                        {
                            var resSplit1 = comboBox1024.SelectedItem.ToString().Split('x');
                            var x1 = resSplit1[0];
                            var y1 = resSplit1[1];
                            var resSplit2 = comboBox1280.SelectedItem.ToString().Split('x');
                            var x2 = resSplit2[0];
                            var y2 = resSplit2[1];
                            Go(new string[] {"800","600", x1,y1,x2,y2}, this.getPatchVersion());
                        }
                        else if (cb_SetManuelReso.Checked && (comboBox800.SelectedItem.ToString() != "Auto") &&
                            (comboBox1024.SelectedItem.ToString() != "Auto")&& (comboBox1280.SelectedItem.ToString() != "Auto")
                        )
                        {
                            var resSplit1 = comboBox800.SelectedItem.ToString().Split('x');
                            var x1 = resSplit1[0];
                            var y1 = resSplit1[1];
                            var resSplit2 = comboBox1024.SelectedItem.ToString().Split('x');
                            var x2 = resSplit2[0];
                            var y2 = resSplit2[1];
                            var resSplit3 = comboBox1280.SelectedItem.ToString().Split('x');
                            var x3 = resSplit3[0];
                            var y3 = resSplit3[1];
                            Go(new string[] { x1, y1, x2, y2 ,x3,y3}, this.getPatchVersion());
                        }
                        else
                        {
                            Go(new string[0] , this.getPatchVersion());
                        }
                    }
                    catch (Exception ex)
                    {
                        UserFeedback.Error(ex);
                    }
                    string[] array = ((IEnumerable<string>)Directory.GetFiles(_gameDirectory, "*.exe", SearchOption.AllDirectories)).Where(x => new FileInfo(x).Length > 2000000L).ToArray();

                    File.Delete(this.gameExe);
                    if (array != null && ((IEnumerable<string>)array).Count<string>() > 0)
                    {
                        string fileAoe = string.Empty;
                        if (version == "2.0" || version == "2.0a" || version == "2.0b")
                            fileAoe = array.Where(x=>Path.GetFileName(x).ToLower()== "Empires2.exe".ToLower()).First();
                        if (version == "1.0" || version == "1.0c" || version == "1.0e")
                            fileAoe = array.Where(x => Path.GetFileName(x).ToLower() == "age2_x1.exe".ToLower()).First();
                        File.Copy(fileAoe, this.gamePath +@"\"+ originalGameName,true);
                    }
                    string str2 = ((IEnumerable<string>)Directory.GetFiles(@"Data", "*.drs", SearchOption.AllDirectories)).Where<string>((Func<string, bool>)(x => !x.Contains("interfac.drs"))).FirstOrDefault<string>().Replace("Data\\", "");


                    if (File.Exists(this.gameData + "\\" + str2))
                        File.Delete(this.gameData + "\\" + str2);

                    if ((this.radioButton__20.Checked || this.radioButton_20a.Checked || this.radioButton_20b.Checked) && (str2 != null ))
                        File.Copy("Data\\" + str2, this.gameData + "\\" + str2,true);
                    if ((this.radioButton_10.Checked || this.radioButton_10c.Checked || this.radioButton_10e.Checked) && (str2 != null && !File.Exists(this.gameData + "\\" + str2)))
                        File.Copy("Data\\" + str2, this.gameData.Replace("Age2_x1", "") + "\\" + str2,true);
                    this.UpdateLanguageDll(this.gamePath);
                    try
                    {
                        UserFeedback.Close();
                    }
                    catch
                    {
                    }
                    //int num4 = (int)MessageBox.Show("Done.");

                }
                else
                {
                    int num = (int)MessageBox.Show("Choose version! ");
                    return;
                }
                ResizeDrsInt_Click(null, null);
                // Set cursor as default arrow
                Cursor.Current = Cursors.Default;
                labelWideScreen.Text = "Done";
                MessageBox.Show("Done.");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Becare you must browser the game or chose the good version of Aoe");
                if (File.Exists(gameExe))
                {
                    File.Delete(gameExe);
                }
                    var split = gameExe.Split('\\');
                    var exe = split.LastOrDefault();
                    File.Copy(exe, gameExe);
            }
        }

        static void ExecuteCommand(string command)
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            process.WaitForExit();

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            exitCode = process.ExitCode;

            Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Executes a shell command synchronously.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="command">string command</param></span>
        /// <span class="code-SummaryComment"><returns>string, as output of the command.</returns></span>
        public void ExecuteDrsbuild(object command)
        {
            //try
            //{
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.

            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo("drsbuild", "" + command);

            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            // Get the output into a string
            string result = proc.StandardOutput.ReadToEnd();
            // Display the command output.
            Console.WriteLine(result);
            //}
            //catch (Exception objException)
            //{
            //    // Log the exception
            //}
        }
        /// <span class="code-SummaryComment"><summary></span>
        /// Execute the command Asynchronously.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="command">string command.</param></span>
        public void ExecuteDrsbuildAsync(string command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                Thread objThread = new Thread(new ParameterizedThreadStart(ExecuteDrsbuild));
                //Make the thread as background thread.
                objThread.IsBackground = true;
                //Set the Priority of the thread.
                objThread.Priority = ThreadPriority.AboveNormal;
                //Start the thread.
                objThread.Start(command);
            }
            catch (ThreadStartException objException)
            {
                // Log the exception
            }
            catch (ThreadAbortException objException)
            {
                // Log the exception
            }
            catch (Exception objException)
            {
                // Log the exception
            }
        }
        /// <span class="code-SummaryComment"><summary></span>
        /// Executes a shell command synchronously.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="command">string command</param></span>
        /// <span class="code-SummaryComment"><returns>string, as output of the command.</returns></span>

        private static string FindPatchFile()
        {
            return FindFile("Patch data", "AoE2Wide*.patch", new long?(), (string)null);
        }

        private string[] FindPatchFiles()
        {
            string fileName = string.Empty;
            if (this.radioButton__20.Checked)
                fileName = "AoK_Wide(2.0).patch";
            if (this.radioButton_20a.Checked)
                fileName = "AoK_Wide(2.0a).patch";
            if (this.radioButton_20b.Checked)
                fileName = "AoK_Wide(2.0b).patch";
            if (this.radioButton_10.Checked)
                fileName = "AoE2Wide(1.0).patch";
            if (this.radioButton_10c.Checked)
                fileName = "AoE2Wide(1.0c).patch";
            if (this.radioButton_10e.Checked)
                fileName = "AoE2Wide(1.0e).patch";
            return FindFiles("Patch data", fileName, new long?(), (string)null);
        }

        private static string FindRootPatchFile()
        {
            return FindFile("Root Patch data", "AoE2Wide.RootPatch", new long?(), (string)null);
        }

        private static Patch FindPatchForExe(string exeFilename, string patchVersion)
        {
            return FindPatchForExe((int)new FileInfo(exeFilename).Length, GetChecksum(exeFilename), exeFilename, patchVersion);
        }

        private static Patch FindPatchForExe(
          int exeFileSize,
          string exeMd5,
          string exeFilenameForFeedback,
          string patchVersion)
        {
            patchFiles = new List<string>()
      {
        patchVersion
      }.ToArray();
            //if (allPatches_ == null)
                allPatches_ = ((IEnumerable<string>)patchFiles).Select<string, Patch>((Func<string, Patch>)(patchFile => Patcher.TryReadPatch(patchFile, true))).ToArray<Patch>();
            Patch[] allPatches = allPatches_;
            if (allPatches.Length == 0)
                return (Patch)null;
            if (allPatches.Length == 1)
                return allPatches[0];
            UserFeedback.Warning("Multiple matching patches found for executable '{0}', using first:", (object)exeFilenameForFeedback);
            foreach (Patch patch in allPatches)
                UserFeedback.Trace("* {0}", (object)patch.PatchFilepath);
            return allPatches[0];
        }

        private static string FindExeFile(int fileSize, string md5)
        {
            return FindFile("unpatched exe", "*.exe", new long?((long)fileSize), md5);
        }

        private static string FindProcessExe()
        {
            try
            {
                return  FindFile("process suspender/resumer", "process.exe", new long?(), (string)null);
            }
            catch (FatalError ex)
            {
                return (string)null;
            }
        }

        private static string[] FindFiles(
          string whatFile,
          string fileName,
          long? expectedSize,
          string expectedHash)
        {
            UserFeedback.Trace(string.Format("Locating {0} file '{1}'", (object)whatFile, (object)fileName));
            string[] strArray = Directory.GetFiles(_gameDirectory, fileName, SearchOption.AllDirectories);
            if (expectedSize.HasValue)
                strArray = ((IEnumerable<string>)strArray).Where<string>((Func<string, bool>)(fn =>IsFileSizeCorrect(fn, expectedSize.Value))).ToArray<string>();
            if (expectedHash != null)
                strArray = ((IEnumerable<string>)strArray).Where<string>((Func<string, bool>)(fn =>IsFileHashCorrect(fn, expectedHash))).ToArray<string>();
            if (strArray.Length == 0)
                throw new FatalError(string.Format("No correct {0} found in current directory or subdirectories", (object)whatFile));
            return strArray;
        }

        private static string FindFile(
          string whatFile,
          string fileName,
          long? expectedSize,
          string expectedHash)
        {
            string[] files =FindFiles(whatFile, fileName, expectedSize, expectedHash);
            if (files.Length > 1)
            {
                UserFeedback.Warning("Multiple correct {0} instances found in current directory and subdirectories:", (object)whatFile);
                foreach (string msg in files)
                    UserFeedback.Trace(msg);
            }
            UserFeedback.Info("Using '{0}'", (object)files[0]);
            return files[0];
        }

        private static bool IsFileSizeCorrect(string fn, long expectedSize)
        {
            if (new FileInfo(fn).Length == expectedSize)
                return true;
            UserFeedback.Trace("'{0}' is not the expected size", (object)fn);
            return false;
        }

        private static string GetChecksum(string file)
        {
            using (FileStream fileStream = File.OpenRead(file))
            {
                using (MD5 md5 = MD5.Create())
                    return BitConverter.ToString(md5.ComputeHash((Stream)fileStream));
            }
        }

        private static string GetChecksum(byte[] data)
        {
            using (MD5 md5 = MD5.Create())
                return BitConverter.ToString(md5.ComputeHash(data));
        }

        private static bool IsFileHashCorrect(string fn, string expectedHash)
        {
            string checksum = GetChecksum(fn);
            if (checksum.Equals(expectedHash, StringComparison.InvariantCultureIgnoreCase))
                return true;
            UserFeedback.Trace(string.Format("'{0}' doesn't meet the expected hashcode '{1}' instead of '{2}'", (object)fn, (object)checksum, (object)expectedHash));
            return false;
        }

        private static void PatchAllDrs()
        {
            HashSet<int> intSet = new HashSet<int>();
            foreach (Screen allScreen in Screen.AllScreens)
            {
                try
                {
                    int width = allScreen.Bounds.Width;
                    int height = allScreen.Bounds.Height;
                    int num = width + height * 65536;
                    if (intSet.Add(num))
                        PatchAllDrs(width, height);
                }
                catch (Exception ex)
                {
                    UserFeedback.Error(ex);
                }
            }
        }

        private static void PatchAllDrs(int newWidth, int newHeight)
        {
            int oldWidth;
            int oldHeight;
            GetOldWidthHeight(newWidth, newHeight, out oldWidth, out oldHeight);
            foreach (string file in Directory.GetFiles("data\\", "*.drs"))
            {
                string str = string.Format("data{0}x{1}", (object)newWidth, (object)newHeight);
                if (!Directory.Exists(str))
                    Directory.CreateDirectory(str);
                string newDrsName = Path.Combine(str, Path.GetFileName(file));
                PatchADrs(file, newDrsName, oldWidth, oldHeight, newWidth, newHeight);
            }
        }

        private static void PatchADrs(
          string _orgDrsPath,
          string newDrsName,
          int oldWidth,
          int oldHeight,
          int newWidth,
          int newHeight)
        {
            UserFeedback.Trace("Opening original {0}", (object)Path.GetFileName(_orgDrsPath));
            using (FileStream fileStream1 = new FileStream(_orgDrsPath, FileMode.Open, FileSystemRights.ReadData, FileShare.Read, 1048576, FileOptions.SequentialScan))
            {
                UserFeedback.Trace("Creating patched drs file '{0}'", (object)newDrsName);
                using (FileStream fileStream2 = new FileStream(newDrsName, FileMode.Create, FileSystemRights.Write, FileShare.None, 1048576, FileOptions.SequentialScan))
                {
                    UserFeedback.Trace("Patching DRS '{0}'", (object)Path.GetFileName(_orgDrsPath));
                    DrsPatcher.Patch((Stream)fileStream1, (Stream)fileStream2, oldWidth, oldHeight, newWidth, newHeight);
                }
            }
        }

        public static void Go(string[] args, string patchVersion)
        {
            _gameDirectory = FindGameDirectory();

            int result1 = 0;
            int result2 = 0;
            if (args.Length <= 2 || args.Length == 0)
            {
                if (args.Length >= 2)
                {
                    result1 = int.Parse(args[0]);
                    result2 = int.Parse(args[1]);
                }
                _orgDrsPath = Path.Combine(Path.Combine(_gameDirectory, "Data"), "interfac.drs");
                if (!File.Exists(_orgDrsPath))
                    throw new FatalError(string.Format("Cannot find drs file '{0}' in the game folder", (object)_orgDrsPath));
                UserFeedback.Info("Trying to find a patch file for all executables > 2MiB in size");
                using (IEnumerator<string> enumerator = ((IEnumerable<string>)FindFiles("executables", "*.exe", new long?(), (string)null)).Where<string>((Func<string, bool>)(exe => new FileInfo(exe).Length > 2000000L)).GetEnumerator())
                {
                    string current =string.Empty;
                    while (enumerator.MoveNext())
                    {
                        //current exe empire or age2_x1
                        current = enumerator.Current;
                        if (current.ToLower().Contains("empires2.exe") || current.ToLower().Contains("age2_x1.exe"))
                        { 
                            FindResAndPatchExecutable(result1, result2, current, patchVersion);
                        }
                    }

                }
            }
            else
            {
                string current = string.Empty;
                using (IEnumerator<string> enumerator = ((IEnumerable<string>)FindFiles("executables", "*.exe", new long?(), (string)null)).Where<string>((Func<string, bool>)(exe => new FileInfo(exe).Length > 2000000L)).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        //current exe empire or age2_x1
                        current = enumerator.Current;
                        if (current.ToLower().Contains("empires2.exe") || current.ToLower().Contains("age2_x1.exe"))
                        {
                            FindResAndPatchExecutable(result1, result2, current, patchVersion);
                        }
                    }
                }
                var length = args.Length / 2;
                int ix = 0;
                int iy = 1;

                //Horizontal 800 => 800
                //Horizontal 1024 => 801
                //Horizontal 1280 => 802
                //Horizontal 1600 => 803
                //Vertical 600 => 600
                //Vertical 768 => 601
                //Vertical 1024 => 602
                //Vertical 1200 => 603
                int[] arrH = new int[] { 800, 1024, 1280 };
                int[] arrW = new int[] { 600, 768, 1024 };
                Dictionary<int, int> dictHeigth = new Dictionary<int, int>() ;
                Dictionary<int, int> dictWidth = new Dictionary<int, int>();
                for (int i = 0; i < length; i++)
                {
                    dictHeigth.Add(arrH[i], int.Parse(args[ix]));
                    dictWidth.Add(arrW[i], int.Parse(args[iy]));
                    ix += 2;
                    iy += 2;
                }
                ix = 0;
                iy = 1;
                //for (int i = 0; i < length; i++)
                //{

                    result1 = int.Parse(args[ix]);
                    result2 = int.Parse(args[iy]);
                    ix += 2;
                    iy += 2;
                    _orgDrsPath = Path.Combine(Path.Combine(_gameDirectory, "Data"), "interfac.drs");
                    if (!File.Exists(_orgDrsPath))
                        throw new FatalError(string.Format("Cannot find drs file '{0}' in the game folder", (object)_orgDrsPath));
                    UserFeedback.Info("Trying to find a patch file for all executables > 2MiB in size");

                    FindResAndPatchExecutable(result1, result2, current, patchVersion, dictHeigth, dictWidth);
                //}

            }
        }

        private static void FindResAndPatchExecutable(
          int newWidth,
          int newHeight,
          string exe,
          string patchVersion,
            Dictionary<int, int> dictHeigth = null,
            Dictionary<int, int> dictWidth = null
            )
        {
            try
            {
                UserFeedback.Trace("");
                _orgExePath = exe;
                Patch patchForExe = FindPatchForExe(_orgExePath, patchVersion);
                if (patchForExe == null)
                {
                    UserFeedback.Trace("No patches found for executable '{0}'", (object)_orgExePath);
                }
                else
                {
                    UserFeedback.Info(string.Format("Patching Exe '{0}' (version {1}) with patch file '{2}'", (object)_orgExePath, (object)patchForExe.Version, (object)patchForExe.PatchFilepath));
                    if (newWidth == 0 && newHeight == 0)
                    {
                        UserFeedback.Info("Auto patching for all current screen sizes. Note that the game will always use the primary screen!");
                        HashSet<int> intSet = new HashSet<int>();
                        foreach (Screen allScreen in Screen.AllScreens)
                        {
                            try
                            {
                                Rectangle bounds = allScreen.Bounds;
                                newWidth = bounds.Width;
                                bounds = allScreen.Bounds;
                                newHeight = bounds.Height;
                                int num = newWidth + newHeight * 65536;
                                if (intSet.Add(num))
                                    PatchExecutable(newWidth, newHeight, patchForExe);
                            }
                            catch (Exception ex)
                            {
                                UserFeedback.Error(ex);
                            }
                        }
                    }
                    else
                    {
                        HashSet<int> intSet = new HashSet<int>();
                        int num = newWidth + newHeight * 65536;
                        if (intSet.Add(num))
                            PatchExecutable(newWidth, newHeight, patchForExe, dictHeigth, dictWidth);
                    }
                }
            }
            catch (Exception ex)
            {
                UserFeedback.Error(ex);
            }
        }

        private static void AddAsmToRootPatchFile()
        {
            UserFeedback.Info("Locating listing file");
            string file = FindFile("listing file", "age2_x1*.lst", new long?(), (string)null);
            UserFeedback.Info("Locating RootPatch file");
            string rootPatchFile = FindRootPatchFile();
            UserFeedback.Trace("Reading the root patch file");
            Patch patch = Patcher.ReadPatch(rootPatchFile, false);
            UserFeedback.Trace("Reading the listing file");
            IDictionary<int, string> asmMap = Patcher.ReadAsmMap(file);
            UserFeedback.Trace("Adding Asm to the patch data");
            Patcher.AddAsm(patch, asmMap);
            string filePath = rootPatchFile + "2";
            UserFeedback.Trace("Writing the patch file '{0}'", (object)filePath);
            Patcher.WritePatch(patch, filePath);
        }

        private static void ConvertPatchFile(string otherExe)
        {
            UserFeedback.Info("Converting RootPatch file to a patch file for '{0}'", (object)otherExe);
            string rootPatchFile = FindRootPatchFile();
            UserFeedback.Trace("Reading the root patch file");
            Patch patch1 = Patcher.ReadPatch(rootPatchFile, true);
            UserFeedback.Trace("Locating the root executable file");
            string exeFile = FindExeFile(patch1.FileSize, patch1.Md5);
            UserFeedback.Trace("Reading the target executable");
            byte[] numArray1 = File.ReadAllBytes(otherExe);
            UserFeedback.Trace("Reading root executable");
            byte[] numArray2 = File.ReadAllBytes(exeFile);
            string checksum = GetChecksum(numArray1);
            UserFeedback.Trace("Detecting version from filename and/or md5");
            string version = GetVersion(otherExe, checksum);
            UserFeedback.Trace("Version: '{0}'", (object)version);
            Patch patch2;
            if (GetChecksum(numArray2).Equals(checksum))
            {
                UserFeedback.Trace("Executable is equal; leaving patch file as-is");
                patch2 = new Patch()
                {
                    FileSize = patch1.FileSize,
                    InterfaceDrsPosition = patch1.InterfaceDrsPosition,
                    Items = patch1.Items,
                    Md5 = patch1.Md5
                };
            }
            else
            {
                UserFeedback.Trace("Locating comparable locations, this may take a while");
                patch2 = Patcher.ConvertPatch(numArray2, numArray1, patch1);
                patch2.FileSize = numArray1.Length;
                patch2.Md5 = checksum;
            }
            patch2.Version = version;
            string path2 = "AoE2Wide_" + version + ".patch";
            string filePath = Path.Combine(Path.GetDirectoryName(otherExe), path2);
            UserFeedback.Trace("Writing the patch file '{0}'", (object)filePath);
            Patcher.WritePatch(patch2, filePath);
        }

        private static string GetVersion(string otherExe, string md5)
        {
            string str = md5;
            if (str == "C5-A1-D9-6F-94-AA-D0-24-FA-0E-31-07-A9-94-12-8C" || str == "32-44-3F-A8-FC-55-96-4A-59-23-39-8F-F2-C7-AB-B6")
                return "2.0";
            if (str == "19-DB-AB-1E-39-B3-D6-8A-6E-AC-20-B2-5F-65-3B-9D" || str == "45-46-DF-6A-B8-01-5C-BA-0A-20-DF-64-BD-BB-49-01")
                return "2.0a";
            if (str == "73-85-E4-9B-B2-E6-CA-61-6E-55-5F-88-42-88-E8-A5")
                return "2.0b";
            return md5.Replace("-", "");
        }


        private static string FindGameDirectory()
        {
            UserFeedback.Trace("Locating game main folder...");
            return new DirectoryInfo(Directory.GetCurrentDirectory()).FullName;
        }

        private static void ShowUsage()
        {
            UserFeedback.Warning("Usage: aoe2wide.exe [width height]");
            UserFeedback.Info("If the new width and height are omitted, the current screen resolution(s) will be used.");
            UserFeedback.Trace("Alternatively, use: aoe2wide.exe createpatches");
            UserFeedback.Trace("  to try to create fresh patch files for all your age2_x1 executables, from the RootPatch file");
        }

        private static void PatchExecutable(
            int newWidth,
            int newHeight,
            Patch patch,
            Dictionary<int, int> dictHeigth = null,
            Dictionary<int, int> dictWidth = null
            )
        {
            try
            {
                int oldWidth;
                int oldHeight;
                GetOldWidthHeight(newWidth, newHeight, out oldWidth, out oldHeight);
                UserFeedback.Info(string.Format("Changing {0}x{1} to {2}x{3}", (object)oldWidth, (object)oldHeight, (object)newWidth, (object)newHeight));
                UserFeedback.Trace("Reading original executable");
                byte[] numArray = File.ReadAllBytes(_orgExePath);
                string str1 = patch.Version.Length == 0 ? "" : patch?.Version;
                string str2 = Path.Combine(Path.Combine(_gameDirectory, "Data"), string.Format("{0:D4}{1:D4}.drs", DateTime.Now.Day, DateTime.Now.Month));// (object)newWidth, (object)newHeight); 
                string str3 = Path.Combine(Path.Combine(_gameDirectory, "Data"), string.Format("{0:D4}{1:D4}_x1.drs", DateTime.Now.Day, DateTime.Now.Month)); //(object)newWidth, (object)newHeight));

                string str4 = string.Empty;
                if (patch.Version == "2.0" || patch.Version == "2.0a" || patch.Version == "2.0b")
                    str4 = Path.Combine(_gameDirectory, string.Format("Empires2.exe", (object)newWidth, (object)newHeight, (object)str1));
                if (patch.Version == "1.0" || patch.Version == "1.0C" || patch.Version == "1.0c" || patch.Version == "1.0e")
                    str4 = Path.Combine(_gameDirectory, string.Format("age2_x1.exe", (object)newWidth, (object)newHeight, (object)str1));
                string path1 = Path.Combine(_gameDirectory, string.Format("Empire2.bat", (object)newWidth, (object)newHeight, (object)patch.Version));
                string path2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), string.Format("Empire2_{2} {0}x{1}.bat", (object)newWidth, (object)newHeight, (object)patch.Version));
                UserFeedback.Trace("Patching the executable: DRS reference");
                Patcher.PatchDrsRefInExe(numArray, Path.GetFileName(str2), patch);
                if (patch.InterfaceX1DrsPosition > 0)
                {
                    UserFeedback.Trace("Patching the executable: X1 DRS reference");
                    Patcher.PatchX1DrsRefInExe(numArray, Path.GetFileName(str3), patch);
                }
                UserFeedback.Trace("Patching the executable: resolutions");
                try
                {
                    Patcher.PatchResolutions(numArray, oldWidth, oldHeight, newWidth, newHeight, patch,dictHeigth,dictWidth);
                }
                catch
                {
                }
                UserFeedback.Trace(string.Format("Writing the patched executable '{0}'", (object)str4));
                File.WriteAllBytes(str4, numArray);
                UserFeedback.Trace("Writing convenience batch file '{0}'", (object)path1);
                List<string> stringList1 = new List<string>()
        {
          "@echo off"
        };
                string str5 = IsVistaOrHigher ? FindProcessExe() : (string)null;
                if (str5 != null)
                {
                    stringList1.Add("ECHO Using process.exe to suspend explorer.exe (win7, vista palette fix)");
                    stringList1.Add(string.Format("\"{0}\" -s explorer.exe", (object)str5));
                }
                stringList1.Add("ECHO Starting Age of Empires II - The Conquerers in the correct screen mode");
                stringList1.Add(string.Format("\"{0}\" {1}", (object)Path.GetFileName(str4), (object)oldWidth));
                if (str5 != null)
                {
                    stringList1.Add("ECHO Resuming explorer (was suspended before)");
                    stringList1.Add(string.Format("\"{0}\" -r explorer.exe", (object)str5));
                }
                //File.WriteAllLines(path1, stringList1.ToArray());
                string str6 = Path.GetPathRoot(_gameDirectory).Replace(Path.DirectorySeparatorChar, ' ');
                UserFeedback.Trace("Writing convenience desktop batch file '{0}'", (object)path2);
                List<string> stringList2 = new List<string>()
        {
          "@echo off",
          str6,
          string.Format("cd \"{0}\"", (object) _gameDirectory)
        };
                string str7 = IsVistaOrHigher ? FindProcessExe() : (string)null;
                if (str7 != null)
                {
                    stringList2.Add("ECHO Using process.exe to suspend explorer.exe (win7, vista palette fix)");
                    stringList2.Add(string.Format("\"{0}\" -s explorer.exe", (object)str7));
                }
                stringList2.Add("ECHO Starting Age of Empires II - The Conquerers in the correct screen mode");
                stringList2.Add(string.Format("\"{0}\" {1}", (object)Path.GetFileName(str4), (object)oldWidth));
                if (str7 != null)
                {
                    stringList2.Add("ECHO Resuming explorer (was suspended before)");
                    stringList2.Add(string.Format("\"{0}\" -r explorer.exe", (object)str7));
                }
                //File.WriteAllLines(path2, stringList2.ToArray());                //File.WriteAllLines(path2, stringList2.ToArray());
                if (dictHeigth != null)
                {
                    string ordrs = _orgDrsPath;
                    for (int i = 0; i < dictHeigth.Count; i++)
                    {
                        oldWidth = dictHeigth.ElementAt(i).Key;
                        oldHeight = dictWidth.ElementAt(i).Key;
                        newWidth = dictHeigth.ElementAt(i).Value;
                        newHeight = dictWidth.ElementAt(i).Value;
                        if(i!=0)
                        {
                            File.Copy(str2, _orgDrsPath, true);
                        }
                        PatchADrs(_orgDrsPath, str2, oldWidth, oldHeight, newWidth, newHeight);
                    }
                }
                else
                {
                     PatchADrs(_orgDrsPath, str2, oldWidth, oldHeight, newWidth, newHeight);
                }

                if (patch.InterfaceX1DrsPosition > 0)
                    PatchADrs(_orgX1DrsPath, str3, oldWidth, oldHeight, newWidth, newHeight);
                try
                {
                    string str8 = Path.Combine(_gameDirectory, "donePatches");
                    if (Directory.Exists(str8))
                    {
                        string str9 = Path.Combine(str8, string.Format("{0}x{1}", (object)newWidth, (object)newHeight));
                        if (!Directory.Exists(str9))
                            Directory.CreateDirectory(str9);
                        string str10 = Path.Combine(str9, "Data");
                        if (!Directory.Exists(str10))
                            Directory.CreateDirectory(str10);
                        string destFileName1 = Path.Combine(str9, Path.GetFileName(str4));
                        string destFileName2 = Path.Combine(str10, Path.GetFileName(str2));
                        File.Copy(str4, destFileName1, true);
                        File.Copy(str2, destFileName2, true);
                    }
                }
                catch (Exception ex)
                {
                    UserFeedback.Warning("Couldn't publish: {0}", (object)ex.ToString());
                }
                UserFeedback.Trace("Done");
            }
            catch (Exception ex)
            {
                UserFeedback.Error(ex);
            }
        }

        private static void GetOldWidthHeight(
          int newWidth,
          int newHeight,
          out int oldWidth,
          out int oldHeight)
        {
            if (newWidth >= 1280 && newHeight >= 1024)
            {
                oldWidth = 1280;
                oldHeight = 1024;
            }
            else if (newWidth < 1024 || newHeight == 600)
            {
                oldWidth = 800;
                oldHeight = 600;
            }
            else
            {
                oldWidth = 1024;
                oldHeight = 768;
            }
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter =
            "empires2 Age2_x1 exe (*.exe)|*.exe";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.gameExe = openFileDialog.FileName;
                this.gamePath = new FileInfo(this.gameExe).Directory.FullName;
                this.gameData = this.gamePath.Contains("Age2_x1")? this.gamePath.Replace("Age2_x1", "data") : this.gamePath + "\\data";
            }
            if(!this.gameExe.ToLower().Contains("empires2")&& !this.gameExe.ToLower().Contains("age2_x1"))
            {
                MessageBox.Show("Browser empires2.exe or Age2_x1.exe");
                return;
            }
            if (string.IsNullOrEmpty(this.gameExe))
                return;
            FileVersionInfo.GetVersionInfo(Path.Combine(Environment.SystemDirectory, this.gameExe));
            FileVersionInfo.GetVersionInfo(this.gameExe);
            List<string> lstDrsFile = Directory.GetFiles(this.gameData).Select(x => Path.GetFileName(x)).ToList();
            foreach (var item in lstDrsFile.Where(x => x.EndsWith(".drs")).ToList())
            {
                comboBoxSelectedDataFile.Items.Add(item);
            }
            if(lstDrsFile.Where(x => x.EndsWith(".drs")).ToList().Count>0)
            comboBoxSelectedDataFile.SelectedIndex = 0;
        }
       
        private void values(out int[] Ar, int v, int Size)
        {
            int num = v;
            List<int> intList = new List<int>();
            for (; num > 0; num /= 10)
                intList.Add(num % 10);
            Ar = intList.ToArray();
        }

        private void writeAdressLanguagedll(
          byte[] languagedll,
          int[] WidthAr,
          int[] HeightAr,
          List<string> xadrs,
          List<string> yadrs)
        {
            List<int> intList = new List<int>();
            List<byte> byteList = new List<byte>();
            foreach (string xadr in xadrs)
                intList.Add((int)languagedll[int.Parse(xadr, NumberStyles.HexNumber)]);
            foreach (int num in WidthAr)
                byteList.Add(((IEnumerable<byte>)Encoding.ASCII.GetBytes(num.ToString())).FirstOrDefault<byte>());
            for (int index = xadrs.Count - 1; index >= 0; --index)
                languagedll[int.Parse(xadrs[index], NumberStyles.HexNumber)] = byteList[xadrs.Count - 1 - index];
            intList.Clear();
            byteList.Clear();
            foreach (string yadr in yadrs)
                intList.Add((int)languagedll[int.Parse(yadr, NumberStyles.HexNumber)]);
            foreach (int num in HeightAr)
                byteList.Add(((IEnumerable<byte>)Encoding.ASCII.GetBytes(num.ToString())).FirstOrDefault<byte>());
            for (int index = yadrs.Count - 1; index >= 0; --index)
                languagedll[int.Parse(yadrs[index], NumberStyles.HexNumber)] = byteList[yadrs.Count - 1 - index];
        }

        private string checkIfAocOrAok()
        {
            string str = string.Empty;
            if (this.radioButton__20.Checked || this.radioButton_20a.Checked || this.radioButton_20b.Checked)
                str = "Aok";
            if (this.radioButton_10.Checked || this.radioButton_10c.Checked || this.radioButton_10e.Checked)
                str = "Aoc";
            return str;
        }

        private void UpdateLanguageDll(string Folderpath)
        {
            string str1 = this.checkIfAocOrAok();
            string empty = string.Empty;
            if (str1 == "Aoc")
                Folderpath = Folderpath.Replace("\\Age2_x1", "");
            string str2 = Path.Combine(Folderpath, "language.dll");
            byte[] numArray = File.ReadAllBytes(str2);
            string path = Path.Combine(Folderpath, "language.dll");
            int v1 = 0;
            int v2 = 0;
            foreach (Screen allScreen in Screen.AllScreens)
            {
                v1 = allScreen.Bounds.Width;
                v2 = allScreen.Bounds.Height;
            }
            int[] Ar1 = new int[v1.ToString().Length];
            int[] Ar2 = new int[v2.ToString().Length];
            this.values(out Ar1, v1, v1.ToString().Length);
            this.values(out Ar2, v2, v2.ToString().Length);
            string str3 = this.LanguageVersion(str2);
            List<string> stringList1 = new List<string>();
            List<string> stringList2 = new List<string>();
            if (v2 >= 1024)
            {
                string str4 = str3;
                if (!(str4 == "en"))
                {
                    if (!(str4 == "fr"))
                    {
                        if (!(str4 == "po"))
                        {
                            if (!(str4 == "es"))
                            {
                                if (str4 == "tu")
                                {
                                    List<string> xadrs1 = this.addAdressInList("00021290", "00021292", "00021294", "00021296");
                                    List<string> yadrs1 = this.addAdressInList("0002129E", "000212A0", "000212A2", "000212A4");
                                    this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs1, yadrs1);
                                    xadrs1.Clear();
                                    yadrs1.Clear();
                                    List<string> xadrs2 = this.addAdressInList("000212A8", "000212AA", "000212AC", "000212AE");
                                    List<string> yadrs2 = this.addAdressInList("000212B6", "000212B8", "000212BA", "000212BC");
                                    this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs2, yadrs2);
                                }
                            }
                            else
                            {
                                List<string> xadrs1 = this.addAdressInList("00024C1C", "00024C1E", "00024C20", "00024C22");
                                List<string> yadrs1 = this.addAdressInList("00024C2A", "00024C2C", "00024C2E", "00024C30");
                                this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs1, yadrs1);
                                xadrs1.Clear();
                                yadrs1.Clear();
                                List<string> xadrs2 = this.addAdressInList("00024C34", "00024C36", "00024C38", "00024C3A");
                                List<string> yadrs2 = this.addAdressInList("00024C42", "00024C44", "00024C46", "00024C48");
                                this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs2, yadrs2);
                            }
                        }
                        else
                        {
                            List<string> xadrs1 = this.addAdressInList("00023E98", "00023E9A", "00023E9C", "00023E9E");
                            List<string> yadrs1 = this.addAdressInList("00023EA6", "00023EA8", "00023EAA", "00023EAC");
                            this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs1, yadrs1);
                            xadrs1.Clear();
                            yadrs1.Clear();
                            List<string> xadrs2 = this.addAdressInList("00023E98", "00023E9A", "00023E9C", "00023E9E");
                            List<string> yadrs2 = this.addAdressInList("00023EBE", "00023EC0", "00023EC2", "00023EC4");
                            this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs2, yadrs2);
                        }
                    }
                    else
                    {
                        List<string> xadrs = this.addAdressInList("00025B94", "00025B96", "00025B98", "00025B9A");
                        List<string> yadrs = this.addAdressInList("00025BA2", "00025BA4", "00025BA6", "00025BA8");
                        this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs, yadrs);
                    }
                }
                else
                {
                    List<string> xadrs = this.addAdressInList("000171BC", "000171BE", "000171C0", "000171C2");
                    List<string> yadrs = this.addAdressInList("000171CA", "000171CC", "000171CE", "000171D0");
                    this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs, yadrs);
                }
            }
            else
            {
                string str4 = str3;
                if (!(str4 == "en"))
                {
                    if (!(str4 == "fr"))
                    {
                        if (!(str4 == "po"))
                        {
                            if (!(str4 == "es"))
                            {
                                if (str4 == "tu")
                                {
                                    List<string> xadrs = this.addAdressInList("00021204", "00021206", "00021208", "0002120A");
                                    List<string> yadrs = this.addAdressInList("00021212", "00021214", "00021216");
                                    this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs, yadrs);
                                }
                            }
                            else
                            {
                                List<string> xadrs = this.addAdressInList("00024B7E", "00024B80", "00024B82", "00024B84");
                                List<string> yadrs = this.addAdressInList("00024B8C", "00024B8E", "00024B90");
                                this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs, yadrs);
                            }
                        }
                        else
                        {
                            List<string> xadrs = this.addAdressInList("00023E16", "00023E18", "00023E1A", "00023E1C");
                            List<string> yadrs = this.addAdressInList("00023E24", "00023E26", "00023E28");
                            this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs, yadrs);
                        }
                    }
                    else
                    {
                        List<string> xadrs = this.addAdressInList("00025AEC", "00025AEE", "00025AF0", "00025AF2");
                        List<string> yadrs = this.addAdressInList("00025AFA", "00025AFC", "00025AFE");
                        this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs, yadrs);
                    }
                }
                else
                {
                    List<string> xadrs = this.addAdressInList("0001712C", "0001712E", "00017130", "00017132");
                    List<string> yadrs = this.addAdressInList("0001713A", "0001713C", "0001713E");
                    this.writeAdressLanguagedll(numArray, Ar1, Ar2, xadrs, yadrs);
                }
            }
            if (File.Exists(str2))
                File.Delete(str2);
            File.WriteAllBytes(path, numArray);
        }

        private List<string> addAdressInList(params string[] Args)
        {
            List<string> stringList = new List<string>();
            foreach (string str in Args)
                stringList.Add(str);
            return stringList;
        }

        private string en(byte[] languagedll)
        {
            string str = string.Empty;
            try
            {
                if (Encoding.ASCII.GetString(new byte[8]
                {
          languagedll[int.Parse("0000D804", NumberStyles.HexNumber)],
          languagedll[int.Parse("0000D806", NumberStyles.HexNumber)],
          languagedll[int.Parse("0000D808", NumberStyles.HexNumber)],
          languagedll[int.Parse("0000D80A", NumberStyles.HexNumber)],
          languagedll[int.Parse("0000D80C", NumberStyles.HexNumber)],
          languagedll[int.Parse("0000D80E", NumberStyles.HexNumber)],
          languagedll[int.Parse("0000D810", NumberStyles.HexNumber)],
          languagedll[int.Parse("0000D812", NumberStyles.HexNumber)]
                }) == "LANGUAGE")
                    str = nameof(en);
            }
            catch
            {
            }
            return str;
        }

        private string fr(byte[] languagedll)
        {
            string str1 = string.Empty;
            try
            {
                string str2 = Encoding.ASCII.GetString(new byte[6]
                {
          languagedll[int.Parse("0008d068", NumberStyles.HexNumber)],
          languagedll[int.Parse("0008d06A", NumberStyles.HexNumber)],
          languagedll[int.Parse("0008d06C", NumberStyles.HexNumber)],
          languagedll[int.Parse("0008d06E", NumberStyles.HexNumber)],
          languagedll[int.Parse("0008d070", NumberStyles.HexNumber)],
          languagedll[int.Parse("0008d072", NumberStyles.HexNumber)]
                });
                if (str2 == "LANGUE" || str2 == "LANGUA")
                    str1 = nameof(fr);
            }
            catch
            {
            }
            return str1;
        }

        private string po(byte[] languagedll)
        {
            string str1 = string.Empty;
            try
            {
                string str2 = Encoding.ASCII.GetString(new byte[8]
                {
          languagedll[int.Parse("00085B18", NumberStyles.HexNumber)],
          languagedll[int.Parse("00085B1A", NumberStyles.HexNumber)],
          languagedll[int.Parse("00085B1C", NumberStyles.HexNumber)],
          languagedll[int.Parse("00085B1E", NumberStyles.HexNumber)],
          languagedll[int.Parse("00085B20", NumberStyles.HexNumber)],
          languagedll[int.Parse("00085B22", NumberStyles.HexNumber)],
          languagedll[int.Parse("00085B24", NumberStyles.HexNumber)],
          languagedll[int.Parse("00085B26", NumberStyles.HexNumber)]
                });
                if (str2 == "IDIOMA" || str2 == "LINGUA" || str2 == "LANGUAGE")
                    str1 = nameof(po);
            }
            catch
            {
            }
            return str1;
        }

        private string es(byte[] languagedll)
        {
            string str = string.Empty;
            try
            {
                if (Encoding.ASCII.GetString(new byte[6]
                {
          languagedll[int.Parse("00088A54", NumberStyles.HexNumber)],
          languagedll[int.Parse("00088A56", NumberStyles.HexNumber)],
          languagedll[int.Parse("00088A58", NumberStyles.HexNumber)],
          languagedll[int.Parse("00088A5A", NumberStyles.HexNumber)],
          languagedll[int.Parse("00088A5C", NumberStyles.HexNumber)],
          languagedll[int.Parse("00088A5E", NumberStyles.HexNumber)]
                }) == "IDIOMA")
                    str = nameof(es);
            }
            catch
            {
            }
            return str;
        }

        private string tu(byte[] languagedll)
        {
            string str = string.Empty;
            try
            {
                if (Encoding.ASCII.GetString(new byte[8]
                {
          languagedll[int.Parse("000786B8", NumberStyles.HexNumber)],
          languagedll[int.Parse("000786BA", NumberStyles.HexNumber)],
          languagedll[int.Parse("000786BC", NumberStyles.HexNumber)],
          languagedll[int.Parse("000786BE", NumberStyles.HexNumber)],
          languagedll[int.Parse("000786C0", NumberStyles.HexNumber)],
          languagedll[int.Parse("000786C2", NumberStyles.HexNumber)],
          languagedll[int.Parse("000786C4", NumberStyles.HexNumber)],
          languagedll[int.Parse("000786C6", NumberStyles.HexNumber)]
                }) == "LANGUAGE")
                    str = nameof(tu);
            }
            catch
            {
            }
            return str;
        }

        private string LanguageVersion(string dllFile)
        {
            byte[] languagedll = File.ReadAllBytes(dllFile);
            string str = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(this.en(languagedll)))
                    str = this.en(languagedll);
                if (!string.IsNullOrEmpty(this.po(languagedll)))
                    str = this.po(languagedll);
                if (!string.IsNullOrEmpty(this.fr(languagedll)))
                    str = this.fr(languagedll);
                if (!string.IsNullOrEmpty(this.es(languagedll)))
                    str = this.es(languagedll);
                if (!string.IsNullOrEmpty(this.tu(languagedll)))
                    str = this.tu(languagedll);
            }
            catch
            {
            }
            return str;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private static string[] GetExports(string ModuleFileName)
        {
            SafeFileHandle file = NativeMethods.CreateFile(ModuleFileName, NativeMethods.EFileAccess.GenericRead, NativeMethods.EFileShare.Read, IntPtr.Zero, NativeMethods.ECreationDisposition.OpenExisting, NativeMethods.EFileAttributes.Normal, IntPtr.Zero);
            if (file.IsInvalid)
                throw new Win32Exception();
            try
            {
                SafeFileHandle fileMapping = NativeMethods.CreateFileMapping(file, IntPtr.Zero, NativeMethods.FileMapProtection.PageReadonly, 0U, 0U, IntPtr.Zero);
                if (fileMapping.IsInvalid)
                    throw new Win32Exception();
                try
                {
                    IntPtr num1 = NativeMethods.MapViewOfFile(fileMapping, NativeMethods.FileMapAccess.FileMapRead, 0U, 0U, UIntPtr.Zero);
                    if (num1 == IntPtr.Zero)
                        throw new Win32Exception();
                    try
                    {
                        IntPtr num2 = NativeMethods.ImageNtHeader(num1);
                        if (num2 == IntPtr.Zero)
                            throw new Win32Exception();
                        NativeMethods.IMAGE_NT_HEADERS structure1 = (NativeMethods.IMAGE_NT_HEADERS)Marshal.PtrToStructure(num2, typeof(NativeMethods.IMAGE_NT_HEADERS));
                        if (structure1.Signature != 17744U)
                            throw new Exception(ModuleFileName + " is not a valid PE file");
                        IntPtr va1 = NativeMethods.ImageRvaToVa(num2, num1, structure1.OptionalHeader.DataDirectory[0].VirtualAddress, IntPtr.Zero);
                        if (va1 == IntPtr.Zero)
                            throw new Win32Exception();
                        NativeMethods.IMAGE_EXPORT_DIRECTORY structure2 = (NativeMethods.IMAGE_EXPORT_DIRECTORY)Marshal.PtrToStructure(va1, typeof(NativeMethods.IMAGE_EXPORT_DIRECTORY));
                        IntPtr va2 = NativeMethods.ImageRvaToVa(num2, num1, structure2.AddressOfNames, IntPtr.Zero);
                        if (va2 == IntPtr.Zero)
                            throw new Win32Exception();
                        IntPtr va3 = NativeMethods.ImageRvaToVa(num2, num1, (uint)Marshal.ReadInt32(va2), IntPtr.Zero);
                        if (va3 == IntPtr.Zero)
                            throw new Win32Exception();
                        string[] strArray = new string[(int)structure2.NumberOfNames];
                        for (int index = 0; index < strArray.Length; ++index)
                        {
                            strArray[index] = Marshal.PtrToStringAnsi(va3);
                            va3 += strArray[index].Length + 1;
                        }
                        return strArray;
                    }
                    finally
                    {
                        if (!NativeMethods.UnmapViewOfFile(num1))
                            throw new Win32Exception();
                    }
                }
                finally
                {
                    fileMapping.Close();
                }
            }
            finally
            {
                file.Close();
            }
        }
        bool isManualResolution;

        private void cb_SetManuelReso_CheckedChanged(object sender, EventArgs e)
        {
            GB_ManualRes.Visible = cb_SetManuelReso.Checked;
        }

        private void AokPatch_Load(object sender, EventArgs e)
        {
            
            GB_ManualRes.Visible= false;
            var scope = new ManagementScope();
            labelDescSIgnal.Visible = false;
            numericUpDownSizeOfX.Visible = false;

            var query = new ObjectQuery("SELECT * FROM CIM_VideoControllerResolution");
            //comboBox800.Items.Add("Auto");
            comboBox1024.Items.Add("Auto");
            comboBox1280.Items.Add("Auto");

            using (var searcher = new ManagementObjectSearcher(scope, query))
            {
                var results = searcher.Get();
                comboBox800.Items.Add("800x600");
                foreach (var result in results)
                {

                    //bug for the 1 st spot when chose another ressolution
                    //comboBox800.Items.Add("" + result["HorizontalResolution"] + "x" + result["VerticalResolution"]);
                    if (int.Parse(result["HorizontalResolution"]+"") >= 1024 && int.Parse(result["HorizontalResolution"] + "") >= 768)
                    {

                        HashSet<int> intSet = new HashSet<int>();
                        int num = int.Parse(result["HorizontalResolution"] + "") + int.Parse(result["HorizontalResolution"] + "") * 65536;
                        if (intSet.Add(num))
                            comboBox1024.Items.Add("" + result["HorizontalResolution"] + "x" + result["VerticalResolution"]);
                    }

                    if (int.Parse(result["HorizontalResolution"] + "") >=1280  && int.Parse(result["HorizontalResolution"] + "") >= 1024)
                    {
                        HashSet<int> intSet = new HashSet<int>();
                        int num = int.Parse(result["HorizontalResolution"] + "") + int.Parse(result["HorizontalResolution"] + "") * 65536;
                        if (intSet.Add(num))
                            comboBox1280.Items.Add("" + result["HorizontalResolution"] + "x" + result["VerticalResolution"]); 
                    }
                }
            }
            comboBox800.SelectedIndex = 0;
            comboBox1024.SelectedIndex = 0;
            comboBox1280.SelectedIndex = 0;
            tabhelper = new TabControlHelper(tabControlAokPatch);

            //comboBoxTypeSlp.SelectedIndex = 0;
            slpView.Show();
            slpView.Visible = false;

            //slpView.Hide();
            //tabhelper.HidePage(tabControlAokPatch.TabPages["DrsEditor"]);
        }
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
        public bool is32Bit;
        public FileStream fs;
        /// <summary>
        /// get correspondance id 1024x768|1280x1024 to resize interface
        /// </summary>
        /// <returns></returns>
        private Dictionary<uint, uint> getIdInterfaceCorrespondance()
        {
            Dictionary<uint, uint> Correspondance = new Dictionary<uint, uint>();
            //1024x768|1280x1024 
             Correspondance.Add(51121,51141);
             Correspondance.Add(51122,51142);
             Correspondance.Add(51123,51143);
             Correspondance.Add(51124,51144);
             Correspondance.Add(51125,51145);
             Correspondance.Add(51126,51146);
             Correspondance.Add(51127,51147);
             Correspondance.Add(51128,51148);
             Correspondance.Add(51129,51149);
             Correspondance.Add(51130,51150);
             Correspondance.Add(51131,51151);
             Correspondance.Add(51132,51152);
             Correspondance.Add(51133,51153);
             Correspondance.Add(51134,51154);
             Correspondance.Add(51135,51157);
             Correspondance.Add(51136,51158);
             Correspondance.Add(51137,51159);
             Correspondance.Add(51138,51160);
            return Correspondance;
        }
        private Bitmap slpToBmp(string SlpFileName)
        {
            #region slp export to bmp
            slpReader slpname = new slpReader();
            String workingDir = Path.GetDirectoryName(SlpFileName);
            if (!workingDir.EndsWith("\\"))
                workingDir += "\\";
            string fi = Path.GetFileName(SlpFileName);
            Console.WriteLine("Extracting frames from " + fi + "...");
            slpname.sample = fi.Equals("int50101.slp") ? "50532.bmp" : "50500.bmp";
            Console.WriteLine(fi + " " + slpname.sample);
            slpname.name = fi.Split('.').First();
            slpname.Read(SlpFileName);
            slpname.save(workingDir);
            var fileName = Path.GetDirectoryName(SlpFileName) + $@"\{slpname.name}.bmp";

            Bitmap bmp = (Bitmap)Bitmap.FromFile(fileName);
            #endregion
            return bmp;
        }
        private bool ConverteBmpToMosaic(Bitmap bmp,string SlpFileName,string fileName,uint id,Color[] lstColor)
        {
            bool needToWriteSlp = true;
            if (bmp.Width==1024 && bmp.Height ==768|| bmp.Width == 1280 && bmp.Height == 1024)
            {
                return false;
            }
            #region converte in mosaic image
            var file = SlpFileName.Replace(Path.GetDirectoryName(SlpFileName) + @"\", "").Replace(".bmp", "");

            var WidthToCut = 0;
            var Width = bmp.Width;
            var Height = bmp.Height;
            if (id <= 51138 && id >= 51121)
                WidthToCut = 1024;
            else if (id <= 51160 && id >= 51141)
            { 
                WidthToCut = 1280;
            }

            Rectangle rec = new Rectangle(0, 0, WidthToCut, Height);
            Bitmap tempBitmap = Extract(bmp, rec);
            string newBmpFile = Path.GetDirectoryName(SlpFileName) + @"\" + file + "_r_" + fileName + ".bmp";
            //if (WidthToCut == 1280)
            //    FixParasite(tempBitmap);
            
            
            //fix parasite
           if (WidthToCut == 1280 || bmp.Width !=1024)
            { 
                using (Graphics graphics = Graphics.FromImage(tempBitmap))
                {
                    using (System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(180, 255, 180)))
                    {

                        int start = Convert.ToInt32(0.03333333 * tempBitmap.Height);
                        int recHeight = tempBitmap.Height - Convert.ToInt32(0.03333333 * tempBitmap.Height) - Convert.ToInt32(0.23 * tempBitmap.Height);

                        graphics.FillRectangle(myBrush, new Rectangle(0, start, tempBitmap.Width, recHeight)); // whatever
                       // graphics.FillRectangle(myBrush, new Rectangle(0, tempBitmap.Width/4, tempBitmap.Width, tempBitmap.Height*3/4- tempBitmap.Height / 3-100)); // whatever

                    } // myBrush will be disposed at this line
                    //tempBitmap.Save(fileName);
                } // graphics will be disposed at 
                tempBitmap.Save(newBmpFile, ImageFormat.Bmp);
            }
            
            Bitmap image = new Bitmap(tempBitmap);
            tempBitmap = new Bitmap(Width, Height);
            //TextureBrush(Image image, WrapMode wrapMode, RectangleF dstRect);
            //RectangleF dstRect = new RectangleF(0, 0, WidthToCut, tempBitmap.Height);
            using (TextureBrush brush = new TextureBrush(image, WrapMode.Tile))
            using (Graphics g = Graphics.FromImage(tempBitmap))
            {
                // Do your painting in here
                g.FillRectangle(brush, 0, 0, Width, Height);
            }

            tempBitmap.Save(newBmpFile, ImageFormat.Bmp);
            

           Color myTransparentColor = Color.FromArgb(180, 255, 180);
            var getDontMakeBlackTransparent = Color.FromArgb(0, 0, 0);
            ReplaceColor(tempBitmap, getDontMakeBlackTransparent, Color.FromArgb(9, 9, 14));
            ReplaceColor(tempBitmap, myTransparentColor, Color.Black);
            ReplaceColor(tempBitmap, Color.FromArgb(29, 0, 29, 50), Color.Black);
            //tempBitmap.MakeTransparent(myTransparentColor);
            tempBitmap.Save(newBmpFile, ImageFormat.Bmp);
            
            //using (var stream = new FileStream(file, FileMode.Create))
            //{
            // You can either use an arbitrary palette,
            var b = tempBitmap.ConvertPixelFormat(PixelFormat.Format8bppIndexed, lstColor,default(Color), 255);// Color backColor = default, byte alphaThreshold = 128
                b.Save(newBmpFile, ImageFormat.Bmp);
                // or, you can let the built-in encoder use dithering with a fixed palette.
                // Pixel format is adjusted so transparency will be preserved.
                //    tempBitmap.SaveAsBmp(stream, allowDithering: true);
            //}
            /*
         using (Image image8Bpp = ConvertPixelFormat(tempBitmap, PixelFormat.Format8bppIndexed, lstColor.ToArray()))
            {
                //Color myTransparentColor = Color.FromArgb(180, 255, 180);
                //((Bitmap)image8Bpp).MakeTransparent(myTransparentColor);
                image8Bpp.Save(newBmpFile, ImageFormat.Bmp);

            }*/
            bmp.Dispose();
            #endregion converte in mosaic image
            return needToWriteSlp;
        }
        private List<Color> GetAokPaletteColor()
        {

            #region Get Aok palette color from .pal file
            List<string> lst = Resource1.AOE2_Alpha_l.Split('\n').ToList();
            lst = lst.Select(w => w.Replace("\r", "")).ToList();
            lst.RemoveAt(0);
            lst.RemoveAt(0);
            lst.RemoveAt(0);
            List<Color> lstColor = new List<Color>();
            string[] color;
            //Color FromArgb(int red, int green, int blue);
            foreach (var item in lst)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    color = item.Split(' ');
                    Color c = Color.FromArgb(int.Parse(color[0]), int.Parse(color[1]), int.Parse(color[2]));
                    lstColor.Add(c);
                }
            }
            #endregion
            return lstColor;
        }
        private void SlpWrite(string workingDir,string fileName,string fil)
        {
            bool transmask = true;
            bool outline1 = false;
            bool outline2 = false;
            bool plcolor = false;
            bool shadowpix = false;
            #region write slp
            slpWriter slpw = new slpWriter();
            slpw.maskfile = Path.GetDirectoryName(fileName) + "mask.bmp";
            int numframes = 1;
            byte b;
            int i;
            String outputname = workingDir + fil;
            slpw.initframes(numframes);
            string Mosaicfile = fil + "_r_" + fileName + ".bmp";
            slpw.getframe(0, workingDir, Mosaicfile, transmask, outline1, outline2, plcolor, shadowpix);
            slpw.compress();
            slpw.Write(outputname);
            #endregion
        }
        private void ResizeDrsInt_Click(object sender, EventArgs e)
        {


            List<DrsItem> lstitems = new List<DrsItem>();
            DrsTable[] drsTableArray;
            List<DrsItem> lstItem = new List<DrsItem>();
            var idToPick = new List<uint>();
            string newDrsName = Path.Combine(Path.Combine(_gameDirectory, "Data"), string.Format("{0:D4}{1:D4}.drs", DateTime.Now.Day, DateTime.Now.Month));
            var file = Path.GetFileName(newDrsName);
            var GameDirectory = this.gamePath + @"\data\";
            using (FileStream fileStream1 = new FileStream(newDrsName, FileMode.Open, FileSystemRights.ReadData, FileShare.Read, 1048576, FileOptions.SequentialScan))
            {
                #region DRS read 
                BinaryReader binaryReader = new BinaryReader(fileStream1);
                bool flag = false;
                while (true)
                {
                    byte num = binaryReader.ReadByte();
                    //binaryWriter.Write(num);
                    if (num == (byte)26)
                        flag = true;
                    else if (num > (byte)0 & flag)
                        break;
                }
                binaryReader.ReadBytes(3);
                binaryReader.ReadBytes(12);
                uint num1 = binaryReader.ReadUInt32();
                uint num2 = binaryReader.ReadUInt32();

                drsTableArray = new DrsTable[(int)num1];
                for (int index = 0; (long)index < (long)num1; ++index)
                    drsTableArray[index] = new DrsTable();
                foreach (DrsTable drsTable in drsTableArray)
                {
                    drsTable.Type = binaryReader.ReadUInt32();
                    drsTable.Start = binaryReader.ReadUInt32();
                    uint num3 = binaryReader.ReadUInt32();
                    DrsItem[] drsItemArray = new DrsItem[(int)num3];
                    for (int index = 0; (long)index < (long)num3; ++index)
                        drsItemArray[index] = new DrsItem();
                    drsTable.Items = (IEnumerable<DrsItem>)drsItemArray;
                }
                foreach (DrsTable drsTable in drsTableArray)
                {
                    Trace.Assert(fileStream1.Position == (long)drsTable.Start);
                    foreach (DrsItem drsItem in drsTable.Items)
                    {
                        drsItem.Id = binaryReader.ReadUInt32();
                        drsItem.Start = binaryReader.ReadUInt32();
                        drsItem.Size = binaryReader.ReadUInt32();
                    }
                }
                foreach (DrsItem drsItem in ((IEnumerable<DrsTable>)drsTableArray).SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>)(table => table.Items)))
                {
                    Trace.Assert(fileStream1.Position == (long)drsItem.Start);
                    drsItem.Data = binaryReader.ReadBytes((int)drsItem.Size);
                }
                lstItem = drsTableArray.Where(w => w.Type == 1936486432).First().Items.ToList();
                binaryReader.Close();
                #endregion
                #region aok slp interface liste
                
                //1024x768 interface
                idToPick.AddRange(
                    new List<uint> {
                        #region 1024x768
                        51121,
                        51122,
                        51123,
                        51124,
                        51125,
                        51126,
                        51127,
                        51128,
                        51129,
                        51130,
                        51131,
                        51132,
                        51133,
                        51134,
                        51135,
                        51136,
                        51137,
                        51138,
                        #endregion 1024x768
                        #region 1280x1024
                        51141,
                        51142,
                        51143,
                        51144,
                        51145,
                        51146,
                        51147,
                        51148,
                        51149,
                        51150,
                        51151,
                        51152,
                        51153,
                        51154,
                        51157,
                        51158,
                        51159,
                        51160,
                        #endregion
                    });


                #endregion
                #region export slp
                //get drs int
                foreach (var item in lstItem)
                {
                    if (idToPick.Contains(item.Id))
                    {
                        lstitems.Add(item);
                    }
                }
                //creat a directory to exporte slp
                if (!Directory.Exists("SlpImport"))
                {
                    Directory.CreateDirectory("SlpImport");
                }
                //extract slp for interface.drs
                foreach (DrsItem DrsItem in lstitems)
                {
                    File.WriteAllBytes(@"SlpImport\" + DrsItem.Id + ".slp", DrsItem.Data);
                }
                #endregion
                fileStream1.Close();
            }
            // opens the folder in explorer
            Process.Start(@"SlpImport\");
            //var log = new  TextBoxListener("richTextBoxWideScreenLog");
            //log.Init();
            DirectoryInfo dirInfo = new DirectoryInfo(@"SlpImport\");
            var ListSlp = dirInfo.GetFiles();



            string SlpFileName = string.Empty;
            string name = string.Empty;
            uint id = 0;
            int cpt = 0;
            int x = 0;
            int y = 0;

            var lstColor = GetAokPaletteColor().ToArray();
            foreach (var item in idToPick)
            {
 
                var fil = ListSlp.Where(z => z.Name == item  + ".slp").First();
                SlpFileName = fil.FullName;
                name = Path.GetFileName(SlpFileName).Replace(".slp", "");
                id = uint.Parse(name);

                Bitmap bmp = slpToBmp(SlpFileName);

                string fileName = Path.GetFileName(SlpFileName).Split('.').First();
                String workingDir = Path.GetDirectoryName(SlpFileName);
                if (!workingDir.EndsWith("\\"))
                    workingDir += "\\";


                if(!ConverteBmpToMosaic(bmp, SlpFileName, fileName, id, lstColor))
                {
                    continue;
                }

                SlpWrite(workingDir, fileName, fileName + ".slp");

                String outputname = workingDir + fil.Name;
                //lstItem.Where(z => z.Id == id).First().Data = File.ReadAllBytes(outputname);
                var data = File.ReadAllBytes(outputname);
                drsTableArray.Where(z => z.Type == 1936486432).First().Items.Where(z => z.Id == id).First().Data = data;
                drsTableArray.Where(z => z.Type == 1936486432).First().Items.Where(z => z.Id == id).First().Size = (uint)data.Length;
               
            }

            
            ////update interface.drs tmp file

             
            using (FileStream fileStream1 = new FileStream(_orgDrsPath, FileMode.Open, FileSystemRights.ReadData, FileShare.Read, 1048576, FileOptions.SequentialScan))
            {
                
                BinaryReader binaryReader = new BinaryReader(fileStream1);
                using (FileStream fileStream2 = new FileStream(newDrsName, FileMode.Create, FileSystemRights.Write, FileShare.None, 1048576, FileOptions.SequentialScan))
                {
                    bool flag = false;
                    BinaryWriter binaryWriter = new BinaryWriter(fileStream2);
                    while (true)
                    {
                        byte num = binaryReader.ReadByte();
                        binaryWriter.Write(num);
                        if (num == (byte)26)
                            flag = true;
                        else if (num > (byte)0 & flag)
                            break;
                    }
                    binaryWriter.Write(binaryReader.ReadBytes(3));
                    binaryWriter.Write(binaryReader.ReadBytes(12));
                    uint num1 = binaryReader.ReadUInt32();
                    binaryWriter.Write(num1);
                    uint num2 = binaryReader.ReadUInt32();
                    binaryWriter.Write(num2);
                    binaryReader.Close();
                    uint num4 = num2;
                    List<DrsTable> source = new List<DrsTable>();// new List<DrsTable>(drsTableArray.Length);
                    //update possitions
                    foreach (DrsTable drsTable1 in drsTableArray)
                    {
                        List<DrsItem> drsItemList = new List<DrsItem>();
                        DrsTable drsTable2 = new DrsTable()
                        {
                            Start = drsTable1.Start,
                            Type = drsTable1.Type,
                            Items = (IEnumerable<DrsItem>)drsItemList
                        };
                        foreach (DrsItem drsItem1 in drsTable1.Items)
                        {
                            DrsItem drsItem2 = new DrsItem()
                            {
                                Id = drsItem1.Id,
                                Start = num4,
                                Data = drsItem1.Data
                            };
                            drsItem2.Size = (uint)drsItem2.Data.Length;
                            num4 += drsItem2.Size;
                            drsItemList.Add(drsItem2);
                        }
                        source.Add(drsTable2);
                    }
                    foreach (DrsTable drsTable in source)
                    {
                        binaryWriter.Write(drsTable.Type);
                        binaryWriter.Write(drsTable.Start);
                        binaryWriter.Write(drsTable.Items.Count<DrsItem>());
                    }
                    foreach (DrsTable drsTable in source)
                    {
                        foreach (DrsItem drsItem in drsTable.Items)
                        {
                            binaryWriter.Write(drsItem.Id);
                            binaryWriter.Write(drsItem.Start);
                            binaryWriter.Write(drsItem.Size);
                        }
                    } 
                    foreach (DrsItem drsItem in source.SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>)(outTable => outTable.Items)))
                    {
                        binaryWriter.Write(drsItem.Data);
                    }
                    binaryWriter.Close();
                    fileStream2.Close();
                }
                fileStream1.Close();
            }

            File.Copy(newDrsName, GameDirectory + file,true);
            Directory.Delete(@"SlpImport\",true);
        }
        // TODO: Use some quantizer

        public static Bitmap Extract(Bitmap src, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(src, 0, 0, section, GraphicsUnit.Pixel);
            }
            return bmp;
        }
        // replace one color with another
        private static void ReplaceColor(Bitmap bmp, Color oldColor, Color newColor)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var c = bmp.GetPixel(x, y);
                    if(c== oldColor)
                    {
                        //var colo = Color.Black;
                            bmp.SetPixel(x, y, newColor);
                    }
                }
            }
        }

        private static void FixParasite(Bitmap bmp)
        {
            int min = bmp.Height / 4;
            int max = bmp.Height * 3 / 4;
            var color = Color.FromArgb(180, 255, 180);
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c != color)
                    {
                        if (min  > y && y < max)
                        {
                            bmp.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }

        private static Color[] GetColors(Bitmap bitmap, int maxColors)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");
            if (maxColors < 0)
                throw new ArgumentOutOfRangeException("maxColors");

            HashSet<int> colors = new HashSet<int>();
            PixelFormat pixelFormat = bitmap.PixelFormat;
            if (Image.GetPixelFormatSize(pixelFormat) <= 8)
                return bitmap.Palette.Entries;

            // 32 bpp source: the performant variant
            if (pixelFormat == PixelFormat.Format32bppRgb ||
                pixelFormat == PixelFormat.Format32bppArgb ||
                pixelFormat == PixelFormat.Format32bppPArgb)
            {
                BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, pixelFormat);
                try
                {
                    unsafe
                    {
                        byte* line = (byte*)data.Scan0;
                        for (int y = 0; y < data.Height; y++)
                        {
                            for (int x = 0; x < data.Width; x++)
                            {
                                int c = ((int*)line)[x];
                                // if alpha is 0, adding the transparent color
                                if ((c >> 24) == 0)
                                    c = 0xFFFFFF;
                                if (colors.Contains(c))
                                    continue;

                                colors.Add(c);
                                if (colors.Count == maxColors)
                                    return colors.Select(Color.FromArgb).ToArray();
                            }

                            line += data.Stride;
                        }
                    }
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }
            else
            {
                // fallback: getpixel
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        int c = bitmap.GetPixel(x, y).ToArgb();
                        if (colors.Contains(c))
                            continue;

                        colors.Add(c);
                        if (colors.Count == maxColors)
                            return colors.Select(Color.FromArgb).ToArray();
                    }
                }
            }

            return colors.Select(Color.FromArgb).ToArray();
        }

        private static Image ConvertPixelFormat(Image image, PixelFormat newPixelFormat, Color[] palette)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            PixelFormat sourcePixelFormat = image.PixelFormat;

            int bpp = Image.GetPixelFormatSize(newPixelFormat);
            if (newPixelFormat == PixelFormat.Format16bppArgb1555 || newPixelFormat == PixelFormat.Format16bppGrayScale)
                throw new NotSupportedException("This pixel format is not supported by GDI+");

            Bitmap result;

            // non-indexed target image (transparency preserved automatically)
            if (bpp > 8)
            {
                result = new Bitmap(image.Width, image.Height, newPixelFormat);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(image, 0, 0, image.Width, image.Height);
                }

                return result;
            }

            int transparentIndex;
            Bitmap bmp;

            // indexed colors: using GDI+ natively
            RGBQUAD[] targetPalette = new RGBQUAD[256];
            int colorCount = InitPalette(targetPalette, bpp, (image is Bitmap) ? image.Palette : null, palette, out transparentIndex);
            BITMAPINFO bmi = new BITMAPINFO();
            bmi.icHeader.biSize = (uint)Marshal.SizeOf(typeof(BITMAPINFOHEADER));
            bmi.icHeader.biWidth = image.Width;
            bmi.icHeader.biHeight = image.Height;
            bmi.icHeader.biPlanes = 1;
            bmi.icHeader.biBitCount = (ushort)bpp;
            bmi.icHeader.biCompression = BI_RGB;
            bmi.icHeader.biSizeImage = (uint)(((image.Width + 7) & 0xFFFFFFF8) * image.Height / (8 / bpp));
            bmi.icHeader.biXPelsPerMeter = 0;
            bmi.icHeader.biYPelsPerMeter = 0;
            bmi.icHeader.biClrUsed = (uint)colorCount;
            bmi.icHeader.biClrImportant = (uint)colorCount;
            bmi.icColors = targetPalette;

            bmp = (image as Bitmap) ?? new Bitmap(image);

            // Creating the indexed bitmap
            IntPtr bits;
            IntPtr hbmResult = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out bits, IntPtr.Zero, 0);

            // Obtaining screen DC
            IntPtr dcScreen = GetDC(IntPtr.Zero);

            // DC for the original hbitmap
            IntPtr hbmSource = bmp.GetHbitmap();
            IntPtr dcSource = CreateCompatibleDC(dcScreen);
            SelectObject(dcSource, hbmSource);

            // DC for the indexed hbitmap
            IntPtr dcTarget = CreateCompatibleDC(dcScreen);
            SelectObject(dcTarget, hbmResult);

            // Copy content
            BitBlt(dcTarget, 0, 0, image.Width, image.Height, dcSource, 0, 0, 0x00CC0020 /*TernaryRasterOperations.SRCCOPY*/);

            // obtaining result
            result = Image.FromHbitmap(hbmResult);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            // cleanup
            DeleteDC(dcSource);
            DeleteDC(dcTarget);
            ReleaseDC(IntPtr.Zero, dcScreen);
            DeleteObject(hbmSource);
            DeleteObject(hbmResult);

            ColorPalette resultPalette = result.Palette;
            bool resetPalette = false;

            // restoring transparency
            if (transparentIndex >= 0)
            {
                // updating palette if transparent color is not actually transparent
                if (resultPalette.Entries[transparentIndex].A != 0)
                {
                    resultPalette.Entries[transparentIndex] = Color.Transparent;
                    resetPalette = true;
                }

                ToIndexedTransparentByArgb(result, bmp, transparentIndex);
            }

            if (resetPalette)
                result.Palette = resultPalette;

            if (!ReferenceEquals(bmp, image))
                bmp.Dispose();
            return result;
        }

        private static int InitPalette(RGBQUAD[] targetPalette, int bpp, ColorPalette originalPalette, Color[] desiredPalette, out int transparentIndex)
        {
            int maxColors = 1 << bpp;

            // using desired palette
            Color[] sourcePalette = desiredPalette;

            // or, using original palette if it has fewer or the same amount of colors as requested
            if (sourcePalette == null && originalPalette != null && originalPalette.Entries.Length > 0 && originalPalette.Entries.Length <= maxColors)
                sourcePalette = originalPalette.Entries;

            // or, using default system palette
            if (sourcePalette == null)
            {
                using (Bitmap bmpReference = new Bitmap(1, 1, GetPixelFormat(bpp)))
                {
                    sourcePalette = bmpReference.Palette.Entries;
                }
            }

            // it is ignored if source has too few colors (rest of the entries will be black)
            transparentIndex = -1;
            bool hasBlack = false;
            int colorCount = Math.Min(maxColors, sourcePalette.Length);
            for (int i = 0; i < colorCount; i++)
            {
                targetPalette[i] = new RGBQUAD(sourcePalette[i]);
                if (transparentIndex == -1 && sourcePalette[i].A == 0)
                    transparentIndex = i;
                if (!hasBlack && (sourcePalette[i].ToArgb() & 0xFFFFFF) == 0)
                    hasBlack = true;
            }

            // if transparent index is 0, relocating it and setting transparent index to 1
            if (transparentIndex == 0)
            {
                targetPalette[0] = targetPalette[1];
                transparentIndex = 1;
            }
            // otherwise, setting the color of transparent index the same as the previous color, so it will not be used during the conversion
            else if (transparentIndex != -1)
            {
                targetPalette[transparentIndex] = targetPalette[transparentIndex - 1];
            }

            // if black color is not found in palette, counting 1 extra colors because it can be used in conversion
            if (colorCount < maxColors && !hasBlack)
                colorCount++;

            return colorCount;
        }

        private unsafe static void ToIndexedTransparentByArgb(Bitmap target, Bitmap source, int transparentIndex)
        {
            int sourceBpp = Image.GetPixelFormatSize(source.PixelFormat);
            int targetBpp = Image.GetPixelFormatSize(target.PixelFormat);

            BitmapData dataTarget = target.LockBits(new Rectangle(Point.Empty, target.Size), ImageLockMode.ReadWrite, target.PixelFormat);
            BitmapData dataSource = source.LockBits(new Rectangle(Point.Empty, source.Size), ImageLockMode.ReadOnly, source.PixelFormat);
            try
            {
                byte* lineSource = (byte*)dataSource.Scan0;
                byte* lineTarget = (byte*)dataTarget.Scan0;
                bool is32Bpp = sourceBpp == 32;

                // scanning through the lines
                for (int y = 0; y < dataSource.Height; y++)
                {
                    // scanning through the pixels within the line
                    for (int x = 0; x < dataSource.Width; x++)
                    {
                        // testing if pixel is transparent (applies both argb and pargb)
                        if (is32Bpp && ((uint*)lineSource)[x] >> 24 == 0
                            || !is32Bpp && ((ulong*)lineSource)[x] >> 48 == 0UL)
                        {
                            switch (targetBpp)
                            {
                                case 8:
                                    lineTarget[x] = (byte)transparentIndex;
                                    break;
                                case 4:
                                    // First pixel is the high nibble
                                    int pos = x >> 1;
                                    byte nibbles = lineTarget[pos];
                                    if ((x & 1) == 0)
                                    {
                                        nibbles &= 0x0F;
                                        nibbles |= (byte)(transparentIndex << 4);
                                    }
                                    else
                                    {
                                        nibbles &= 0xF0;
                                        nibbles |= (byte)transparentIndex;
                                    }

                                    lineTarget[pos] = nibbles;
                                    break;
                                case 1:
                                    // First pixel is MSB.
                                    pos = x >> 3;
                                    byte mask = (byte)(128 >> (x & 7));
                                    if (transparentIndex == 0)
                                        lineTarget[pos] &= (byte)~mask;
                                    else
                                        lineTarget[pos] |= mask;
                                    break;
                            }
                        }
                    }

                    lineSource += dataSource.Stride;
                    lineTarget += dataTarget.Stride;
                }
            }
            finally
            {
                target.UnlockBits(dataTarget);
                source.UnlockBits(dataSource);
            }
        }

        private static PixelFormat GetPixelFormat(int bpp)
        {
            switch (bpp)
            {
                case 1:
                    return PixelFormat.Format1bppIndexed;
                case 4:
                    return PixelFormat.Format4bppIndexed;
                case 8:
                    return PixelFormat.Format8bppIndexed;
                case 16:
                    return PixelFormat.Format16bppRgb565;
                case 24:
                    return PixelFormat.Format24bppRgb;
                case 32:
                    return PixelFormat.Format32bppArgb;
                case 48:
                    return PixelFormat.Format48bppRgb;
                case 64:
                    return PixelFormat.Format64bppArgb;
                default:
                    throw new ArgumentOutOfRangeException("bpp");
            }
        }

        private const int BI_RGB = 0;
        private const int DIB_RGB_COLORS = 0;

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BITMAPINFO pbmi, int iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [StructLayout(LayoutKind.Sequential)]
        private struct RGBQUAD
        {
            internal byte rgbBlue;
            internal byte rgbGreen;
            internal byte rgbRed;
            internal byte rgbReserved;

            internal RGBQUAD(Color color)
            {
                rgbRed = color.R;
                rgbGreen = color.G;
                rgbBlue = color.B;
                rgbReserved = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFO
        {
            public BITMAPINFOHEADER icHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public RGBQUAD[] icColors;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            internal uint biSize;
            internal int biWidth;
            internal int biHeight;
            internal ushort biPlanes;
            internal ushort biBitCount;
            internal uint biCompression;
            internal uint biSizeImage;
            internal int biXPelsPerMeter;
            internal int biYPelsPerMeter;
            internal uint biClrUsed;
            internal uint biClrImportant;
        }

        private void AokPatch_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(Directory.Exists(@"SlpImport\"))
            Directory.Delete(@"SlpImport\",true);
        }

        private void comboBox1024_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1280.Text!="Auto"&& comboBox1280.Text.Contains("x") && comboBox1024.Text.Contains("x")
                && int.Parse(comboBox1280.Text.Split('x').First())< int.Parse(comboBox1024.Text.Split('x').First())
                && int.Parse(comboBox1280.Text.Split('x').Last()) < int.Parse(comboBox1024.Text.Split('x').Last())
                )
            {
                MessageBox.Show("Midle resolution is more bigger than last " + Environment.NewLine + "You need chose inferior than the last one");
                comboBox1024.Text = "Auto";
            }
        }

        private void comboBox1280_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1024.Text != "Auto" && comboBox1024.Text.Contains("x") && comboBox1280.Text.Contains("x")
                && int.Parse(comboBox1280.Text.Split('x').First()) < int.Parse(comboBox1024.Text.Split('x').First())
                && int.Parse(comboBox1280.Text.Split('x').Last()) < int.Parse(comboBox1024.Text.Split('x').Last())
                )
            {
                MessageBox.Show("Midle resolution is more bigger than last " + Environment.NewLine + "You need chose inferior than the last one");
                comboBox1280.Text = "Auto";
            }
        }
        private Byte[] exe;
        public void Injection(UInt32 Addresse, string value)
        {
            string Value = string.Empty;
            string[] MybyteValue;
            UInt32 cpt = 0;
            //Byte[] aocExe = File.ReadAllBytes(GamePath);
            Byte[] aocExe = exe;
            if (aocExe.Length < 0x293040)
            {
                ExtendExeFile();
                //remap aoc byetes
                aocExe = File.ReadAllBytes(this.gameExe);
            }

            if (Addresse > 0x2FFFFF)
            {
                if (Addresse < 0x7A5000)
                {
                    Addresse -= 0x400000;
                }
                else
                {
                    Addresse -= 0x512000;
                }
            }


            //if (Addresse < 0x2FFFFF)
            //{
            Value = Regex.Replace(value, ".{2}", "$0 ");

            MybyteValue = Value.Split(' ');
            foreach (var item in MybyteValue)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int val = int.Parse(item, System.Globalization.NumberStyles.HexNumber);
                    var b = aocExe[Addresse + cpt];
                    aocExe[Addresse + cpt] = (byte)val;
                    var bb = aocExe[Addresse + cpt];
                }
                cpt++;
            }
            cpt = 0;
            exe = aocExe;
            //File.WriteAllBytes(GamePath, aocExe);
            //}
        }

        public void ExtendExeFile()
        {
            UInt32 CurrentAddresse = 0;
            Byte[] aocExe = File.ReadAllBytes(this.gameExe);
            Byte[] NewExe = new byte[0x300000];
            //copy old in new 
            foreach (var bytes in aocExe)
            {
                if (CurrentAddresse < 0x29302D)
                {
                    NewExe[CurrentAddresse] = bytes;// aocExe[CurrentAddresse];
                    CurrentAddresse++;
                }
                else
                {
                    break;
                }
            }
            File.WriteAllBytes(this.gameExe, NewExe);
        }
        private bool isVersionChosed()
        {
            return radioButton__20.Checked || radioButton_20a.Checked || radioButton_20b.Checked || radioButton_20b.Checked || radioButton_10.Checked || radioButton_10c.Checked || radioButton_10e.Checked;
        }
        private void btnPatchMinMapColor_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.gameExe))
            {
                MessageBox.Show("Browser Game Exe !!");
                return;
            }
            if (!isVersionChosed())
            {
                MessageBox.Show("Chose a version!!");
                return;
            }
            if (radioButton__20.Checked)
            {

                #region mini-map color 2.0
                exe = File.ReadAllBytes(this.gameExe);
                #region empir2.exe extension PEHeader => .pdata .patch
                Injection(0x100, "504500004C010900");
                Injection(0x150, "00F03E00");
                Injection(0x310, "2E7064617461000000100000002038000010000000102700000000000000000000000000E00000E02E7061746368000000C006000030380000C0060000202700000000000000000000000000E00000E0");
                #endregion

                //import mini-map color aoc
                Injection(0x2BF000, "0000004D4F8B410885C0E97B7BDDFF000000008B513083EA0239D7E9847BDDFF9000008B413083E80239C5E9817BDDFF900000E81B68D7FF31C08B0DE8117900890504527A00685EC17C006A01E8217BD7FF83F8FF751B8B0DE8117900685EC17C006A00E80A7BD7FF83F8FF0F844CE7C3FF890504527A00E941E7C3FF000000000000E8CB67D7FF0FB64E04BA01000000D3E2231504527A00F7DA19D283E21001CA8A82C0C07C003CFF0F849361E1FF884620E98B61E1FF9000000000000000000000FF23FFFFFFFF0B53FFFFFFFFFF84FFFFFF22FFFFFF73FF53FFFFFFFFFF84FFFF8B96F80000008B4A4C8B5424308B0C918B96FC0000008B899C0000003B8A9C0000000F85CA6AC6FFD987E8010000E99F6AC6FF900000000000000000000000008A4D1C884F308B4C24108B5104E9D5CBC9FF00000000000000000000000000000FBE4F308B86F80000008B404C8B0C888B91600100008B4220E9256BC6FF4D696E692D6D617020436F6C6F727300000000");
                Injection(0x58D0F0, "E92E3024006690");
                Injection(0x5C3F12, "84C00F8429C22000C1E80884C07405A00ADD6600D9473C");
                Injection(0x2BF159, "909090");

                //mini-map 2.0 fix address
                Injection(0x2BF00A, "E91812C9FF");
                Injection(0x2BF01B, "E92112C9FF");
                Injection(0x2BF02B, "E91E12C9FF");
                Injection(0x2BF033, "E848E9CEFF");
                Injection(0x2BF046, "6861017D00");
                Injection(0x2BF04D, "E82EE0CEFF");
                Injection(0x2BF05D, "6861017D00");
                Injection(0x2BF064, "E817E0CEFF");
                Injection(0x2BF06C, "0F84893FE1FF");
                Injection(0x2BF078, "E97E3FE1FF");
                Injection(0x2BF083, "E8F8E8CEFF");
                Injection(0x2BF0A2, "8A82C3007D00");
                Injection(0x2BF0AA, "0F841732C5FF");
                Injection(0x2BF0B3, "E90F32C5FF");
                Injection(0x2BF105, "0F856A3DDFFF");
                Injection(0x2BF111, "E93F3DDFFF");
                Injection(0x2BF130, "E9C2CFDBFF");
                Injection(0x2BF15C, "E9C53DDFFF");

                Injection(0x4612E0, "C744242000000000DA742414D95C2418D9442418D80588C56100E815B719008B0DB0517A0083C103894C241C6666903BFD7E0AD9442418D9E0D95C24188B6C241485ED7E358B7C242481E7FF0000008B4C241CD94424205157E8D6B6190003C650E85AD01900D944242CD844242483C40C03F34DD95C242075D55F5E5D5BC214002075D15F5E5D5BC21400");
                Injection(0x461222, "E9DCED3600");
                Injection(0x46123C, "E9D2ED3600");
                Injection(0x461249, "E9D5ED3600");

                //big x
                string Xsize = string.Empty;
                if (numericUpDownSizeOfX.Value.ToString().Length == 1)
                {
                    Xsize = "0" + numericUpDownSizeOfX.Value.ToString();
                }
                else
                {
                    Xsize = numericUpDownSizeOfX.Value.ToString();
                }

                Injection(0x5C3F29, "6A" + Xsize);//08


                //fix crash
                Injection(0x5C5221, "668B919C0000008B486852E8CF94FCFFEB118B0DD43366006A00");
                //fix ebx
                Injection(0x5C3839, "8D8E7C0100008941FC31C038010F940174073841010F944101");

                //new aoc:00432F80 -> aok:005C41F0
                Injection(0x2BD000, "5355568BF18B4C2410570FBF86340100000FBFD12BC28B5424180FBFFA03C78BBE30010000663BCA8D04C08D7C87DC7C028BCAD94720D81D98C56100DFE0F6C40174100FBFC9894C2414DB442414D84F20EB0E0FBFD189542414DB442414D84F18E8AEE9E2FF668B178BBE2C0100000FBFCA6A008D0C49668B4C4F0266034E0C03C8668B8638010000660346108BF903C28B5424248BD82BFA2BDA8D2C1103C20FBFD38944241C895424180FBFC00FBFCD0FBFD7508B44241C4189542428518B4E20425052E80631C9FF0FBF4424188B4C241C8B54241451508B4E200FBFC5508B44242C5250E8E530C9FF0FBF4424180FBFFF0FBFED0FBFDB6A008D4F0150518B4E208D5501535289442428E8BF30C9FF8B44241C8B4C241450518B4E20575355E8AA30C9FF5F5E5D5BC21000");
                //connection aok
                Injection(0x5C3F3D, "E8BEA02000");


                //Injection(0x2BF143, "0FBE4F308B86F80000008B404C8B4424109090909090909090E9C53DDFFF");
                Injection(0x2BF143, "0FBE4F308B86F80000008B404C8B0C888B91580100008B4210E9C53DDFFF");


                File.WriteAllBytes(this.gameExe, exe);
                MessageBox.Show("Done.");
                #endregion mini-map color 2.0
            }
            if(radioButton_10c.Checked)
            {
                labelDescSIgnal.Visible = false;
                numericUpDownSizeOfX.Visible = false;
                #region mini map color 1.0C
                // Fix mini-map visibility bugs
                exe = File.ReadAllBytes(this.gameExe);
                Injection(0xA7B0, "028D7B785755E9CBE62100668B0783C4");
                Injection(0xBD70, "8B0A6A0DE8C7A014005F5E5D5B83C410");
                Injection(0xBDE0, "C4048B086A0D");
                Injection(0x324B0, "CB81F9A600000072198D8E7C0100008941FC31C038010F940174073841010F9441018B4E206A0157E82302170085C00F");
                Injection(0x328B0, "0084C074328A867C01000084C074288B");
                Injection(0x32910, "0100008A867C01000084C00F84D80100");
                Injection(0x32BA0, "75308B87E001000085C07426E985631F00906A0368FF000000E8260E1E00D987");
                Injection(0x32C60, "0866817A10120175368B867C01000084C00F841F631F00C1E80884C07405A00AEE6600D9473C6A0650E8560D1E00D947");
                Injection(0x32EB0, "004283F8048996B401000075358A867C01000084");
                Injection(0x32EF0, "EB2583F86475208A867C01000084C074");
                Injection(0x33600, "BC02000075628A877C0100006A0084C0");
                Injection(0x33640, "83B83C03000009752F8A877C0100006A");
                Injection(0x33650, "0084C074158B44241C8BCF505056E81D");
                Injection(0x48600, "14020000C20400B8010000005E5B81C414020000C204006690C644241144C644");
                Injection(0x68D00, "E971021C0066900FBF82940000003BF0");
                Injection(0x1A3B80, "1400E9CF520800894424180F84290100008B41283BF87D048BF8EB09E9C55208007E028BFA3BE87C07E9C85208007E02");
                Injection(0x1A3c50, "D9442418D80578596300E885FD06008B0DB032790083C103894C241C6666903B");
                Injection(0x1e2230, "1C06F6FF8D4E206A045157E9966C0400");
                Injection(0x1eb280, "8B44240453B30166908A9408BD090000");
                Injection(0x1eb2D0, "8B5424048A44240888840ABD090000C2");
                Injection(0x228E50, "0000000000004D4F8B410885C0E925ADF7FF000000008B513083EA0239D7E92EADF7FF9000008B413083E80239C5E92BADF7FF900000E8C599F1FF31C08B0DE8117900A3003379009068B48F62006A01E8CBACF1FF83F8FF751B8B0DE811790068B48F62006A00E8B4ACF1FF83F8FF0F84F618DEFFA30033790090E9EB18DEFF000000000000E87599F1FF0FB64E04BA01000000D3E2231500337900F7DA19D283E21001CA8A82168F62003CFF0F843D93FBFF884620E93593FBFF9000000000000000000000FF23FFFFFFFF0B53FFFFFFFFFF84FFFFFF22FFFFFF73FF53FFFFFFFFFF84FFFF8B96F80000008B4A4C8B5424308B0C918B96FC0000008B899C0000003B8A9C0000000F85749CE0FFD987E8010000E9499CE0FF900000000000000000000000008A4D1C884F308B4C24108B5104E97FFDE3FF00000000000000000000000000000FBE4F308B86F80000008B404C8B0C888B91600100008B4220E9CF9CE0FF4D696E692D6D617020436F6C6F721300000000000000000000000000");
                File.WriteAllBytes(this.gameExe, exe);
                MessageBox.Show("Done.");
                #endregion mini map color 1.0C
            }
        }

        private void btnSetChatColor_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.gameExe))
            {
                MessageBox.Show("Browser Game Exe !!");
                return;
            }
            if (!isVersionChosed())
            {
                MessageBox.Show("Chose a version!!");
                return;
            }
            if (radioButton__20.Checked)
            {
                #region chat color aok 2.0
                exe = File.ReadAllBytes(this.gameExe);

                //aok extension exe
                Injection(0x100, "504500004C010900");
                Injection(0x150, "00F03E00");
                Injection(0x310, "2E7064617461000000100000002038000010000000102700000000000000000000000000E00000E02E7061746368000000C006000030380000C0060000202700000000000000000000000000E00000E0");

                //chat color
                Injection(0x2BF203, "009090909090909090909090909090909090909090909090909090A30020780090890D10207800891520207800891D30207800892540207800892D50207800893560207800893D702078000FB601908B15C44566008B8A1C0400008B514C83F801742D83F802742883F803742383F804741E83F805741983F806741483F807740F83F808740A9090909090EB4E9090908B04828B88580100008B41108B0D102078008B15202078008B1D302078008B25402078008B2D502078008B35602078008B3D702078003DF2000000744C50A100207800E9EFC7D0FF9090908B0D102078008B15202078008B1D302078008B25402078008B2D502078008B35602078008B3D7020780068FF000000A100207800E9B3C7D0FF0000000090B815000000EBAD90909090909090909090");

                //leave loop when 8
                //cmp EBX,20 get postion mov to eax
                //Injection(0x2BF198, "90909090833D50237800000F84AB000000BE000000000FB63C3189BE002578004683FE20740583FF3A75EBC786FF24780000000000904683FE20C786002578000000000075F090BC00000000BB00000000BB00000000BC0023780083C430BA00257800BE000000000FB63C323E0FB62C34464383FB20740A3BEF75DF74EA90909090EB389090A30020780090890D10207800891520207800891D30207800892540207800892D50207800893560207800893D7020780090E947FFFFFF9090909090909090909090909083F801742D83F802742883F803742383F804741E83F805741983F806741483F807740F83F808740A9090909090EB4E9090908B04828B88580100008B41108B0D102078008B15202078008B1D302078008B25402078008B2D502078008B35602078008B3D702078003DF2000000744C50A100207800E9EFC7D0FF9090908B0D102078008B15202078008B1D302078008B25402078008B2D502078008B35602078008B3D7020780068FF000000A100207800E9B3C7D0FF0000000090B815000000EBAD90909090909090909090");
                //Injection(0x2BF186, "908BC483C020E9C30000009090909090909090909090833D50237800000F84AB000000BE000000000FB63C3189BE002578004683FE20740583FF3A75EBC786FF24780000000000904683FE20C786002578000000000075F090BC00000000BB00000000B900000000BC0023780083C430BA00257800BE000000000FB63C323E0FB62C34464383FB200F8473FFFFFF3BFD4183F9087438EB36A30020780090890D10207800891520207800891D30207800892540207800892D50207800893560207800893D7020780090E947FFFFFF9090909090909090909090909083F801742D83F802742883F803742383F804741E83F805741983F806741483F807740F83F808740A9090909090EB4E9090908B04828B88580100008B41108B0D102078008B15202078008B1D302078008B25402078008B2D502078008B35602078008B3D702078003DF2000000744C50A100207800E9EFC7D0FF9090908B0D102078008B15202078008B1D302078008B25402078008B2D502078008B35602078008B3D7020780068FF000000A100207800E9B3C7D0FF0000000090B815000000EBAD90909090909090909090");

                //player positions
                Injection(0x2BF700, "90891520217800891D30217800892540217800892D50217800893560217800893D70217800A300217800890D102078008B81C8120000BB000000008BC881C180000000433BD375F590909090909090909090909090909090900FB639903BFA744B90909090BC000000003E0FB63C0C3E89BC24902078004483FF0075ED90909090909090909090909090BD90207800891190BC000000003E0FB6BC24902078003E897C0C014483FF0075EC909090908B15202178008B1D302178008B25402178008B2D502178008B35602178008B3D70217800A1002178008B0D1020780033C0668B81D8120000E991D3C5FF90");

                Injection(0x42DB74, "9090E9852B3A009090");

                Injection(0x4DCA3B, "E904392F00");


                Injection(0x2BF343, "90A30020780090890D10207800891520207800891D30207800892540207800892D50207800893560207800893D702078000FB601908B15C44566008B8A1C0400008B514C83F801742D83F802742883F803742383F804741E83F805741983F806741483F807740F83F808740A9090909090EB4E9090908B04828B88580100008B41108B0D102078008B15202078008B1D302078008B25402078008B2D502078008B35602078008B3D702078003DF2000000745450A100207800E93FC6D0FF9090908B0D102078008B15202078008B1D302078008B25402078008B2D502078008B35602078008B3D7020780068FF000000A100207800E903C6D0FF00000000000000000000000090B815000000EBA59090909090909090");
                Injection(0x4DCAC5, "E954372F00");
                //Fix player name F4
                Injection(0x4E0857, "E994FF2E009090");
                Injection(0x2BF7F0, "A3002478000FB60083F801743783F802743283F803742D83F804742883F805742383F806741E83F807741983F808741490A1002478008D8C245C010000E92C00D1FF9090A1002478008D8C245B010000E91900D1FF");

                //fix name chat
                Injection(0x4DCACA, "E991392F00909090");
                //Injection(0x2BF45E, "9090890D10247800A3202478000FB60183F801743F83F802743A83F803743583F804743083F805742B83F806742683F807742190909090908B0D10247800A120247800518B8C9378110000E924C6D0FF90909090C601209090909090908B0D10247800A120247800518B8C937811000090E9FEC5D0FF9090909090909090909090");
                Injection(0x2BF45E, "9090890D10247800A3202478000FB60183F801743F83F802743A83F803743583F804743083F805742B83F806742683F807742183F808741C8B0D10247800A120247800518B8C9378110000E924C6D0FF90909090C601209090909090908B0D10247800A120247800518B8C937811000090E9FEC5D0FF909090909090909090909090");

                //fix name chatHisto
                Injection(0x4DCA40, "E9943A2F0090");
                Injection(0x2BF4D9, "9090909090909090909090909090909090909090909090909090909090909090890D30247800A3402478000FB60183F801743683F802743183F803742C83F804742783F805742283F806741D83F8077418908B0D30247800A140247800518B4D006A03E905C5D0FF90C601208B0D30247800A14024780090518B4D006A03E9EAC4D0FF9090");


                File.WriteAllBytes(this.gameExe, exe);
                MessageBox.Show("Done.");
                #endregion chat color aok 2.0
            }
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.gameExe))
            {
                MessageBox.Show("Browser Game Exe !!");
                return;
            }
            if (!isVersionChosed())
            {
                MessageBox.Show("Chose a version!!");
                return;
            }
            if (radioButton__20.Checked)
            {
                #region Add civ on aok 2.0
                exe = File.ReadAllBytes(this.gameExe);
                //.patch .data
                Injection(0x100, "504500004C010900");
                Injection(0x150, "00F03E00");
                Injection(0x310, "2E7064617461000000100000002038000010000000102700000000000000000000000000E00000E02E7061746368000000C006000030380000C0060000202700000000000000000000000000E00000E0");


                //in aok D civ = 13 civ
                int nbToAdd = 13 + (int)numericUpDown_nb_2_add.Value;
                string ciVAdd = nbToAdd.ToString("X").PadLeft(2, '0');
                //83F80F
                Injection(0x4F501A, "83F8" + ciVAdd);//0x586bb7
                                                     //Injection(0x4F500A, "057EB80000");//0B87E new id in language.dll
                                                     //Injection(0x5B3081, "057EB80000");

                //fix civ name
                Injection(0x5B3079, "83F8" + ciVAdd);



                //write stream on .data
                Injection(0x271700, "73747265616D5C7370616E6973682E6D7033000000000000000000000000000073747265616D5C617A746563732E6D703300000000000000000000000000000073747265616D5C6D6179616E732E6D703300000000000000000000000000000073747265616D5C68756E732E6D7033000000000000000000000000000000000073747265616D5C6B6F7265616E732E6D7033000000000000000000000000000073747265616D5C6974616C69616E732E6D70330000000000000000000000000073747265616D5C696E6469616E732E6D7033000000000000000000000000000073747265616D5C696E6361732E6D70330000000000000000000000000000000073747265616D5C6D6167796172732E6D7033000000000000000000000000000073747265616D5C736C6176732E6D70330000000000000000000000000000000073747265616D5C706F72747567756573652E6D7033000000000000000000000073747265616D5C657468696F7069616E732E6D7033000000000000000000000073747265616D5C6D616C69616E732E6D7033000000000000000000000000000073747265616D5C626572626572732E6D7033000000000000000000000000000073747265616D5C6B686D65722E6D70330000000000000000000000000000000073747265616D5C6D616C61792E6D70330000000000000000000000000000000073747265616D5C6275726D6573652E6D7033000000000000000000000000000073747265616D5C766965746E616D6573652E6D70330000000000000000000000");

                //.patch jmp new switch case
                //Injection(0x2BFA00, "3EFF248DF0674F008D542408680027780052E9FB5CD2FF8D542408682027780052E9EC5CD2FF8D542408684027780052E9DD5CD2FF8D542408686027780052E9CE5CD2FF8D542408688027780052E9BF5CD2FF8D54240868A027780052E9B05CD2FF8D54240868C027780052E9A15CD2FF8D54240868E027780052E9925CD2FF8D542408680028780052E9835CD2FF8D542408682028780052E9745CD2FF8D542408684028780052E9655CD2FF8D542408686028780052E9565CD2FF8D542408688028780052E9475CD2FF8D54240868A028780052E9385CD2FF8D54240868C028780052E9295CD2FF8D54240868E028780052E91A5CD2FF8D542408680029780052E90B5CD2FF8D542408682029780052E9FC5BD2FF5F33C05E81C400040000E9C85CD2FF90");
                Injection(0x2BFA00, "3EFF2485FC0A7D008D542408680027780052E9FB5CD2FF8D542408682027780052E9EC5CD2FF8D542408684027780052E9DD5CD2FF8D542408686027780052E9CE5CD2FF8D542408688027780052E9BF5CD2FF8D54240868A027780052E9B05CD2FF8D54240868C027780052E9A15CD2FF8D54240868E027780052E9925CD2FF8D542408680028780052E9835CD2FF8D542408682028780052E9745CD2FF8D542408684028780052E9655CD2FF8D542408686028780052E9565CD2FF8D542408688028780052E9475CD2FF8D54240868A028780052E9385CD2FF8D54240868C028780052E9295CD2FF8D54240868E028780052E91A5CD2FF8D542408688C49650052E90B5CD2FF8D542408680029780052E9FC5BD2FF8D542408682029780052E9ED5BD2FF90");
                //fix switch case 
                //JMP DWORD PTR DS:[EAX*4+7D0AFC]
                Injection(0x2BFA00, "3EFF2485FC0A7D00");


                //jmp of 2nd switch case
                Injection(0x2BFB30, "080A7D00170A7D00260A7D00350A7D00440A7D00530A7D00620A7D00710A7D00800A7D008F0A7D009E0A7D00AD0A7D00BC0A7D00CB0A7D00DA0A7D00E90A7D00F80A7D00070B7D00160B7D00");
                //Injection(0x2BFB30, "080A7D00");
                //Injection(0x2BFB34, "170A7D00");
                //Injection(0x2BFB38, "260A7D00");
                //Injection(0x2BFB42, "350A7D00");
                //Injection(0x2BFB46, "440A7D00");
                //Injection(0x2BFB50, "530A7D00");
                //Injection(0x2BFB54, "620A7D00");
                //Injection(0x2BFB58, "710A7D00");
                //Injection(0x2BFB62, "800A7D00");
                //Injection(0x2BFB66, "8F0A7D00");
                //Injection(0x2BFB70, "9E0A7D00");
                //Injection(0x2BFB74, "AD0A7D00");
                //Injection(0x2BFB78, "BC0A7D00");
                //Injection(0x2BFB82, "CB0A7D00");
                //Injection(0x2BFB86, "DA0A7D00");
                //Injection(0x2BFB90, "E90A7D00");
                //Injection(0x2BFB94, "F80A7D00");
                //Injection(0x2BFB94, "070B7D00");
                //Injection(0x2BFB94, "160B7D00");


                int nbCivStream = 13 + (int)numericUpDown_nb_2_add.Value - 1;
                string ciVStream = nbCivStream.ToString("X").PadLeft(2, '0');

                Injection(0x4F6665, "83F8" + ciVStream);
                Injection(0x4F6668, "0F8492A32D00");

                //jmp to an other switch case for stream civ sound
                Injection(0x4F6828, "000A7D00");

                //fix indian
                //Injection(0x2BFA62, "9090909068C027780090E99C5CD2FF");

                //fix burmerse 
                Injection(0x4F6670, "E92BA32D0090");
                //Injection(0x2BF9A0, "83FA03740C8A882C684F00E9C75CD2FF90EB4E");
                Injection(0x2BF9A0, "83FA03740C8A882C684F00E9C55CD2FF90EB4E");

                //27F6
                //Injection(0x4F5030, "687EB80000");//0B87E
                //Injection(0x4F5030, "687EB80000");//0B87E
                //Injection(0x4F41DB, "057EB80000");//0B87E
                //Injection(0x4FF2AA, "687EB80000");//0B87E


                //interface draw
                //51fbaf
                ////--Injection(0x11fbaf, "8B46188BBE281700003D000400007E4B31C08D4C24108A835D0100005068DC9C670051E854480F0031D283C40C8A935D0100008BFA8B1594507A00A198507A008D0C428D410E01CF39C70F8E97000000033D9C507A00E98C0000003D000300007C3731C08D4C24108A835D0100005068CC9C670051E802480F0031D283C40C8A935D0100008BFA8B1594507A00A198507A008D0C0201CFEB4E31C08D4C24108A835D0100005068BC9C670051E8CB470F0031D283C40C8A935D0100008BFA033D94507A00EB216690");
                //4DF4B1
                //(51101..51131) 800x600 (51141..51172)1024x768 (51181..51212)
                //0C7CE‬ = 51150 51170 add 20 0C7E2‬
                //Injection(0x4DF508, "81C7D8C70000");
                //0C7C4 = 51140‬ 51180 add 40 0C7D8
                Injection(0x4DF548, "81C768150000");//51180   1568‬‬
                                                    //0C7B0 = 51120‬ 51140 add 20 0C7C4
                Injection(0x4DF585, "81C740150000");//5440   1540‬
                                                    //0C79C = 51100‬ 51100 5400 
                Injection(0x4DF5B2, "81C718150000");
                //(51101..51131) 800x600 (51141..51172)1024x768 (51181..51212)
                //(5401..5431) 800x600 (5441..5472)1024x768 (5481..5512)


                //fix burmese
                //Injection(0x4DF5B2, "81C718150000");

                //maybe random
                //005E12D4 |.E8 6CB70100 CALL Empires2.005FCA45
                //005E12D9 |. 0FBFD7 MOVSX EDX,DI
                //005E12DC |. 8BC8 MOV ECX,EAX
                //005E12DE |.B8 03000180    MOV EAX,80010003
                //005E12E3 |. 0FAFCA IMUL ECX,EDX
                //005E12E6 |.F7E9           IMUL ECX
                //005E12E8 |. 03D1           ADD EDX, ECX
                //005E12EA |.C1FA 0E        SAR EDX,0E
                //005E12ED |. 8BC2 MOV EAX,EDX
                //005E12EF |.C1E8 1F        SHR EAX,1F
                //005E12F2 |. 03D0           ADD EDX, EAX
                //005E12F4 |. 66:3BD7 CMP DX,DI

                //fix random civ
                //Injection(0x4F87F9, "B91F000000");
                Injection(0x4F87F9, "B9" + (nbCivStream - 2).ToString("X").PadLeft(2, '0') + "000000");
                File.WriteAllBytes(this.gameExe, exe);

                #endregion Add civ on aok 2.0
                //copy sound stream
                string gamestream_ = Path.GetDirectoryName(this.gameExe);
                gamestream_ = gamestream_.ToLower().Contains("age2_x1") ? gamestream_.ToLower().Replace("age2_x1", @"\Sound") : gamestream_ + @"\Sound";
                var currentDir = Directory.GetCurrentDirectory();
                if(Directory.Exists("Stream"))
                {
                    foreach(var f in Directory.GetFiles("Stream"))
                    {
                        var fileSource = Path.Combine(currentDir, f);
                        var fileTarget = Path.Combine(gamestream_, f);
                        if(!File.Exists(fileTarget))
                        File.Copy(fileSource, fileTarget);
                    }
                }
                MessageBox.Show("Done.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.gameExe))
            {
                MessageBox.Show("Browser Game Exe !!");
                return;
            }
            if (!isVersionChosed())
            {
                MessageBox.Show("Chose a version!!");
                return;
            }
            exe = File.ReadAllBytes(this.gameExe);

            if (radioButton_10.Checked || radioButton_10c.Checked || radioButton_10e.Checked)
            {


                FileInfo fileInf = new FileInfo(this.gameExe);
                var d = fileInf.Directory;
                if (!File.Exists(d + @"\wndmode.dll"))
                    File.WriteAllBytes(fileInf.Directory.ToString() + @"\wndmode.dll", Resource1.wndmode);
                //File.Copy(Properties.Resources.wndmode, fileInf.Directory.ToString() + @"\wndmode.dll");
            }
            if (radioButton__20.Checked)
            {
                FileInfo fileInf = new FileInfo(this.gameExe);
                var d = fileInf.Directory;
                if (!Directory.Exists(d + @"\Age2_x1"))
                {
                    Directory.CreateDirectory(d + @"\Age2_x1");
                }
                if (!File.Exists(d + @"\Age2_x1\wndmode.dll"))
                {
                    //File.WriteAllBytes(fileInf.Directory.ToString() + @"\wndmode.dll", Properties.Resources.wndmode);
                    File.WriteAllBytes(d + @"\Age2_x1\wndmode.dll", Resource1.wndmode);
                }
            }
            //File.Copy(Properties.Resources.wndmode, fileInf.Directory.ToString() + @"\wndmode.dll");

            if (radioButton_10.Checked)
            {
                #region windowed 1.0
                //.pdata
                //.patch
                Injection(0x100, "504500004C01090004FC58390000000000000000E0000F010B010600003023000000170000000000E16A210000100000004023000000400000100000001000000400000000000000040000000000000000F04000001000000000000002000000");
                Injection(0x310, "2E706461746100000010000000403A000010000000202900000000000000000000000000E00000E02E7061746368000000A0060000503A0000A0060000302900000000000000000000000000E00000E0");

                //windoowed mod
                //0x7C2800
                Injection(0x2b0800, "8B8C24D81D000068FC2D7C00E81F05000031D284C0750D68802D7C00FF15E05163008BD0E9A54BDCFF90");
                Injection(0x481747, "E9B4103400");
                Injection(0x2b0DFC, "4E4F574E44");
                Injection(0x2b0D30, "81EC0001000068FF0000008BC18D4C24045051E81820E5FF83C40C8D1424C68424FF0000000052FF153C5363008B8424040100008D0C245051E8F220E5FF85C00F95C081C408010000C20400");
                Injection(0x2b0D80, "616765325F78315C776E646D6F64652E646C6C00");
                //Injection(0x2b0824, "E99343DCFF");
                Injection(0x2b0824, "E923EFCBFF90");
                //lib A         [6341E0]
                Injection(0x2b081C, "FF15E0416300");
                //CharUpperA    [634338]
                Injection(0x2b0D57, "FF1538436300");
                //00613360
                Injection(0x2b0D43, "E81806E5FF");
                //006134A0
                Injection(0x2b0D69, "E83207E5FF");

                //fix cursor
                Injection(0x42093A, "6690");
                Injection(0x5f23c8, "6690");
                Injection(0x4915FD, "6690");

                //fix entry point
                //00616AE1
                Injection(0x2b07F0, "83FF00750BBFE16A6100909090909090");
                //JMP 007C27F0
                Injection(0x481747, "E9A4103400");
                #endregion windowed 1.0

            }
            if (radioButton_10c.Checked || radioButton_10e.Checked)
            {
                #region windowed 1.0C
                //.pdata .patch
                Injection(0x48656, "03EB0D5EB001");
                Injection(0x48695, "EB");
                Injection(0x486b9, "33C0B001");
                Injection(0xff240, "EB");
                Injection(0x1eb2d4, "B0099090");
                Injection(0x228ffd, "05431F");
                Injection(0x234ffd, "05431F");
                Injection(0x28104b, "63");
                Injection(0x283ffd, "05431F");
                Injection(0x28effd, "05431F");
                Injection(0x2b0e24, "00000000000000000000");
                Injection(0x26b17f, "20392E33");
                Injection(0x280a18, "20");
                Injection(0xfe, "09");
                Injection(0x149, "2041");
                Injection(0x308, "2E706461746100000010000000503A000010000000302900000000000000000000000000400000C02E7061746368000000C0060000603A0000C006000040290000000000000000000000000020000060");
                Injection(0x48607, "B8010000005E5B81C414020000C204006690");
                Injection(0x1eb285, "B3016690");//oak(421BB0)
                Injection(0x1eb2d4, "8A442408");
                Injection(0x280a18, "04");

                // Create game extensions
                //Injection(0x2bc000, "57538B3D80507A00A190507A0069CF900000008D1C088D431057508BCEE80E95E2FF81EB900000004F75EB5B5FE92650CDFF85C90F841144CDFF8B1590507A0069C9900000000FBF4411060FBF74110C0FBF7C11080FBF5C110A0FBF6C110EE9E743CDFF85C90F846145CDFF8B1590507A0069C9900000008D54113052E9CE45CDFF813BC00100000F8508EDD6FF510FBE885D01000085C9741969C9900000008B1590507A00668B541104668913C64424170159E9DDECD6FF3B2D80507A000F83C861D4FF45A180507A008BC869C9900000008B1590507A008B141139EA74034875E88B1590507A008D441150508D44241C50E94D61D4FF3B0580507A000F870229D3FF69C0900000008B1590507A008D44105050E9F128D3FF3B0580507A000F870BEDD2FF69C0900000008B1590507A008D44105050E9FAECD2FF85C00F84583FDEFF488B0D88507A0001C8508DBE40140000680008000057508BCEE84E4BD9FF588986E40E0000E9503FDEFF8B44241483F8017C233B0580507A007F1B8B1155030584507A005650FF5228C64435FF008BC65F5E5DC214008B1584507A00428B01555652FF5028C64435FF008BC65F5E5DC2140057538B3584507A008BFE468BDE8BCE29F9518B4D0056E89F26D8FF468BD629DA3B1580507A007CE55B5FE92002D2FF8B4424108B0F50030584507A0050E87826D8FF8B442410403B0580507A00894424107EDC8B0F6A00E80E25D8FF8B0F6A1EFF3584507A00E9E9D4D2FF2B1D84507A004B8B1584507A008D043A01DA8A0C3A80F90175098B0E5750E82C26D8FF473B3D80507A007EDB8A44242B84C00F843395D3FF8B0E6A00E8BE24D8FF8B0E6A1EFF3584507A00E91695D3FF6A0A528B157C507A00895424108D94B647040000C1E20283FE0D0F8EAB90C7FF03158C507A00E9A090C7FF81EC040100008D0C248D851D030000506884B76700FF356C507A0068B4BD670051E86961E4FF83C4148D0C2451E8724DD7FF83C40484C074158D0C2451FF15E051630081C404010000E9D457E1FF81C4040100008D851D030000E9BC57E1FF81EC040100008D0C248D850C170000506884B76700FF356C507A0068B4BD670051E80A61E4FF83C4148D0C2451E8134DD7FF83C40484C074118D0C2451FFD681C404010000E97ACCC6FF81C4040100008D850C170000E966CCC6FF81EC040100008D0C2468FCF566006884B76700FF356C507A0068B4BD670051E8B160E4FF83C4148D0C2451E8BA4CD7FF83C40484C074118D0C2451FFD681C404010000E93DCCC6FF81C40401000068FCF56600E92BCCC6FF8BAC241C0100008DB3840000005556FF356C507A008D44242068B4BD670050E85960E4FF68000100008D4C242C6A0051E8B76BE4FF83C42083F8FF740E50E89374E4FF83C404E9EE9CD0FF8BAC241C010000E9CA9BD0FF568BF08D94248800000056FF356C507A006836EB7C0052E80A60E4FF8D94249800000068000100006A0052E8656BE4FF83C41C3DFFFFFFFF741250E83F74E4FF83C4045E83EC0CE93B5CD4FF8BC65E508D942488000000E9205CD4FF31C9894C241083CDFF8D4C2454576884B76700FF356C507A0068B4BD670051E8A65FE4FF83C4148D4C245451E8AE4BD7FF83C40484C0740C8B842468010000E9094FD7FF8B84246401000057508D4C245CE9D74ED7FF56E81E0000006A006880000000E97D48D7FF5050E80B000000586800800000E92AC8C3FF56575381EC080200008BB424180200008D0C2456FF356C507A00688CB1660051E82B5FE4FF83C4108D0C2451E8344BD7FF83C40484C0740D8D0C245156E80E5FE4FF83C40881C4080200005B5F5EC204008D942454030000A168507A00833800741E686AEB7C00FF3568507A00688CB1660052E8D85EE4FF83C408E9D1CFD2FF68008F670052E8C55EE4FFE9C1CFD2FF8B0DA01279008B512881C26C15000052E9B3FED1FF8B0DA01279008B512881C2440D000052E999FFD1FF807E013A75138D44240C6A005056FF1558516300E9AE00DFFF81EC040100008D04248B0DA01279008B512881C2440D00005652688CB1660050E8585EE4FF83C4108D14248D8424100100006A005052FF155851630081C404010000E96700DFFF568D8C2450110000E917D3C3FF8D84245C11000052E92AD3C3FF81C720040000F2AEFDB85C000000F2AEFC83C70231C083C9FFE96888C7FF55578B0DA01279008B792881C77B1800008D6C241468B8FD6600525768B4BD670055E8D75DE4FF83C4148D84241801000050E9F770C7FFE97070C7FFFF3568507A00E89E80E4FF83C4048BCFE8D2ACE1FFFF3554507A00E88980E4FF83C404E9019FC7FFFF3568507A00E87680E4FF83C4048B168BCEFF9290000000FF3554507A00E85E80E4FF83C404E923B1E1FFFF3568507A00E84B80E4FF83C4048B0DBC127900E8CB45D7FFFF3554507A00E83280E4FF83C404E9A9C5E1FF837C240405751E518B0DA01279008B412805490E00006808B7670050E8225DE4FF83C4085964A100000000E94E17D4FF8B44240483F80D742F83F805742A83F804740583F8137549518B0DA01279008B412805490E00006808B7670050E8E15CE4FF83C40859EB29518B0DA01279008B412805490E00006814B76700FF3568507A00688CB1660050E8B65CE4FF83C4105964A100000000E9E21FD4FF5657538B5C241053E8FF48D7FF83C40484C075228BFBB904010000B85C000000F2AE751231D28B37881753E80C0000008BD68817EBE05B5F5EC20400568B74240856E8C548D7FF83C40484C0750956E8F848D7FF83C4045EC20400895C241C8DB424580A00006884B7670056E8365CE4FF83C4088DB42476100000687CB7670056E8215CE4FF83C4088DB4247B110000686CB7670056E80C5CE4FF83C4088DB42480120000685CB7670056E8F75BE4FF83C4088DB4248A140000684CB7670056E8E25BE4FF83C4088DB4248F1500006884B7670056E8CD5BE4FF83C4088DB424991700006800B7670056E8B85BE4FF83C4088DB424710F00006808B7670056E8A35BE4FF83C4088DB4249E18000068F8B6670056E88E5BE4FF83C4088DB4245D0B000068E83C680056E8795BE4FF83C4086843EB7C00E88147D7FF83C40484C0752831C08B3568507A0089068A055C507A0084C0751FFF356C507A00FF3568507A00E8405BE4FF83C408FF3568507A00E88BFEFFFF8DB424620C00006830B76700FF3568507A00688CB1660056E8155BE4FF56E8A4FEFFFF83C4108DB424670D00006820B76700FF3568507A00688CB1660056E8EF5AE4FF56E87EFEFFFF83C4108DB4246C0E0000685DEB7C00FF3568507A00688CB1660056E8C95AE4FF56E858FEFFFF83C4108DB4246C0E00006814B76700FF3568507A00688CB1660056E8A35AE4FF56E832FEFFFF83C4108DB42485130000683CB76700FF356C507A00688CB1660056E87D5AE4FF56E80CFEFFFF83C4108D3500547A0068B40B6800FF3568507A00688CB1660056E8585AE4FF83C41031C08B3568507A00390674568DB424941600006879EB7C00FF3568507A00688CB1660056E82C5AE4FF56E8BBFDFFFF83C4108DB424A31900006884EB7C00FF3568507A00688CB1660056E8065AE4FF56E895FDFFFF83C4106830200000E9EE88DBFF8DB4249416000068F48B6700FF3568507A00688CB1660056E8D659E4FF56E865FDFFFF83C4108DB424A319000068C0FD6600FF3568507A00688CB1660056E8B059E4FF56E83FFDFFFF83C4106830200000E99888DBFF5252E846FAFFFF5A6800800000E9C806DCFF8B156C507A008DB1130100003A020F8407BEC9FF8B84242002000085F60F848FBEC9FF85C00F8487BEC9FF50568D84248C00000052688FEB7C0050E84B59E4FF8D8C24180100008D9424980000005152E81B71010083C41C83F8FF750C8B842420020000E9C9BDC9FF8B156C507A008D8424180100008D4C240450565268B4BD670051E80359E4FF83C4148B15A0127900E9EEBDC9FF2573486973746F72795C257300616765325F78315C736176652D7065722D757365722E7478740053637265656E73686F74735C005363726970742E41495C2A2E6169005363726970742E41495C005363726970742E524D5C0025732573252E32642A2E6D703300");


                // Create loader extensions
                Injection(0x2b0800, "8B8C24D81D000068FC2D7C00E81F05000031D284C0750D68802D7C00FF15E05163008BD0E9A54BDCFF90");
                Injection(0x586bb7, "E944BC2300");
                Injection(0x2b0DFC, "4E4F574E44");
                Injection(0x2b0D30, "81EC0001000068FF0000008BC18D4C24045051E81820E5FF83C40C8D1424C68424FF0000000052FF153C5363008B8424040100008D0C245051E8F220E5FF85C00F95C081C408010000C20400");
                Injection(0x2b0D80, "616765325F78315C776E646D6F64652E646C6C00");
                Injection(0x2b0824, "E99343DCFF");
                //Injection(0x5873c9, "E9B2B42300");

                // Extend windowed support
                //Injection(0x2b0000, "833D4C507A000175096A00FF158C536300C38B0DA01279008B81E001000083F80475288B416C85C074215683EC108D34248B48045651FF156053630085C0740756FF158C53630083C4105EC30000000031C03B86EC00000075333B86840000007F1A3B86C4000000740750FF152C5363008B86C4000000E91A53DBFF89864C0100008B86C4000000E90953DBFF89864C010000E97C55DBFF000000000000000031C03B86EC0000000F852F5BDBFF3B86840000000F8F235BDBFF3B86C40000000F84175BDBFF8B8630010000E9A758DBFF90000000000000000000000000000051E81AFFFFFF5964A100000000E964F3DDFF00000000000000000000000000003D32020000750AE8F4FEFFFFE96758E2FF3D18020000E97E5CE2FF90000000008B44242085C07505E8D3FEFFFF8B466C85C0E95B58E2FF900000000000000000528B460450FF15B853630085C074258B46046A0150FF15045363005AFF5254508B46046A0250FF15045363005885C0E95CF0DDFF5AFF525485C0E951F0DDFF906A00FF158C5363008A86CC140000E9740CC8FF900000000000000000000000008B4424048B91E00100008981E001000083F8047508E846FEFFFFC2040085C0751A813C243C9C520074118B15001279008B8A14160000E8B5E6E1FF6A00FF158C536300C20400000000000000000000008BCE8B166A00FF520C8B8624040000E93A0CC8FF000000000000000000000000E89BE7D9FF8B1550507A0085D274376A0181C2711A0100685B227C00526A0E83C245685C227C0052E8A3EE0200E89EEE02008B4E6C6A0868580200006820030000E8FAEEDDFFE9B335E2FFEBB820000000508B5510528B4D0C51");

                //Injection(0x577390, "E9BBAC2400");
                //Injection(0x577972, "E929A72400");
                //Injection(0x5a1450, "E98B0C2200");
                //Injection(0x5e7d94, "E967A31D00");
                //Injection(0x5e798d, "E98EA71D00");
                //Injection(0x5a11cb, "E9700F2200");
                //Injection(0x442e01, "E97AF33700");
                //Injection(0x1a1232, "03");
                //Injection(0x1779cc, "E9D701000090");
                //Injection(0x177a35, "E96E01000090");
                //Injection(0x177a7a, "E92901000066666690");
                //Injection(0x177b2e, "EB7866666690");
                Injection(0x1e752a, "6690");
                Injection(0x1ffe8, "6690");
                Injection(0x17703d, "6690");
                //Injection(0x177860, "8B410485C07502C39083EC10568B30");
                //Injection(0x2b20b2, "0F1F440000");
                //Injection(0x2b0000, "C3");
                //Injection(0x2b9876, "83C404666690");
                //Injection(0x2b01b5, "0F1F440000");
                //Injection(0x2b01dd, "83C404666690");
                //Injection(0x442e2e, "E9BDF33700");
                //Injection(0x236c5c, "A0217C00");
                //Injection(0x247fb8, "A0217C00");
                //Injection(0x293048, "01");
                //Injection(0x3ea68, "8B566C8B4A2C8B4230EB146690");

                //Injection(0x4751b, "0F1F44000031C9898EF4160000898EF81600008B4808898E04170000A150507A0085C0");

                //Injection(0x1b87e7, "EBCA0F1F440000");
                //Injection(0x269cd0, "436C6970437572736F7200");
                //Injection(0x11fbaf, "8B46188BBE281700003D000400007E4B31C08D4C24108A835D0100005068DC9C670051E854480F0031D283C40C8A935D0100008BFA8B1594507A00A198507A008D0C428D410E01CF39C70F8E97000000033D9C507A00E98C0000003D000300007C3731C08D4C24108A835D0100005068CC9C670051E802480F0031D283C40C8A935D0100008BFA8B1594507A00A198507A008D0C0201CFEB4E31C08D4C24108A835D0100005068BC9C670051E8CB470F0031D283C40C8A935D0100008BFA033D94507A00EB216690");

                // Enable window library support
                //0x2b0815 = 7C2815
                //Injection(0x2b0815, "75");

                //fix entry point
                //00617522
                Injection(0x2b07F2, "83FF007509BF2275610090909090");
                //JMP 007C27F2
                Injection(0x586bb7, "E936BC2300");
                #endregion windowed 1.0C
            }
            if (radioButton__20.Checked)
            {
                #region windowed 2.0
                //.pdata .patch
                Injection(0x100, "504500004C010900");
                Injection(0x150, "00F03E00");
                Injection(0x310, "2E7064617461000000100000002038000010000000102700000000000000000000000000E00000E02E7061746368000000C006000030380000C0060000202700000000000000000000000000E00000E0");




                //0x7C2800
                //Windowed  
                //[61C110]
                //[61C2D8]
                //007D0C00 
                //005FCB40
                //005Fcc80
                //47AE30
                //5FEB15
                //Injection(0x2BFC00, "53BB010000005653908D8424441A000090909090909068D20C7D00E8F5DEE2FF9090909090909090909090909090909090909090909053BB010000008B8C24D81D000068BB0C7D00E81C00000031D284C0750D68802D7C00FF1510C161008BD0E901A2CAFF0000000081EC0001000068FF0000008BC18D4C24045051E8BFBEE2FF83C40C8D1424C68424FF0000000052FF15D8C261008B8424040100008D0C245051E8D9BFE2FF85C00F95C081C408010000C204000000000000004E4F574E440000656D7069726573322E65786500000000656D70697265732E6578650000");
                Injection(0x2BFC00, "53BB010000005653908D8424441A000090909090909068D20C7D00E8F5DEE2FF9090909090909090909090909090909090909090909053BB010000008B8C24D81D000068BB0C7D00E81C00000031D284C0750D68E10C7D00FF1510C161008BD0E901A2CAFF0000000081EC0001000068FF0000008BC18D4C24045051E8BFBEE2FF83C40C8D1424C68424FF0000000052FF15D8C261008B8424040100008D0C245051E8D9BFE2FF85C00F95C081C408010000C204000000000000004E4F574E440000656D7069726573322E65786500000000656D70697265732E65786500000000616765325F78315C776E646D6F64652E0000");
                //jmp
                Injection(0x047AE2A, "E9D15D350090");
                Injection(0x2BFC60, "E9CBA1CAFF");
                //
                Injection(0x2BFCE1, "616765325F78315C776E646D6F64652E646C6C");


                Injection(0x2BFBE0, "8BCA81C1C62B0000EB7F");
                Injection(0x2BFC48, "E893FFFFFF");


                Injection(0x2BFB9F, "00B8881B0000E8C6D7E2FF53BB01000000E97BA2CAFF");
                Injection(0x2BFC60, "E93BFFFFFF");




                /*
                ////jmp
                ////Injection(0x047AE2A, "E9D15D350090");
                //Injection(0x2BFC60, "E9CBA1CAFF");
                ////
                //Injection(0x2BFCE1, "616765325F78315C776E646D6F64652E646C6C");


                //Injection(0x2BFBE0, "8BCA81C1C62B0000EB7F");
                //Injection(0x2BFC48, "E893FFFFFF");

                //Injection(0x047AEBA, "E9415D35009090");
                //Injection(0x2BFC00, "F3A4BF4C306500909090909090909090909090909090909090909090909090909090909090");
                //Injection(0x2BFC36, "909090909090");
                //Injection(0x2BFC48, "E81C000000");
                ////jmp 0x047AEC1
                //Injection(0x2BFC60, "E95CA2CAFF");
                */



                Injection(0x2cA0A0, "B8881B0000E8C632E2FF5355568D8424981C00005750686AB17D00E8553AE2FF83C40833DB83F8FF756D8D4C2418680401000051E85441E2FF83C40885C07457BF7DB17D0083C9FF33C0F2AEF7D1498D7C24188BD183C9FFF2AEF7D1498D42023BC87E338BC12BC28D7404188A440C173C5C75014E52687DB17D0056E89F4FE2FF83C40C85C0750F8D4C2418881E51E88C00000083C404BF88B17D0083C9FF33C0BD01000000F2AEF7D12BF9558BD18BF7BF70797800C1E902F3A58BCA83E103F3A40000000000000000656D70697265732E65786500000000000000005C616765325F783100000031303A32393A3035000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000558BEC81EC08010000FF7508FF1550B27D0085C0745E8D85F8FEFFFF506805010000FF15B4C1610085C074488A85F8FEFFFF3C5C74043C2F75083A85F9FEFFFF742E0FB6C050C645083DE85600000080650B00884509598D85F8FEFFFF508D450850C6450A3AFF15E8C1610085C0740433C0C9C3FF1588C1610050E8AD69E2FF5983C8FFC9C300000000000000000000F05D6D750000000000000000000000000000000000558BEC51538B5D0881FBFF00000076518BC3885D0BC1E80888450A0FB6C0F680C52B770004744F6A018D45FCFF359C2A77006A02508D450A6A02506800020000FF35C42C7700E83095E2FF83C42085C074240FB645FC0FB64DFDC1E00803C1EB178A83C52B770024203C2075090FB683B42A7700EB028BC35BC9C300000000000000000000000000000000");

                //fix 
                Injection(0x2cA0AD, "8D8424441A0000");

                //JMP 7DB0A0
                Injection(0x47AE20, "E97B023600");
                //jmp back to aok 47AE32
                Injection(0x2cA162, "E9CBFCC9FF90");

                //windowed add test
                Injection(0x2cA137, "E9005BFFFF");
                //7DB13C
                Injection(0x2BFC60, "E9005BFFFF");
                //CALL 007D0c69
                Injection(0x2BFC48, "E81C000000");


                //fix call CALL 007DB1CC
                //Injection(0x2BFC48, "E898000000");
                Injection(0x2BFC00, "909090909090909090909090909090909090909090909090909090909090909090909090909090909090909090909090909090909090909090909090");
                //call SetCurrentDirectoryA
                //CALL DWORD PTR DS:[782980]
                Injection(0x2cA1cc, "FF1580297800");
                //fix empires.exe to empires2.exe
                Injection(0x2cA168, "0000656D7069726573322E6578650000");
                //try to save registerand restore it 
                //Injection(0x2cA060, "36A30029780036890D102978003689152029780036891D302978003689254029780036892D502978003689356029780036893D70297800909090909090909090");
                Injection(0x2cA030, "A1B4C16100A380297800810580297800E016000036A100297800EB4D000000000000000000000000000000000000000036A30029780036890D102978003689152029780036891D302978003689254029780036892D502978003689356029780036893D70297800EB9790909090909090");
                //JMP 007DB060
                Injection(0x47AE20, "E93B023600");

                //
                Injection(0x2BFC3C, "8BC881E9F6A4030090909090");
                //jmp 007D0D00
                Injection(0x2BFC60, "E99B000000");

                //restore register
                //007D0D00 restore register
                Injection(0x2BFd00, "36A100297800368B0D10297800368B1520297800368B1D30297800368B2540297800368B2D50297800368B3560297800368B3D70297800B8881B0000E82FD6E2FF53BB010000005653E9E4A0CAFF");

                //fix mouse cursor 
                Injection(0x41f80A, "6690");


                Injection(0x48AD1D, "6690");

                Injection(0x41f80A, "6690");
                Injection(0x2BFC3C, "909090909090");
                //MOV EDI,DWORD PTR SS:[782970]
                //MOV EAx, DWORD PTR SS:[782970]
                //Injection(0x2BFC81, "E9DA00000090");
                //Injection(0x2BFD60, "368B3D7029780036A170297800E915FFFFFF");
                ////


                Injection(0x2cA000, "83FF007505BF8102600036893D70297800E980000000");
                Injection(0x2cA090, "E96BFFFFFF9090");




                #endregion windowed 2.0

            }
            File.WriteAllBytes(this.gameExe, exe);
            MessageBox.Show("Done.");
        }

        private void radioButton_20a_CheckedChanged(object sender, EventArgs e)
        {
            tabhelper.HideAllPages();
            tabhelper.ShowAllPages();
            tabhelper.HidePage(tabControlAokPatch.TabPages["MiniMapColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AokChatColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AddCivOnAok20"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["Windowed"]);
        }

        private void radioButton__20_CheckedChanged(object sender, EventArgs e)
        {
            tabhelper.HideAllPages();
            tabhelper.ShowAllPages();
            labelDescSIgnal.Visible = true;
            numericUpDownSizeOfX.Visible = true;
        }

        private void radioButton_20b_CheckedChanged(object sender, EventArgs e)
        {
            tabhelper.HideAllPages();
            tabhelper.ShowAllPages();
            tabhelper.HidePage(tabControlAokPatch.TabPages["MiniMapColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AokChatColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AddCivOnAok20"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["Windowed"]);
        }

        private void radioButton_10_CheckedChanged(object sender, EventArgs e)
        {
            tabhelper.HideAllPages();
            tabhelper.ShowAllPages();
            tabhelper.HidePage(tabControlAokPatch.TabPages["MiniMapColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AokChatColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AddCivOnAok20"]);
            //tabhelper.HidePage(tabControlAokPatch.TabPages["Windowed"]);
        }

        private void radioButton_10c_CheckedChanged(object sender, EventArgs e)
        {
            tabhelper.HideAllPages();
            tabhelper.ShowAllPages();
            //tabhelper.HidePage(tabControlAokPatch.TabPages["MiniMapColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AokChatColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AddCivOnAok20"]);
            //tabhelper.HidePage(tabControlAokPatch.TabPages["Windowed"]);
            labelDescSIgnal.Visible = false;
            numericUpDownSizeOfX.Visible = false;
        }

        private void radioButton_10e_CheckedChanged(object sender, EventArgs e)
        {
            tabhelper.HideAllPages();
            tabhelper.ShowAllPages();
            tabhelper.HidePage(tabControlAokPatch.TabPages["MiniMapColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AokChatColor"]);
            tabhelper.HidePage(tabControlAokPatch.TabPages["AddCivOnAok20"]);
            //tabhelper.HidePage(tabControlAokPatch.TabPages["Windowed"]);
        }



        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).AccessibleName);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }
        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(@"Mods\"))
            {
                MessageBox.Show("Can't restore, no Mods directory exist");
                return;
            }
            if (!Directory.Exists(@"SlpTerrainRecord\") && !Directory.Exists(@"SlpGraphicRecord\"))
            {
                MessageBox.Show("Can't restore, no SlpTerrainRecord and SlpGraphicRecord directory exist");
                return;
            }
            if(string.IsNullOrEmpty(this.gameExe))
            {
                MessageBox.Show("Browser game EXe!!");
                return;
            }
            Cursor.Current = Cursors.WaitCursor;
            List<string> lstTer = new List<string>();
            List<string> lstGra = new List<string>();
            if (Directory.Exists(@"SlpTerrainRecord\"))
            {
                lstTer.AddRange(Directory.GetFiles(@"SlpTerrainRecord\"));
            }
            if (Directory.Exists(@"SlpGraphicRecord\"))
            {
                lstGra.AddRange(Directory.GetFiles(@"SlpGraphicRecord\"));
            }
            //restore slp
            string gameData_ = Path.GetDirectoryName(this.gameExe);
            gameData_ = gameData_.ToLower().Contains("age2_x1") ? gameData_.ToLower().Replace("age2_x1", "data") : gameData_ + @"\data";
            var currentDir = Directory.GetCurrentDirectory();
            var lstTerWithFullPath = lstTer.Select(x => string.Format(@"{0}\{1}", currentDir, x)).ToList();
            var lstGraWithFullPath = lstGra.Select(x => string.Format(@"{0}\{1}", currentDir, x)).ToList();
            if (lstTerWithFullPath.Count > 0)
            {
                UpdateSlpInDrsFile(lstTerWithFullPath, gameData_ + @"\terrain.drs", gameData_ + @"\terrainTmp.drs", "ter");
                File.Copy(gameData_ + @"\terrainTmp.drs", gameData_ + @"\terrain.drs", true);
                File.Delete(gameData_ + @"\terrainTmp.drs");
            }
            if (lstGraWithFullPath.Count > 0)
            {
                UpdateSlpInDrsFile(lstGraWithFullPath, gameData_ + @"\Graphics.drs", gameData_ + @"\GraphicsTmp.drs", "gra");
                File.Copy(gameData_ + @"\GraphicsTmp.drs", gameData_ + @"\Graphics.drs", true);
                File.Delete(gameData_ + @"\GraphicsTmp.drs");
            }
            Cursor.Current = Cursors.Default;
            if (Directory.Exists(@"SlpTerrainRecord\"))
                Directory.Delete(@"SlpTerrainRecord\", true);
            if (Directory.Exists(@"SlpGraphicRecord\"))
                Directory.Delete(@"SlpGraphicRecord\", true);
            MessageBox.Show("Done.");
        }
        private void buttonAddMod_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.gameExe))
            {
                MessageBox.Show("Browser Game Directory !!");
                return;
            }
            string message ="Are you sure that you would like to save this Mods?"+Environment.NewLine+"A record of original slp exist if you continue you will eraze it, I advanvice to restore and apply the Mods to slit have old one." ;
            const string caption = "Form Closing";
            DialogResult result = DialogResult.Yes ;
            if (Directory.Exists(@"Mods\"))
            {
                //@"SlpTerrainRecord\  @"SlpGraphicRecord\"
                if(Directory.Exists(@"SlpTerrainRecord\") || Directory.Exists(@"SlpGraphicRecord\"))
                {
                    result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
                }
            }
            if (result == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;

                var listSelectedMods = new List<String>();
                if (checkBoxBlueBerrys.Checked)
                {
                    listSelectedMods.Add("blue-berries");
                }
                if (checkBoxEnchantedFarms.Checked)
                {
                    listSelectedMods.Add("enchanced-farms");
                }
                if (checkBoxEnhancedBlood.Checked)
                {
                    listSelectedMods.Add("enhanced-blood");
                }
                if (checkBoxFarms.Checked)
                {
                    listSelectedMods.Add("farms");
                }
                if (checkBoxGridTerrains.Checked)
                {
                    listSelectedMods.Add("grid-terrains");
                }
                if (checkBoxhdCliffs.Checked)
                {
                    listSelectedMods.Add("hd-cliffs");
                }
                if (checkBoxHdFire.Checked)
                {
                    listSelectedMods.Add("hd-fire-arrows");
                }
                if (checkBoxHdFire.Checked)
                {
                    listSelectedMods.Add("hdfirev2");
                }
                if (checkBoxHdmines.Checked)
                {
                    listSelectedMods.Add("hd-mines");
                }
                if (checkBoxRealTerrain.Checked)
                {
                    listSelectedMods.Add("RealTerrain");
                }
                if (checkBoxSmallWall.Checked)
                {
                    listSelectedMods.Add("short-walls");
                }
                if (checkBoxSmallTree.Checked)
                {
                    listSelectedMods.Add("small-trees");
                }
                if (checkBoxTerrainV2.Checked)
                {
                    listSelectedMods.Add("terrain-v2");
                }

                List<string> lstFile = new List<string>();
                foreach (var d in listSelectedMods)
                {
                    lstFile.AddRange(Directory.GetFiles(@"Mods\" + d));
                }
                var lstFileWithoutExt = lstFile.Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
                //sort in to list ter and gra
                List<string> lstTer = lstFileWithoutExt.Where(x => x.StartsWith("ter")).Select(x => x.Replace("ter", "")).ToList();
                List<string> lstGra = lstFileWithoutExt.Where(x => x.StartsWith("gra")).Select(x => x.Replace("gra", "")).ToList();
                //extract slp 
                string gameData_ = Path.GetDirectoryName(this.gameExe);
                gameData_ = gameData_.ToLower().Contains("age2_x1") ? gameData_.ToLower().Replace("age2_x1", "data") : gameData_ + @"\data";
                //extract slp to save if player want roollback ,he can
                ExtractSlpFromDrs(lstTer, gameData_ + @"\terrain.drs", @"SlpTerrainRecord\");
                ExtractSlpFromDrs(lstGra, gameData_ + @"\Graphics.drs", @"SlpGraphicRecord\");

                //update slp from selected mod
                var currentDir = Directory.GetCurrentDirectory();
                var lstTerWithFullPath = lstFile.Where(x => x.Contains("ter")).Select(x => string.Format(@"{0}\{1}", currentDir, x)).ToList();
                var lstGraWithFullPath = lstFile.Where(x => x.Contains("gra")).Select(x => string.Format(@"{0}\{1}", currentDir, x)).ToList();
                if (lstTerWithFullPath.Count > 0)
                {
                    UpdateSlpInDrsFile(lstTerWithFullPath, gameData_ + @"\terrain.drs", gameData_ + @"\terrainTmp.drs", "ter");
                    File.Copy(gameData_ + @"\terrainTmp.drs", gameData_ + @"\terrain.drs", true);
                    File.Delete(gameData_ + @"\terrainTmp.drs");
                }
                if (lstGraWithFullPath.Count > 0)
                {
                    UpdateSlpInDrsFile(lstGraWithFullPath, gameData_ + @"\Graphics.drs", gameData_ + @"\GraphicsTmp.drs", "gra");
                    File.Copy(gameData_ + @"\GraphicsTmp.drs", gameData_ + @"\Graphics.drs", true);
                    File.Delete(gameData_ + @"\GraphicsTmp.drs");
                }
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Done.");
            }
            else
                return;
        }

        private void UpdateSlpInDrsFile(List<string> slpToUpdate, string drsPathFile,string newDrsFile,string FilePrefix)
        {

            List<DrsItem> lstitems = new List<DrsItem>();
            DrsTable[] drsTableArray;
            List<DrsItem> lstItem = new List<DrsItem>();
            var idToPick = new List<uint>();
            //string newDrsName = Path.Combine(Path.Combine(_gameDirectory, "Data"), string.Format("{0:D4}{1:D4}.drs", DateTime.Now.Day, DateTime.Now.Month));
            var file = Path.GetFileName(newDrsFile);
            var GameDirectory = this.gamePath + @"\data\";
            using (FileStream fileStream1 = new FileStream(drsPathFile, FileMode.Open))//, FileSystemRights.ReadData, FileShare.Read, 1048576, FileOptions.SequentialScan))
            {
                #region DRS read 
                BinaryReader binaryReader = new BinaryReader(fileStream1);
                bool flag = false;
                while (true)
                {
                    byte num = binaryReader.ReadByte();
                    //binaryWriter.Write(num);
                    if (num == (byte)26)
                        flag = true;
                    else if (num > (byte)0 & flag)
                        break;
                }
                binaryReader.ReadBytes(3);
                binaryReader.ReadBytes(12);
                uint num1 = binaryReader.ReadUInt32();
                uint num2 = binaryReader.ReadUInt32();

                drsTableArray = new DrsTable[(int)num1];
                for (int index = 0; (long)index < (long)num1; ++index)
                    drsTableArray[index] = new DrsTable();
                foreach (DrsTable drsTable in drsTableArray)
                {
                    drsTable.Type = binaryReader.ReadUInt32();
                    drsTable.Start = binaryReader.ReadUInt32();
                    uint num3 = binaryReader.ReadUInt32();
                    DrsItem[] drsItemArray = new DrsItem[(int)num3];
                    for (int index = 0; (long)index < (long)num3; ++index)
                        drsItemArray[index] = new DrsItem();
                    drsTable.Items = (IEnumerable<DrsItem>)drsItemArray;
                }
                foreach (DrsTable drsTable in drsTableArray)
                {
                    Trace.Assert(fileStream1.Position == (long)drsTable.Start);
                    foreach (DrsItem drsItem in drsTable.Items)
                    {
                        drsItem.Id = binaryReader.ReadUInt32();
                        drsItem.Start = binaryReader.ReadUInt32();
                        drsItem.Size = binaryReader.ReadUInt32();
                    }
                }
                foreach (DrsItem drsItem in ((IEnumerable<DrsTable>)drsTableArray).SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>)(table => table.Items)))
                {
                    Trace.Assert(fileStream1.Position == (long)drsItem.Start);
                    drsItem.Data = binaryReader.ReadBytes((int)drsItem.Size);
                }
                //lstItem = drsTableArray.Where(w => w.Type == 1936486432).First().Items.ToList();
                binaryReader.Close();
                #endregion
            }

                using (FileStream fileStream1 = new FileStream(drsPathFile, FileMode.Open))//, FileSystemRights.ReadData, FileShare.Read, 1048576, FileOptions.SequentialScan))
            {

                BinaryReader binaryReader = new BinaryReader(fileStream1);
                using (FileStream fileStream2 = new FileStream(newDrsFile, FileMode.Create))//, FileSystemRights.Write, FileShare.None, 1048576, FileOptions.SequentialScan))
                {
                    bool flag = false;
                    BinaryWriter binaryWriter = new BinaryWriter(fileStream2);
                    while (true)
                    {
                        byte num = binaryReader.ReadByte();
                        binaryWriter.Write(num);
                        if (num == (byte)26)
                            flag = true;
                        else if (num > (byte)0 & flag)
                            break;
                    }
                    binaryWriter.Write(binaryReader.ReadBytes(3));
                    binaryWriter.Write(binaryReader.ReadBytes(12));
                    uint num1 = binaryReader.ReadUInt32();
                    binaryWriter.Write(num1);
                    uint num2 = binaryReader.ReadUInt32();
                    binaryWriter.Write(num2);
                    binaryReader.Close();
                    uint num4 = num2;
                    List<DrsTable> source =  new List<DrsTable>();// new List<DrsTable>(drsTableArray.Length);
                    uint id;
                    //update slp in list 
                    foreach (var s in slpToUpdate)
                    {
                        var data = File.ReadAllBytes(s);
                        id = uint.Parse(Path.GetFileNameWithoutExtension(s).Replace(FilePrefix, ""));
                        //update only slp typenot .way or .bina
                        drsTableArray.Where(z => z.Type == 1936486432).First().Items.Where(z => z.Id == id).First().Data = data;
                    }
                    //update possitions
                    foreach (DrsTable drsTable1 in drsTableArray)
                    {
                        List<DrsItem> drsItemList = new List<DrsItem>();
                        DrsTable drsTable2 = new DrsTable()
                        {
                            Start = drsTable1.Start,
                            Type = drsTable1.Type,
                            Items = (IEnumerable<DrsItem>)drsItemList
                        };
                        foreach (DrsItem drsItem1 in drsTable1.Items)
                        {
                            DrsItem drsItem2 = new DrsItem()
                            {
                                Id = drsItem1.Id,
                                Start = num4,
                                Data = drsItem1.Data
                            };
                            drsItem2.Size = (uint)drsItem2.Data.Length;
                            num4 += drsItem2.Size;
                            drsItemList.Add(drsItem2);
                        }
                        source.Add(drsTable2);
                    }

                    foreach (DrsTable drsTable in source)
                    {
                        binaryWriter.Write(drsTable.Type);
                        binaryWriter.Write(drsTable.Start);
                        binaryWriter.Write(drsTable.Items.Count<DrsItem>());
                    }
                    foreach (DrsTable drsTable in source)
                    {
                        foreach (DrsItem drsItem in drsTable.Items)
                        {
                            binaryWriter.Write(drsItem.Id);
                            binaryWriter.Write(drsItem.Start);
                            binaryWriter.Write(drsItem.Size);
                        }
                    }
 
                    foreach (DrsItem drsItem in source.SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>)(outTable => outTable.Items)))
                    {
                        binaryWriter.Write(drsItem.Data);
                    }
                    binaryWriter.Close();
                    fileStream2.Close();
                }
                fileStream1.Close();
            }
        }
        private void ExtractSlpFromDrs(List<string> slpToExtract,string drsPathFile,string dirImport)
        {
            List<DrsItem> lstitems = new List<DrsItem>();
            DrsTable[] drsTableArray;
            List<DrsItem> lstItem = new List<DrsItem>();
            var idToPick = slpToExtract.Select(x=> uint.Parse(x)).ToList();

            using (FileStream fileStream1 = new FileStream(drsPathFile, FileMode.Open))//, FileSystemRights.ReadData, FileShare.Read, 1048576, FileOptions.SequentialScan))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream1);
                bool flag = false;
                while (true)
                {
                    byte num = binaryReader.ReadByte();
                    if (num == (byte)26)
                        flag = true;
                    else if (num > (byte)0 & flag)
                        break;
                }
                binaryReader.ReadBytes(3);
                binaryReader.ReadBytes(12);
                uint num1 = binaryReader.ReadUInt32();
                uint num2 = binaryReader.ReadUInt32();

                drsTableArray = new DrsTable[(int)num1];
                for (int index = 0; (long)index < (long)num1; ++index)
                    drsTableArray[index] = new DrsTable();
                foreach (DrsTable drsTable in drsTableArray)
                {
                    drsTable.Type = binaryReader.ReadUInt32();
                    drsTable.Start = binaryReader.ReadUInt32();
                    uint num3 = binaryReader.ReadUInt32();
                    DrsItem[] drsItemArray = new DrsItem[(int)num3];
                    for (int index = 0; (long)index < (long)num3; ++index)
                        drsItemArray[index] = new DrsItem();
                    drsTable.Items = (IEnumerable<DrsItem>)drsItemArray;
                }
                foreach (DrsTable drsTable in drsTableArray)
                {
                    //Trace.Assert(fileStream1.Position == (long)drsTable.Start);
                    foreach (DrsItem drsItem in drsTable.Items)
                    {
                        drsItem.Id = binaryReader.ReadUInt32();
                        drsItem.Start = binaryReader.ReadUInt32();
                        drsItem.Size = binaryReader.ReadUInt32();
                    }
                }
                foreach (DrsItem drsItem in ((IEnumerable<DrsTable>)drsTableArray).SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>)(table => table.Items)))
                {
                    //Trace.Assert(fileStream1.Position == (long)drsItem.Start);
                    drsItem.Data = binaryReader.ReadBytes((int)drsItem.Size);
                }
                //where type is slp not .way or .bina
                lstItem = drsTableArray.Where(w => w.Type == 1936486432).First().Items.ToList();
                binaryReader.Close();

                //get drs int
                foreach (var item in lstItem)
                {
                    if (idToPick.Contains(item.Id))
                    {
                        lstitems.Add(item);
                    }
                }
                //creat a directory to exporte slp
                if (!Directory.Exists(dirImport))
                {
                    Directory.CreateDirectory(dirImport);
                }
                //extract slp for interface.drs
                foreach (DrsItem DrsItem in lstitems)
                {
                    File.WriteAllBytes(dirImport + DrsItem.Id + ".slp", DrsItem.Data);
                }
            }
        }

        private void checkBoxTerrainV2_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRealTerrain.Checked = false;
            checkBoxGridTerrains.Checked = false;
        }

        private void checkBoxRealTerrain_CheckedChanged(object sender, EventArgs e)
        {
            
            checkBoxTerrainV2.Checked = false;
            checkBoxGridTerrains.Checked = false;
        }

        private void checkBoxEnchantedFarms_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxFarms.Checked = false;
        }

        private void checkBoxFarms_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxEnchantedFarms.Checked = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.gameExe))
            {
                MessageBox.Show("Browser Game Exe !!");
                return;
            }
            if (!isVersionChosed())
            {
                MessageBox.Show("Chose a version!!");
                return;
            }
            //copy mgx fix 
            string gameSaveGameDir_ = Path.GetDirectoryName(this.gameExe);
            gameSaveGameDir_ = gameSaveGameDir_.ToLower().Contains("age2_x1") ? gameSaveGameDir_.ToLower().Replace("age2_x1", @"\SaveGame") : gameSaveGameDir_ + @"\SaveGame";
            var currentDir = Directory.GetCurrentDirectory();
            //var mgxFixDir = Path.Combine(currentDir, @"Mgx fix\");
            if (Directory.Exists(@"Mgx fix\"))
            {
                foreach (var f in Directory.GetFiles("Mgx fix"))
                {
                    var fileSource = Path.Combine(currentDir, f);
                    var fileTarget = Path.Combine(gameSaveGameDir_, f.Replace(@"Mgx fix\", ""));
                    if (!File.Exists(fileTarget))
                        File.Copy(fileSource, fileTarget);
                }
                System.Diagnostics.Process.Start(Path.Combine(gameSaveGameDir_, @"mgxfix.bat"));
            }

            MessageBox.Show("Done.");
        }

        private void checkBoxGridTerrains_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxRealTerrain.Checked = false;
            checkBoxTerrainV2.Checked = false;
        }

        private void tabControlAokPatch_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(((TabControl)sender).SelectedTab!=null &&((TabControl)sender).SelectedTab.Name =="DrsEditor")
            {
                if (!string.IsNullOrEmpty(this.gameExe))
                {
                    string fd = Path.Combine(this.gameData,comboBoxSelectedDataFile.Text);
                    lstDrsTable = LoadDrsInList(fd);
                    fillDataGridView(dataGridViewSlpViewer, lstDrsTable);
                }

            }
        }
        private void fillDataGridView(DataGridView dgv, List<DrsTable> lstDrs)
        {
            DataTable table = new DataTable();
            DataColumn column;
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "ID";
            table.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Type";
            table.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "Length";
            table.Columns.Add(column);
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Comment";
            table.Columns.Add(column);
            DataRow r;
            //uint type = 0;
            //if (comboBoxTypeSlp.Text == "slp")
            //    type = 1936486432;
            if (lstDrs.Where(x => x.Type == type).FirstOrDefault() != null)
            {
                foreach (var item in lstDrs.Where(x => x.Type == type).First().Items)
                {
                    r = table.NewRow();
                    r[0] = item.Id;
                    r[1] = (string.IsNullOrEmpty(comboBoxTypeSlp.Text) ? "slp" : comboBoxTypeSlp.Text);
                    r[2] = item.Size;
                    r[3] = "";
                    table.Rows.Add(r);
                }
            }
            dataGridViewSlpViewer.DataSource = table;
        }
        private List<DrsTable> LoadDrsInList(string drsPathFile)
        {
            //List<DrsItem> lstitems = new List<DrsItem>();
            DrsTable[] drsTableArray;
            List<DrsItem> lstItem = new List<DrsItem>();

            using (FileStream fileStream1 = new FileStream(drsPathFile, FileMode.Open))//, FileSystemRights.ReadData, FileShare.Read, 1048576, FileOptions.SequentialScan))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream1);
                bool flag = false;
                while (true)
                {
                    byte num = binaryReader.ReadByte();
                    if (num == (byte)26)
                        flag = true;
                    else if (num > (byte)0 & flag)
                        break;
                }
                binaryReader.ReadBytes(3);
                binaryReader.ReadBytes(12);
                uint num1 = binaryReader.ReadUInt32();
                uint num2 = binaryReader.ReadUInt32();

                drsTableArray = new DrsTable[(int)num1];
                for (int index = 0; (long)index < (long)num1; ++index)
                    drsTableArray[index] = new DrsTable();
                foreach (DrsTable drsTable in drsTableArray)
                {
                    drsTable.Type = binaryReader.ReadUInt32();
                    drsTable.Start = binaryReader.ReadUInt32();
                    uint num3 = binaryReader.ReadUInt32();
                    DrsItem[] drsItemArray = new DrsItem[(int)num3];
                    for (int index = 0; (long)index < (long)num3; ++index)
                        drsItemArray[index] = new DrsItem();
                    drsTable.Items = (IEnumerable<DrsItem>)drsItemArray;
                }
                foreach (DrsTable drsTable in drsTableArray)
                {
                    //Trace.Assert(fileStream1.Position == (long)drsTable.Start);
                    foreach (DrsItem drsItem in drsTable.Items)
                    {
                        drsItem.Id = binaryReader.ReadUInt32();
                        drsItem.Start = binaryReader.ReadUInt32();
                        drsItem.Size = binaryReader.ReadUInt32();
                    }
                }
                foreach (DrsItem drsItem in ((IEnumerable<DrsTable>)drsTableArray).SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>)(table => table.Items)))
                {
                    //Trace.Assert(fileStream1.Position == (long)drsItem.Start);
                    drsItem.Data = binaryReader.ReadBytes((int)drsItem.Size);
                }
                //where type is slp not .way or .bina
                //lstItem = drsTableArray.Where(w => w.Type == 1936486432).First().Items.ToList();
                binaryReader.Close();
            }
            return drsTableArray.ToList();

        }

        private void comboBoxSelectedDataFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.gameExe))
            {
                string fd = Path.Combine(this.gameData, comboBoxSelectedDataFile.Text);
                lstDrsTable = LoadDrsInList(fd);
                fillDataGridView(dataGridViewSlpViewer, lstDrsTable);
            }
        }

        private void comboBoxTypeSlp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.gameExe))
            {
                string fd = Path.Combine(this.gameData, comboBoxSelectedDataFile.Text);
                var lst = LoadDrsInList(fd);
                //1651076705
                //1936486432
                //2002875936
                switch(comboBoxTypeSlp.Text)
                {
                    case "slp":
                        type = 1936486432;
                        break;
                    case "way":
                        type = 2002875936;
                        break;
                    case "bina":
                        type =   1651076705;
                        break;
                }
                fillDataGridView(dataGridViewSlpViewer, lst);
            }
        }
        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        private SlpViewer slpView =new SlpViewer();
        private BinaViewer binaViewer = new BinaViewer();
        private List<DrsTable> lstDrsTable;
        public string imageFileName;
        private uint type = 1936486432;
        private void dataGridViewSlpViewer_CellClick(object sender, DataGridViewCellEventArgs e)
        {


            if (!string.IsNullOrEmpty(this.gameExe) && e.RowIndex>0)
            {               
                var dtw = ((DataGridView)sender).Rows[e.RowIndex];
                var id = dtw.Cells[0].Value.ToString();
                if(!string.IsNullOrEmpty(id))
                    showDrsInformation(id);

            }

        }
        private void showDrsInformation(string index)
        {
            if (!string.IsNullOrEmpty(this.gameExe) && int.Parse(index) > 0)
            {
                //if slp
                if (type == 1936486432)
                {
                    if (!binaViewer.IsDisposed)
                    {
                        binaViewer.Close();
                    }
                    String workingDir = Directory.GetCurrentDirectory();
                    if (!workingDir.EndsWith("\\"))
                        workingDir += "\\tmpBmp\\";

                    if (!Directory.Exists(@"tmpBmp\"))
                        Directory.CreateDirectory(@"tmpBmp\");
                    //string fd = Path.Combine(this.gameData, comboBoxSelectedDataFile.Text);
                    //var dtw = ((DataGridView)sender).Rows[e.RowIndex];
                    //var id = dtw.Cells[0].Value.ToString();
                    var id = index;
                    if (lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id)).FirstOrDefault() != null)
                    {
                        File.WriteAllBytes(@"tmpBmp\tmpReadSlp.slp", lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id)).First().Data);
                        string SlpFileName = "tmpReadSlp.slp";
                        slpReader slpname = new slpReader();

                        string fi = Path.GetFileName(SlpFileName);
                        Console.WriteLine("Extracting frames from " + fi + "...");
                        slpname.sample = fi.Equals("int50101.slp") ? "50532.bmp" : "50500.bmp";
                        Console.WriteLine(fi + " " + slpname.sample);
                        slpname.name = fi.Split('.').First();
                        SlpFileName = Path.Combine(workingDir, SlpFileName);
                        slpname.Read(SlpFileName);
                        slpname.saveMultiFames(workingDir);
                        var fileName = Path.GetDirectoryName(SlpFileName) + $@"\{slpname.name}.bmp";
                        var myListFiles = Directory.GetFiles(workingDir).Where(x => x.Contains("tmpReadSlp") && x.EndsWith(".bmp")).ToList();
                        var list = myListFiles.Select(x => x.Replace(workingDir + @"tmpReadSlp", "").Replace(".bmp", "")).Select(x => int.Parse(x)).OrderBy(x => x).ToList();

                        List<string> lstbmp = list.Select(x => string.Format("{0}tmpReadSlp{1}.bmp", workingDir, x)).Take(slpname.numframes).ToList();

                        imageFileName = fileName;

                        var disposed = slpView.IsDisposed;
                        if (disposed)
                        {
                            slpView = new SlpViewer(lstbmp);
                            slpView.Show();
                            slpView.Play();
                        }
                        else
                            slpView.Visible = true;
                        slpView.lstbitemap = lstbmp;
                        slpView.Play();
                    }
                }
                if (type == 2002875936)
                {
                    //var dtw = ((DataGridView)sender).Rows[e.RowIndex];
                    //var id = dtw.Cells[0].Value.ToString();
                    var id = index;
                    var file = lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id)).First().Data;
                    Stream filestream = new MemoryStream(file);
                    NAudio.Wave.WaveStream pcm = new NAudio.Wave.WaveChannel32(new NAudio.Wave.WaveFileReader(filestream));//new NAudio.Wave.WaveFileReader("")
                    var stream = new NAudio.Wave.BlockAlignReductionStream(pcm);
                    var output = new NAudio.Wave.DirectSoundOut();
                    output.Init(stream);
                    output.Play();
                }
                if (type == 1651076705)
                {
                    //var dtw = ((DataGridView)sender).Rows[e.RowIndex];
                    //var id = dtw.Cells[0].Value.ToString();
                    var id = index;
                    var buffer = lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id)).First().Data;
                    if (!slpView.IsDisposed)
                    {
                        slpView.Close();
                    }
                    var disposed = binaViewer.IsDisposed;
                    if (disposed)
                    {
                        binaViewer = new BinaViewer(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                    }
                    else
                    {
                        binaViewer.setText(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
                    }
                    binaViewer.Show();

                }
            }
        }
        public Image byteArrayToImage(byte[] bytesArr)
        {
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                Image img = Image.FromStream(memstr);
                return img;
            }
        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int myIdToSearch;
            if(int.TryParse(textBoxSearchId.Text, out myIdToSearch))
            {
                dataGridViewSlpViewer.ClearSelection();
                int rowIndex = -1;
                foreach (DataGridViewRow row in dataGridViewSlpViewer.Rows)
                {
                    var val = row.Cells[0].Value;
                    if (row.Cells[0].Value !=null && int.Parse(row.Cells[0].Value.ToString()) == myIdToSearch)
                    {
                        rowIndex = row.Index;
                        break;
                    }
                }
                if(rowIndex>=0)
                {
                   dataGridViewSlpViewer.Rows[rowIndex].Selected = true;
                   showDrsInformation(dataGridViewSlpViewer.Rows[rowIndex].Cells[0].Value.ToString());
                }
            }
        }

        private void dataGridViewSlpViewer_CurrentCellChanged(object sender, EventArgs e)
        {

        }

        private void buttonOpenFileDrs_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lstDrsTable = LoadDrsInList(openFileDialog.FileName);
                fillDataGridView(dataGridViewSlpViewer, lstDrsTable);

            }
        }

        private void dataGridViewSlpViewer_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                var hitTestInfo = dataGridViewSlpViewer.HitTest(e.X, e.Y);
                // If column is first column
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell && hitTestInfo.ColumnIndex == 0)
                    contextMenuStripDrsEditorOptions.Show(dataGridViewSlpViewer, new Point(e.X, e.Y));
                // If column is second column
                if (hitTestInfo.Type == DataGridViewHitTestType.Cell && hitTestInfo.ColumnIndex == 1)
                    contextMenuStripDrsEditorOptions.Show(dataGridViewSlpViewer, new Point(e.X, e.Y));
            }
        }

        private void replaceFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter =
            "slp files (*.slp)|*.slp" +
            "|way files (*.way)|*.way" +
            "|bina files (*.bina)|*.bina";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName.EndsWith("."+ (string.IsNullOrEmpty(comboBoxTypeSlp.Text) ? "slp" : comboBoxTypeSlp.Text)))
                {
                    var data = File.ReadAllBytes(openFileDialog.FileName);
                    var id = dataGridViewSlpViewer.SelectedCells[0].Value;
                    lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id + "")).First().Data = data;
                }
                else
                {
                    MessageBox.Show("File format is wrong, you need " +(string.IsNullOrEmpty(comboBoxTypeSlp.Text)?"slp":comboBoxTypeSlp.Text)+ "Format");
                }
            }
        }

        private void dataGridViewSlpViewer_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hti = dataGridViewSlpViewer.HitTest(e.X, e.Y);
                dataGridViewSlpViewer.ClearSelection();
                dataGridViewSlpViewer.Rows[hti.RowIndex].Selected = true;
            }
        }

        private void extractFileToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var id = dataGridViewSlpViewer.SelectedCells[0].Value;
                var data = lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id + "")).First().Data;
                File.WriteAllBytes(Path.Combine(folderBrowserDialog1.SelectedPath, id+ "." + (string.IsNullOrEmpty(comboBoxTypeSlp.Text) ? "slp" : comboBoxTypeSlp.Text)), data);
            }
        }

        private void deleteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var id = dataGridViewSlpViewer.SelectedCells[0].Value;
            var item = lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id + "")).First();
            lstDrsTable.Where(x => x.Type == type).First().Items = lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id != uint.Parse(id + "")).ToList();
            fillDataGridView(dataGridViewSlpViewer, lstDrsTable);
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {


            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Allow the user to select multiple Files
            openFileDialog.Multiselect = true;
            openFileDialog.Filter =
            "slp files (*.slp)|*.slp" +
            "|way files (*.way)|*.way" +
            "|bina files (*.bina)|*.bina";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileName.EndsWith("." + (string.IsNullOrEmpty(comboBoxTypeSlp.Text) ? "slp" : comboBoxTypeSlp.Text)))
                {
                    List<DrsItem> lstDrsItem = lstDrsTable.Where(x => x.Type == type).First().Items.ToList();
                    foreach (var f in openFileDialog.FileNames)
                    {
                        var data = File.ReadAllBytes(f);
                        var lastDrsItem = lstDrsItem.OrderByDescending(x => x.Id).First();
                        //var preccItem = lstDrsItem.ElementAt(lstDrsItem.Count-2);
                        ////56616688+238= 56616926
                        ////56616926
                        //uint precDrsItem = preccItem.Start+ preccItem.Size;
                        uint LastId = lastDrsItem.Id;
                        uint lastStartPos = (uint) lastDrsItem.Start + (uint)lastDrsItem.Size;
                        //lastStartPos += lastDrsItem.Size;
  
                        var drsItem = new DrsItem()
                        {
                            Id= (uint)LastId +1,
                            Data = data,
                            Size = (uint)data.Length,
                            Start = (uint)lastStartPos
                        };
                        //lastStartPos += (uint)data.Length;
                        lstDrsItem.Add(drsItem);
                    }
                    lstDrsTable.Where(x => x.Type == type).First().Items = lstDrsItem;
                    fillDataGridView(dataGridViewSlpViewer, lstDrsTable);
                }
                else
                {
                    MessageBox.Show("File format is wrong, you need " + (string.IsNullOrEmpty(comboBoxTypeSlp.Text) ? "slp" : comboBoxTypeSlp.Text) + "Format");
                }
            }
    
        }

        private void buttonUpdateSlpID_Click(object sender, EventArgs e)
        {
            uint idToUpdate ;
            if (uint.TryParse(textBoxUpdateSlpId.Text, out idToUpdate))
            {
                var id = dataGridViewSlpViewer.SelectedCells[0].Value;

                lstDrsTable.Where(x => x.Type == type).First().Items.Where(w => w.Id == uint.Parse(id + "")).First().Id = idToUpdate;
                fillDataGridView(dataGridViewSlpViewer, lstDrsTable);
            }
            else
            {
                MessageBox.Show("Invalide input id to update, u need a number");
                return;
            }
        }

        private void buttonSaveDrs_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(this.gameData))
            { 
                var drsFile = Path.Combine(this.gameData,comboBoxSelectedDataFile.Text);
                var tmpDrsFile = drsFile.Replace(".drs", "Tmp.drs");
                saveDrsFromLis(drsFile, tmpDrsFile, lstDrsTable);
                if (File.Exists(drsFile))
                {
                    //update Drs file
                    File.Copy(tmpDrsFile, drsFile, true);
                    File.Delete(tmpDrsFile);
                }
            }
            else
            {
                MessageBox.Show("Browser Game Exe");
                return;
            }
        }
        //save drs as
        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter =
            "Ds file (*.drs)|*.drs";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(this.gameData))
                {
                    var drsFile = Path.Combine(this.gameData, comboBoxSelectedDataFile.Text);
                    var tmpDrsFile = saveFileDialog1.FileName;
                    saveDrsFromLis(drsFile, tmpDrsFile, lstDrsTable);
                }
                else
                {
                    MessageBox.Show("Browser Game Exe");
                    return;
                }
            }
        }
        private uint getfirstStartDrs(List<DrsTable> lstDrsTable)
        {
            uint res = 0;
            uint precStart = 0;
            uint result = 0;
            uint itemCount = 0;
            uint countallItem = 0;
            uint firstStart = 0;
            DrsTable precDrsTable = new DrsTable();
            foreach (DrsTable drsTable in lstDrsTable)
            {

                if (precStart == 0)
                {
                    firstStart = drsTable.Start;
                }
                else
                {
                    itemCount = (uint)precDrsTable.Items.Count<DrsItem>();
                    result = (uint)itemCount * 12 + precStart;

                }
                uint count = (uint)drsTable.Items.Count<DrsItem>();
                countallItem += count;
                precStart = drsTable.Start;
                precDrsTable = drsTable;
            }
            res = (uint)12 * countallItem + firstStart;
            return res;
        }
        private void saveDrsFromLis(string drsPathFile,string newDrsFile,List<DrsTable> lstDrsTable)
        {


            using (FileStream fileStream1 = new FileStream(drsPathFile, FileMode.Open))
            {

                BinaryReader binaryReader = new BinaryReader(fileStream1);
                using (FileStream fileStream2 = new FileStream(newDrsFile, FileMode.Create))
                {
                    bool flag = false;
                    BinaryWriter binaryWriter = new BinaryWriter(fileStream2);
                    string res = string.Empty;
                    while (true)
                    {
                        byte num = binaryReader.ReadByte();
                        binaryWriter.Write(num);
                        res += num;
                        if (num == (byte)26)
                            flag = true;
                        else if (num > (byte)0 & flag)
                            break;
                    }
                    var tb = binaryReader.ReadBytes(3);
                    binaryWriter.Write(tb);
                    var db = binaryReader.ReadBytes(12);
                    binaryWriter.Write(db);
                    //nb of type exemple :3 slp bina way
                    uint num1 = binaryReader.ReadUInt32();
                    binaryWriter.Write(num1);
                    //first start position drs item
                    uint num2 = binaryReader.ReadUInt32();
                    uint nume2_ = getfirstStartDrs(lstDrsTable);
                    binaryWriter.Write(nume2_);
                    //binaryWriter.Write(num2);
                    binaryReader.Close();
                    uint num4 = nume2_;
                    //uint num4 = num2;
                    List<DrsTable> source = new List<DrsTable>();
                    uint id;

                    //update possitions
                    foreach (DrsTable drsTable1 in lstDrsTable)
                    {
                        List<DrsItem> drsItemList = new List<DrsItem>();
                        DrsTable drsTable2 = new DrsTable()
                        {
                            Start = drsTable1.Start,
                            Type = drsTable1.Type,
                            Items = (IEnumerable<DrsItem>)drsItemList
                        };
                        foreach (DrsItem drsItem1 in drsTable1.Items)
                        {
                            DrsItem drsItem2 = new DrsItem()
                            {
                                Id = drsItem1.Id,
                                Start = num4,
                                Data = drsItem1.Data
                            };
                            drsItem2.Size = (uint)drsItem2.Data.Length;
                            num4 += drsItem2.Size;
                            drsItemList.Add(drsItem2);
                        }
                        source.Add(drsTable2);
                    }
                    //start 100
                    //126*1512  //126 = item cout
                    //1512+100 =1612
                    //224*12 =2688‬   //224 = item count
                    //2688+1612 = 4300‬
                    uint precStart = 0;
                    uint result = 0;
                    uint itemCount = 0;
                    uint countallItem = 0;
                    uint firstStart = 0;
                    DrsTable precDrsTable = new DrsTable();
                    foreach (DrsTable drsTable in source)
                    {
                        binaryWriter.Write(drsTable.Type);
                        if (precStart == 0)
                        {
                            binaryWriter.Write(drsTable.Start);
                            firstStart = drsTable.Start;
                        }
                        else
                        {
                            itemCount = (uint)precDrsTable.Items.Count<DrsItem>();
                            result =(uint) itemCount * 12 + precStart;
                            binaryWriter.Write(result);
                        }
                        uint count = (uint) drsTable.Items.Count<DrsItem>();
                        binaryWriter.Write(count);
                        countallItem += count;
                        precStart = drsTable.Start;
                        precDrsTable = drsTable;
                    }
                    precStart = 0;
                    uint num_ = num2;
                    //4264 221
                    //4864
                    //+3 SLP+> 4900
                    //4888
                    //4900 -4888 + 12
                    //conclusion num2 = 12*(126+223+50) = 4 788‬ +100

                    num_ = (uint) 12 * countallItem + firstStart;
                    foreach (DrsTable drsTable in source)
                    {

                        DrsItem precDrsItem = new DrsItem();
                        foreach (DrsItem drsItem in drsTable.Items)
                        {
                            //611477
                            //611501
                            binaryWriter.Write(drsItem.Id);
                            //binaryWriter.Write(drsItem.Start);
                            if (precStart == 0)
                            {
                                binaryWriter.Write(num_);//result);//
                                drsItem.Start = num_;
                            }
                            else
                            {
                                uint newStart = (uint)precStart + (uint)precDrsItem.Size;
                                binaryWriter.Write(newStart);
                            }
                            binaryWriter.Write(drsItem.Size);
                            precStart = drsItem.Start;
                            precDrsItem = drsItem;
                        }
                    }
                    foreach (DrsItem drsItem in source.SelectMany<DrsTable, DrsItem>((Func<DrsTable, IEnumerable<DrsItem>>)(outTable => outTable.Items)))
                    {
                        binaryWriter.Write(drsItem.Data);
                    }
                    binaryWriter.Close();
                    fileStream2.Close();
                }
                fileStream1.Close();
            }
        }

 
    }
    public class TextBoxListener : TraceListener
    {
        RichTextBox _box;
        string _data;
        public TextBoxListener(string initializeData)
        {
            _data = initializeData;
        }

        public bool Init()
        {
            if (_box != null && _box.IsDisposed)
            {
                // back to null if the control is disposed
                _box = null;
            }
            // find the logger text box
            if (_box == null)
            {
                // open forms
                foreach (Form f in Application.OpenForms)
                {
                    // controls on those forms
                    foreach (Control c in f.Controls)
                    {
                        // does the name match 
                        if (c.Name == _data && c is RichTextBox)
                        {
                            // found one!
                            _box = (RichTextBox)c;
                            break;
                        }
                    }
                }
            }
            return _box != null && !_box.IsDisposed;
        }

        public override void WriteLine(string message)
        {
            if (Init())
            {
                _box.Text = _box.Text + message + "\r\n";
            }
        }

        public override void Write(string message)
        {
            if (Init())
            {
                _box.Text = _box.Text + message;
            }
        }
    }
}
