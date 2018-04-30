using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public abstract class Shot : MonoBehaviour
    {
        public abstract int RepeatActivations { get; set; }
        public abstract bool CheckActivation();
        public abstract void ResetShot();
    }
}
