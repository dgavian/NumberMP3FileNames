using NumberMP3FileNames.ApplicationServices;
using System;
using System.Configuration;
using System.IO;

namespace NumberMP3FileNames
{
    class Program
    {
        void Run()
        {
            var tagService = new MusicTagService();
            var fileIOService = new FileIOService();
            var fileService = new MusicFileService(tagService, fileIOService);

            var musicDir = ConfigurationManager.AppSettings["MusicDir"];
            var subDirs = Directory.GetDirectories(musicDir, "*", SearchOption.AllDirectories);

            foreach (var subDir in subDirs)
            {
                var di = new DirectoryInfo(subDir);
                if (fileService.SkipDirectory(subDir))
                {
                    Console.WriteLine("Skipping {0}", di.Name);
                    continue;
                }
                var files = Directory.GetFiles(subDir, "*.mp3", SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    if (fileService.IsBandcamp(subDir))
                    {
                        fileService.FixBandcampFileNames(files);
                    }
                    else if (fileService.SkipFiles(files))
                    {
                        Console.WriteLine("Skipping {0} mp3 files in {1}", files.Length, di.Name);
                    }
                    else
                    {
                        fileService.PrependNumberToFiles(files);
                        Console.WriteLine("Successfully processed {0} mp3 files in {1}", files.Length, di.Name);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            var p = new Program();

            var outputFile = ConfigurationManager.AppSettings["OutputFile"];

            if (!string.IsNullOrEmpty(outputFile))
            {
                Console.WriteLine("Console logs will be written to {0}", outputFile);

                using (var writer = new StreamWriter(outputFile))
                {
                    Console.SetOut(writer);
                    p.Run();
                }
            }
            else
            {
                p.Run();
            }
        }
    }
}
