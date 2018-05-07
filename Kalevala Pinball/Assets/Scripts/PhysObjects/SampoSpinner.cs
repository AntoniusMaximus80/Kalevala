using System;
using UnityEngine;

namespace Kalevala
{
    public class SampoSpinner : Spinner
    {
        public event Action HalfTurn;

        /// <summary>
        /// Gives score each half round the spinner does.
        /// Also fires an event so any listening class can react to it.
        /// </summary>
        /// <returns>Is score given</returns>
        protected override bool CheckScoring()
        {
            bool givesScore = base.CheckScoring();

            if (givesScore && HalfTurn != null)
            {
                HalfTurn();
            }

            return givesScore;
        }
    }
}
