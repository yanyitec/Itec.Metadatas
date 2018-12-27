using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    /// <summary>
    /// 一种验证
    /// </summary>
    public interface IRule
    {
        string Type { get; }
        object Arguments { get; }
        bool Check(object input);
    }
}
