using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Kalevala.WaypointSystem;

namespace Kalevala.Editor
{
    [UnityEditor.CustomEditor(typeof(GameManager))]
    public class GameManagerInspector : UnityEditor.Editor
    {
        private GameManager targetGM;

        protected void OnEnable()
        {
            targetGM = target as GameManager;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
