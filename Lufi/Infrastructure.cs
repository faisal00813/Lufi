using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lufi
{
    public static class Infrastructure
    {
        public static string rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string IndexDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Index";
        public static string LogDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Log"; 
    }
    public class FileFolder
    {
        public FileFolder(string filePath)
        {
            _FilePath = filePath;
            if (FilePath == "")
            {

                Name = this.FilePath;
            }
            else
            {

                var _tempPath = FilePath.Split('\\');
                string FileName = "";
                if (_tempPath.Length > 0)
                {
                    FileName = _tempPath[_tempPath.Length - 1];
                }
                var fileStruct = FileName.Split('.');
                string ext = "";
                if (fileStruct.Length > 0)
                {
                    ext = fileStruct[fileStruct.Length - 1];
                }
                if (FileName.Contains(".") && ext.Length > 0)
                {
                    Type = FileType.File;
                }
                else
                {
                    Type = FileType.Folder;
                }
                Name = FileName;
            }
        }

        public string Name { get; set; }


        private string _FilePath;

        public string FilePath
        {
            get { return _FilePath; }

        }

        public FileType Type { get; set; }

        public override string ToString()
        {
            return Name;
        }
        public enum FileType
        {
            File = 0,
            Folder = 1
        }

        
    }
}
