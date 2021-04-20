using System.Collections.Generic;
using System.Management.Automation;
using CodeBase.Domain.Models;

namespace CodeBase.Domain.Services
{
    public class FilesProvider
    {
        public FilesProvider()
        {

        }

        public IEnumerable<string> GetFilesList(string basePath, string[] filters) 
        {
            GetFilesListFromGit(basePath);

            return new List<string>();
        }

        public IEnumerable<FileData> GetFilesData() 
        {
            return null;
        }

        private void GetFilesListFromGit(string basePath) 
        {
            using var powershell = PowerShell.Create();
            var results = powershell
                .AddScript($"cd {basePath}")
                .AddScript("git ls-tree --full-tree -r --name-only HEAD")
                .Invoke();

            ;
        }
    }
}
