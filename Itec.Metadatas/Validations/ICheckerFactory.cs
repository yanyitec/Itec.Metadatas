using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public interface ICheckerFactory
    {
        IChecker GetChecker(string fieldnames);
    }
}
