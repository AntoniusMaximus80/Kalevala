using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public enum NotePitch
    {
        A = 0,
        D = 1,
        E = 2,
        F = 3,
        G = 4
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
