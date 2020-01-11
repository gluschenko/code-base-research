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
     * https://git-scm.com/docs/gitignore
     * 
     * Решение с решулярками частично подсмотрено здесь:
     * https://github.com/codemix/gitignore-parser/blob/master/lib/index.js
     * Базовое решение было урезано, но была добавлена валидация синтаксиса регулярных выражений.
     */

    /// <summary>
    /// Reads .gitignore files
    /// </summary>
    public class GitIgnoreReader
    {
        const string gitFileName = ".gitignore";
        // public
        public string Path { get; private set; }
        public bool IsParsed { get => positives != null && negatives != null; }
        public IReadOnlyCollection<GitIgnoreRule> Rules { get => rules; }
        // private
        private readonly List<GitIgnoreRule> rules;
        private Regex positives, negatives;

        // ctor
        public GitIgnoreReader(string path)
        {
            Path = path;
            rules = new List<GitIgnoreRule>();
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
            var rules = lines
                .Select(s => s.Trim())
                .Where(s => !s.StartsWith("#") && !string.IsNullOrWhiteSpace(s));

            InspectorConfig.DirsBlackList.ForEach(dir => rules.Append(dir));

            this.rules.AddRange(rules.Select(r => new GitIgnoreRule(r)));

            CreateRegex();
        }

        public bool IsMatch(string path)
        {
            if (IsParsed)
            {
                if (path.StartsWith("/"))
                    path = path.Substring(1);

                return !positives.IsMatch(path) || negatives.IsMatch(path);
            }
            return true;
        }

        public static string[] Find(string path, SearchOption search)
        {
            return Directory.GetFiles(path, gitFileName, search);
        }

        public static bool HasFile(string path)
        {
            path = PathIO.Combine(path, gitFileName);
            return File.Exists(path);
        }

        #endregion

        #region PRIVATE

        private void CreateRegex() 
        {
            List<GitIgnoreRule> 
                positiveRules = new List<GitIgnoreRule>(),
                negativeRules = new List<GitIgnoreRule>();

            rules.ForEach(e => {
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
            return pattern
                .Replace("/", @"\/")
                .Replace("**", "(.+)")
                .Replace("*", "([^\\/]+)");
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
        public string Text { get; private set; }
        public string Pattern { get; private set; }
        public bool IsNegative { get; private set; }
        public bool IsValid { get; private set; }

        public GitIgnoreRule(string text)
        {
            Text = text;
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
            if (!string.IsNullOrWhiteSpace(Text))
            {
                var rule = Text;

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
                try
                {
                    Regex.IsMatch("(" + Pattern + ")", "");
                    IsValid = true;
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
