using System;
using System.IO;
using Newtonsoft.Json;

namespace Wishmaster.Services
{
    public class DataManager<T>
    {
        private readonly string _fileName;

        public DataManager(string fileName)
        {
            _fileName = fileName;
        }

        public T Load()
        {
            var path = GetFilePath();

            if (File.Exists(path))
            {
                var data = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(data);
            }

            return Activator.CreateInstance<T>();
        }

        public void Save(T data)
        {
            var path = GetFilePath();
            var text = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, text);
        }

        private string GetFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);
        }
    }
}
