using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class LanguageState_Finnish : LanguageStateBase
    {
        public LanguageState_Finnish() : base(LanguageStateType.Finnish)
        {
        }

        protected override string play { get { return "Pelaa"; } }
        protected override string mainMenu { get { return "Päävalikko"; } }
        protected override string pauseMenu { get { return "Tauko"; } }
        protected override string settingsMenu { get { return "Asetukset"; } }
        protected override string resume { get { return "Jatka peliä"; } }
        protected override string quitGame { get { return "Poistu pelistä"; } }
        protected override string back { get { return "Takaisin"; } }

        protected override string launchHelp
        { get { return "Pidä välilyöntinäppäintä pohjassa ja päästä irti laukaistaksesi pallon."; } }
        protected override string flippersHelp
        { get { return "Paina A-näppäintä käyttääksesi vasemmalla ja L-näppäintä käyttääksesi oikealla olevia mailoja."; } }
        protected override string nudgeHelp
        { get { return "Paina Q- tai O-näppäintä tönäistäksesi palloa vasemmalle tai oikealle."; } }
    }
}
