using Lucene.Net.Index;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Lufi
{
    public class MainThread
    {
        #region Variables
        List<DriveInfo> diskDrives = new List<DriveInfo>();
        ConcurrentQueue<FileFolder> MainQueue = new ConcurrentQueue<FileFolder>();
        int DocumentsInserted = 0;
        int unDeniedAcccessCount = 0;
        int ThreadCount = 0;

        string RegKey = @"Software\Lufi";
        List<BackgroundWorker> bThreads = new List<BackgroundWorker>();
        bool FullDiskIndexingInProgress = false;
        LuceneManager manager = new LuceneManager(Infrastructure.IndexDir);
        #endregion
        public MainThread()
        {
            AppInitilaize();
            getDiskDrives();
            if (Registry.LocalMachine.OpenSubKey(RegKey+@"\FirstIndex")==null)
            {
                FirstIndex();
                Registry.LocalMachine.OpenSubKey(RegKey,true).SetValue("FirstIndex", DateTime.Now.ToString(), RegistryValueKind.String);
            }
        }

        public void AppInitilaize()
        {
            using (var key=Registry.LocalMachine.OpenSubKey(RegKey)) {
                if (key==null)
                {

                    Registry.LocalMachine.CreateSubKey(RegKey);
                    Registry.LocalMachine.OpenSubKey(RegKey,true).SetValue("RootDir", Infrastructure.rootDir, RegistryValueKind.String);
                    try
                    {
                        Directory.CreateDirectory(Infrastructure.IndexDir);
                        Directory.CreateDirectory(Infrastructure.LogDir);
                        Registry.LocalMachine.OpenSubKey(RegKey,true).SetValue("FirstStart", DateTime.Now.ToString(), RegistryValueKind.String);
                    }
                    catch (UnauthorizedAccessException )
                    {

                        MessageBox.Show("Cannot create directory.Please make sure you have write permissions to folder:\n" + Infrastructure.rootDir);
                    }
                    catch (Exception ){

                    }
                }
            
            }
        }
        public void getDiskDrives()
        {

            foreach (var dDrive in DriveInfo.GetDrives())
            {
                if (dDrive.IsReady && dDrive.DriveType == DriveType.Fixed)
                {
                    diskDrives.Add(dDrive);
                    
                }
            }

        }

        public void FirstIndex()
        {
            FullDiskIndexingInProgress = true;
            foreach (var drive in diskDrives)
            {
                GetAllFiles(drive.RootDirectory.FullName);

            }
            while (bThreads.Count>0)
            {
                
            }

            manager.CloseWriter();
            FullDiskIndexingInProgress = false;
        }
        
        public void GetAllFiles(string path)
        {

            try
            {
               
                foreach (var folder in System.IO.Directory.GetDirectories(path))
                {
                    MainQueue.Push(new FileFolder(folder));
                    while (bThreads.Count <= 4)
                    {
                        
                        BackgroundWorker bw = new BackgroundWorker();
                        bw.DoWork += bw_DoWork;
                        bw.RunWorkerCompleted += bw_RunWorkerCompleted;
                        bw.RunWorkerAsync();
                        bThreads.Add(bw);
                       
                        
                    }
                    GetAllFiles(folder);
                }
                
            }
            catch (UnauthorizedAccessException ex)
            {

                unDeniedAcccessCount += 1;
            }

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
           
            bThreads.Remove(bw);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            Process();

            
        }
       
        public void Process()
        {
            while (MainQueue.Any())
            {
                var dir = MainQueue.Pop();
                if (dir!=null)
                {
                     DocumentsInserted=manager.InsertDocument(dir)?Utililties.ConcurrentIncrement(ref DocumentsInserted):DocumentsInserted;

                try
                {
                    foreach (string filePath in System.IO.Directory.GetFiles(dir.FilePath))
                    {
                        var fd = new FileFolder(filePath);

                        DocumentsInserted = manager.InsertDocument(dir) ? Utililties.ConcurrentIncrement(ref DocumentsInserted) : DocumentsInserted;
                    }
                    if (DocumentsInserted%20000==0)
                    {
                        manager.FushAndOptimize();
                    }

                }
                catch (UnauthorizedAccessException ex)
                {
                    unDeniedAcccessCount += 1;

                }
                }
               

            }

            ThreadCount -= 1;
        }
    }
}
