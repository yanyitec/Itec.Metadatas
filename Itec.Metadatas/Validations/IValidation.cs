using System.Collections.Generic;

namespace Itec.Validations
{
    public interface IValidation
    {
        string Fieldnames { get; }
        
        
        IDictionary<string, IList<IRule>> Valids { get; }
    }
}