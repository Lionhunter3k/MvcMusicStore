using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHibernate.Cfg;
using System.Text;
using System.IO;
using System.IO.Compression;
using Castle.MicroKernel.Facilities;
using NHibernate.Tool.hbm2ddl;
using Castle.MicroKernel.Registration;
using NHibernate;

namespace MvcMusicStore.Infrastructure.Persistence
{
    public class FileUtil
    {
        public static string ResolveFile(string fName)
        {
            if (fName != null && fName.StartsWith("~"))
            {
                // relative to run directory
                fName = fName.Substring(1);
                if (fName.StartsWith("/") || fName.StartsWith("\\"))
                {
                    fName = fName.Substring(1);
                }
                fName = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, fName);
            }

            return fName;
        }

        public static void WriteStreamToFile(Stream stream, string fileFullPath)
        {
            using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
            {
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);
                fileStream.Write(bytesInStream, 0, (int)bytesInStream.Length);
            }
        }

        public static byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        public static void Compress(string path, string name, string folder)
        {
            // Get the stream of the source file.
            using (FileStream inFile = new FileStream(FileUtil.ResolveFile(path), FileMode.Open))
            {
                // Create the compressed file.
                string filepath = string.Format("~/{0}/{1}", folder, name + ".zip");
                using (FileStream outFile = File.Create(FileUtil.ResolveFile(filepath)))
                {
                    using (GZipStream Compress = new GZipStream(outFile,
                            CompressionMode.Compress))
                    {
                        // Copy the source file into the compression stream.
                        byte[] buffer = new byte[4096];
                        int numRead;
                        while ((numRead = inFile.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            Compress.Write(buffer, 0, numRead);
                        }
                    }
                }
            }
        }

        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }
    }

    public class NHibernateFacility : AbstractFacility
    {
        protected override void Init()
        {
            var config = new Configuration().Configure();
            var schemaExport = new SchemaExport(config);
            schemaExport
            .SetOutputFile(FileUtil.ResolveFile(@"~\db.sql"))
            .Execute(true, true, false);
            Kernel.Register(
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod((kernel, context) => config.BuildSessionFactory()).Named("default.DBFactory"),
                Component.For<ISession>()
                    .UsingFactoryMethod((kernel, context) => kernel.Resolve<ISessionFactory>("default.DBFactory").OpenSession())
                    .Named("default.DBSession")
                    .LifestylePerWebRequest().IsDefault(),
                     Component.For<IStatelessSession>()
                    .UsingFactoryMethod((kernel, context) => kernel.Resolve<ISessionFactory>().OpenStatelessSession())
                    .Named("default.DBStatelessSession")
                    .LifestylePerWebRequest().IsDefault()
                );
        }
    }
}
