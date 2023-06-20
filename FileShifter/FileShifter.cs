using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace FileShifter
{
    public class FileShifter
    {
        private string SourceFolder= ConfigurationManager.AppSettings["SourceFolder"];
        private string DestinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];
        private int No_Of_SubDir = Convert.ToInt32(ConfigurationManager.AppSettings["No_Of_SubDir"]);
        
        //Fn for creating sub sir
        public void Fn_Make_SubDir()
        {
            DirectoryInfo desDir = new DirectoryInfo(DestinationFolder);
            if (desDir.Exists)
            {
                string sub_dir_prefix = "Dest_";
                int postFix = 1;
                while (postFix <= No_Of_SubDir)
                {
                    Directory.CreateDirectory(DestinationFolder + "/" + sub_dir_prefix + postFix.ToString());
                    postFix++;
                }
            }
        }

        //fn for storing all the source file in one array
        public string[] Fn_ReadFilesFromSource()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(SourceFolder);
            if (directoryInfo.Exists)
            {
                FileInfo[] sourceFileList = directoryInfo.GetFiles("*.*");
                Console.WriteLine("Following files Found");
                foreach (var file in sourceFileList)
                {
                    System.Console.WriteLine(file.Name);
                }
                int totalFiles = sourceFileList.Length;
                System.Console.WriteLine("Total File Found: " + totalFiles);
                string[] fileNames=new string[totalFiles];
                for (int i= 0;i < totalFiles; i++)
                {
                    fileNames[i] = sourceFileList[i].FullName;
                }
                return fileNames;
            }
           else
            {
                System.Console.WriteLine("No Such Directory Present");
                return null;
            }
        }

        // Create threads based on number of sub dir and start moving the files using the threads
        public void Fn_MoveFilesToTarget(string[] fileList)
        {
            try
            {
                int totalFiles = fileList.Length;
                int filesPerThread = totalFiles / No_Of_SubDir;
                Thread[] threads = new Thread[No_Of_SubDir];
                DirectoryInfo[] subDirInfos = new DirectoryInfo(DestinationFolder).GetDirectories();
                int fileIndex = 0;
                int threadIndex = 0;
                int subDirIndex = 0;
                while (fileIndex < totalFiles)
                {
                    int filesToMove = Math.Min(filesPerThread, totalFiles - fileIndex);

                    Thread thread = new Thread(() =>
                    {
                        for (int i = 0; i < filesPerThread && fileIndex < totalFiles; i++)
                        {
                            string sourcePath = fileList[fileIndex];
                            string fileName = Path.GetFileName(sourcePath);
                            string destinationPath = Path.Combine(subDirInfos[subDirIndex].FullName, fileName);
                            Fn_MoveFile(sourcePath, destinationPath);

                            fileIndex++;
                        }
                    });

                    thread.Start();
                    threads[threadIndex] = thread;

                    fileIndex += filesToMove;
                    threadIndex++;
                    subDirIndex++;
                }

                // Wait for all threads to finish
                foreach (Thread thread in threads)
                {
                    thread.Join();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
      
        public void Fn_MoveFile(string sourcePath, string destinationPath)
        {
            try
            {
                File.Move(sourcePath, destinationPath);
                Console.WriteLine("Moved file: " + Path.GetFileName(sourcePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error moving file: " + ex.Message);
            }
        }
        public void StartFileMoveMent()
        {
            try
            {
                Fn_Make_SubDir();
                string[] files = Fn_ReadFilesFromSource();
                if (files!=null)
                {
                    Fn_MoveFilesToTarget(files);
                    Console.WriteLine("Files move  successfully");
                }
            }
            catch (Exception  ex)
            {
                Console.WriteLine("Error while Moving  Files: "+ex.Message);
            }

        }
    }
}
