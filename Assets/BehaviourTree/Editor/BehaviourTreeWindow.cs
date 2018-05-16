using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using NodeEditor;
using System.Reflection;
using BTCore;

public class BehaviourTreeWindow : EditorWindow
{
    private static float titleHeight = 20;
    private static float leftAreaWidth = 200;
    private static float rightAreaWidth = 200;

    public NECanvas canvas { get { return m_cCanvas; } }
    private NECanvas m_cCanvas;
    private List<Type> m_lstNodeType;
    private List<Type> m_lstNodeDataType;
    private GUIStyle m_cToolBarBtnStyle;
    private GUIStyle m_cToolBarPopupStyle;

    private NENode m_cRoot;

    [MenuItem("Tools/BehaviourTree")]
    static public void OpenWindow()
    {
        var window = EditorWindow.GetWindow<BehaviourTreeWindow>();
        window.titleContent = new GUIContent("BehaviourTreeWindow");
        var position = window.position;
        position.center = new Rect(0f, 0f, Screen.currentResolution.width, Screen.currentResolution.height).center;
        window.position = position;
       
        window.Show();
        //之后设置位置
        if (window.canvas != null)
        {
            float canvasWidth = window.position.width - leftAreaWidth - rightAreaWidth;
            float canvasHeight = window.position.height - titleHeight;
            Vector2 firstScrollPos = new Vector2((window.canvas.scrollViewRect.width - canvasWidth) / 2, (window.canvas.scrollViewRect.height - canvasHeight) / 2);
            window.canvas.scrollPos = firstScrollPos;
        }
    }

    void OnEnable()
    {
        LoadByAttribute(new List<Type> { typeof(NENodeAttribute) });
        BTData btData = null;
        if(m_cRoot != null)
        {
            var btNode = m_cRoot.node as BTNode;
            btData = btNode.GetData();
            m_cRoot = null;
        }
        m_cCanvas = new NECanvas(m_lstNodeType, CreateNodeData);
        //这里设置位置会有些问题
        m_cToolBarBtnStyle = null;
        m_cToolBarPopupStyle = null;
        if(btData == null)
        {
            object data = CreateNodeData(typeof(BTRoot));
            Vector2 center = m_cCanvas.scrollViewRect.center;
            m_cRoot = m_cCanvas.CreateNode(center, data);
        }
        else
        {
            m_cRoot = CreateBTData(btData);
        }
    }

    public void SetCanvasFirstPos(Vector2 pos)
    {
        if(null != m_cCanvas)
        {
            m_cCanvas.scrollPos = pos;
        }
    }

    private NENode CreateBTData(BTData btData)
    {
        if(btData.data == null)
        {
            Debug.LogError("btData.data == null");
        }
        if(btData.lstChild != null)
        {
            for (int i = 0; i < btData.lstChild.Count; i++)
            {

            }
        }
        return null;
    }

    private void LoadByAttribute(List<Type> types)
    {
        m_lstNodeType = new List<Type>();
        m_lstNodeDataType = new List<Type>();
        for (int i = 0; i < types.Count; i++)
        {
            var assembly = types[i].Assembly;
            var lstTypes = assembly.GetTypes();
            for (int j = 0; j < lstTypes.Length; j++)
            {
                if (lstTypes[i] == typeof(BTRoot)) continue;
                var arr = lstTypes[j].GetCustomAttributes(types[i], true);
                if (arr.Length > 0)
                {
                    m_lstNodeType.Add(lstTypes[j]);
                    var attr = arr[0] as NENodeAttribute;
                    m_lstNodeDataType.Add(attr.nodeDataType);
                }
            }
        }
    }

    private object CreateNodeData(Type type)
    {
        int index = m_lstNodeType.IndexOf(type);
        if (index == -1)
        {
            Debug.LogError("类型为:" + type + "的节点没有配置节点数据类型");
            return null;
        }
        Type dataType = m_lstNodeDataType[index];
        object node = Activator.CreateInstance(type);
        var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        foreach (var item in fieldInfos)
        {
            if (item.GetCustomAttributes(typeof(NENodeDataAttribute), true).Length > 0)
            {
                object nodeShowData = Activator.CreateInstance(dataType);
                item.SetValue(node, nodeShowData);
                break;
            }
        }
        return node;
    }

    void OnDisable()
    {
        m_cCanvas.Clear();
        m_cCanvas = null;
        m_cToolBarBtnStyle = null;
        m_cToolBarPopupStyle = null;
    }

    private int toolBarIndex = 0;
    private Vector3 leftScrollPos;
    void OnGUI()
    {
        if (m_cToolBarBtnStyle == null)
        {
            m_cToolBarBtnStyle = new GUIStyle((GUIStyle)"toolbarbutton");
        }

        if(m_cToolBarPopupStyle == null)
        {
            m_cToolBarPopupStyle = new GUIStyle((GUIStyle)"ToolbarPopup");
        }
        
        float centerAreaWidth = position.width - leftAreaWidth - rightAreaWidth;
        if (centerAreaWidth < 0) centerAreaWidth = 0;

        //画布整体描述区域
        Rect leftArea = new Rect(0, titleHeight, leftAreaWidth, position.height - titleHeight);
        GUILayout.BeginArea(leftArea);
        GUILayout.Label("总描述", m_cToolBarBtnStyle, GUILayout.Width(leftArea.width));
        leftScrollPos = GUILayout.BeginScrollView(leftScrollPos, false, true);
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
        GUILayout.Label("节点描述", m_cToolBarBtnStyle, GUILayout.Width(rightArea.width));
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
        m_cToolBarBtnStyle.fixedHeight = titleRect.height;
        m_cToolBarPopupStyle.fixedHeight = titleRect.height;
        GUILayout.BeginArea(titleRect);
        //GUILayout.Label("", tt,GUILayout.Width(50),GUILayout.Height(20));
        GUILayout.BeginHorizontal();
        GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(10));
        if (GUILayout.Button("创建", m_cToolBarBtnStyle, GUILayout.Width(100)))
        {
            CreateData();
        }
        if (GUILayout.Button("加载", m_cToolBarBtnStyle, GUILayout.Width(100)))
        {
            LoadData();
        }
        if (GUILayout.Button("保存", m_cToolBarBtnStyle, GUILayout.Width(100)))
        {
            SaveData();
        }
        toolBarIndex = EditorGUILayout.Popup(toolBarIndex,new string[] { "tt"},m_cToolBarPopupStyle);
        GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(position.width - 10 - 100 - 100 - 10));
        GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(10));
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
        if (path.Length != 0)
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
