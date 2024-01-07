using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NumberMP3FileNames.ApplicationServices
{
    public class MusicFileService
    {
        private readonly MusicTagService _tagService;
        private readonly FileIOService _fileService;

        public MusicFileService(MusicTagService tagService, FileIOService fileService)
        {
            _tagService = tagService;
            _fileService = fileService;
        }

        public bool SkipDirectory(string path)
        {
            var pattern = @"\\(Amazon MP3|Beabadoobee|Backing Tracks|BBBT|My Recordings|__MACOSX)\\?";
            return Regex.IsMatch(path, pattern);
        }

        public bool SkipFiles(IList<string> filePaths)
        {

            var fileNames = filePaths.Select(Path.GetFileName);
            return AllStartWithTrackNumber(fileNames) || AllContainTrackNumber(fileNames, filePaths);
        }

        public bool IsBandcamp(string path)
        {
            var pattern = @"\\bandcamp\\";
            return Regex.IsMatch(path, pattern);
        }

        public void FixBandcampFileNames(IList<string> filePaths)
        {
            var pattern = @"(?<= - )\d{2} .+\.mp3$";
            foreach (var f in filePaths)
            {
                var fileName = Path.GetFileName(f);
                var m = Regex.Match(fileName, pattern);
                if (m.Success)
                {
                    var newFileName = m.Value;
                    _fileService.RenameFile(f, newFileName);
                    Console.WriteLine("Successfully renamed {0} to {1}", fileName, newFileName);
                }
                else
                {
                    Console.WriteLine("Skipping bandcamp file {0}", fileName);
                }
            }
        }

        public void PrependNumberToFiles(IList<string> filePaths)
        {
            foreach (var f in filePaths)
            {
                var fileName = Path.GetFileName(f);
                var trackNumber = _tagService.GetTrackNumber(f);
                if (trackNumber == 0)
                {
                    Console.WriteLine("Skipping {0}; file is missing track number", fileName);
                    continue;
                }
                var formattedTrackNumber = trackNumber.ToString("D2");
                var newFileName = $"{formattedTrackNumber}-{fileName}";
                _fileService.RenameFile(f, newFileName);
                // Console.WriteLine("Successfully renamed {0} to {1}", fileName, newFileName);
            }
        }

        private bool AllStartWithTrackNumber(IEnumerable<string> fileNames)
        {
            var pattern = @"^\d{2}( |-)";
            return fileNames.All(f => Regex.IsMatch(f, pattern));
        }

        private bool AllContainTrackNumber(IEnumerable<string> fileNames, IEnumerable<string> filePaths)
        {
            var findPattern = @"( |-)?\d{2}( |-)";
            if (fileNames.All(f => Regex.IsMatch(f, findPattern)))
            {
                var matchPattern = @"\d{2}(?=( |-))";
                foreach (var filePath in filePaths)
                {
                    var fileNameTrackNumber = Regex.Match(Path.GetFileName(filePath), matchPattern).Value;
                    var trackNumber = _tagService.GetTrackNumber(filePath);
                    if (int.Parse(fileNameTrackNumber) != trackNumber)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
