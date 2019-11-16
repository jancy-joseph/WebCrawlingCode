﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Net;

namespace WebcrawlerDemo
{
    class DownloadManager
    {
        public void DownloadFile(string sourceUrl, string targetFolder)
        {
            WebClient downloader = new WebClient();
            // fake as if you are a browser making the request.
            downloader.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
            downloader.DownloadFileCompleted += new AsyncCompletedEventHandler(Downloader_DownloadFileCompleted);
            downloader.DownloadProgressChanged +=
                new DownloadProgressChangedEventHandler(Downloader_DownloadProgressChanged);
            downloader.DownloadFileAsync(new Uri(sourceUrl), targetFolder);
            // wait for the current thread to complete, since the an async action will be on a new thread.
            while (downloader.IsBusy) { }
        }

        private void Downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // print progress of download.
            Console.WriteLine(e.BytesReceived + " " + e.ProgressPercentage);
        }

        private void Downloader_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // display completion status.
            if (e.Error != null)
                Console.WriteLine(e.Error.Message);
            else
                Console.WriteLine("Download Completed!!!");
        }
    }
}
