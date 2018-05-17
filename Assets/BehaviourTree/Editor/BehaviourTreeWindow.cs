using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using NodeEditor;
using System.Reflection;
using BTCore;
using System.Xml.Serialization;

public class BehaviourTreeWindow : EditorWindow
{
    public class BTTreeComposeType
    {
        public Type rootType { get; private set; }
        public List<Type> lstNodeAttribute { get; private set; }
        public string filePre { get; private set; }//前缀
        public string fileExt { get; private set; }//后缀

        public BTTreeComposeType(Type rootType,List<Type> lstNodeAttr,string filePre,string fileExt)
        {
            this.rootType = rootType;
            this.lstNodeAttribute = lstNodeAttr;
            this.filePre = filePre;
            this.fileExt = fileExt;
        }
    }
    private static float titleHeight = 20;
    private static float leftAreaWidth = 200;
    private static float rightAreaWidth = 200;

    public NECanvas canvas { get { return m_cCanvas; } }
    private NECanvas m_cCanvas;

    private BTTreeComposeType[] m_arrTreeComposeData = new BTTreeComposeType[] {
        //一般数据
        new BTTreeComposeType(typeof(BTRoot),new List<Type> { typeof(NENodeAttribute) },"",""),
        new BTTreeComposeType(typeof(BTRoot1),new List<Type> { typeof(NENodeAttribute) },"",""),
    };

    private string[] m_sBTTreeComposeTypeDesc = new string[] {
        "节点",
        "节点1"
    };
    private int m_nTreeComposeIndex = 0;

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
        window.FocusCanvasCenterPosition();
    }

    void OnEnable()
    {
        m_cToolBarBtnStyle = null;
        m_cToolBarPopupStyle = null;

        Load(m_arrTreeComposeData[m_nTreeComposeIndex]);
    }

    private void Load(BTTreeComposeType containType)
    {
        BTData btData = null;
        if (m_cRoot != null)
        {
            foreach (var item in m_arrTreeComposeData)
            {
                if (item.rootType == m_cRoot.node.GetType())
                {
                    if(item == containType)
                    {
                        var btNode = m_cRoot.node as BTNode;
                        btData = btNode.GetData();
                    }
                    break;
                }
            }
            m_cRoot = null;
        }
        LoadByAttribute(containType.rootType, containType.lstNodeAttribute);
        
        //移除根节点
        List<Type> lst = new List<Type>();
        for (int i = 0; i < m_lstNodeType.Count; i++)
        {
            if (!IsRootType(m_lstNodeType[i])) lst.Add(m_lstNodeType[i]);
        }
        if (m_cCanvas != null) m_cCanvas.Dispose();
        m_cCanvas = new NECanvas(lst, CreateNodeData);
        CreateTree(btData);
    }

    public void FocusCanvasCenterPosition()
    {
        if (m_cCanvas != null)
        {
            float canvasWidth = position.width - leftAreaWidth - rightAreaWidth;
            float canvasHeight = position.height - titleHeight;
            Vector2 firstScrollPos = new Vector2((m_cCanvas.scrollViewRect.width - canvasWidth) / 2, (m_cCanvas.scrollViewRect.height - canvasHeight) / 2);
            m_cCanvas.scrollPos = firstScrollPos;
        }
    }

    public void SetCanvasFirstPos(Vector2 pos)
    {
        if(null != m_cCanvas)
        {
            m_cCanvas.scrollPos = pos;
        }
    }

    private NENode CreateTree(BTData btData)
    {
        if (m_cCanvas != null) m_cCanvas.Clear();
        NENode node = null;
        if(btData == null)
        {
            var composeData = m_arrTreeComposeData[m_nTreeComposeIndex];
            object data = CreateNodeData(composeData.rootType);
            Vector2 center = m_cCanvas.scrollViewRect.center;
            node = m_cCanvas.CreateNode(center, data);
        }
        else
        {
            node = CreateBTData(btData);
        }
        m_cRoot = node;
        FocusCanvasCenterPosition();
        return node;
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

    private void LoadByAttribute(Type rootType, List<Type> types)
    {
        m_lstNodeType = new List<Type>();
        m_lstNodeDataType = new List<Type>();
        for (int i = 0; i < types.Count; i++)
        {
            var assembly = types[i].Assembly;
            var lstTypes = assembly.GetTypes();
            for (int j = 0; j < lstTypes.Length; j++)
            {
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

    private bool IsRootType(Type type)
    {
        for (int i = 0; i < m_arrTreeComposeData.Length; i++)
        {
            if (type == m_arrTreeComposeData[i].rootType) return true;
        }
        return false;
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
        m_cCanvas.Dispose();
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
        int oldTreeComposeIndex = m_nTreeComposeIndex;
        m_nTreeComposeIndex = EditorGUILayout.Popup(m_nTreeComposeIndex, m_sBTTreeComposeTypeDesc, m_cToolBarPopupStyle,GUILayout.Width(100));
        if(oldTreeComposeIndex != m_nTreeComposeIndex)
        {
            Load(m_arrTreeComposeData[m_nTreeComposeIndex]);
        }
        GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(position.width - 10 - 100 - 50 - 50 - 50 - 10));
        if (GUILayout.Button("创建", m_cToolBarBtnStyle, GUILayout.Width(50))) { CreateTree(null); }
        if (GUILayout.Button("加载", m_cToolBarBtnStyle, GUILayout.Width(50))) { LoadTree(); }
        if (GUILayout.Button("保存", m_cToolBarBtnStyle, GUILayout.Width(50))) { SaveTree(); }
        GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(10));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (GUI.changed) Repaint();
    }

    private void LoadTree()
    {
        string path = EditorUtility.OpenFilePanel("加载数据", Application.dataPath, "bytes");
        if (path.Length != 0)
        {
            BTData btData = BTUtil.DeSerializerObject(path, typeof(BTData), m_lstNodeDataType.ToArray()) as BTData;
            CreateTree(btData);
        }
    }

    private void SaveTree()
    {
        if (m_cRoot == null) return;
        var node = m_cRoot.node as BTNode;
        if (node == null) return;
        BTData data = node.GetData();
        string path = EditorUtility.SaveFilePanel("保存数据", Application.dataPath, "", "bytes");
        if (path.Length != 0)
        {
            BTUtil.SerializerObject(path, data, m_lstNodeDataType.ToArray()); 
        }
    }
}

