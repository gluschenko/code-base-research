using CodeBase;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace CodeBase.Tests
{
    [TestFixture]
    public class GitIgnoreReader_Unit
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            GitIgnoreReader reader = new GitIgnoreReader("");
            reader.Parse(new string[] {
                ".git/",
                ".vs/",
            });

            var cases = new Dictionary<string, bool>() 
            {
                { "/.git/objects/5t45g45g4g4", false },
                { "/.git/", false },
                { "/main.cs", true },
                { "src/main.cpp", true },
            };

            foreach (var pair in cases) 
            {
                Assert.AreEqual(pair.Value, reader.IsMatch(pair.Key), pair.Key);
            }
        }
    }
}