using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BTCore
{
    public class BTAndConditionData
    {

    }
    [NENode(typeof(BTAndConditionData))]
    public class BTAndCondition : BTCondition
    {
        private List<BTCondition> m_lstCondition;
        public BTAndCondition()
        {
            m_lstCondition = new List<BTCondition>();
        }

        public override void AddChild(BTNode child)
        {
            if (child is BTCondition)
            {
                m_lstCondition.Add((BTCondition)child);
            }
            else
            {
                Debug.LogError("BTAndCondition AddChild is not BTCondition");
            }
        }

        public override bool Evaluate(BTBlackBoard blackBoard)
        {
            bool result = true;
            int count = m_lstCondition.Count;
            for (int i = 0; i < count; i++)
            {
                result = result && m_lstCondition[i].Evaluate(blackBoard);
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
