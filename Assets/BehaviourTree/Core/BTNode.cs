using NodeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BTCore
{
    public enum BTResult
    {
        Success,
        Running,
        Failure,
    }
    abstract public class BTNode {
        protected List<BTNode> m_lstChild;
        [NENodeData]
        protected object m_cData;
       
        public virtual BTResult OnTick(BTBlackBoard blackBoard){ return BTResult.Success; }

        public virtual void AddChild(BTNode child) {
            if(m_lstChild == null)
            {
                m_lstChild = new List<BTNode>();
            }
            m_lstChild.Add(child);
        }

        public virtual BTData GetData()
        {
            if(m_cData == null)
            {
                Debug.LogError(this.GetType() + " m_cData == null,need initialize！");
                return null;
            }
            BTData btData = new BTData();
            btData.data = m_cData;
            if (m_lstChild != null && m_lstChild.Count > 0)
            {
                btData.lstChild = new List<BTData>();
                for (int i = 0; i < m_lstChild.Count; i++)
                {
                    BTData data = m_lstChild[i].GetData();
                    if (data != null)
                    {
                        btData.lstChild.Add(data);
                    }
                }
            }
            return btData;
        }

        public virtual void Clear() {
            if (m_lstChild != null)
            {
                int count = m_lstChild.Count;
                for (int i = 0; i < count; i++)
                {
                    m_lstChild[i].Clear();
                }
            }
        }

    }
}
