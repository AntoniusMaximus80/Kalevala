using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public enum NotePitch
    {
        D = 0,
        E = 1,
        F = 2,
        G = 3
    }

    public class Note
    {

        public Note(float spawnTime, NotePitch notePitch)
        {
            SpawnTime = spawnTime;
            NotePitch = notePitch;
        }

        public float SpawnTime
        {
            get;
            private set;
        }
        public NotePitch NotePitch
        {
            get;
            private set;
        }

    }
}
