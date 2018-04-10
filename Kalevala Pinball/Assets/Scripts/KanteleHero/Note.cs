using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class Note
    {
        private float _spawnTime;
        private float _pitch;

        public Note(float spawnTime, float pitch)
        {
            SpawnTime = spawnTime;
            Pitch = pitch;
        }

        public float SpawnTime
        {
            get { return _spawnTime; }
            private set { _spawnTime = value; }
        }
        public float Pitch
        {
            get { return _pitch; }
            private set { _pitch = value; }
        }

    }
}
