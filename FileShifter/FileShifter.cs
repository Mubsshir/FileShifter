using System;
using System.Collections.Generic;
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
        public FileInfo[] Fn_ReadFilesFromSource()
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
               
                return sourceFileList;
            }
           else
            {
                System.Console.WriteLine("No Such Directory Present");
                return null;
            }
        }

        public void Fn_MoveFilesToTarget(FileInfo[] fileList)
        {
            try
            {
                int totalFiles = fileList.Length;
                DirectoryInfo[] subdirInfo = new DirectoryInfo(DestinationFolder).GetDirectories();
                int totalSubDir = subdirInfo.Length;
                int filePerSubDir =(int)Math.Ceiling( (double)totalFiles / totalSubDir);
                Thread.Sleep(2000);
                int fileIndex = 0;
                int subDirIndex = 0;
                List<Thread> threads=new List<Thread>() ;
                while (fileIndex < totalFiles && subDirIndex<No_Of_SubDir)
                {
                    int fileToMove = Math.Min(filePerSubDir, totalFiles - fileIndex);
                    Thread thread = new Thread(()=> {
                        for (int i = 0; i < fileToMove && (fileIndex + i) < totalFiles; i++)
                        {
                            string sourceFilePath = fileList[fileIndex + i].FullName;
                            string destination = Path.Combine(subdirInfo[subDirIndex].FullName, fileList[fileIndex+i].Name);
                            Fn_MoveFile(sourceFilePath, destination);
                        }
                    });
                    
                    thread.Start();
                   // threads.Add(thread);
                    thread.Join();
                    subDirIndex++;
                    fileIndex += fileToMove;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while processing files: "+ex.Message);
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
                FileInfo[] files = Fn_ReadFilesFromSource();
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
