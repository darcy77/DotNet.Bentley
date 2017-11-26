using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace AddInManager.Helper
{
    class AssemblyResolveHelper
    {
        string m_CurrentFile;

        public Assembly Registered(string file)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

                var newfile = GlobalHelper.CopyFileTo(file);
                var result = ResolveAssembly(newfile);

                m_CurrentFile = newfile;

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void UnRegistered()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            lock (this)
            {
                var assemblyName = new AssemblyName(args.Name);

                var path = Path.GetDirectoryName(m_CurrentFile);

                var file = string.Format("{0}.dll", System.IO.Path.Combine(path, assemblyName.Name));

                if (File.Exists(file))
                {
                    return ResolveAssembly(file);
                }

                return null;
            }
        }

        private Assembly ResolveAssembly(string file)
        {
            try
            {
                Monitor.Enter(this);
                return Assembly.LoadFile(file);
            }
            finally
            {
                Monitor.Exit(this);
            }
        }
    }
}
