using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    /// <summary>
    /// 并行或，只要有一个不在运行中，那么返回结果
    /// </summary>
    public class BTParallelOR : BTComposite
    {
        public BTParallelOR() : base()
        {
        }

        public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            int count = m_lstChild.Count;
            for (int i = 0; i < count; i++)
            {
                BTResult childResult = m_lstChild[i].OnTick(blackBoard);
                if(childResult != BTResult.Running)
                {
                    return childResult;
                }
            }
            return BTResult.Running;
        }

        public override void Clear()
        {
            base.Clear();
        }
    }
}

