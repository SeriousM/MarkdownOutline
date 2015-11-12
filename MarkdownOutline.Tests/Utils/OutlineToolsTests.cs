using MarkdownOutline.Utils;
using NUnit.Framework;

namespace MarkdownOutline.Tests.Utils
{
    [TestFixture]
    public class OutlineToolsTests
    {
        [Test]
        public void CountHash_EmptyString_ReturnZero()
        {
            // arrange
            var line = string.Empty;

            // act
            var count = OutlineTools.CountHash(line);

            // assert
            Assert.AreEqual(0, count);
        }

        [Test]
        public void CountHash_WhitespaceString_ReturnZero()
        {
            // arrange
            var line = new string(' ', 100);

            // act
            var count = OutlineTools.CountHash(line);

            // assert
            Assert.AreEqual(0, count);
        }

        [Test]
        public void CountHash_StartsWith1Hash_Return1()
        {
            // arrange
            var line = "#";

            // act
            var count = OutlineTools.CountHash(line);

            // assert
            Assert.AreEqual(1, count);
        }

        [Test]
        public void CountHash_StartsWith2Hash_Return2()
        {
            // arrange
            var line = "##";

            // act
            var count = OutlineTools.CountHash(line);

            // assert
            Assert.AreEqual(2, count);
        }

        [Test]
        public void CountHash_StartsWith2HashThenWithSpaceAndHash_Return2()
        {
            // arrange
            var line = "## #";

            // act
            var count = OutlineTools.CountHash(line);

            // assert
            Assert.AreEqual(2, count);
        }

        [Test]
        public void CountHash_StartsWhitespaceThenWith2Hash_Return2()
        {
            // arrange
            var line = " ##";

            // act
            var count = OutlineTools.CountHash(line);

            // assert
            Assert.AreEqual(2, count);
        }

        [Test]
        public void ParseOutline_SingleEmptyLine_ReturnOneBlock()
        {
            // arrange
            var lines = new[] { string.Empty };

            // act
            var blocks = OutlineTools.ParseOutline(lines);

            // assert
            Assert.AreEqual(1, blocks.Count);
        }

        [Test]
        public void ParseOutline_EmptyLineArray_ReturnNoBlocks()
        {
            // arrange
            var lines = new string[] { };

            // act
            var blocks = OutlineTools.ParseOutline(lines);

            // assert
            Assert.AreEqual(0, blocks.Count);
        }

        [Test]
        public void ParseOutline_EmptyLineThenWithHash_Return2Blocks()
        {
            // arrange
            var lines = new[] { string.Empty, "#" };

            // act
            var blocks = OutlineTools.ParseOutline(lines);

            // assert
            Assert.AreEqual(2, blocks.Count);
            Assert.AreEqual(1, blocks[1].Level);
            Assert.AreEqual("#", blocks[1].Lines[0]);
        }

        [Test]
        public void ParseOutline_EmptyLineThenWithHashThenEmptyLineThenWithTwoHash_Return3Blocks()
        {
            // arrange
            var lines = new[] { string.Empty, "#", string.Empty, "##" };

            // act
            var blocks = OutlineTools.ParseOutline(lines);

            // assert
            Assert.AreEqual(3, blocks.Count);
            Assert.AreEqual(2, blocks[1].Lines.Count);
        }

        [Test]
        public void ParseOutline_WithHashThenEmptyLineThenWithTwoHash_Return3Blocks()
        {
            // arrange
            var lines = new[] { "#", string.Empty, "##" };

            // act
            var blocks = OutlineTools.ParseOutline(lines);

            // assert
            Assert.AreEqual(2, blocks.Count);
            Assert.AreEqual(1, blocks[1].Lines.Count);
        }

        [Test]
        public void ParseOutline_WithHashThenEmptyLineThenWithTwoHashThenWithEmptyLine_Return3Blocks()
        {
            // arrange
            var lines = new[] { "#", string.Empty, "##", string.Empty };

            // act
            var blocks = OutlineTools.ParseOutline(lines);

            // assert
            Assert.AreEqual(2, blocks.Count);
            Assert.AreEqual(2, blocks[1].Lines.Count);
        }
    }
}
