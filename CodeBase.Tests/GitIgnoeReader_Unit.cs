using CodeBase;
using NUnit.Framework;

namespace CodeBase.Tests
{
    [TestFixture]
    public class GitIgnoeReader_Unit
    {
        [SetUp]
        public void Setup()
        {
        }

        /* https://regex101.com/ */
        [Test]
        public void Test1()
        {
            GitIgnoreReader reader = new GitIgnoreReader("");
            reader.ParseRules(new string[] {
                ".git/",
                ".vs/",
            });

            Assert.Pass();
        }
    }
}