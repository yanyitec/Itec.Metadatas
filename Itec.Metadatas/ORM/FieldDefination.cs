using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.ORM
{
    public class FieldDefination
    {
        public string Field { get; set; }

        public bool? NotField { get; set; }

        
        public bool? Index { get; set; }

        public int? Length { get; set; }

        public int? Precision { get; set; }

        public bool? Nullable { get; set; }

    }
}
