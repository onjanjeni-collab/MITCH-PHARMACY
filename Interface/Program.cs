//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace Revmax_Interface_Promun
//{
//    internal static class Program
//    {
//        private const string MutexName = "Global\\Revmax_Interface_Promun_TrinityPharmacy";

//        /// <summary>
//        /// The main entry point for the application.
//        /// </summary>
//        [STAThread]
//        static void Main()
//        {
//            using (Mutex mutex = new Mutex(true, MutexName, out bool isNewInstance))
//            {
//                if (!isNewInstance)
//                {
//                    MessageBox.Show("Revmax Interface is already running.", "Revmax Interface", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                Application.EnableVisualStyles();
//                Application.SetCompatibleTextRenderingDefault(false);
//                if (ConfigurationManager.AppSettings.Get("Trained") == "0")
//                {
//                    Application.Run(new Wizard());


//                }
//                else
//                {

//                    /*RevMaxInterfaceWizard form = new RevMaxInterfaceWizard();
//                    form.WindowState = FormWindowState.Normal;
//                    form.ShowInTaskbar = true;
//                    Application.Run(form);*/

//                    RevMaxInterfaceWizard form = new RevMaxInterfaceWizard();
//                    form.WindowState = FormWindowState.Minimized;
//                    form.ShowInTaskbar = false;
//                    Application.Run();

//                }

//                GC.KeepAlive(mutex);
//            }
//        }
//    }
//}
using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;

namespace Revmax_Interface_Promun
{
    internal static class Program
    {
        private const string MutexName = "Global\\Revmax_Interface_Promun_TrinityPharmacy";

        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(true, MutexName, out bool isNewInstance))
            {
                if (!isNewInstance)
                {
                    MessageBox.Show("Revmax Interface is already running.", "Revmax Interface",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                try
                {
                    if (ConfigurationManager.AppSettings.Get("Trained") == "0")
                    {
                        Application.Run(new Wizard());
                    }
                    else
                    {
                        RevMaxInterfaceWizard form = new RevMaxInterfaceWizard();
                        form.WindowState = FormWindowState.Minimized;
                        form.ShowInTaskbar = false;
                        Application.Run(form);
                    }
                }
                catch (Exception ex)
                {
                    // ex.ToString() includes the inner exceptions and stack traces
                    MessageBox.Show(ex.ToString(), "Revmax Interface - Startup Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}