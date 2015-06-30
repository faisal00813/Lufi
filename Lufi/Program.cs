using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lufi
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] Args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
                      
            if (Args.Length>0)
            {
                switch (Args[0])
                {
                    case "first":
                        //Application.Run();  
                        MainThread main = new MainThread();
                        //main.FirstIndex();
                        break;
                    default:
                        MessageBox.Show("Application cannot be started without arguments!");
                        Application.Exit();

                        break;
                }
            }
            else
            {
                MessageBox.Show("Application cannot be started without arguments!");
                Application.Exit();
            }
           
            
        }
    }
}
