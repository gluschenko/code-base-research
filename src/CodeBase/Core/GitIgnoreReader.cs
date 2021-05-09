using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PathIO = System.IO.Path;

namespace CodeBase
{
    /* 
     * Данный класс является контейнером правил, выделенных из .gitignore файлов, найденных в репозитории.
     * Также здесь содержатся статические методы для парсинга.
     * 
     * Парсинг выполняется, согласно формата, изложенного в официальной документации: 
     * https://git-scm.com/docs/gitignore#_pattern_format
     * 
     * Решение с решулярками частично подсмотрено здесь:
     * https://github.com/codemix/gitignore-parser/blob/master/lib/index.js
     * Базовое решение не покрывало все кейсы. Пришлось писать юнит-тесты и фиксить руками, 
     * а также делать валидацию синтаксиса регулярных выражений.
     */

    /// <summary>
    /// Reads .gitignore files
    /// </summary>
    public class GitIgnoreReader
    {
        const string GIT_FILE_NAME = ".gitignore";
        const string GIT_DEFAULT_DIRECTORY = ".git/";
        // public
        /// <summary>Path to the .gitignore file</summary>
        public string Path { get; private set; }

        /// <summary>Directiry where .gitignoe is located</summary>
        public string BaseDir { get; private set; }

        /// <summary>Does this reader instance is ready to work?</summary>
        public bool IsParsed { get => _positives != null && _negatives != null; }

        public IReadOnlyCollection<GitIgnoreRule> Rules { get => _rules; }
        // private
        private readonly List<GitIgnoreRule> _rules;
        private Regex _positives, _negatives;

        // ctor
        public GitIgnoreReader(string path = "")
        {
            Path = PreparePath(path, relative: false);
            BaseDir = PathIO.GetDirectoryName(Path) ?? "";

            _rules = new List<GitIgnoreRule>();
        }

        #region PUBLIC

        public static GitIgnoreReader Load(string path)
        {
            if (!path.EndsWith(GIT_FILE_NAME))
                path = PathIO.Combine(path, GIT_FILE_NAME);

            var reader = new GitIgnoreReader(path);

            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                reader.Parse(lines);
            }

            return reader;
        }

        public void Parse(string[] lines)
        {
            var rules = new string[] { GIT_DEFAULT_DIRECTORY } // Git исключает из репозитория сам себя (c) Кэп
                .Concat(lines)
                .Select(s => s.Trim())
                .Where(s => !s.StartsWith("#")         // Исключаем комментарии
                    && !string.IsNullOrWhiteSpace(s)   // Пустые строки
                    && !s.All(ch => ch == '-')
                    && !s.All(ch => ch == '_'));       // Исключаем строки разделения ("black lines" в документации Git)

            _rules.Clear();
            _rules.AddRange(rules.Select(r => new GitIgnoreRule(r)));

            CreateRegex();
        }

        public bool IsMatch(string path)
        {
            if (IsParsed)
            {
                path = PreparePath(path, relative: true);
                return !_positives.IsMatch(path) || _negatives.IsMatch(path);
            }
            return true;
        }

        public (MatchCollection pos, MatchCollection neg) GetMatches(string path)
        {
            if (IsParsed)
            {
                path = PreparePath(path, relative: true);
                return (_positives.Matches(path), _negatives.Matches(path));
            }
            return (null, null);
        }

        public string MatchesToString(MatchCollection pos, MatchCollection neg)
        {
            static string Bake(MatchCollection matches)
            {
                var mt = matches.Cast<Match>().Select(m =>
                {
                    var res = new List<string>();
                    for (int i = 0; i < m.Groups.Count; i++)
                    {
                        var group = m.Groups[i];
                        var captures = new List<string>();
                        for (int j = 0; j < group.Captures.Count; j++)
                        {
                            captures.Add(group.Captures[j].Value);
                        }

                        if (captures.Count > 0)
                            res.Add($"{string.Join(", ", captures)} ({i})");
                    }
                    return string.Join(" | ", res);
                });
                return string.Join(" match -> ", mt);
            }

            var res = new List<string>();
            if (pos.Count > 0)
                res.Add("pos: " + Bake(pos) + "\n");
            if (neg.Count > 0)
                res.Add("neg: " + Bake(neg) + "\n");

            return string.Join("\n", res);
        }

        public string PreparePath(string path, bool relative)
        {
            path = path.Replace(@"\", "/");

            if (relative)
            {
                if (!string.IsNullOrEmpty(BaseDir) && path.StartsWith(BaseDir))
                {
                    path = path.Substring(BaseDir.Length);
                }
            }

            if (path.StartsWith("/"))
                path = path.Substring(1);

            return path;
        }

        public static string[] Find(string path, SearchOption search)
        {
            return Directory.GetFiles(path, GIT_FILE_NAME, search);
        }

        public static bool HasFile(string path)
        {
            path = PathIO.Combine(path, GIT_FILE_NAME);
            return File.Exists(path);
        }

        public static bool IsChildedPath(string parent, string child)
        {
            bool result = false;
            parent = parent.Replace('\\', '/').TrimEnd('\\', '/');
            child = child.Replace('\\', '/').TrimEnd('\\', '/');

            if (child.Length > parent.Length)
            {
                result = child.StartsWith(parent + '/');
            }

            if (child == parent)
            {
                result = true;
            }

            return result;

            /*var potentialBase = new Uri(parent);
            var potentialChild = new Uri(child);
            return potentialBase.IsBaseOf(potentialChild);*/
        }

        #endregion

        #region PRIVATE

        private void CreateRegex()
        {
            List<GitIgnoreRule>
                positiveRules = new List<GitIgnoreRule>(),
                negativeRules = new List<GitIgnoreRule>();

            _rules.ForEach(e =>
            {
                if (e.IsValid)
                {
                    if (e.IsNegative)
                    {
                        negativeRules.Add(e);
                    }
                    else
                    {
                        positiveRules.Add(e);
                    }
                }
            });
            //
            _positives = BuildRegex(positiveRules.Select(e => e.Pattern).ToArray());
            _negatives = BuildRegex(negativeRules.Select(e => e.Pattern).ToArray());
        }

        internal static string PrepareRegexPattern(string pattern)
        {
#if DEBUG
            Console.WriteLine(pattern);
#endif

            pattern = pattern
                .Replace(".", @"\.");

            if (pattern.StartsWith("**/"))
            {
                pattern = pattern.Substring(3);
                pattern = "(.+|.?)(/?)" + pattern;
            }

            if (pattern.EndsWith("/**"))
            {
                pattern = pattern.Substring(0, pattern.Length - 3);
                pattern += "(/?)(.+|.?)";
            }

            /*if (pattern.EndsWith("/"))
            {
                pattern = pattern.TrimEnd('/');
                pattern += "(/?)";
            }*/

            pattern = pattern
                .Replace("/**/", "(/?)(.+|.?)/") // ?. -- для случая, когда последовательность пуста и есть только '/'
                                                 //
                .Replace("/", @"\/")
                .Replace("**", "(.+)")      // любая последрвательность любых символов
                .Replace("*", @"([^\/]+)"); // аналогично за исключением '/'

#if DEBUG
            Console.WriteLine(pattern);
#endif

            return pattern;
        }

        internal static Regex BuildRegex(string[] items)
        {
            string regex = items.Length > 0 ?
                "((" + string.Join(")|(", items) + "))" : "$^";

            return new Regex(regex);
        }

        #endregion
    }

    public struct GitIgnoreRule
    {
        public string Source { get; private set; }
        public string Pattern { get; private set; }
        public bool IsNegative { get; private set; }
        public bool IsValid { get; private set; }

        public GitIgnoreRule(string text)
        {
            Source = text;
            Pattern = "";
            IsNegative = false;
            IsValid = true;
            //
            Bake();
            Validate();
        }

        #region PRIVATE
        private void Bake()
        {
            if (!string.IsNullOrWhiteSpace(Source))
            {
                var rule = Source;

                IsNegative = rule.StartsWith("!");

                if (IsNegative)
                    rule = rule.Substring(1);

                if (rule.StartsWith("/"))
                    rule = rule.Substring(1);

                Pattern = GitIgnoreReader.PrepareRegexPattern(rule);
            }
        }

        private void Validate()
        {
            if (!string.IsNullOrWhiteSpace(Pattern))
            {
                IsValid = true;

                try
                {
                    Regex.IsMatch("", "(" + Pattern + ")");
                }
                catch
                {
                    IsValid = false;
                }
            }
            else
            {
                IsValid = false;
            }
        }
        #endregion
    }
}
