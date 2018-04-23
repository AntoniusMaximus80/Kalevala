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

        // Menus
        protected override string play { get { return "Pelaa"; } }
        protected override string mainMenu { get { return "Päävalikko"; } }
        protected override string pauseMenu { get { return "Tauko"; } }
        protected override string settingsMenu { get { return "Asetukset"; } }
        protected override string resume { get { return "Jatka peliä"; } }
        protected override string quitGame { get { return "Poistu pelistä"; } }
        protected override string setName { get { return "Syötä pelaajan nimi"; } }
        protected override string highscores { get { return "Huippupisteet"; } }
        protected override string player { get { return "Pelaaja"; } }
        protected override string next { get { return "Seuraava"; } }
        protected override string previous { get { return "Edellinen"; } }
        protected override string back { get { return "Takaisin"; } }
        protected override string accept { get { return "Hyväksy"; } }
        protected override string cancel { get { return "Peruuta"; } }
        protected override string yes { get { return "Kyllä"; } }
        protected override string no { get { return "Ei"; } }
        protected override string apply { get { return "Tallenna"; } }
        protected override string revert { get { return "Hylkää"; } }

        // Confirmation
        protected override string confirmExitGame
        { get { return "Lopeta peli?"; } }
        protected override string confirmStartGame
        { get { return "Aloita peli?"; } }
        protected override string confirmReturnToMainMenu
        { get { return "Palaa päävalikkoon? Pisteitäsi ei tallenneta."; } }
        protected override string confirmSaveSettings
        { get { return "Tallenna asetukset?"; } }
        protected override string confirmEraseHighscores
        { get { return "Oletko varma että haluat poistaa tallennetut huippupisteet?"; } }

        // Help
        protected override string launchHelp
        { get { return "Pidä välilyöntinäppäintä pohjassa ja päästä irti laukaistaksesi pallon."; } }
        protected override string flippersHelp
        { get { return "Paina A-näppäintä käyttääksesi vasemmalla ja L-näppäintä käyttääksesi oikealla olevia mailoja."; } }
        protected override string nudgeHelp
        { get { return "Paina Q- tai O-näppäintä tönäistäksesi palloa vasemmalle tai oikealle."; } }
    }
}
