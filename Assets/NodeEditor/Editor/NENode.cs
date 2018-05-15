using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class NENode
    {
        public Rect rect;
        public object node { get; private set; }
        public NEDataProperty[] dataProperty { get; private set; }
        private GUIStyle m_cNormalStyle;
        private GUIStyle m_cSelectStyle;
        private GUIStyle m_cStyle;
        private GUIStyle m_cContentStyle;
        private GUIStyle m_cCloseStyle;
        private Texture2D m_cImg;
        private float m_fImgWidth = 40;
        private string m_sName;
        public string desc { get; private set; }
        private bool m_bShowInPoint;
        private bool m_bShowOutPoint;
        private bool m_bShowClose;
        public bool isSelected { get { return m_bIsSelected; } }
        private bool m_bIsSelected;

        public NENodePoint inPoint { get { return m_cInPoint; } }
        private NENodePoint m_cInPoint;
        public NENodePoint outPoint { get { return m_cOutPoint; } }
        private NENodePoint m_cOutPoint;

        public NENode(Vector2 position,object node)
        {
            this.node = node;
            m_cNormalStyle = new GUIStyle((GUIStyle)"flow node 0");
            m_cSelectStyle = new GUIStyle((GUIStyle)"flow node 0 on");
            m_cCloseStyle = new GUIStyle((GUIStyle)"TL SelectionBarCloseButton");
            m_cImg = EditorGUIUtility.FindTexture("Favorite Icon");

            m_sName = "";
            desc = "";
            m_bShowInPoint = true;
            m_bShowOutPoint = true;
            m_bShowClose = true;
            if (this.node != null)
            {
                var type = this.node.GetType();
                m_sName = NENameAttribute.GetName(type);
                desc = NEDescAttribute.GetDesc(type);
                var attributes = type.GetCustomAttributes(typeof(NENodeDataAttribute), true);
                object nodeData = null;
                if(attributes.Length > 0)
                {
                    nodeData = this.node;
                }
                else
                {
                    var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    foreach (var item in fieldInfos)
                    {
                        if (item.GetCustomAttributes(typeof(NENodeDataAttribute), true).Length > 0)
                        {
                            nodeData = item.GetValue(this.node);
                            break;
                        }
                    }
                }
                if(nodeData != null)
                {
                    dataProperty = NEDataProperties.GetProperties(nodeData);
                }

                attributes = type.GetCustomAttributes(typeof(NENodeDisplayAttribute), true);
                if(attributes.Length > 0)
                {
                    NENodeDisplayAttribute displayAttribute = attributes[0] as NENodeDisplayAttribute;
                    m_bShowInPoint = displayAttribute.showInPoint;
                    m_bShowOutPoint = displayAttribute.showOutPoint;
                    m_bShowClose = displayAttribute.showClose;
                }
            }
            m_cStyle = m_cNormalStyle;
            m_cContentStyle = new GUIStyle();
            m_cContentStyle.fontSize = 14;
            m_cContentStyle.normal.textColor = Color.white;
            m_cContentStyle.alignment = TextAnchor.MiddleCenter;


            float width = 100;
            float height = 70;
            var descSize = m_cContentStyle.CalcSize(new GUIContent(m_sName));
            width = Mathf.Max(descSize.x, width) + 10;
            rect = new Rect(position.x - width / 2, position.y - height / 2, width, height);

            if (m_bShowInPoint)
            {
                m_cInPoint = new NENodePoint(this, NENodePointType.In);
            }
            if (m_bShowOutPoint)
            {
                m_cOutPoint = new NENodePoint(this, NENodePointType.Out);
            }
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
            if (!string.IsNullOrEmpty(m_sName))
            {
                GUI.Label(new Rect(0, m_fImgWidth + 4, rect.width, rect.height - m_fImgWidth - 4), m_sName, m_cContentStyle);
            }
            GUILayout.EndArea();
            float closeWidth = m_cCloseStyle.normal.background.width;
            float closeHeight = m_cCloseStyle.normal.background.height;
            if (m_bShowClose && GUI.Button(new Rect(rect.x + rect.width - closeWidth / 2, rect.y - closeHeight / 2, closeWidth, closeHeight), "", m_cCloseStyle))
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

        public void MovePosition(Vector2 pos)
        {
            rect.position += pos;
        }
    }
}