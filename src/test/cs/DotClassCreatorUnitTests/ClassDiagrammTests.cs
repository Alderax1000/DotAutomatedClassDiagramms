
using NUnit.Framework;

namespace DotClassCreatorUnitTests
{
    public class ClassDiagrammTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void giveNoFilePathForParsing()
        {
            var noFilePath = "";
            isFirstArgumentAnExistingFilePath(noFilePath)
            Assert.Pass();
        }
    }
}