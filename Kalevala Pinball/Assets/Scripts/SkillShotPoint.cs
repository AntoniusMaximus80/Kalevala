using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class SkillShotPoint: MonoBehaviour
    {

        [SerializeField]
        private Transform _child;
        
        [SerializeField, Tooltip("The speed how fast the door moves")]
        private float _speed;

        [SerializeField, Tooltip("Position where the door is in it's closed state")]
        private Vector3 _blockPos;

        private Vector3 _startPos;

        [SerializeField]
        private bool _drawBlockPos;

        private bool _moveWall = false;


        /// <summary>
        /// Saves the startposition of the skillshot door, to move it back open when ball is returned to launcher.
        /// </summary>
        private void Awake()
        {
            _startPos = _child.position;
        }


        private void OnTriggerEnter( Collider other )
        {
            _moveWall = false;
        }

        private void Update()
        {
            if(_moveWall)
            {
                BlockWay();
            }
            else
            {
                ToStartPosition();
            }
        }

        /// <summary>
        /// Moves the skillshot door to block the way.
        /// </summary>
        private void BlockWay()
        {
            _child.position = Vector3.Lerp(_child.position, _blockPos, _speed * Time.deltaTime);
        }

        /// <summary>
        /// Moves the skillshot door back to its starting position
        /// </summary>
        private void ToStartPosition()
        {
            _child.position = Vector3.Lerp(_child.position, _startPos, _speed * Time.deltaTime);
        }

        /// <summary>
        /// Initiates closing the skillshot door
        /// </summary>
        public void CloseSkillShot()
        {
            _moveWall = true;
        }


        /// <summary>
        /// Draws the position of collider to see it clearer.
        /// </summary>
        private void OnDrawGizmos()
        {
            
            if(_drawBlockPos)
            {
                //transform.rotation = Quaternion.Euler(0f, 12f, 0f);
                Gizmos.color = Color.black;
                Gizmos.matrix = Matrix4x4.TRS(_child.position, _child.rotation, transform.lossyScale);
                Gizmos.DrawCube(_blockPos - _child.position, _child.localScale);

                //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }

        }

    }
}
