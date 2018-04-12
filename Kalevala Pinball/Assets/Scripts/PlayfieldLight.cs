using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfieldLight : MonoBehaviour {


    public Light Up, Down;

	
    public void Switch(bool onoff)
    {
        Up.enabled = onoff;
        Down.enabled = onoff;
    }
}
