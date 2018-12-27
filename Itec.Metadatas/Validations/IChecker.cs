using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public interface IChecker
    {
        bool Check(object input, object arguments);
    }
}
