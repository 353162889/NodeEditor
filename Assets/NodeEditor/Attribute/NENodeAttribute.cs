using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    /// <summary>
    /// 标识当前类是一个节点
    /// </summary>
    public class NENodeAttribute : Attribute
    {
        public NENodeAttribute()
        {
        }
    }
}