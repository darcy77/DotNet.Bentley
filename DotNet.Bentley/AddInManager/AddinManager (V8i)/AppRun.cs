using AddinManager.View;
using Bentley.MicroStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddinManager
{
    [AddIn(KeyinTree = "CommandTable.xml", MdlTaskID = "AppRun")]
    public class AppRun : AddIn
    {
        protected AppRun(IntPtr mdlDescriptor)
            : base(mdlDescriptor)
        {

        }

        protected override int Run(string[] commandLine)
        {
            return 0;
        }
    }

    class AppRunKeyins
    {
        public static void ShowMain(string unparsed)
        {
            var main = new MainAddinManager();

            main.ShowDialog();
        }
    }
}
