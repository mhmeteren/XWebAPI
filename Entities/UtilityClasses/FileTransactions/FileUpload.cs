
using Entities.Enums;
using Microsoft.AspNetCore.Http;

namespace Entities.UtilityClasses.FileTransactions
{
    public class FileUpload
    {

        public  IFormFile File { get; set; }
        public  FolderPaths FilePath { get; set; }


        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set {fileName = String.Concat(value ?? Guid.NewGuid().ToString(), Path.GetExtension(File.FileName));}
        }

        public string? PastFileName { get; set; }

        public FileUpload(IFormFile file, FolderPaths filePath, string? pastFileName = null, string? filename = null)
        {
            File = file;
            FilePath = filePath;
            FileName = filename;
            PastFileName = pastFileName;
        }

    }
}
