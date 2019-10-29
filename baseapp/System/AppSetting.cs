using Insight.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BaseApp
{
    public interface IAppSettings
    {
        string GetVal(string key);
        T GetVal<T>(string key);
        void SetVal(string key, string val);
        void SetVal(Dictionary<string, string> list);
    }
}

namespace BaseApp.System
{
    public class DatabaseAppSettings : IAppSettings
    {
        private readonly Dictionary<string, string> d;
        private readonly object locker;
        private AppSettingRepository repo;

        public DatabaseAppSettings(IDbConnection db)
        {
            repo = db.As<AppSettingRepository>();
            locker = new object();
            d = repo.GetAll();
        }

        public string GetVal(string key)
        {
            return d.ContainsKey(key) ? d[key] : null;
        }

        public T GetVal<T>(string key)
        {
            try
            {
                return d.ContainsKey(key) ? (T)Convert.ChangeType(d[key], typeof(T)) : default(T);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        public void SetVal(string key, string val)
        {
            lock (locker)
            {
                repo.SetValue(key, val);
                d[key] = val;
            }
        }

        public void SetVal(Dictionary<string, string> list)
        {
            lock (locker)
            {
                foreach (var v in list)
                {
                    repo.SetValue(v.Key, v.Value);
                    d[v.Key] = v.Value;
                }
            }
        }
    }

    public abstract class AppSettingRepository
    {
        public abstract IDbConnection GetConnection();

        [Sql("AppSettings_SetVal")]
        public abstract void SetValue(string id, string value);

        public Dictionary<string, string> GetAll()
        {
            return GetConnection().Query<dynamic>("AppSettings_GetAll").ToDictionary(x => (string)x.Id, y => (string)y.Value);
        }
    }
}
