using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class Validation : IValidation
    {
        public Validation(string fieldnames,string configName) {
            this.Fieldnames = fieldnames;
            this._Valids = new Dictionary<string, IList<IRule>>();
            this.ConfigName = configName;
        }

        public string Fieldnames { get; private set; }
        public string ConfigName { get; private set; }

        Dictionary<string, IList<IRule>> _Valids;

        public bool IsValid { get; internal set; }
        public IDictionary<string, IList<IRule>> Valids { get { return _Valids; } }

        

        public void AddValid(string fieldname, IList<IRule> invalids) {
            _Valids.Add(fieldname, invalids);

        }
        
    }
}
