using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public class NECanvas
{
    private Vector2 scrollPos;
    private Rect m_sPosition;
    private Rect m_sScrollViewRect = new Rect(0,0,10000,10000);
    private List<NENode> m_lstNode = new List<NENode>();
    private NENode m_cSelectedNode;
    private NENode m_cDragNode;

    public NECanvas()
    {
        scrollPos = new Vector2(m_sScrollViewRect.width / 2f,m_sScrollViewRect.height / 2f);
        m_cSelectedNode = null;
        m_cDragNode = null;
        //LoadNodes(nodeAttribute);
    }

    private void LoadNodes()
    {
        //Assembly assembly = typeof(NECanvas).Assembly;
        //var lst = assembly.GetTypes();
        //foreach (var item in lst)
        //{
        //    object[] arrAttrs = item.GetCustomAttributes(nodeAttribute, false);
        //    if (0 == arrAttrs.Length) continue;
        //}
    }

    public void Draw(Rect position)
    {
        m_sPosition = position;
        Rect rect = new Rect(0, 0, position.width, position.height);
        scrollPos = GUI.BeginScrollView(rect, scrollPos, m_sScrollViewRect, true, true);
        DrawGrid();
        DrawNodes();
        HandleEvent(Event.current);
        GUI.EndScrollView();
    }

    private void DrawNodes()
    {
        for (int i = 0; i < m_lstNode.Count; i++)
        {
            m_lstNode[i].Draw();
        }
    }

    private void HandleEvent(Event e)
    {
        Rect rect = new Rect(scrollPos.x, scrollPos.y, m_sPosition.width, m_sPosition.height);
        bool isInWindow = rect.Contains(e.mousePosition);
        //左键按下
        if (e.button == 0)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (isInWindow)
                    {
                        NENode selectNode = GetNodeByPosition(e.mousePosition);
                        if (selectNode != m_cSelectedNode)
                        {
                            SelectNode(selectNode);
                            GUI.changed = true;
                        }
                        m_cDragNode = selectNode;
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    m_cDragNode = null;
                    break;
                case EventType.MouseDrag:
                    if(isInWindow && null != m_cDragNode)
                    {
                        m_cDragNode.SetPosition(e.mousePosition);
                        e.Use();
                        GUI.changed = true;
                    }
                    break;
            }
        }
        //右键按下
        else if (e.button == 1)
        {
            if (isInWindow)
            {
                NENode selectNode = GetNodeByPosition(e.mousePosition);
                if (selectNode != null)
                {
                    HandleNodeMenu(selectNode, e.mousePosition);
                    e.Use();
                }
                else
                {
                    HandleBlankMenu(e.mousePosition);
                    e.Use();
                }
            }
        }
    }

    private NENode GetNodeByPosition(Vector2 pos)
    {
        NENode selectNode = null;
        for (int i = 0; i < m_lstNode.Count; i++)
        {
            if (m_lstNode[i].Contains(pos))
            {
                selectNode = m_lstNode[i];
                break;
            }
        }
        return selectNode;
    }

    private void SelectNode(NENode node)
    {
        for (int i = 0; i < m_lstNode.Count; i++)
        {
            if(m_lstNode[i] == node)
            {
                m_lstNode[i].SetSelected(true);
            }
            else
            {
                if (m_lstNode[i].isSelected) m_lstNode[i].SetSelected(false);
            }
        }
        m_cSelectedNode = node;
    }

    private void HandleBlankMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("测试"), false, ()=> { m_lstNode.Add(new NENode(mousePosition)); });
        menu.ShowAsContext();
    }

    private void HandleNodeMenu(NENode node,Vector2 mousePosition)
    {

    }

    private void DrawGrid()
    {
        DrawGrid(10, new Color(0.5f, 0.5f, 0.5f, 0.2f));
        DrawGrid(50, new Color(0.5f, 0.5f, 0.5f, 0.4f));
    }

    private void DrawGrid(float gridSpacing,Color gridColor)
    {
        int column = Mathf.CeilToInt(m_sScrollViewRect.height / gridSpacing);
        int row = Mathf.CeilToInt(m_sScrollViewRect.width / gridSpacing);
        Handles.BeginGUI();
        Color oldColor = Handles.color;
        Handles.color = gridColor;
        for (int i = 0; i < column; i++)
        {
            Handles.DrawLine(new Vector3(0, i * gridSpacing, 0), new Vector3(m_sScrollViewRect.width,i * gridSpacing,0));
        }
        for (int i = 0; i < row; i++)
        {
            Handles.DrawLine(new Vector3(i * gridSpacing,0,0),new Vector3(i * gridSpacing, m_sScrollViewRect.height,0));
        }
        Handles.color = oldColor;
        Handles.EndGUI();
    }

    public void Clear()
    {
        m_lstNode.Clear();
    }
}
