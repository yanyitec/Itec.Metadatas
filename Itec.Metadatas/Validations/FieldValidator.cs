using Itec.Metas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class FieldValidator
    {
        public FieldValidator(Property prop, List<IRule> rules, bool ignoreRequired=false) {
            this.Property = prop;
            this._Rules = rules;
            this.IgnoreRequired = ignoreRequired;
        }

        public Property Property { get; private set; }

        public bool IgnoreRequired { get; private set; }
        List<IRule> _Rules;
        public IReadOnlyList<IRule> Rules { get { return _Rules; } }

        public IList<IRule> Validate(object value) {
            var invalids = new List<IRule>();
            foreach (var rule in this.Rules) {
                if (!rule.Check(value)) invalids.Add(rule);
            }
            return invalids;
        }

        public FieldValidator Clone(bool ignoreRequired) {
            var result = new FieldValidator(this.Property,this._Rules);
            return result;
        }

    }
}
