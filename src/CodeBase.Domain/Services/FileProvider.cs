using System;
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

        public IEnumerable<string> GetFilesData(Project project, HashSet<string> extensions, HashSet<string> blackList, GetFilesUpdate onProgress = null)
        {
            if (!Directory.Exists(project.Location))
            {
                throw new System.Exception($"Project '{project.Title}' has no existing folder on path '{project.Location}'");
            }

            var location = project.Location;
            var repos = FindGitRepositories(location).ToArray();
            var allFiles = new List<string>();

            if (repos.Any())
            {
                var nonGitFiles = Directory
                    .EnumerateFiles(location, "*", SearchOption.AllDirectories)
                    .Where(x => repos.All(y => !IsChildedPath(y, x)));

                allFiles.AddRange(nonGitFiles);

                foreach (var repo in repos)
                {
                    var files = FindFilesInGitRepo(repo);
                    allFiles.AddRange(files);
                }
            }
            else
            {
                var nonGitFiles = Directory
                    .EnumerateFiles(location, "*", SearchOption.AllDirectories);

                allFiles.AddRange(nonGitFiles);
            }

            var filteredFiles = allFiles
                .Where(x => extensions.Contains(Path.GetExtension(x).Trim(), StringComparer.OrdinalIgnoreCase))
                .Where(x => blackList.All(end => !x.EndsWith(end, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            var allowedFiles = GetAllowedFiles(project, filteredFiles);

            var result = allowedFiles.AsParallel().ToArray();
            return result;
        }

        private static IEnumerable<string> FindFilesInGitRepo(string basePath)
        {
            static string Normalize(string a) => a.Replace('/', '\\');

            var shellResult = ShellHelper.Execute($"cd {basePath} | git ls-tree --full-tree -r --name-only HEAD");

            if (!shellResult.IsSuccess)
            {
                throw new System.Exception("Failed to run PowerShell command");
            }

            var files = shellResult.Result.Split("\n")
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => Normalize(x))
                .Select(x => Path.Combine(basePath, x));

            return files;
        }

        private static IEnumerable<string> FindGitRepositories(string basePath)
        {
            return Directory
                .EnumerateDirectories(basePath, GIT_FOLDER_NAME, SearchOption.AllDirectories)
                .Select(x => Path.GetDirectoryName(x))
                .Distinct();
        }

        private static IEnumerable<string> GetAllowedFiles(Project project, IEnumerable<string> files)
        {
            var location = project.Location;

            var folders = files.Select(x => Path.GetDirectoryName(x)).Distinct().ToArray();

            var allowedFolders = new List<string>();
            var excludedFolders = new List<string>();

            if (project.AllowedFolders?.Count == 0)
            {
                allowedFolders.AddRange(folders);
            }
            else
            {
                var dirs = SearchFoldersByPattern(location, project.AllowedFolders);
                allowedFolders.AddRange(dirs);
            }

            if (project.ExcludedFolders?.Count != 0)
            {
                var dirs = SearchFoldersByPattern(location, project.ExcludedFolders);
                excludedFolders.AddRange(dirs);
            }

            var filteredFiles = files
                .Where(x => allowedFolders.Any(y => IsChildedPath(y, x, false)))
                .Where(x => excludedFolders.All(y => !IsChildedPath(y, x, false)));

            return filteredFiles;
        }

        public static bool IsChildedPath(string parent, string child, bool normalize = true)
        {
            var result = false;
            if (normalize)
            {
                parent = NormalizePath(parent);
                child = NormalizePath(child);
            }

            if (child.Length > parent.Length)
            {
                result = child.StartsWith(parent + '\\');
            }

            if (child == parent)
            {
                result = true;
            }

            return result;
        }

        public static IEnumerable<string> SearchFoldersByPattern(string location, IEnumerable<string> patterns)
        {
            var normPatterns = patterns.Select(x => x.Replace('/', '\\'));
            location = NormalizePath(location);

            var result = normPatterns.Select(x => 
            {
                var isRelative = x.StartsWith('.');

                if (isRelative)
                {
                    return new[] 
                    {
                        NormalizePath(Path.Combine(location, x)),
                    };
                }
                else
                {
                    return Directory.EnumerateDirectories(location, x, SearchOption.AllDirectories);
                }
            });

            return result.SelectMany(x => x).Distinct();
        }

        public static string NormalizePath(string path)
        {
            var words = path.Split(new char[] { '\\', '/' }, System.StringSplitOptions.RemoveEmptyEntries);

            var list = new LinkedList<string>();

            foreach (var word in words)
            {
                if (word == ".")
                {
                    continue;
                }

                if (word == "..")
                {
                    list.RemoveLast();
                }

                list.AddLast(word);
            }

            return string.Join('\\', list);
        }
    }
}
