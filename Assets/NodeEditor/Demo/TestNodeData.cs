using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NodeEditor
{
    public enum TestEnum
    {
        enum1,
        enum2
    }
    [NENodeCategory("test1")]
    [NENodeData]
    public class TestNodeData
    {
        [NEName("这是一个测试")]
        public int aa = 2;
        private float bb1;
        protected string cc;
        public bool asd1;
        public Vector2 v2;
        public Vector3 v31111;
        public TestEnum ee;
        public int[] arrInt;
        public bool[] arrBool;
        public float[] arrFloat;
        [NEName("这是一个测试1")]
        public TestEnum[] arrEnum;
        public Vector2[] arrVector2;
    }
    [NENodeCategory("test2")]
    [NENodeData]
    public class TestNodeData1
    {
        [NEName("这是一个测试")]
        public int aa;
        public float bb1;
        public string cc;
        public bool asd1;
        public Vector2 v2;
        public Vector3 v31111;
        public TestEnum ee;
        public int[] arrInt;
        public bool[] arrBool;
        public float[] arrFloat;
        [NEName("这是一个测试1")]
        public TestEnum[] arrEnum;
        public Vector2[] arrVector2;
    }
    [NENodeDisplay(false,true,false)]
    public class TestNodeData2
    {
        [NEName("这是一个测试")]
        public int aa;
        public float bb1;
        public string cc;
        public bool asd1;
        public Vector2 v2;
        public Vector3 v31111;
        public TestEnum ee;
        public int[] arrInt;
        public bool[] arrBool;
        public float[] arrFloat;
        [NEName("这是一个测试1")]
        public TestEnum[] arrEnum;
        public Vector2[] arrVector2;
        [NENodeData]
        public TestNodeData data = new TestNodeData();
    }
}
