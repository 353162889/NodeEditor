using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTNotCondition : BTCondition
    {
        protected BTCondition m_cChild;
        public BTNotCondition(BTCondition child) : base()
        {
            m_cChild = child;
        }

        public override bool Evaluate(BTBlackBoard blackBoard)
        {
            return !m_cChild.Evaluate(blackBoard);
        }

        public override void Clear()
        {
            m_cChild.Clear();
        }
    }
}
