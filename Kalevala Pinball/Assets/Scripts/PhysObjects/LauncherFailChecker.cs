using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class LauncherFailChecker: MonoBehaviour
    {

        private void OnCollisionEnter( Collision collision )
        {
            Pinball ball = collision.gameObject.GetComponent<Pinball>();
            if(ball != null && !Launcher.Instance.BallOnLauncher)
            {
                Launcher.Instance.StartLaunch(ball);
            }
        }

    }
}
