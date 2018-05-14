using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    public class NENodeAttribute : Attribute
    {
        public string name { get; private set; }
        public string desc { get; private set; }
        /// <summary>
        /// 节点
        /// </summary>
        /// <param name="name">节点名称(如果为空，使用类名)</param>
        /// <param name="desc">节点描述</param>
        public NENodeAttribute(string name = null,string desc = null)
        {
            this.name = name;
            this.desc = desc;
        }
    }
}