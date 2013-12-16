using Microsoft.Phone.Shell;

namespace Sapienza_WiFi.DataStorage
{
    public class TransientDataStorage : IDataStorage
    {

        public bool Backup(string token, object value)
        {
            if (null == value)
                return false;

            var store = PhoneApplicationService.Current.State;
            if (store.ContainsKey(token))
                store[token] = value;
            else
                store.Add(token, value);

            return true;
        }

        public T Restore<T>(string token)
        {
            var store = PhoneApplicationService.Current.State;
            if (!store.ContainsKey(token))
                return default(T);

            return (T)store[token];
        }
    }
}
