using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class NENode
    {
        public Rect rect;
        private GUIStyle m_cNormalStyle;
        private GUIStyle m_cSelectStyle;
        private GUIStyle m_cStyle;
        private GUIStyle m_cContentStyle;
        private GUIStyle m_cCloseStyle;
        private Texture2D m_cImg;
        private float m_fImgWidth = 60;
        private string m_sDesc;
        public bool isSelected { get { return m_bIsSelected; } }
        private bool m_bIsSelected;

        private NENodePoint m_cInPoint;
        private NENodePoint m_cOutPoint;

        public NENode(Vector2 position)
        {
            m_cNormalStyle = (GUIStyle)"flow node 0";
            m_cSelectStyle = (GUIStyle)"flow node 0 on";
            m_cCloseStyle = (GUIStyle)"TL SelectionBarCloseButton";
            m_cImg = EditorGUIUtility.FindTexture("Favorite Icon");

            m_sDesc = "Action Node";
            m_cStyle = m_cNormalStyle;
            m_cContentStyle = new GUIStyle();
            m_cContentStyle.fontSize = 16;
            m_cContentStyle.normal.textColor = Color.white;
            m_cContentStyle.alignment = TextAnchor.MiddleCenter;


            float width = 120;
            float height = 100;
            var descSize = m_cContentStyle.CalcSize(new GUIContent(m_sDesc));
            width = Mathf.Max(descSize.x, width) + 20;
            rect = new Rect(position.x - width / 2, position.y - height / 2, width, height);

            m_cInPoint = new NENodePoint(this, NENodePointType.In);
            m_cOutPoint = new NENodePoint(this, NENodePointType.Out);
        }

        public virtual void Draw(Action<NENode> onClickRemoveNode, Action<NENodePoint> onClickNodePoint)
        {
            if (m_cInPoint != null)
            {
                m_cInPoint.Draw(onClickNodePoint);
            }
            if (m_cOutPoint != null)
            {
                m_cOutPoint.Draw(onClickNodePoint);
            }
            GUILayout.BeginArea(rect, m_cStyle);
            if (m_cImg != null)
            {
                GUI.DrawTexture(new Rect((rect.width - m_fImgWidth) / 2, 4, m_fImgWidth, m_fImgWidth), m_cImg);
            }
            if (!string.IsNullOrEmpty(m_sDesc))
            {
                GUI.Label(new Rect(0, m_fImgWidth + 4, rect.width, rect.height - m_fImgWidth - 4), m_sDesc, m_cContentStyle);
            }
            GUILayout.EndArea();
            float closeWidth = m_cCloseStyle.normal.background.width;
            float closeHeight = m_cCloseStyle.normal.background.height;
            if (GUI.Button(new Rect(rect.x + rect.width - closeWidth / 2, rect.y - closeHeight / 2, closeWidth, closeHeight), "", m_cCloseStyle))
            {
                if (null != onClickRemoveNode)
                {
                    onClickRemoveNode(this);
                }
            }
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
    }
}