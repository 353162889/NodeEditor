using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class NEDemoWindow : EditorWindow
{
    private NECanvas m_cCanvas;
    private float m_fSideAreaWidth = 100;
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
        m_cLeftAreaStyle.border = new RectOffset(2, 2, 2, 2);

        m_cLeftAreaStyle = new GUIStyle();
        m_cLeftAreaStyle.border = new RectOffset(2, 2, 2, 2);

        m_cRightAreaStyle = new GUIStyle();
        m_cRightAreaStyle.border = new RectOffset(2, 2, 2, 2);
    }

    void OnDisable()
    {
        m_cCanvas.Clear();
        m_cCanvas = null;
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Height(position.height));
        GUILayout.BeginArea(new Rect(0, 0, m_fSideAreaWidth, position.height));
        GUILayout.EndArea();
        float width = position.width - m_fSideAreaWidth;
        if (width < m_fSideAreaWidth) width = m_fSideAreaWidth;
        GUILayout.BeginArea(new Rect(m_fSideAreaWidth, 0, width, position.height));
        m_cCanvas.Draw(new Rect(m_fSideAreaWidth, 0,width,position.height));
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(0, 0, m_fSideAreaWidth, position.height));
        GUILayout.EndArea();
        EditorGUILayout.EndHorizontal();
        //position = new Rect(position.x,position.y, 300 + width + 300,position.height);
    }
}
