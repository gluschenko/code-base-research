using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
        const string gitFileName = ".gitignore";
        // public
        public string Path { get; private set; }
        public string BaseDir { get; private set; }
        public bool IsParsed { get => positives != null && negatives != null; }
        public IReadOnlyCollection<GitIgnoreRule> Rules { get => rules_; }
        // private
        private readonly List<GitIgnoreRule> rules_;
        private Regex positives, negatives;

        // ctor
        public GitIgnoreReader(string path = "")
        {
            Path = PreparePath(path, relative: false);
            BaseDir = PathIO.GetDirectoryName(Path) ?? "";

            rules_ = new List<GitIgnoreRule>();
        }

        #region PUBLIC

        public static GitIgnoreReader Load(string path)
        {
            if (!path.EndsWith(gitFileName)) 
                path = PathIO.Combine(path, gitFileName);

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
            var rules = new string[] { ".git/" }       // Git исключает из репозитория сам себя (c) Кэп
                .Concat(lines)
                .Select(s => s.Trim())
                .Where(s => !s.StartsWith("#")         // Исключаем комментарии
                    && !string.IsNullOrWhiteSpace(s)   // Пустые строки
                    && !s.All(ch => ch == '-')
                    && !s.All(ch => ch == '_'));       // Исключаем строки разделения ("black lines" в документации Git)

            rules_.Clear();
            rules_.AddRange(rules.Select(r => new GitIgnoreRule(r)));

            CreateRegex();
        }

        public bool IsMatch(string path)
        {
            if (IsParsed)
            {
                path = PreparePath(path, relative: true);

                return !positives.IsMatch(path) || negatives.IsMatch(path);
            }
            return true;
        }

        /*public string Filter(string path, string alt = "_")
        {
            if (IsParsed)
            {
                path = PreparePath(path, relative: true);

                return positives.Replace(path, alt);
                //return !positives.IsMatch(path) || negatives.IsMatch(path);
            }
            return path;
        }*/

        public static string[] Find(string path, SearchOption search)
        {
            return Directory.GetFiles(path, gitFileName, search);
        }

        public static bool HasFile(string path)
        {
            path = PathIO.Combine(path, gitFileName);
            return File.Exists(path);
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

        #endregion

        #region PRIVATE

        private void CreateRegex() 
        {
            List<GitIgnoreRule> 
                positiveRules = new List<GitIgnoreRule>(),
                negativeRules = new List<GitIgnoreRule>();

            rules_.ForEach(e => {
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
            positives = BuildRegex(positiveRules.Select(e => e.Pattern).ToArray());
            negatives = BuildRegex(negativeRules.Select(e => e.Pattern).ToArray());
        }

        internal static string PrepareRegexPattern(string pattern)
        {
            Console.WriteLine(pattern);

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

            pattern = pattern
                .Replace("/**/", "(/?)(.+|.?)/") // ?. -- для случая, когда последовательность пуста и есть только '/'
                //
                .Replace("/", @"\/")
                .Replace("**", "(.+)")      // любая последрвательность любых символов
                .Replace("*", @"([^\/]+)"); // аналогично за исключением '/'

            Console.WriteLine(pattern);

            return pattern;
        }

        internal static Regex BuildRegex(string[] items) 
        {
            string regex = items.Length > 0 ? 
                "((" + string.Join(")|(", items) + "))" : "$^";

            return new Regex(regex, RegexOptions.Compiled);
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
