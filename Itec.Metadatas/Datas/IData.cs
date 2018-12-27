using Newtonsoft.Json.Linq;

namespace Itec.Datas
{
    public interface IData: IReadOnlyData
    {
        
        new string this[string key] { get; set; }

        
        IData Remove(string key);
        IData Set<T>(string key, T value);
        IData Set(string key, object value);
        IData SetString(string key, string value);

        IData SetJToken(string key, JToken value);

        
    }
}