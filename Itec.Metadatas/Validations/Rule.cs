using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Validations
{
    public abstract class Rule:IRule
    {
        public Rule(string validType, object arguments,IChecker checker) {
            this.Type = validType;
            this.Arguments = arguments;
            this._Checker = checker;
        }

        IChecker _Checker;

        public bool Check(object input) {
            return _Checker.Check(input, this.Arguments)
        }

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
