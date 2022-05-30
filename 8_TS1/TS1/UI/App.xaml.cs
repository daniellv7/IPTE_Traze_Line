using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Ipte.Machine;
using Ipte.TS1.UI.i18n;
using Ipte.Properties;
using System.Windows.Threading;
using Ipte.TS1.UI.Controls;

namespace Ipte.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        # region import user32.dll

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        public const UInt32 FLASHW_ALL = 3;

        private const Int32 SW_SHOWNORMAL = 1;
        private const Int32 SW_SHOWMAXIMIZED = 3;
        private const Int32 SW_SHOW = 5;
        private const Int32 SW_MINIMIZE = 6;
        private const Int32 SW_RESTORE = 9;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern bool IsIconic(IntPtr hwnd);

        # endregion

        static App()
        {
            var args = Environment.GetCommandLineArgs();
            if (!args.Contains("SKIPRUNNINGCHECK"))
            {
                //only allow one instance of the application
                Process thisProcess = Process.GetCurrentProcess();
                Process[] processes = Process.GetProcessesByName(thisProcess.ProcessName);

                if (processes.Length > 1)
                {
                    Process otherProcess = processes.First(p => (p != thisProcess));
                    if (otherProcess.MainWindowHandle == IntPtr.Zero)
                    {
                        GuiMessageBox.Show("Application has not been closed properly. Please restart computer and retry.", thisProcess.ProcessName);
                    }
                    else
                    {
                        GuiMessageBox.Show("Application is already running.", thisProcess.ProcessName);
                        try
                        {
                            IntPtr hWnd = new IntPtr(FindWindow(null, thisProcess.ProcessName));

                            // bring to top
                            ShowWindow(hWnd, SW_SHOW);
                            if (IsIconic(hWnd)) ShowWindow(hWnd, SW_RESTORE);
                            SetForegroundWindow(hWnd);

                            // flash window
                            FLASHWINFO fInfo = new FLASHWINFO();
                            fInfo.cbSize = Convert.ToUInt32(System.Runtime.InteropServices.Marshal.SizeOf(fInfo));
                            fInfo.hwnd = hWnd;
                            fInfo.dwFlags = FLASHW_ALL;
                            fInfo.uCount = 3;
                            fInfo.dwTimeout = 0;
                            FlashWindowEx(ref fInfo);
                        }
                        catch { }
                    }

                    Environment.Exit(0);
                }
            }
        }

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += Current_DispatcherUnhandledException;

            ////take default culture without custom region settings. Can also enforce some settings by specifying custom culture such as "en-us" or "de-de"
            //Thread.CurrentThread.CurrentCulture = new CultureInfo(Thread.CurrentThread.CurrentCulture.Name, false);

            ////make the wpf binding respect the default culture
            //FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));

            ////override the language; does not affect the decimal separator
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.UICulture);
        }

        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                System.Diagnostics.Trace.TraceError(e.Exception.GetBaseException().StackTrace);
            }
            catch { }
            try
            {
                MessageBox.Show(
                    this.i18nTranslate("Whoops! Please contact the developers with the following information:") + "\r\n" +
                    e.Exception.GetBaseException().Message + "\r\n" + e.Exception.GetBaseException().StackTrace,
                    this.i18nTranslate("Fatal Error"),
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Stop);
            }
            finally
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
                e.Handled = true;
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                System.Diagnostics.Trace.TraceError(((Exception)e.ExceptionObject).GetBaseException().StackTrace);
            }
            catch { }
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                MessageBox.Show(
                    this.i18nTranslate("Whoops! Please contact the developers with the following information:") + "\r\n" +
                    ex.GetBaseException().Message + "\r\n" + ex.GetBaseException().StackTrace,
                    this.i18nTranslate("Fatal Error"),
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Stop);
            }
            finally
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            try
            {
                Controller.Instance.Dispose();
            }
            catch { }
        }
    }
}

