using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class SkillShotHandler: MonoBehaviour
    {

        private void OnTriggerEnter( Collider other )
        {
            Pinball ball = other.GetComponent<Pinball>();
            if(ball != null)
            {
                ball.transform.position = new Vector3(0, 1f, 0f);
            }
        }

    }
}
