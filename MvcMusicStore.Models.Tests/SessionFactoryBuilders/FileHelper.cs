using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MvcMusicStore.Models.Tests.SessionFactoryBuilders
{
    public class FileHelper
    {
        public static string GetDbFileName()
        {
            var path = Path.GetFullPath(Path.GetRandomFileName() + ".Test.db");
            return !File.Exists(path) ? path : GetDbFileName();
        }

        public static void DeletePreviousDbFiles()
        {
            var files = Directory.GetFiles(".", "*.Test.db*");
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
