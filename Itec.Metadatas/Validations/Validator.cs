using Itec.Datas;
using Itec.Metas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Itec.Validations
{
    public class Validator
    {
        public Validator(string fieldnames,string configName, Class cls, IDataSource configs, IRuleFactory checkerFactory) {
            this.Fieldnames = fieldnames;
            this.Class = cls;
            this.Configs = configs;
            this.RuleFactory = checkerFactory;
            this.ConfigName = configName;
        }
        public Class Class { get; private set; }
        public string Fieldnames { get; private set; }

        public IRuleFactory RuleFactory { get; private set; }

        public IDataSource Configs { get; private set; }

        public string ConfigName { get; private set; }

        Func<IDataSource, Validation> _Validate;
        public IValidation Validate(IDataSource inputs) {
            if (_Validate == null) {
                lock (this) {
                    if (_Validate == null) _Validate = MakeValidate();
                }
            }
            return _Validate(inputs);
        }

         Func<IDataSource,Validation> MakeValidate() {
            var fieldnames = this.Fieldnames;
            var fields = fieldnames.Split(',');
            var valids = new Dictionary<string, FieldValidator>();
            foreach (var prop in this.Class) {
                if (!fields.Contains(prop.Name)) continue;
                var fieldConfig = this.Configs.GetRaw(prop.Name) as JObject;
                var fieldValidator = MakeFieldValidator(prop,fieldConfig);
                var isRange = fieldConfig["QueryType"];
                if (isRange == null || isRange.Type == JTokenType.Null || isRange.Type == JTokenType.Undefined || isRange.ToString()!="Range") {
                    valids.Add(prop.Name,fieldValidator);
                } else {
                    var minKey = prop.Name + "_MIN";
                    valids.Add(minKey,fieldValidator.Clone(true));
                    var maxKey = prop.Name + "_MAX";
                    valids.Add(maxKey,fieldValidator.Clone(true));
                }
            }
            
            return new Func<IDataSource,Validation>((inputs)=> {
                var rs = new Validation(this.Fieldnames, this.ConfigName);
                bool isValid = true;
                foreach (var pair in valids) {
                    var value = inputs.GetRaw(pair.Key);
                    var invalids = pair.Value.Validate(value);
                    if (invalids!=null && invalids.Count > 0)
                    {
                        isValid = false;
                        rs.AddValid(pair.Key, invalids);
                    }

                }
                rs.IsValid = isValid;
                return rs;
            });
        }

        FieldValidator MakeFieldValidator(Property prop,JObject fieldConfig) {
            /*
             
             */
            var rules = new List<IRule>();
            foreach (var rule in prop.ValidateRules) {
                rules.Add(rule);
            }

            var cRules = fieldConfig ==null?null:fieldConfig["Rules"] as JArray;
            if (!(cRules == null || cRules.Type == JTokenType.Undefined || cRules.Type == JTokenType.Null))
            {
                for (var i = 0; i < cRules.Count; i++)
                {
                    var ruleConfig = cRules[i];
                    if (!(ruleConfig == null || ruleConfig.Type == JTokenType.Undefined || ruleConfig.Type == JTokenType.Null))
                    {
                        var typeJ = ruleConfig["Type"];
                        if (!(typeJ == null || typeJ.Type == JTokenType.Undefined || typeJ.Type == JTokenType.Null))
                        {
                            var typeName = typeJ.ToString().ToLower();
                            if (rules.Any(p => p.Type == typeName)) continue;
                            var rule = this.RuleFactory.GetRule(typeName, ruleConfig);
                            if (rule == null) continue;
                            rules.Add(rule);

                        }
                    }
                }
            }

            return new FieldValidator(prop,rules);
        }

    }
}
