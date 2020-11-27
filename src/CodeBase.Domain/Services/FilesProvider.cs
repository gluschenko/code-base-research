using System.Collections.Generic;

namespace CodeBase.Domain.Services
{
    public class FilesProvider
    {
        public FilesProvider()
        {

        }

        public IEnumerable<string> GetFilesList(string basePath, string[] filters) 
        {
            // cd basePath
            // git ls-tree --full-tree -r --name-only HEAD

            return new List<string>();
        }
    }
}
