using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NodeEditor
{
    /// <summary>
    /// 显示的名称
    /// </summary>
    public class NENameAttribute : Attribute
    {
        public string name { get; private set; }
        public NENameAttribute(string name)
        {
            this.name = name;
        }

        public static string GetName(Type type)
        {
            var arr = type.GetCustomAttributes(typeof(NENameAttribute), false);
            if (arr.Length > 0) return ((NENameAttribute)arr[0]).name;
            return type.Name;
        }

        public static string GetName(FieldInfo info)
        {
            var arr = info.GetCustomAttributes(typeof(NENameAttribute), false);
            if (arr.Length > 0) return ((NENameAttribute)arr[0]).name;
            return info.Name;
        }
    }
}
