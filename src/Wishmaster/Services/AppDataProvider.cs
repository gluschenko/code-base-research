using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wishmaster.Models;

namespace Wishmaster.Services
{
    public interface IAppDataProvider
    {
        AppData GetAppData();
        void SaveAppData(AppData appData);
    }

    public class AppDataProvider : IAppDataProvider
    {
        private readonly DataManager<AppData> _dataManager;
        private AppData? _data;

        public AppDataProvider()
        {
            _dataManager = new DataManager<AppData>("prefs.json");
        }

        public AppData GetAppData()
        {
            if (_data is null)
            {
                _data = _dataManager.Load();
            }

            return _data;
        }

        public void SaveAppData(AppData appData)
        {
            _dataManager.Save(appData);
        }
    }
}
