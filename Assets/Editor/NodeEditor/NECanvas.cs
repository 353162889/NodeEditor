using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NECanvas
{
    private Rect m_sPosition;
    private List<NENode> m_lstNode = new List<NENode>();

    public void Draw(Rect position)
    {

    }

    public void Clear()
    {
        m_lstNode.Clear();
    }
}
