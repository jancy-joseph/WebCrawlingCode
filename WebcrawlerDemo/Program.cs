using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;


namespace WebcrawlerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string sourceUrl = "https://nvd.nist.gov/feeds/json/cve/1.1/nvdcve-1.1-2019.json.zip";
            string targetdownloadedFile = @"C:\Test\TestZip.zip";
            DownloadManager downloadManager = new DownloadManager();
            downloadManager.DownloadFile(sourceUrl, targetdownloadedFile);

            string extractPath = @"C:\Test\extract";
            ZipFile.ExtractToDirectory(targetdownloadedFile, extractPath);

            DirectoryInfo directorySelected = new DirectoryInfo(extractPath);

            foreach (FileInfo fileToDecompress in directorySelected.GetFiles("*.json"))
            {
                Console.WriteLine("Found Json file");
                JObject o1 = JObject.Parse(File.ReadAllText(fileToDecompress.ToString()));
                var resultDict = ToDictionary(o1);
                // Display all keys and values.
                foreach (KeyValuePair<string, object> pair in resultDict)
                {
                    Console.WriteLine(pair);
                }
                //    Console.WriteLine(o1 );
            }

            //WebClient webClient = new WebClient();
            //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloader_DownloadProgressChanged);
            //webClient.DownloadFileAsync(new Uri("https://nvd.nist.gov/feeds/json/cve/1.1/nvdcve-1.1-2019.json.gzip"), @"c:\mytest\nvdcve-1.1-2019.json.gzip");

            ////Console.ReadKey();
            ////string directoryPath = @"c:\mytest";
            ////DirectoryInfo directorySelected = new DirectoryInfo(directoryPath);

            ////foreach (FileInfo fileToCompress in directorySelected.GetFiles())
            ////{
            ////    Compress(fileToCompress);
            ////}


            ////foreach (FileInfo fileToDecompress in directorySelected.GetFiles(" *.gzip"))
            ////{
            ////    Decompress(fileToDecompress);
            ////}
            Console.ReadKey();
        }

        //private static void downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    Console.WriteLine(e.BytesReceived + " " + e.ProgressPercentage);
        //    //throw new NotImplementedException();
        //}

        //private static void Completed(object sender, AsyncCompletedEventArgs e)
        //{
        //    Console.WriteLine("Download Completed");
        //    //throw new NotImplementedException();
        //}
        //public static void Compress(FileInfo fileToCompress)
        //{
        //    using (FileStream originalFileStream = fileToCompress.OpenRead())
        //    {
        //        if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
        //        {
        //            using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
        //            {
        //                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
        //                {
        //                    originalFileStream.CopyTo(compressionStream);
        //                    Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
        //                        fileToCompress.Name, fileToCompress.Length.ToString(), compressedFileStream.Length.ToString());
        //                }
        //            }
        //        }
        //    }
        //}

        //public static void Decompress(FileInfo fileToDecompress)
        //{
        //    using (FileStream originalFileStream = fileToDecompress.OpenRead())
        //    {
        //        string currentFileName = fileToDecompress.FullName;
        //        string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

        //        using (FileStream decompressedFileStream = File.Create(newFileName))
        //        {
        //            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
        //            {
        //                decompressionStream.CopyTo(decompressedFileStream);
        //                Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
        //            }
        //        }
        //    }


        //}

        public static IDictionary<string, object> ToDictionary(JObject @object)
        {
            var result = @object.ToObject<Dictionary<string, object>>();

            var JObjectKeys = (from r in result
                               let key = r.Key
                               let value = r.Value
                               where value.GetType() == typeof(JObject)
                               select key).ToList();

            var JArrayKeys = (from r in result
                              let key = r.Key
                              let value = r.Value
                              where value.GetType() == typeof(JArray)
                              select key).ToList();

            JArrayKeys.ForEach(key => result[key] = ((JArray)result[key]).Values().Select(x => ((JValue)x).Value).ToArray());
            JObjectKeys.ForEach(key => result[key] = ToDictionary(result[key] as JObject));

            return result;
        }


    }
}
