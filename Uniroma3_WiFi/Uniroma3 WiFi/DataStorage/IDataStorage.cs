using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma_Tre_WiFi.DataStorage
{

    //http://www.imaginativeuniversal.com/blog/post/2010/08/26/WP7-Tip-tombstoning-simplified.aspx

    public interface IDataStorage
    {
        bool Backup(string token, object value);
        T Restore<T>(string token);
    }
}
