using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Playfield : MonoBehaviour
    {
        private DropTarget[] _dropTargets;

        private void Start()
        {
            _dropTargets = GetComponentsInChildren<DropTarget>();
        }

        public void ResetPlayfield()
        {
            foreach (DropTarget dt in _dropTargets)
            {
                dt.ResetDropTarget();
            }
        }
    }
}
