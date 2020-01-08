using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeBase
{
    /* Наглый и беспардонный плагиат из модуля для node.js:
     * https://github.com/codemix/gitignore-parser/blob/master/lib/index.js
     * https://git-scm.com/docs/gitignore
     */

    public class GitIgnoreReader
    {
        public string Path { get; private set; }
        public List<string> Rules { get; private set; }
        public bool IsParsed { get; private set; }
        
        private Regex positives, negatives;

        public GitIgnoreReader(string path)
        {
            Path = path;
            Rules = new List<string>();
        }

        public static GitIgnoreReader Load(string path, bool direct = false)
        {
            if (direct) 
                path = System.IO.Path.Combine(path, ".gitignore");

            var reader = new GitIgnoreReader(path);

            if (File.Exists(path)) 
            {
                var lines = File.ReadAllLines(path);
                reader.ParseRules(lines);
            }

            return reader;
        }

        public void ParseRules(string[] lines)
        {
            lines = lines
                .Select(s => s.Trim())
                .Where(s => !s.StartsWith("#") && !string.IsNullOrWhiteSpace(s))
                .ToArray();

            Rules.AddRange(InspectorConfig.DirsBlackList);
            Rules.AddRange(lines);
            //
            Parse();
        }

        public static string[] Find(string path, SearchOption search)
        {
            return Directory.GetFiles(path, ".gitignore", search);
        }

        public bool IsMatch(string path) 
        {
            if (IsParsed) 
            {
                if (path[0] == '/') path = path.Substring(1);
                return !positives.IsMatch(path) || negatives.IsMatch(path);
            }
            return true;
        }

        /*public bool IsMatchPartial(string path)
        {
            if (IsParsed)
            {
                //if (path[0] == '/') path = path.Substring(1);
                return !positives.partial.IsMatch(path) || negatives.partial.IsMatch(path);
            }
            return true;
        }*/

        private void Parse() 
        {
            List<string> positives = new List<string>();
            List<string> negatives = new List<string>();

            Rules.ForEach(rule => {
                if (rule.Length > 0)
                {
                    var isNegative = rule[0] == '!';

                    if (isNegative)
                        rule = rule.Substring(1);

                    if (rule[0] == '/')
                        rule = rule.Substring(1);

                    if (isNegative)
                    {
                        negatives.Add(rule);
                    }
                    else 
                    {
                        positives.Add(rule);
                    }
                }
            });

            //var prepared_pos = positives.Select(p => PrepareRegexes(p));
            //var prepared_neg = negatives.Select(p => PrepareRegexes(p));
            //
            this.positives = BuildRegex(positives.Select(p => PrepareRegexPattern(p)).ToArray());
            //this.positives.partial = BuildRegex(prepared_pos.Select(r => r.partial).ToArray());

            this.negatives = BuildRegex(negatives.Select(p => PrepareRegexPattern(p)).ToArray());
            //this.negatives.partial = BuildRegex(prepared_neg.Select(r => r.partial).ToArray());
            //
            IsParsed = true;
        }

        /*public struct RegexSet<T>
        {
            public T exact;
            public T partial;
        }*/

        /*private RegexSet<string> PrepareRegexes(string pattern)
        {
            return new RegexSet<string>(){
                exact = PrepareRegexPattern(pattern),
                partial = PreparePartialRegex(pattern)
            };
        }*/

        private string PrepareRegexPattern(string pattern)
        {
            return /*EscapeRegex(pattern).*/ pattern.Replace("/", @"\/").Replace("**", "(.+)").Replace("*", "([^\\/]+)");
        }

        /*private string PreparePartialRegex(string pattern)
        {
            var sss = pattern
                .Split('/')
                .Select((item) => {

                    return PrepareRegexPattern(item);
                    */
                    /*if (index > 0)
                        return "([\\/]?(" + PrepareRegexPattern(item) + "\\b|$))";
                    else
                        return "(" + PrepareRegexPattern(item) + "\\b)";*/
                //});
    /*
            return string.Join("", sss);
        }*/

        /*private string EscapeRegex(string pattern)
        {
            return Regex.Replace(pattern, @"[\-\[\]\/\{\}\(\)\+\?\.\\\^\$\|]", "\\$&");
        }*/

        private Regex BuildRegex(string[] items) 
        {
            string regex = items.Length > 0 ? 
                $"(({string.Join(")|(", items)}))" :
                "$^";

            ///MessageHelper.Alert(regex);

            return new Regex(regex, RegexOptions.Compiled);
        }
    }
}
