namespace NumberMP3FileNames.ApplicationServices
{
    public class MusicTagService
    {
        public virtual int GetTrackNumber(string filePath)
        {
            var tagLibFile = TagLib.File.Create(filePath);
            var trackNumber = tagLibFile.Tag.Track;
            return (int)trackNumber;
        }
    }
}
