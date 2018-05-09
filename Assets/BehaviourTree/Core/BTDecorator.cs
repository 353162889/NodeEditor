using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTDecorator : BTNode
    {
        protected BTNode m_cChild;
        public BTDecorator(BTNode child) : base()
        {
            m_cChild = child;
        }

        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            BTResult result = OnEnter(blackBoard);
            return Decorate(blackBoard, result);
        }

        public virtual BTResult OnEnter(BTBlackBoard blackBoard)
        {
            return m_cChild.OnTick(blackBoard);
        }

        public virtual BTResult Decorate(BTBlackBoard bloackBoard,BTResult result)
        {
            return result;
        }

        public override void Clear()
        {
            if (m_cChild != null) m_cChild.Clear();
        }
    }
}
