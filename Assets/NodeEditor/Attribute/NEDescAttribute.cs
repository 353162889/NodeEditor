using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    /// <summary>
    /// 显示的描述
    /// </summary>
    public class NEDescAttribute : Attribute
    {
        public string desc { get; private set; }
        public NEDescAttribute(string desc)
        {
            this.desc = desc;
        }

        public static string GetDesc(Type type)
        {
            var arr = type.GetCustomAttributes(typeof(NEDescAttribute), false);
            if (arr.Length > 0) return ((NEDescAttribute)arr[0]).desc;
            return "";
        }
    }
}
