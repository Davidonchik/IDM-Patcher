using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows.Forms;

namespace IDMPatcherInstaller
{
    public class InstallerForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, ushort wLanguage, byte[] lpData, uint cbData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const uint WM_CLOSE = 0x0010;
        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        private const int RT_ICON = 3;
        private const int RT_GROUP_ICON = 14;

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private Label statusLabel;
        private ProgressBar progressBar;
        private Button installButton;
        private RichTextBox logBox;
        private string patcherDir = @"C:\ProgramData\IDM_Patcher";
        private string idmDir = @"C:\Program Files (x86)\Internet Download Manager";
        
        private bool isDarkTheme;
        private System.Drawing.Color bgColor;
        private System.Drawing.Color fgColor;
        private System.Drawing.Color accentColor;
        private System.Drawing.Color buttonBgColor;
        private System.Drawing.Color logBgColor;
        private System.Drawing.Color logFgColor;

        public InstallerForm()
        {
            DetectTheme();
            InitializeUI();
        }

        private void DetectTheme()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var value = key.GetValue("AppsUseLightTheme");
                        isDarkTheme = value != null && (int)value == 0;
                    }
                }
            }
            catch
            {
                isDarkTheme = false;
            }

            if (isDarkTheme)
            {
                bgColor = System.Drawing.Color.FromArgb(32, 32, 32);
                fgColor = System.Drawing.Color.FromArgb(240, 240, 240);
                accentColor = System.Drawing.Color.FromArgb(0, 120, 215);
                buttonBgColor = System.Drawing.Color.FromArgb(45, 45, 45);
                logBgColor = System.Drawing.Color.FromArgb(20, 20, 20);
                logFgColor = System.Drawing.Color.FromArgb(0, 255, 127);
            }
            else
            {
                bgColor = System.Drawing.Color.FromArgb(240, 240, 240);
                fgColor = System.Drawing.Color.FromArgb(32, 32, 32);
                accentColor = System.Drawing.Color.FromArgb(0, 120, 215);
                buttonBgColor = System.Drawing.Color.FromArgb(225, 225, 225);
                logBgColor = System.Drawing.Color.White;
                logFgColor = System.Drawing.Color.FromArgb(0, 128, 0);
            }
        }

        private void InitializeUI()
        {
            this.Text = Localization.Get("Title");
            this.Size = new System.Drawing.Size(600, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = bgColor;

            var titleLabel = new Label
            {
                Text = Localization.Get("Title"),
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(550, 40),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = accentColor,
                BackColor = bgColor
            };

            statusLabel = new Label
            {
                Text = Localization.Get("ReadyToInstall"),
                Location = new System.Drawing.Point(20, 70),
                Size = new System.Drawing.Size(550, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                ForeColor = fgColor,
                BackColor = bgColor
            };

            progressBar = new ProgressBar
            {
                Location = new System.Drawing.Point(20, 110),
                Size = new System.Drawing.Size(550, 25),
                Style = ProgressBarStyle.Continuous,
                ForeColor = accentColor
            };

            logBox = new RichTextBox
            {
                Location = new System.Drawing.Point(20, 150),
                Size = new System.Drawing.Size(550, 180),
                ReadOnly = true,
                BackColor = logBgColor,
                ForeColor = logFgColor,
                Font = new System.Drawing.Font("Consolas", 9),
                BorderStyle = BorderStyle.FixedSingle
            };

            installButton = new Button
            {
                Text = Localization.Get("Install"),
                Location = new System.Drawing.Point(250, 350),
                Size = new System.Drawing.Size(100, 40),
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                BackColor = accentColor,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            installButton.FlatAppearance.BorderSize = 0;
            installButton.Click += InstallButton_Click;

            this.Controls.Add(titleLabel);
            this.Controls.Add(statusLabel);
            this.Controls.Add(progressBar);
            this.Controls.Add(logBox);
            this.Controls.Add(installButton);
        }

        private void Log(string message)
        {
            if (logBox.InvokeRequired)
            {
                logBox.Invoke(new Action(() => Log(message)));
                return;
            }
            logBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            logBox.ScrollToCaret();
        }

        private void UpdateStatus(string status, int progress)
        {
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action(() => UpdateStatus(status, progress)));
                return;
            }
            statusLabel.Text = status;
            progressBar.Value = progress;
        }

        private string TryFindIDMFromShortcut()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string[] possibleNames = { 
                    "Internet Download Manager.lnk",
                    "IDM.lnk",
                    "Internet Download Manager.url"
                };

                foreach (string shortcutName in possibleNames)
                {
                    string shortcutPath = Path.Combine(desktopPath, shortcutName);
                    
                    if (File.Exists(shortcutPath))
                    {
                        Log($"Found shortcut: {shortcutName}");
                        string targetPath = GetShortcutTarget(shortcutPath);
                        
                        if (!string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
                        {
                            string directory = Path.GetDirectoryName(targetPath);
                            Log($"Resolved IDM path from shortcut: {directory}");
                            return directory;
                        }
                    }
                }

                string publicDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                foreach (string shortcutName in possibleNames)
                {
                    string shortcutPath = Path.Combine(publicDesktop, shortcutName);
                    
                    if (File.Exists(shortcutPath))
                    {
                        Log($"Found shortcut in public desktop: {shortcutName}");
                        string targetPath = GetShortcutTarget(shortcutPath);
                        
                        if (!string.IsNullOrEmpty(targetPath) && File.Exists(targetPath))
                        {
                            string directory = Path.GetDirectoryName(targetPath);
                            Log($"Resolved IDM path from shortcut: {directory}");
                            return directory;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Error searching for shortcuts: {ex.Message}");
            }

            return null;
        }

        private string GetShortcutTarget(string shortcutPath)
        {
            try
            {
                if (shortcutPath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                    dynamic shell = Activator.CreateInstance(shellType);
                    var shortcut = shell.CreateShortcut(shortcutPath);
                    string targetPath = shortcut.TargetPath;
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);
                    return targetPath;
                }
            }
            catch (Exception ex)
            {
                Log($"Error reading shortcut: {ex.Message}");
            }

            return null;
        }

        private bool CheckAndPromptForIDMPath()
        {
            string idmExe = Path.Combine(idmDir, "IDMan.exe");
            
            while (!File.Exists(idmExe))
            {
                string foundPath = TryFindIDMFromShortcut();
                if (foundPath != null)
                {
                    idmDir = foundPath;
                    idmExe = Path.Combine(idmDir, "IDMan.exe");
                    Log(Localization.Get("IDMPathSet", idmDir));
                    
                    if (File.Exists(idmExe))
                    {
                        return true;
                    }
                }

                // Create custom dialog
                Form dialog = new Form
                {
                    Text = Localization.Get("ErrorIDMNotFound"),
                    Size = new System.Drawing.Size(500, 220),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                Label messageLabel = new Label
                {
                    Text = Localization.Get("ErrorIDMNotFoundMsg", idmDir),
                    Location = new System.Drawing.Point(20, 20),
                    Size = new System.Drawing.Size(440, 80),
                    Font = new System.Drawing.Font("Segoe UI", 10)
                };

                Button browseButton = new Button
                {
                    Text = Localization.Get("BrowsePath"),
                    Location = new System.Drawing.Point(150, 120),
                    Size = new System.Drawing.Size(180, 40),
                    Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                    DialogResult = DialogResult.Retry
                };

                Button closeButton = new Button
                {
                    Text = Localization.Get("Close"),
                    Location = new System.Drawing.Point(340, 120),
                    Size = new System.Drawing.Size(100, 40),
                    Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                    DialogResult = DialogResult.Cancel
                };

                dialog.Controls.Add(messageLabel);
                dialog.Controls.Add(browseButton);
                dialog.Controls.Add(closeButton);
                dialog.AcceptButton = browseButton;
                dialog.CancelButton = closeButton;

                DialogResult result = dialog.ShowDialog(this);

                if (result == DialogResult.Retry)
                {
                    using (var folderDialog = new FolderBrowserDialog())
                    {
                        folderDialog.Description = Localization.Get("SelectIDMPath");
                        folderDialog.SelectedPath = idmDir;
                        
                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            idmDir = folderDialog.SelectedPath;
                            idmExe = Path.Combine(idmDir, "IDMan.exe");
                            Log(Localization.Get("IDMPathSet", idmDir));
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            
            return true;
        }

        private async void InstallButton_Click(object sender, EventArgs e)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show(Localization.Get("ErrorAdmin"), Localization.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!CheckAndPromptForIDMPath())
            {
                return;
            }

            installButton.Enabled = false;
            await System.Threading.Tasks.Task.Run(() => Install());
        }

        private bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void Install()
        {
            try
            {
                UpdateStatus(Localization.Get("ClosingIDM"), 5);
                CloseIDM();

                UpdateStatus(Localization.Get("CreatingDirectory", patcherDir), 10);
                Log(Localization.Get("CreatingDirectory", patcherDir));
                Directory.CreateDirectory(patcherDir);

                UpdateStatus(Localization.Get("ExtractingFiles"), 20);
                ExtractEmbeddedFiles();

                UpdateStatus(Localization.Get("PatchingIDM"), 40);
                PatchIDMInstallation();

                UpdateStatus(Localization.Get("InstallComplete"), 100);
                Log(Localization.Get("InstallSuccess"));
                Log("");
                Log(Localization.Get("Features"));

                MessageBox.Show(
                    Localization.Get("InstallSuccessMsg"),
                    Localization.Get("InstallCompleteTitle"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                
                if (ex.Message.Contains("being used by another process") || 
                    ex.Message.Contains("занят другим процессом") ||
                    ex.Message.Contains("DLL file is locked") ||
                    ex.Message.Contains("Access to the path") ||
                    ex.Message.Contains("is denied"))
                {
                    Form errorDialog = new Form
                    {
                        Text = Localization.Get("ErrorTitle"),
                        Size = new System.Drawing.Size(500, 220),
                        StartPosition = FormStartPosition.CenterParent,
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        MaximizeBox = false,
                        MinimizeBox = false
                    };

                    Label messageLabel = new Label
                    {
                        Text = Localization.Get("IDMStillRunning"),
                        Location = new System.Drawing.Point(20, 20),
                        Size = new System.Drawing.Size(440, 100),
                        Font = new System.Drawing.Font("Segoe UI", 10)
                    };

                    Button retryButton = new Button
                    {
                        Text = Localization.Get("Retry"),
                        Location = new System.Drawing.Point(250, 130),
                        Size = new System.Drawing.Size(100, 40),
                        Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                        DialogResult = DialogResult.Retry
                    };

                    Button cancelButton = new Button
                    {
                        Text = Localization.Get("Close"),
                        Location = new System.Drawing.Point(360, 130),
                        Size = new System.Drawing.Size(100, 40),
                        Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                        DialogResult = DialogResult.Cancel
                    };

                    errorDialog.Controls.Add(messageLabel);
                    errorDialog.Controls.Add(retryButton);
                    errorDialog.Controls.Add(cancelButton);
                    errorDialog.AcceptButton = retryButton;
                    errorDialog.CancelButton = cancelButton;

                    DialogResult result = errorDialog.ShowDialog(this);

                    if (result == DialogResult.Retry)
                    {
                        installButton.Enabled = true;
                        InstallButton_Click(null, null);
                    }
                }
                else
                {
                    MessageBox.Show(Localization.Get("ErrorInstallFailed", ex.Message), Localization.Get("ErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CloseIDM()
        {
            Log(Localization.Get("ClosingIDM"));
            try
            {
                string signalPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "idm_signal_unload.exe");
                if (File.Exists(signalPath))
                {
                    var signalProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = signalPath,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    });
                    signalProcess.WaitForExit(3000);
                    System.Threading.Thread.Sleep(2000);
                }

                string[] processNames = { "IDMan", "IDMan_original" };
                
                foreach (var processName in processNames)
                {
                    var processes = Process.GetProcessesByName(processName);
                    if (processes.Length == 0) continue;

                    foreach (var process in processes)
                    {
                        Log(Localization.Get("KillingProcess", process.Id) + $" ({processName})");
                        
                        bool closed = false;
                        EnumWindows((hWnd, lParam) =>
                        {
                            uint processId;
                            GetWindowThreadProcessId(hWnd, out processId);
                            if (processId == process.Id)
                            {
                                PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                                closed = true;
                            }
                            return true;
                        }, IntPtr.Zero);

                        if (closed)
                        {
                            if (!process.WaitForExit(5000))
                            {
                                process.Kill();
                                process.WaitForExit(3000);
                            }
                        }
                        else
                        {
                            process.Kill();
                            process.WaitForExit(3000);
                        }
                    }
                }
                
                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Log($"Warning: {ex.Message}");
            }
        }

        private void ExtractEmbeddedFiles()
        {
            Log(Localization.Get("ExtractingFiles"));
            
            string[] files = { "idm_patch.dll", "idm_injector.exe", "IDMLauncher.exe", "idm_signal_unload.exe" };

            string sourceDir = AppDomain.CurrentDomain.BaseDirectory;
            
            foreach (var file in files)
            {
                string sourcePath = Path.Combine(sourceDir, file);
                string destPath = Path.Combine(patcherDir, file);
                
                if (File.Exists(sourcePath))
                {
                    Log(Localization.Get("CopyingFile", file));
                    File.Copy(sourcePath, destPath, true);
                }
                else
                {
                    Log(Localization.Get("WarningFile", file));
                }
            }
        }

        private void PatchIDMInstallation()
        {
            Log(Localization.Get("PatchingIDM"));
            
            string idmExe = Path.Combine(idmDir, "IDMan.exe");
            string idmOriginal = Path.Combine(idmDir, "IDMan_original.exe");
            
            if (!File.Exists(idmExe))
            {
                throw new Exception(Localization.Get("ErrorIDMNotFoundMsg", idmExe));
            }
            
            if (!File.Exists(idmOriginal))
            {
                Log(Localization.Get("BackingUp"));
                File.Copy(idmExe, idmOriginal, false);
            }
            
            Log(Localization.Get("ReplacingIDM"));
            string launcherPath = Path.Combine(patcherDir, "IDMLauncher.exe");
            
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    File.Copy(launcherPath, idmExe, true);
                    break;
                }
                catch (IOException)
                {
                    if (i == 4) throw;
                    Log(Localization.Get("Retrying", i + 1));
                    System.Threading.Thread.Sleep(1000);
                }
            }
            
            Log("Copying icon from original IDM to launcher...");
            CopyIconToLauncher(idmOriginal, idmExe);
            
            Log(Localization.Get("CopyingPatchFiles"));
            
            string injectorSrc = Path.Combine(patcherDir, "idm_injector.exe");
            string injectorDst = Path.Combine(idmDir, "idm_injector.exe");
            string dllSrc = Path.Combine(patcherDir, "idm_patch.dll");
            string dllDst = Path.Combine(idmDir, "idm_patch.dll");
            
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (File.Exists(dllDst))
                    {
                        File.Delete(dllDst);
                        Log("Old DLL deleted successfully");
                    }
                    break;
                }
                catch (IOException)
                {
                    if (i == 2)
                    {
                        throw new IOException("DLL file is locked. Please close IDM completely and try again.");
                    }
                    Log(Localization.Get("WaitingUnlock", i + 1));
                    System.Threading.Thread.Sleep(3000);
                }
            }
            
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    File.Copy(injectorSrc, injectorDst, true);
                    File.Copy(dllSrc, dllDst, true);
                    break;
                }
                catch (IOException)
                {
                    if (i == 2) throw;
                    Log(Localization.Get("Retrying", i + 1));
                    System.Threading.Thread.Sleep(2000);
                }
            }
            
            Log(Localization.Get("PatchedSuccess"));
        }

        private void CopyIconToLauncher(string sourceExe, string targetExe)
        {
            try
            {
                IntPtr hModule = LoadLibraryEx(sourceExe, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (hModule == IntPtr.Zero)
                {
                    Log("Warning: Could not load source executable for icon extraction");
                    return;
                }

                try
                {
                    IntPtr hUpdate = BeginUpdateResource(targetExe, false);
                    if (hUpdate == IntPtr.Zero)
                    {
                        Log("Warning: Could not begin resource update");
                        return;
                    }

                    try
                    {
                        bool iconCopied = false;

                        for (int i = 1; i <= 10; i++)
                        {
                            IntPtr iconId = new IntPtr(i);
                            IntPtr hResInfo = FindResource(hModule, iconId, new IntPtr(RT_GROUP_ICON));
                            
                            if (hResInfo != IntPtr.Zero)
                            {
                                IntPtr hResData = LoadResource(hModule, hResInfo);
                                if (hResData != IntPtr.Zero)
                                {
                                    IntPtr pResource = LockResource(hResData);
                                    uint size = SizeofResource(hModule, hResInfo);
                                    
                                    if (pResource != IntPtr.Zero && size > 0)
                                    {
                                        byte[] iconData = new byte[size];
                                        Marshal.Copy(pResource, iconData, 0, (int)size);
                                        
                                        UpdateResource(hUpdate, new IntPtr(RT_GROUP_ICON), iconId, 0, iconData, size);
                                        iconCopied = true;
                                        Log($"Copied icon group resource ID {i}");
                                    }
                                }
                            }

                            hResInfo = FindResource(hModule, iconId, new IntPtr(RT_ICON));
                            if (hResInfo != IntPtr.Zero)
                            {
                                IntPtr hResData = LoadResource(hModule, hResInfo);
                                if (hResData != IntPtr.Zero)
                                {
                                    IntPtr pResource = LockResource(hResData);
                                    uint size = SizeofResource(hModule, hResInfo);
                                    
                                    if (pResource != IntPtr.Zero && size > 0)
                                    {
                                        byte[] iconData = new byte[size];
                                        Marshal.Copy(pResource, iconData, 0, (int)size);
                                        
                                        UpdateResource(hUpdate, new IntPtr(RT_ICON), iconId, 0, iconData, size);
                                        Log($"Copied icon resource ID {i}");
                                    }
                                }
                            }
                        }

                        if (!iconCopied)
                        {
                            Log("Warning: No icon resources found in source executable");
                        }
                    }
                    finally
                    {
                        EndUpdateResource(hUpdate, false);
                    }
                }
                finally
                {
                    FreeLibrary(hModule);
                }

                Log("Icon copy completed successfully");
            }
            catch (Exception ex)
            {
                Log($"Warning: Failed to copy icon: {ex.Message}");
            }
        }



        private void RunCommand(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = Process.Start(psi))
            {
                process.WaitForExit();
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstallerForm());
        }
    }
}
