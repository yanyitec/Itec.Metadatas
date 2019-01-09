using Newtonsoft.Json.Linq;

namespace Itec.Datas
{
    public interface IRWDataSource: IDataSource
    {
        
        new string this[string key] { get; set; }

        
        IRWDataSource Remove(string key);
        IRWDataSource Set<T>(string key, T value);
        IRWDataSource Set(string key, object value);
        IRWDataSource SetString(string key, string value);

        IRWDataSource SetJToken(string key, JToken value);

        
    }
}