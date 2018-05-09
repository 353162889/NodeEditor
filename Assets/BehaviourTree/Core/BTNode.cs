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

        //name可以用attribute做
        public virtual BTResult OnTick(BTBlackBoard blackBoard){    return BTResult.Success; }

        public virtual void Clear() { }

    }
}
