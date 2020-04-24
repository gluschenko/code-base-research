using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace CodeBase.Tests
{
    /* Тесты составлены на основе подраздела "PATTERN FORMAT" оф.документации GIT:
     * https://git-scm.com/docs/gitignore#_pattern_format
     */
    [TestFixture]
    public class GitIgnoreReader_Unit
    {
        [Test]
        public void PathIsChilded()
        {
            var path = "C:/Data/Abc/Project";

            var cases = new Dictionary<string, bool>()
            {
                { "C:/Data/Abc/Project", true },
                { "C:/Data/Abc/NotProject", false },
                { "C:/Data/Abc/NotProject/NotChild", false },
                { @"C:\Data\Abc\Project", true },
                { "C:/Data/Abc/Project/Foo", true },
                { "C:/Data/Abc/Project/hello.c", true },
                { "C:/Data/Abc/Project/Abc/hello.py", true },
                { "C:/", false },
                { "C:/Data/Project", false },
                { "C:/Data/Project/Abc", false },
            };

            foreach (var pair in cases)
            {
                Assert.AreEqual(pair.Value, GitIgnoreReader.IsChildedPath(path, pair.Key), pair.Key);
            }
        }

        [Test]
        public void OneAsterisk()
        {
            GitIgnoreReader reader = new GitIgnoreReader();

            reader.Parse(new string[] { 
                "hello.*", 
                "hello*.*", 
                "node_modules/", 
            });

            var cases = new Dictionary<string, bool>()
            {
                { "hello.java", true },
                { "hello.cs", true },
                { "abc/hello_world.c", true },
                { "abc/foo/hello_hello.hello", true },
                { "hello_world", false },
                { "goodbye.c", false },
                { "bar/goodbye", false },

                { "C:/foo/.git/", true },
                { "C:/foo/.git/abc", true },
                { "C:/foo/node_modules/", true },
                { "C:/foo/node_modules/abc", true },
            };

            foreach (var pair in cases)
            {
                var (pos, neg) = reader.GetMatches(pair.Key);

                Console.WriteLine(pair.Key + ": ");
                Console.WriteLine(reader.MatchesToString(pos, neg));

                Assert.AreEqual(pair.Value, !reader.IsMatch(pair.Key), pair.Key);
            }
        }

        /* Из доки:
         * A leading "**" followed by a slash means match in all directories. 
         * For example, "** /foo" matches file or directory "foo" anywhere, 
         * the same as pattern "foo". "** /foo/bar" matches file or 
         * directory "bar" anywhere that is directly under directory "foo".
         */
        [Test]
        public void TwoAsterisksAtStart()
        {
            GitIgnoreReader reader = new GitIgnoreReader();

            reader.Parse(new string[] { "**/foo", "**/foo/bar" });

            var cases = new Dictionary<string, bool>()
            {
                { "foo", true },
                { "aa/foo", true },
                { "aa/bb/foo", true },
                { "foo/bar", true },
                { "aa/bb/foo/bar", true },
                { "bar", false },
                { "aaa/bar", false },
                { "zhopa", false },
            };

            foreach (var pair in cases)
            {
                Assert.AreEqual(pair.Value, !reader.IsMatch(pair.Key), pair.Key);
            }
        }

        /* Из доки:
         * A trailing "/**" matches everything inside. For example, "abc/**" matches all files 
         * inside directory "abc", relative to the location of the .gitignore file, with infinite depth.
         */
        [Test]
        public void TwoAsterisksBeforeEnd()
        {
            GitIgnoreReader reader = new GitIgnoreReader();

            reader.Parse(new string[] { "abc/**" });

            var cases = new Dictionary<string, bool>()
            {
                { "abc", true },
                { "abc/bbb", true },
                { "abc/abc", true },
                { "abc/bbb/ccc", true },
                { "bbb/ccc", false },
                { "bbb", false },
                { "zhopa", false },
            };

            foreach (var pair in cases)
            {
                Assert.AreEqual(pair.Value, !reader.IsMatch(pair.Key), pair.Key);
            }
        }

        /* Из доки:
         * A slash followed by two consecutive asterisks then a slash matches 
         * zero or more directories. For example, 'a/** /b' matches 'a/b',
         * 'a/x/b', 'a/x/y/b' and so on.
         */
        [Test]
        public void TwoAsterisksInMiddle()
        {
            GitIgnoreReader reader = new GitIgnoreReader();

            reader.Parse(new string[] { "a/**/b" });

            var cases = new Dictionary<string, bool>()
            {
                { "a/", false },
                { "/b", false },
                { "axb", false },
                { "ab", false },
                { "a/b", true },
                { "a/x/b", true },
                { "a/x/y/b", true },
                { "a/x/y/z/b", true },
                { "a/x/y/z/b/sub", true },
            };

            foreach (var pair in cases)
            {
                Assert.AreEqual(pair.Value, !reader.IsMatch(pair.Key), pair.Key);
            }
        }

        [Test]
        public void NegativeRules()
        {
            var cases = new Dictionary<string, bool>()
            {
                { ".git/", false },
                { "aaa", false },
                { "!aaa", true },
                { "important!.txt", false },
                { "!important!.txt", true },
            };

            GitIgnoreReader reader = new GitIgnoreReader();
            reader.Parse(cases.Keys.ToArray());

            foreach (var rule in reader.Rules)
            {
                Assert.AreEqual(cases[rule.Source], rule.IsNegative, $"{rule.Source} -> {rule.Pattern}");
            }
        }
    }
}