﻿using Moq;
using NumberMP3FileNames.ApplicationServices;
using NUnit.Framework;

namespace NumberMP3FileNames.Test.ApplicationServices
{
    [TestFixture]
    public class MusicFileServiceTests
    {
        private Mock<MusicTagService> musicTagServiceFake;
        private Mock<FileIOService> fileIOServiceFake;

        [SetUp]
        public void SetUpTests()
        {
            musicTagServiceFake = new Mock<MusicTagService>();
            fileIOServiceFake = new Mock<FileIOService>();
        }

        [TestCase(@"C:\Music\Alvvays\Alvvays", false)]
        [TestCase(@"C:\Music\Amazon MP3\Belle And Sebastian\The Boy With The Arab Strap", true)]
        [TestCase(@"C:\Music\Beabadoobee\Beabadoobee - Space Cadet - mp3", true)]
        public void SkipDirectory_MixOfPaths_ReturnsExpectedResult(string path, bool expected)
        {
            var sut = MakeSut();

            var actual = sut.SkipDirectory(path);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SkipFiles_FileNamesStartingWithTrackNumbersAndSpaces_ReturnsTrue()
        {
            var files = GetFilesStartingWithTrackNumbersAndSpaces();
            var sut = MakeSut();

            var actual = sut.SkipFiles(files);

            Assert.That(actual, Is.True);
        }

        [Test]
        public void SkipFiles_FileNamesStartingWithTrackNumbersAndDashes_ReturnsTrue()
        {
            var files = GetFilesStartingWithTrackNumbersAndDashes();
            var sut = MakeSut();

            var actual = sut.SkipFiles(files);

            Assert.That(actual, Is.True);
        }

        [Test]
        public void SkipFiles_FileNamesWithoutTrackNumbers_ReturnsFalse()
        {
            var files = GetFilesWithoutTrackNumbers();
            var sut = MakeSut();

            var actual = sut.SkipFiles(files);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void SkipFiles_FileNamesContainMatchingTrackNumbers_ReturnsTrue()
        {
            var files = GetFilesContainingTrackNumbers();
            musicTagServiceFake.Setup(f => f.GetTrackNumber(files[0])).Returns(1);
            musicTagServiceFake.Setup(f => f.GetTrackNumber(files[1])).Returns(2);
            var sut = MakeSut();

            var actual = sut.SkipFiles(files);
            
            Assert.That(actual, Is.True);
        }

        [Test]
        public void SkipFiles_FileNamesContainTrackNumbersThatDoNotMatch_ReturnsFalse()
        {
            var files = GetFilesContainingTrackNumbers();
            musicTagServiceFake.Setup(f => f.GetTrackNumber(files[0])).Returns(3);
            musicTagServiceFake.Setup(f => f.GetTrackNumber(files[1])).Returns(4);
            var sut = MakeSut();

            var actual = sut.SkipFiles(files);

            Assert.That(actual, Is.False);
        }

        private MusicFileService MakeSut()
        {
            return new MusicFileService(musicTagServiceFake.Object, fileIOServiceFake.Object);
        }

        private string[] GetFilesStartingWithTrackNumbersAndSpaces()
        {
            return new[]
            {
                @"C:\Music\Big Star\#1 Record-Radio City [Bonus Tracks]\01 Feel.mp3",
                @"C:\Music\Big Star\#1 Record-Radio City [Bonus Tracks]\02 The Ballad of el Goodo.mp3"
            };
        }

        private string[] GetFilesStartingWithTrackNumbersAndDashes()
        {
            return new[]
            {
                @"C:\Music\Buffalo Springfield\Buffalo Springfield [Collection]\01-For What It's Worth.mp3",
                @"C:\Music\Buffalo Springfield\Buffalo Springfield [Collection]\02-Sit Down I Think I Love You.mp3",
                @"C:\Music\Buffalo Springfield\Buffalo Springfield [Collection]\03-Nowadays Clancy Can't Even Sing.mp3"
            };
        }

        private string[] GetFilesContainingTrackNumbers()
        {
            return new[]
            {
                @"C:\Music\The Beths\Expert In A Dying Field\The Beths - Expert In A Dying Field - 01 Expert In A Dying Field.mp3",
                @"C:\Music\The Beths\Expert In A Dying Field\The Beths - Expert In A Dying Field - 02 Knees Deep.mp3"
            };
        }


        private string[] GetFilesWithoutTrackNumbers()
        {
            return new[]
            {
                @"C:\Music\Buffalo Springfield\Buffalo Springfield [Collection]\For What It's Worth.mp3",
                @"C:\Music\Buffalo Springfield\Buffalo Springfield [Collection]\Sit Down I Think I Love You.mp3",
                @"C:\Music\Buffalo Springfield\Buffalo Springfield [Collection]\Nowadays Clancy Can't Even Sing.mp3"
            };
        }
    }
}
