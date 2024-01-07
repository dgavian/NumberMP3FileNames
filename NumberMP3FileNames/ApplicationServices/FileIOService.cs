using System.IO;

namespace NumberMP3FileNames.ApplicationServices
{
    public class FileIOService
    {
        public virtual void RenameFile(string filePath, string newFileName)
        {
            var dir = Path.GetDirectoryName(filePath);
            var newFilePath = Path.Combine(dir, newFileName);
            File.Move(filePath, newFilePath);
        }
    }
}
