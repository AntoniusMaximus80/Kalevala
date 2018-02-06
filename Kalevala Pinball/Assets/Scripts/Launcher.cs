using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour {

    public float launcherForceMultiplier;
    public float timeToMaxForce;
    private float launcherForce;

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            PoweringUp();
        } else if(Input.GetKeyUp(KeyCode.Space))
        {
            Launch();
        }
    }

    private void PoweringUp()
    {
        launcherForce += Mathf.Clamp01(Time.deltaTime / timeToMaxForce);
    }

    private void Launch()
    {
        Collider [] colliders = Physics.OverlapSphere(transform.position, 1f);

        foreach (Collider coll in colliders )
        {
            coll.GetComponent<Rigidbody>().AddForce(-Vector3.forward * launcherForce * launcherForceMultiplier, ForceMode.Impulse);
        }
        launcherForce = 0;
    }


}
