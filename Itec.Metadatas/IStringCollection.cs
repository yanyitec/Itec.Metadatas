using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public interface IStringCollection:IEnumerable<KeyValuePair<string,string>>
    {
        string this[string key] { get; }
        int Count { get; }
    }
}
