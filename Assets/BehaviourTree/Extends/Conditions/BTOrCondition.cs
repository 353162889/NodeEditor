using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTOrCondition : BTCondition
    {
        private List<BTCondition> m_lstCondition;
        public BTOrCondition() : base() {
            m_lstCondition = new List<BTCondition>();
        }

        public void AddChild(BTCondition child)
        {
            m_lstCondition.Add(child);
        }

        public override bool Evaluate(BTBlackBoard blackBoard)
        {
            bool result = true;
            int count = m_lstCondition.Count;
            for (int i = 0; i < count; i++)
            {
                result = result || m_lstCondition[i].Evaluate(blackBoard);
            }
            return result;
        }

        public override void Clear()
        {
            int count = m_lstCondition.Count;
            for (int i = 0; i < count; i++)
            {
                m_lstCondition[i].Clear();
            }
        }
    }
}
