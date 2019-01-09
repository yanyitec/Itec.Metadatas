using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public interface IRuleFactory
    {
        IRule GetRule(string typeName,object ruleArguments=null);
    }
}
