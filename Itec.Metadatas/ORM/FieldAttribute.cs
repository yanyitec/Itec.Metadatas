using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.ORM
{
    public class FieldAttribute:Attribute
    {
        public FieldAttribute(string name,bool isIndex=false) {
            this.Name = name;
            this.IsIndex = isIndex;
        }

        public string Name { get; private set; }

        public bool IsIndex { get; private set; }


    }
}
