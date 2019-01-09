using Itec.Datas;
using Itec.Metas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public interface IConfigFactory
    {
        IDataSource GetClassConfig(Class cls,string configName);
    }
}
