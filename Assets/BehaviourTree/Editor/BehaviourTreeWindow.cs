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
        new BTTreeComposeType(typeof(BTRoot),new List<Type> { typeof(NENodeAttribute) },"","bytes"),
    };

    private string[] m_sBTTreeComposeTypeDesc = new string[] {
        "节点",
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
                        btData = GetCurrentTreeBTData();
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
        m_cCanvas = new NECanvas(lst, CreateBTNodeByBTNodeType);
        CreateTreeByTreeData(btData);
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

    private NENode CreateTreeByTreeData(BTData btData)
    {
        if (m_cCanvas != null) m_cCanvas.Clear();
        NENode node = null;
        if(btData == null)
        {
            var composeData = m_arrTreeComposeData[m_nTreeComposeIndex];
            object data = CreateBTNodeByBTNodeType(composeData.rootType);
            Vector2 center = m_cCanvas.scrollViewRect.center;
            node = m_cCanvas.CreateNode(center, data);
        }
        else
        {
            node = CreateNENode(btData);
        }
        m_cRoot = node;
        FocusCanvasCenterPosition();
        return node;
    }

    private NENode CreateNENode(BTData btData)
    {
        if(btData.data == null)
        {
            Debug.LogError("btData.data == null");
            return null;
        }
        object btNode = CreateBTNodeByData(btData.data);
        NENode parentNode = m_cCanvas.CreateNode(btData.editorPos, btNode);
        if(btData.lstChild != null)
        {
            for (int i = 0; i < btData.lstChild.Count; i++)
            {
                NENode childNode = CreateNENode(btData.lstChild[i]);
                NEConnection connection = m_cCanvas.CreateConnect(parentNode, childNode);
            }
        }
        return parentNode;
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

    private object CreateBTNodeByBTNodeType(Type btNodeType)
    {
        int index = m_lstNodeType.IndexOf(btNodeType);
        if (index == -1)
        {
            Debug.LogError("类型为:" + btNodeType + "的节点没有配置节点数据类型");
            return null;
        }
        Type dataType = m_lstNodeDataType[index];
        return CreateBTNode(btNodeType,dataType,null);
    }

    private object CreateBTNodeByData(object data)
    {
        Type dataType = data.GetType();
        int index = m_lstNodeDataType.IndexOf(dataType);
        if(index == -1)
        {
            Debug.LogError("类型为:" + dataType + "的数据没有配置节点类型");
            return null;
        }
        Type nodeType = m_lstNodeType[index];
        return CreateBTNode(nodeType,dataType,data);
    }

    private object CreateBTNode(Type btNodeType,Type btNodeDataType,object data = null)
    {
        object node = Activator.CreateInstance(btNodeType);
        var fieldInfos = btNodeType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        foreach (var item in fieldInfos)
        {
            if (item.GetCustomAttributes(typeof(NENodeDataAttribute), true).Length > 0)
            {
                if (data == null)
                {
                    data = Activator.CreateInstance(btNodeDataType);
                }
                item.SetValue(node, data);
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
        if (GUILayout.Button("创建", m_cToolBarBtnStyle, GUILayout.Width(50))) { CreateTreeByTreeData(null); }
        if (GUILayout.Button("加载", m_cToolBarBtnStyle, GUILayout.Width(50))) { LoadTreeByTreeData(); }
        if (GUILayout.Button("保存", m_cToolBarBtnStyle, GUILayout.Width(50))) { SaveTreeToTreeData(); }
        GUILayout.Label("", m_cToolBarBtnStyle, GUILayout.Width(10));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (GUI.changed) Repaint();
    }

    private void LoadTreeByTreeData()
    {
        string path = EditorUtility.OpenFilePanel("加载数据", Application.dataPath, "bytes");
        if (path.Length != 0)
        {
            //通过前后缀确定当前数据是哪种类型,需要先切换到当前类型，在加载数据，否则数据有可能不对
            BTData btData = BTUtil.DeSerializerObject(path, typeof(BTData), m_lstNodeDataType.ToArray()) as BTData;
            CreateTreeByTreeData(btData);
        }
    }

    private void SaveTreeToTreeData()
    {
        if (m_nTreeComposeIndex < 0 || m_nTreeComposeIndex > m_arrTreeComposeData.Length)
        {
            Debug.Log("需要选择树的类型");
            return;
        }
        var composeData = m_arrTreeComposeData[m_nTreeComposeIndex];
        BTData data = GetCurrentTreeBTData();
        if (data == null)
        {
            Debug.Log("没有树数据");
            return;
        }
        string path = EditorUtility.SaveFilePanel("保存数据", Application.dataPath, "", composeData.fileExt);
        if (path.Length != 0)
        {
            BTUtil.SerializerObject(path, data, m_lstNodeDataType.ToArray()); 
        }
    }

    private BTData GetCurrentTreeBTData()
    {
        if (m_cRoot == null) return null;
        var lstConnection = m_cCanvas.lstConnection;
        List<NENode> handNodes = new List<NENode>();
        BTData btData = GetNodeBTData(m_cRoot, lstConnection, handNodes);
        return btData;
    }

    private BTData GetNodeBTData(NENode node,List<NEConnection> lst,List<NENode> handNodes)
    {
        if (handNodes.Contains(node))
        {
            Debug.LogError("树的连线进入死循环，节点="+node.node.GetType());
            return null;
        }
        handNodes.Add(node);
       
        BTNode btNode = node.node as BTNode;
        BTData btData = new BTData();
        btData.data = btNode.data;
        btData.editorPos = node.rect.center;

        for (int i = 0; i < lst.Count; i++)
        {
            NEConnection connection = lst[i];
            if(connection.outPoint.node == node)
            {
                NENode childNode = connection.inPoint.node;
                BTData childBTData = GetNodeBTData(childNode, lst, handNodes);
                if (btData.lstChild == null) btData.lstChild = new List<BTData>();
                btData.lstChild.Add(childBTData);
            }
        }
        return btData;
    }
}

