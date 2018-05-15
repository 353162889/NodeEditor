using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BTCore
{
    public class BTRootData
    {
    }
    [NENode(typeof(BTRootData))]
    [NENodeDisplay(false,true, false)]
    [NEName("Root")]
    public class BTRoot : BTNode
    {
        protected BTNode m_cChild;

        public override void AddChild(BTNode child)
        {
            if (m_cChild != null)
            {
                Debug.LogError("BTRoot has exist child node! add has override it");
            }
            m_cChild = child;
        }

        public override BTData GetData()
        {
            if (m_cData == null)
            {
                Debug.LogError(this.GetType() + " m_cData == null,need initialize！");
                return null;
            }
            BTData btData = new BTData();
            btData.data = m_cData;
            if (m_cChild != null)
            {
                btData.lstChild = new List<BTData>();
                BTData data = m_cChild.GetData();
                if (data != null)
                {
                    btData.lstChild.Add(data);
                }
            }
            return btData;
        }

        sealed public override BTResult OnTick(BTBlackBoard blackBoard)
        {
            if (m_cChild != null)return m_cChild.OnTick(blackBoard);
            return BTResult.Success;
        }

        public override void Clear()
        {
            if (m_cChild != null) m_cChild.Clear();
        }
    }
}
