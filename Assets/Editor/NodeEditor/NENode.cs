using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class NENode
{
    private Rect rect;
    private GUIStyle m_cNormalStyle;
    private GUIStyle m_cSelectStyle;
    private GUIStyle m_cStyle;
    private GUIStyle m_cContentStyle;
    private Texture2D m_cImg;
    private Texture2D m_cPointImg;
    private string m_sDesc;
    public bool isSelected { get { return m_bIsSelected; } }
    private bool m_bIsSelected;

    public NENode(Vector2 position)
    {
        m_cNormalStyle = (GUIStyle)"flow node 0";
        m_cSelectStyle = (GUIStyle)"flow node 0 on";
        m_cImg = EditorGUIUtility.FindTexture("Favorite Icon");
        m_cPointImg = null;

        m_sDesc = "Action Node";
        m_cStyle = m_cNormalStyle;
        m_cContentStyle = new GUIStyle();
        m_cContentStyle.fontSize = 16;
        m_cContentStyle.normal.textColor = Color.white;
        m_cContentStyle.alignment = TextAnchor.MiddleCenter;
        var descSize = m_cContentStyle.CalcSize(new GUIContent(m_sDesc));
        float imgHeight = m_cImg.height;
        float width = Mathf.Max(descSize.x, m_cImg.width) + 20;
        float height = descSize.y + m_cImg.height + 10;
        rect = new Rect(position.x - width / 2,position.y - height / 2,width,height);
    }

    public virtual void Draw()
    {
        //GUI.DrawTexture(new Rect(rect.x + (rect.width - m_cPointImg.width) / 2,rect.y -10, m_cPointImg.width, m_cPointImg.height), m_cPointImg);
        //GUI.DrawTexture(new Rect(rect.x + (rect.width - m_cPointImg.width) / 2,rect.y + rect.height - m_cPointImg.height + 10, m_cPointImg.width, m_cPointImg.height), m_cPointImg);
        GUILayout.BeginArea(rect, m_cStyle);
        GUI.DrawTexture(new Rect((rect.width - m_cImg.width) / 2,0,m_cImg.width,m_cImg.height),m_cImg);
        GUI.Label(new Rect(0,m_cImg.height,rect.width,rect.height - m_cImg.height), m_sDesc, m_cContentStyle);
        GUILayout.EndArea();
    }

    public void SetSelected(bool selected)
    {
        m_bIsSelected = selected;
        m_cStyle = m_bIsSelected ? m_cSelectStyle : m_cNormalStyle;
    }

    public void SetPosition(Vector2 pos)
    {
        rect.position = new Vector2(pos.x - rect.width / 2, pos.y - rect.height / 2);
    }

    public bool Contains(Vector2 pos)
    {
        return rect.Contains(pos);
    }
}
