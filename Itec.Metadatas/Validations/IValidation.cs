using System.Collections.Generic;

namespace Itec.Validations
{
    public interface IValidation
    {
        string Fieldnames { get; }
        object Target { get; }
        IList<IRule> this[string fieldname] { get; }
        IDictionary<string, IList<IRule>> Valids { get; }
    }
}