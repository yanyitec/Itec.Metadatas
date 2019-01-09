using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Datas
{
    public interface IDataSource:System.Dynamic.IDynamicMetaObjectProvider,IEnumerable<KeyValuePair<string,object>>
    {
        bool HasChanges { get; }
        
        string this[string key] { get;  }

        T Get<T>(string key=null);

        Noneable<T> GetNoneable<T>(string key = null);
        string GetString(string key=null);

        object GetRaw(string key=null);
        object Get( Type objectType, string key=null);

        string ToJSON();
    }
}
