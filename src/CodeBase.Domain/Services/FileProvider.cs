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
            if (project.Title != "GitHub")
            {
                return System.Array.Empty<FileItem>();
            }

            var location = project.Location;
            var repos = FindGitRepositories(location).ToList();
            var hasGit = repos.Any();

            if (hasGit)
            {
                foreach (var path in repos)
                {
                    var files = FindFilesFromGitRepo(path);
                    ;
                }
            }
            else
            {

            }

            var list = new List<FileItem>();

            if (Directory.Exists(location))
            {
                var allowedDirs = GetAllowedFolders(project);

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

        private IEnumerable<string> FindFilesFromGitRepo(string basePath)
        {
            var shellResult = ShellHelper.Execute($"cd {basePath} | git ls-tree --full-tree -r --name-only HEAD");

            if (!shellResult.IsSuccess)
            {
                throw new System.Exception("Failed to run PowerShell command");
            }

            var files = shellResult.Result.Split("\n").Select(x => x.Trim()).ToArray();
            return files;
        }

        private IEnumerable<string> FindGitRepositories(string basePath)
        {
            return Directory
                .GetDirectories(basePath, GIT_FOLDER_NAME, SearchOption.AllDirectories)
                .Select(x => Path.GetDirectoryName(x))
                .Distinct();
        }

        /*public bool IsIgnoredForder(string path)
        {
            return IgnoredFolders?.Any(f => GitIgnoreReader.IsChildedPath(PathIO.Combine(Path, f), path)) ?? false;
        }*/

        public List<string> GetAllowedFolders(Project project)
        {
            if (project.AllowedFolders?.Count == 0)
            {
                return new List<string>(Directory.GetDirectories(project.Location));
            }
            else
            {
                var dirs = project.AllowedFolders
                    .Select(folder => Path.Combine(project.Location, folder))
                    .Where(dir => Directory.Exists(dir));

                return dirs.ToList();
            }
        }
    }
}
