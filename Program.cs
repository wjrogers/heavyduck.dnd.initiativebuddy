using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HeavyDuck.Dnd.InitiativeBuddy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // change the appearance of toolstrips from their hideous default
            ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;

            // start message loop
            Application.Run(new Forms.Main());
        }
    }
}
