using Itec.Datas;
using Itec.Metas;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class ValidatorFactoryFactory
    {
        public ValidatorFactoryFactory(
            IConfigFactory cfgFactory
            , IRuleFactory ruleFactory)
        {
            
            this.ConfigFactory = cfgFactory;
            this.RuleFactory = ruleFactory;
            this._Factories = new ConcurrentDictionary<string, ValidatorFactory>();
        }
        public string ConfigName { get; private set; }

        public Class Class { get; private set; }

        ConcurrentDictionary<string, ValidatorFactory> _Factories;

        public IConfigFactory ConfigFactory { get; private set; }

        public IRuleFactory RuleFactory { get; private set; }
        public ValidatorFactory GetFactory(Class cls,string configName)
        {
            var key = cls.Type.FullName + "#" + configName;
            return _Factories.GetOrAdd(key, (fns) => new ValidatorFactory(
                 cls, configName,this.ConfigFactory,this.RuleFactory
             ));
        }
    }
}
