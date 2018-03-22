using System;
using System.Collections.Generic;

namespace Kalevala.Persistence
{
    [Serializable]
    public class GameData
    {
        public List<HighscoreData> HighscoreDataList =
            new List<HighscoreData>();
    }
}
