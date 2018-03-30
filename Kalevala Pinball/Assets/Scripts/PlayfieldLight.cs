using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldLight : MonoBehaviour {


    public Light Up, Down;

	public void TurnOn()
    {
        Up.enabled = true;
        Down.enabled = true;
    }

    public void TurnOff()
    {
        Up.enabled = false;
        Down.enabled = false;
    }

    public void Switch(bool onoff)
    {
        Up.enabled = onoff;
        Down.enabled = onoff;
    }
}
