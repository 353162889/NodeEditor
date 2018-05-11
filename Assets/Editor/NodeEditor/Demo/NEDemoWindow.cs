using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class NEDemoWindow : EditorWindow
{
    private NECanvas m_cCanvas;
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
        m_cCanvas = new NECanvas();

        m_cLeftAreaStyle = new GUIStyle();
       // m_cLeftAreaStyle.border = new RectOffset(2, 2, 2, 2);
        m_cLeftAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        m_cCenterAreaStyle = new GUIStyle();
        m_cCenterAreaStyle.border = new RectOffset(2, 2, 2, 2);

        m_cRightAreaStyle = new GUIStyle();
        m_cRightAreaStyle.border = new RectOffset(2, 2, 2, 2);
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
        Rect titleRect = new Rect(0,0,position.width,20);
        GUILayout.BeginArea(titleRect,"这是工具栏", toolBarStyle);
        GUILayout.EndArea();

        GUIStyle titleBarStyle = (GUIStyle)"IN BigTitle";
        titleBarStyle.alignment = TextAnchor.MiddleLeft;
        //画布整体描述区域
        Rect leftArea = new Rect(0,titleRect.height, 150, position.height - titleRect.height);
        GUILayout.BeginArea(leftArea);
        GUILayout.Label("总描述", titleBarStyle, GUILayout.Width(leftArea.width));
        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        //画布区域
        float width = position.width - leftArea.width * 2;
        if (width < 0) width = 0;
        Rect centerArea = new Rect(leftArea.width, titleRect.height, width, position.height - titleRect.height);
        GUILayout.BeginArea(centerArea);
        m_cCanvas.Draw(centerArea);
        GUILayout.EndArea();

        //单个节点描述区域
        Rect rightArea = new Rect(leftArea.width + width, titleRect.height, 150, position.height - titleRect.height);
        GUILayout.BeginArea(rightArea);
        GUILayout.Label("节点描述", titleBarStyle, GUILayout.Width(leftArea.width));
        GUILayout.EndArea();

        if (GUI.changed) Repaint();
    }
}
