using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBase.Domain.Models;

namespace CodeBase.Domain.Services
{
    public class FileProvider
    {
        private const string GIT_FOLDER_NAME = ".git";

        public delegate void GetFilesUpdate(int files, int dirs, string currentDir);

        public FileProvider()
        {

        }

        public IEnumerable<FileItem> GetFilesData(Project project, HashSet<string> extensions, HashSet<string> blackList, GetFilesUpdate onProgress = null)
        {
            if (project.Title != "GitHub") // УДОЛИ!
            {
                return System.Array.Empty<FileItem>();
            }

            if (!Directory.Exists(project.Location))
            {
                throw new System.Exception($"Project '{project.Title}' has no existing folder on path '{project.Location}'");
            }

            var location = project.Location;
            var repos = FindGitRepositories(location).ToList();
            var hasGit = repos.Any();
            var allFiles = new List<string>();

            static string Normalize(string a) => a.Replace('\\', '/');

            if (hasGit)
            {
                var reposNorm = repos.Select(x => x + '\\');

                var nonGitFiles = Directory
                    .GetFiles(location, "*", SearchOption.AllDirectories)
                    .Where(x => !reposNorm.Any(y => x.StartsWith(y)))
                    .Select(x => Normalize(x));

                allFiles.AddRange(nonGitFiles);

                foreach (var repo in repos)
                {
                    var files = FindFilesInGitRepo(repo)
                        .Select(x => Path.Combine(repo, x))
                        .Select(x => Normalize(x));

                    allFiles.AddRange(files);
                }
            }
            else
            {
                var nonGitFiles = Directory
                    .GetFiles(location, "*", SearchOption.AllDirectories)
                    .Select(x => Normalize(x));

                allFiles.AddRange(nonGitFiles);
            }

            var allowedFiles = GetAllowedFiles(project, allFiles);

            var list = new List<FileItem>();

            if (Directory.Exists(location))
            {
                var allowedDirs = GetAllowedFiles(project, allFiles);

                //var gitIgnores = GitIgnoreReader.Find(location, SearchOption.AllDirectories).Select(p => GitIgnoreReader.Load(p));
                //var queue = new Queue<KeyValuePair<string, IEnumerable<GitIgnoreReader>>>();

                var dirsCount = 0;

                GetFiles(location);

                void GetFiles(string dir)
                {
                    //var relevantGitFiles = gitIgnores.Where(f => GitIgnoreReader.IsChildedPath(f.BaseDir, dir));

                    //var isAllowedDir =
                    //    allowedDirs.Count > 0 ? allowedDirs.Any(p => GitIgnoreReader.IsChildedPath(p, dir)) : true;

                    var subs = Directory.EnumerateDirectories(dir)
                        //.Where(p => !IsIgnoredForder(p))
                        .Select(s => s.Replace('\\', '/'));

                    var files = Directory.EnumerateFiles(dir)
                        //.Where(s => isAllowedDir)
                        .Where(s => extensions.Contains(Path.GetExtension(s)))
                        .Where(s => blackList.All(end => !s.EndsWith(end)))
                        .Select(s => s.Replace('\\', '/'));

                    dirsCount += subs.Count();

                    //onProgress?.Invoke(queue.Count, dirsCount, dir);

                    foreach (var file in files)
                    {
                        //queue.Enqueue(new KeyValuePair<string, IEnumerable<GitIgnoreReader>>(file, relevantGitFiles));
                    }

                    foreach (var sub in subs.Select(s => s + '/'))
                    {
                        //var isMatch = relevantGitFiles.All(r => r.IsMatch(sub));

                        //if (isMatch)
                        {
                            GetFiles(sub);
                        }
                    }
                }

                //while (queue.Count > 0)
                {
                    //var pair = queue.Dequeue();
                    //var isMatch = pair.Value.All(r => r.IsMatch(pair.Key));
                    //list.Add(new FileItem(pair.Key, isMatch));

                    //onProgress?.Invoke(queue.Count, 0, pair.Key);
                }
            }

            return list;
        }

        private IEnumerable<string> FindFilesInGitRepo(string basePath)
        {
            var shellResult = ShellHelper.Execute($"cd {basePath} | git ls-tree --full-tree -r --name-only HEAD");

            if (!shellResult.IsSuccess)
            {
                throw new System.Exception("Failed to run PowerShell command");
            }

            var files = shellResult.Result.Split("\n")
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            return files;
        }

        private IEnumerable<string> FindGitRepositories(string basePath)
        {
            return Directory
                .GetDirectories(basePath, GIT_FOLDER_NAME, SearchOption.AllDirectories)
                .Select(x => Path.GetDirectoryName(x))
                .Distinct();
        }

        private List<string> GetAllowedFiles(Project project, IEnumerable<string> files)
        {
            var location = project.Location;

            var allowedFolders = new HashSet<string>();
            var disallowedFolders = new HashSet<string>();

            if (project.AllowedFolders?.Count == 0)
            {
                var dirs = Directory.GetDirectories(location, "*", SearchOption.AllDirectories);
                dirs.ToList().ForEach(x => allowedFolders.Add(x));
            }
            else
            {
                var searchRules = project.AllowedFolders.Select(x => x.Trim('/', '\\'));

                var dirs = searchRules
                    .Select(x => Directory.GetDirectories(location, x, SearchOption.AllDirectories))
                    .SelectMany(x => x);

                ;
            }

            return new List<string>();
        }
    }
}
