using UnityEngine;
using UnityEngine.UI;

namespace Kalevala
{
    public interface IMenu
    {
        Button[] GetMenuButtons();
        Button GetDefaultSelectedButton();
    }
}
