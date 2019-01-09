using Itec.Datas;
using Itec.Metas;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class ValidatorFactory
    {
        public ValidatorFactory(
            Class cls
            , string cfgName
            , IConfigFactory cfgFactory
            ,IRuleFactory ruleFactory) {
            this.Class = cls;
            this.ConfigName = cfgName;
            this.ConfigFactory = cfgFactory;
            this.RuleFactory = ruleFactory;
            this._Validators = new ConcurrentDictionary<string, Validator>();
        }
        public string ConfigName { get; private set; }

        public Class Class { get; private set; }

        ConcurrentDictionary<string, Validator> _Validators;

        public IConfigFactory ConfigFactory { get; private set; }

        public IRuleFactory RuleFactory { get; private set; }
        public Validator GetValidator( string fieldnames) {
            return _Validators.GetOrAdd(fieldnames,(fns)=> new Validator(
                fieldnames
                ,this.ConfigName
                ,this.Class
                , ConfigFactory.GetClassConfig(this.Class,this.ConfigName)
                ,this.RuleFactory
            ));
        }
    }
}
