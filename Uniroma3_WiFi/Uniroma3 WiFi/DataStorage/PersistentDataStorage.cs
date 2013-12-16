using System.IO.IsolatedStorage;

namespace Roma_Tre_WiFi.DataStorage
{
    public class PersistentDataStorage : IDataStorage
    {

        public bool Backup(string token, object value)
        {
            if (null == value)
                return false;

            var store = IsolatedStorageSettings.ApplicationSettings;
            if (store.Contains(token))
                store[token] = value;
            else
                store.Add(token, value);

            store.Save();
            return true;
        }

        public T Restore<T>(string token)
        {
            var store = IsolatedStorageSettings.ApplicationSettings;
            if (!store.Contains(token))
                return default(T);

            return (T)store[token];
        }
    }
}
