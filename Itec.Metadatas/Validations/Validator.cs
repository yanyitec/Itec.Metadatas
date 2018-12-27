using Itec.Datas;
using Itec.Metas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Itec.Validations
{
    public class Validator
    {
        public Validator(string fieldnames, Class cls, IReadOnlyData configs, ICheckerFactory checkerFactory) {
            this.Fieldnames = fieldnames;
            this.Class = cls;
            this.CheckerFactory = checkerFactory;
        }
        public Class Class { get; private set; }
        public string Fieldnames { get; private set; }

        public ICheckerFactory CheckerFactory { get; private set; }

        public IReadOnlyData Configs { get; private set; }

        Func<IValueProvider, Validation> _Validate;
        public IValidation Validate(IValueProvider vp) {
            return _Validate(vp);
        }

        static Expression MakeValidate(Validator validator) {
            var fieldnames = validator.Fieldnames;
            var fields = fieldnames.Split(',');
            var codes = new List<Expression>();
            var locals = new List<ParameterExpression>();
            foreach (var fieldname in fields) {
                var field = fieldname.Trim();
                if (string.IsNullOrEmpty(field)) continue;
                var prop = validator.Class[field];
                if (prop == null) continue;
            }
        }

        static void MakeValidate(Validator validator,Property prop, List<Expression> codes, List<ParameterExpression> locals,ParameterExpression targetExpr) {
            /*
             
             */
            var rules = new List<IRule>();
            foreach (var rule in prop.ValidateRules) {
                rules.Add(rule);
            }

            var token = validator.Configs.GetJToken(prop.Name) as JObject;
            if (!(token == null || token.Type == JTokenType.Undefined || token.Type == JTokenType.Null)) {
                var cRules = token["Rules"] as JArray;
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
                                

                            }
                        }
                    }
                }
                
            }
            

        }

    }
}
