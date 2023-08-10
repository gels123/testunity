using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

namespace TestTileMap
{
    public class TestTileMapScript : MonoBehaviour
    {
        public Transform tf;

        public void Start()
        {
            //public Transform tf;
            tf.SetAsFirstSibling(); // 设置最先渲染, 其它盖上面
            tf.SetAsLastSibling(); // 设置最后渲染, 即盖别的
            tf.SetSiblingIndex(100); // 自定义排序, 大的盖小的
        }
    }
}