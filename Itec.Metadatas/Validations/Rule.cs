using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public abstract class Rule:IRule
    {
        public Rule(string validType, object arguments) {
            this.Type = validType;
            this.Arguments = arguments;
            
        }



        public abstract bool Check(object input);

        /// <summary>
        /// 验证类型
        /// </summary>
        public string Type { get; private set; }
        /// <summary>
        /// 验证参数
        /// </summary>
        public object Arguments { get; private set; }
    }
}
