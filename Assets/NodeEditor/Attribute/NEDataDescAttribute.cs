using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    //数据的描述默认使用字段名称
    public class NEDataDescAttribute : Attribute
    {
        public string desc { get; private set; }
        /// <summary>
        /// 数据属性描述
        /// </summary>
        /// <param name="desc">属性描述</param>
        public NEDataDescAttribute(string desc)
        {
            this.desc = desc;
        }
    }
}
