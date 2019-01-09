using Itec.Metas;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Itec.Models
{
    public class ListModel<T>:Criteria<T>
    {
        public ListModel(Class cls
            , Validations.ValidatorFactoryFactory validatorFactoryFactory
            , IConfigFactory configFactory
            , Expression<Func<T, bool>> initCriteria = null)
            :base(cls,validatorFactoryFactory,configFactory,initCriteria)
        {
            
        }
        

    }
}
