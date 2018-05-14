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
        private GUIStyle m_cLeftAreaStyle;
        private GUIStyle m_cCenterAreaStyle;
        private GUIStyle m_cRightAreaStyle;
        private TestData m_cData;
        private NEDataProperty[] m_arrNEDataProperty;

        [MenuItem("Tools/OpenNEDemoWindow")]
        static public void OpenWindow()
        {
            var window = EditorWindow.GetWindow<NEDemoWindow>();
            window.titleContent = new GUIContent("NEDemoWindow");
            window.Show();
        }

        void OnEnable()
        {
            m_cCanvas = new NECanvas();

            m_cLeftAreaStyle = new GUIStyle();
            // m_cLeftAreaStyle.border = new RectOffset(2, 2, 2, 2);
            m_cLeftAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            m_cCenterAreaStyle = new GUIStyle();
            m_cCenterAreaStyle.border = new RectOffset(2, 2, 2, 2);

            m_cRightAreaStyle = new GUIStyle();
            m_cRightAreaStyle.border = new RectOffset(2, 2, 2, 2);
            m_cData = new TestData();
            m_arrNEDataProperty = NEDataProperties.GetProperties(m_cData);
        }

        void OnDisable()
        {
            m_cCanvas.Clear();
            m_cCanvas = null;
        }

        private int toolBarIndex = 0;
        private Vector3 scrollPos;
        void OnGUI()
        {
            GUIStyle toolBarStyle = (GUIStyle)"toolbarbutton";
            toolBarStyle.alignment = TextAnchor.MiddleLeft;
            //标题区域
            Rect titleRect = new Rect(0, 0, position.width, 20);
            GUILayout.BeginArea(titleRect, "这是工具栏", toolBarStyle);
            GUILayout.EndArea();

            float leftAreaWidth = 200;
            float rightAreaWidth = 200;
            float centerAreaWidth = position.width - leftAreaWidth - rightAreaWidth;
            if (centerAreaWidth < 0) centerAreaWidth = 0;

            GUIStyle titleBarStyle = (GUIStyle)"IN BigTitle";
            titleBarStyle.alignment = TextAnchor.MiddleLeft;
            //画布整体描述区域
            Rect leftArea = new Rect(0, titleRect.height, leftAreaWidth, position.height - titleRect.height);
            GUILayout.BeginArea(leftArea);
            GUILayout.Label("总描述", titleBarStyle, GUILayout.Width(leftArea.width));
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            //画布区域
            
            Rect centerArea = new Rect(leftArea.width, titleRect.height, centerAreaWidth, position.height - titleRect.height);
            GUILayout.BeginArea(centerArea);
            m_cCanvas.Draw(centerArea);
            GUILayout.EndArea();

            //单个节点描述区域
            Rect rightArea = new Rect(leftArea.width + centerAreaWidth, titleRect.height, rightAreaWidth, position.height - titleRect.height);
            GUILayout.BeginArea(rightArea);
            GUILayout.Label("节点描述", titleBarStyle, GUILayout.Width(rightArea.width));
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            NEDataProperties.Draw(m_arrNEDataProperty, GUILayout.Width(rightArea.width - 50));
            EditorGUIUtility.labelWidth = oldLabelWidth;
            GUILayout.EndArea();

            if (GUI.changed) Repaint();
        }
    }

    public enum TestEnum
    {
        enum1,
        enum2
    }
    public class TestData
    {
        [NEDataDesc("这是一个测试")]
        public int aa;
        public float bb;
        public string cc;
        public bool asd;
        public Vector2 v2;
        public Vector3 v31111;
        public TestEnum ee;
        public int[] arrInt;
        public bool[] arrBool;
        public float[] arrFloat;
        [NEDataDesc("这是一个测试1")]
        public TestEnum[] arrEnum;
        public Vector2[] arrVector2;
    }
}