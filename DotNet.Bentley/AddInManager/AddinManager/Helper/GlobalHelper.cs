using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AddInManager.Helper
{
    static class GlobalHelper
    {
        public static string WorkPath = Environment.CurrentDirectory;

        public static string AddInManagerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AddIn Manager V1.0");
        public static string AddInManagerTempPath = Path.Combine(AddInManagerPath, "Temp");
        public static string AddInManagerAssemblyFile = Path.Combine(AddInManagerPath, "Addin Assembly File.json");


        /// <summary>
        /// 删除临时目录
        /// </summary>

        public static void DeleteTemp()
        {
            try
            {
                if (Directory.Exists(AddInManagerTempPath))
                {
                    Directory.Delete(AddInManagerTempPath, true);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 拷贝指定文件和此文件目录下的所有文件到临时目录，并返回临时目录所在的新文件路径.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        public static string CopyFileTo(string file)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException();
            }

            var path = Path.GetDirectoryName(file);
            var files = Directory.GetFiles(path);

            var newPath = Path.Combine(AddInManagerTempPath, Guid.NewGuid().ToString().Replace("-", ""));
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            var result = Path.GetFileName(file);

            for (int i = 0; i < files.Length; i++)
            {
                var fileName = files[i].Replace(path + "\\", "");

                var newfile = Path.Combine(newPath, fileName);

                var p = Path.GetDirectoryName(newfile);

                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }

                File.Copy(files[i], newfile, true);

                if (fileName == result)
                {
                    result = newfile;
                }
            }

            return result;
        }
    }
}
