using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeEditor
{
    public class NENodeData
    {
        public NENodeData(object data)
        {
            Type t = data.GetType();
            var arr = t.GetProperties();
        }

        public void Draw(object data)
        {
        }
    }
}
