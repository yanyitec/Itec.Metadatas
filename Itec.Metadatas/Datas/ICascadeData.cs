using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Datas
{
    public interface ICascadeReadOnlyData:IReadOnlyData
    {
        ICascadeReadOnlyData Super { get; }
        T Overall<T>(string key);
        string OverallString(string key);
        object Overall(string key, Type objectType);

        
    }
}
