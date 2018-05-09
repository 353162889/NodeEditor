﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BTCore
{
    public class BTSequence : BTRunningComposite
    {
        protected int m_iSelectedIndex;
        public BTSequence():base()
        {
            m_iSelectedIndex = 0;
        }
        protected override BTResult OnCompositeTick(BTBlackBoard blackBoard)
        {
            int curIndex = m_iSelectedIndex;
            int totalCount = m_lstChild.Count;
            if (curIndex < totalCount)
            {
                var child = m_lstChild[curIndex];
                BTResult childResult = this.OnRunningChildTick(child, blackBoard);
                if (childResult == BTResult.Failure)
                {
                    return BTResult.Failure;
                }
                else if(childResult == BTResult.Success)
                {
                    m_iSelectedIndex++;
                }
            }
            if(m_iSelectedIndex < totalCount)
            {
                return BTResult.Running;
            }
            else
            {
                return BTResult.Success;
            }
        }

        public override void Clear()
        {
            m_iSelectedIndex = 0;
            base.Clear();
        }
    }
}
