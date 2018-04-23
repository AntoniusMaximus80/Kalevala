using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kalevala {
    public class ResourceMeter: MonoBehaviour {

        private Image _myImage;
        [Range(0f, 1f)]
        public float fill;
        // Use this for initialization
        void Start() {
            _myImage = GetComponent<Image>();
            PinballManager.Instance.ResourcesChanged += ResourcesChanged;
        }

        // Update is called once per frame
        void Update() {
            _myImage.fillAmount = fill;
        }

        private void OnDestroy()
        {
            PinballManager.Instance.ResourcesChanged -= ResourcesChanged;
        }

        private void ResourcesChanged ()
        {
            float resources = PinballManager.Instance.Resources;
            fill = resources / PinballManager.Instance.MaxResources;
        }
    }
}
