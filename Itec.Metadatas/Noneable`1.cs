using System;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public struct Noneable<T>
    {
        public Noneable(T value) {
            HasValue = true;
            Value = value;
            
        }
        public bool HasValue { get;private set; }
        public T Value { get;private set; }

        public T GetValue() {
            return this.HasValue ? this.Value : default(T);
        }

        public static implicit operator Noneable<T>(T value) {
            return new Noneable<T>(value);
        }



        public static explicit operator T(Noneable<T> nullable) {
            return nullable.HasValue ? nullable.Value : default(T);
        }
    }
}
