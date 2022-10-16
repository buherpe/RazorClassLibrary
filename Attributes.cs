using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorClassLibrary
{
    public class FieldAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public bool Serviced { get; set; }

        public string Format { get; set; }
    }

    public class BindAttribute : Attribute
    {
        //public Func<string> Bind2 { get; set; }

        public string Bind { get; set; }

        //public BindAttribute()
        //{

        //}

        public BindAttribute(string bind)
        {
            Bind = bind;
        }

        //public BindAttribute(Func<string> func)
        //{
        //    Bind2 = func;
        //}
    }
}
