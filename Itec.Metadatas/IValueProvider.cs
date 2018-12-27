using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public interface IValueProvider
    {
        Itec.Noneable<T> Get<T>(string name = null);
        object GetRaw(string name);
    }
}
