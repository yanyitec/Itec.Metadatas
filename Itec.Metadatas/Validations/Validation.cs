using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public class Validation : IValidation
    {
        public Validation(string fieldnames, Func<object> targetGetter) {
            this.Fieldnames = fieldnames;
            this._Valids = new Dictionary<string, IList<IRule>>();
            this._TargetGetter = targetGetter;
        }

        public string Fieldnames { get; private set; }

        Dictionary<string, IList<IRule>> _Valids;
        public IDictionary<string, IList<IRule>> Valids { get { return _Valids; } }

        public IList<IRule> this[string name] {
            get {
                IList<IRule> _results = null;
                this._Valids.TryGetValue(name ,out _results);
                return _results;
            }
        }

        public void AddValidation(string fieldname, IRule validation) {
            IList<IRule> valids = null;
            if (!_Valids.TryGetValue(fieldname, out valids)) {
                _Valids.Add(fieldname, valids = new List<IRule>());
            }
            valids.Add(validation);
            
        }
        Func<object> _TargetGetter;
        object _target;

        public object Target {
            get {
                if (_target == null) {
                    _target = _TargetGetter();
                }
                return _target;
            }
        }
    }
}
