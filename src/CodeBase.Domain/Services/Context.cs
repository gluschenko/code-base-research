using System;
using System.Windows;
using CodeBase.Domain.Models;

namespace CodeBase.Domain.Services
{
    public class Context
    {
        public AppData AppData { get; set; }
        public DataManager<AppData> DataManager { get; set; }
        public Window MainWindow { get; set; }

        public Project CurrentProject { get; set; }

        public Action<Type> Navigate { get; set; }
        public Action<Project> OnProjectCreated { get; set; }
    }
}
