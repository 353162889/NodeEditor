using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class NEDemoWindow : EditorWindow
    {
        private NECanvas m_cCanvas;
        private GUIStyle m_cToolBarStyle;
        private GUIStyle m_cLeftAreaStyle;
        private GUIStyle m_cCenterAreaStyle;
        private GUIStyle m_cRightAreaStyle;

        [MenuItem("Tools/OpenNEDemoWindow")]
        static public void OpenWindow()
        {
            var window = EditorWindow.GetWindow<NEDemoWindow>();
            window.titleContent = new GUIContent("NEDemoWindow");
            window.Show();
        }

        void OnEnable()
        {
            List<Type> types = new List<Type>();
            var assembly = typeof(NENodeAttribute).Assembly;
            var lstTypes = assembly.GetTypes();
            for (int i = 0; i < lstTypes.Length; i++)
            {
                var arr = lstTypes[i].GetCustomAttributes(typeof(NENodeAttribute), true);
                if(arr.Length > 0)
                {
                    types.Add(lstTypes[i]);
                }
            }
            m_cCanvas = new NECanvas(types, CreateNodeData);
            m_cToolBarStyle = null;

            m_cLeftAreaStyle = new GUIStyle();
            // m_cLeftAreaStyle.border = new RectOffset(2, 2, 2, 2);
            m_cLeftAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            m_cCenterAreaStyle = new GUIStyle();
            m_cCenterAreaStyle.border = new RectOffset(2, 2, 2, 2);

            m_cRightAreaStyle = new GUIStyle();
            m_cRightAreaStyle.border = new RectOffset(2, 2, 2, 2);
        }

        object CreateNodeData(Type type)
        {
            object data = Activator.CreateInstance(type);
            return data;    
        }

        void OnDisable()
        {
            m_cCanvas.Clear();
            m_cCanvas = null;
            m_cToolBarStyle = null;
        }

        private int toolBarIndex = 0;
        private Vector3 scrollPos;
        void OnGUI()
        {
            if (m_cToolBarStyle == null)
            {
                m_cToolBarStyle = new GUIStyle((GUIStyle)"toolbarbutton");
            }
            float titleHeight = 20;

            float leftAreaWidth = 200;
            float rightAreaWidth = 200;
            float centerAreaWidth = position.width - leftAreaWidth - rightAreaWidth;
            if (centerAreaWidth < 0) centerAreaWidth = 0;

            //画布整体描述区域
            Rect leftArea = new Rect(0, titleHeight, leftAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(leftArea);
            GUILayout.Label("总描述", m_cToolBarStyle, GUILayout.Width(leftArea.width));
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            //画布区域
            
            Rect centerArea = new Rect(leftArea.width, titleHeight, centerAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(centerArea);
            m_cCanvas.Draw(centerArea);
            GUILayout.EndArea();

            //单个节点描述区域
            Rect rightArea = new Rect(leftArea.width + centerAreaWidth, titleHeight, rightAreaWidth, position.height - titleHeight);
            GUILayout.BeginArea(rightArea);
            GUILayout.Label("节点描述", m_cToolBarStyle, GUILayout.Width(rightArea.width));
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            if (m_cCanvas.selectNode != null && m_cCanvas.selectNode.dataProperty != null)
            {
                NEDataProperties.Draw(m_cCanvas.selectNode.dataProperty, GUILayout.Width(rightArea.width - 50));
            }
            EditorGUIUtility.labelWidth = oldLabelWidth;
            GUILayout.EndArea();

            //标题区域
            Rect titleRect = new Rect(0, 0, position.width, titleHeight);
            m_cToolBarStyle.fixedHeight = titleRect.height;
            GUILayout.BeginArea(titleRect);
            //GUILayout.Label("", tt,GUILayout.Width(50),GUILayout.Height(20));
            GUILayout.BeginHorizontal();
            GUILayout.Label("", m_cToolBarStyle, GUILayout.Width(10));
            if (GUILayout.Button("创建", m_cToolBarStyle, GUILayout.Width(100)))
            {
                CreateData();
            }
            if (GUILayout.Button("加载", m_cToolBarStyle, GUILayout.Width(100)))
            {
                LoadData();
            }
            if (GUILayout.Button("保存", m_cToolBarStyle, GUILayout.Width(100)))
            {
                SaveData();
            }
            GUILayout.Label("", m_cToolBarStyle, GUILayout.Width(position.width - 10 - 100 - 100 - 10));
            GUILayout.Label("", m_cToolBarStyle, GUILayout.Width(10));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (GUI.changed) Repaint();
        }

        private void CreateData()
        {
        }

        private void LoadData()
        {
            string path = EditorUtility.OpenFilePanel("加载数据", Application.dataPath, "txt");
            if(path.Length != 0)
            {
            }
        }

        private void SaveData()
        {
            string path = EditorUtility.SaveFilePanel("保存数据", Application.dataPath, "", "txt");
            if (path.Length != 0)
            {
            }
        }
    }
}