using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public enum LanguageStateType
    {
        English = 0,
        Finnish = 1,
        Russian = 2,
        German = 3
    }

    public class LanguageStateBase
    {
        public LanguageStateType State { get; protected set; }

        public List<LanguageStateType> TargetStates { get; protected set; }

        /// <summary>
        /// The owner GameManager of this state
        /// (GameManager is the state controller class)
        /// </summary>
        public GameManager Owner { get; protected set; }

        public LanguageStateBase()
        {
            Init(LanguageStateType.English);
        }

        public LanguageStateBase(LanguageStateType state)
        {
            Init(state);
        }

        private void Init(LanguageStateType state)
        {
            TargetStates = new List<LanguageStateType>();
            State = state;
            Owner = GameManager.Instance;
        }

        public string GetTranslation(string textID)
        {
            if (textID.Equals("play"))
            {
                return play;
            }
            else if (textID.Equals("mainMenu"))
            {
                return mainMenu;
            }
            else if (textID.Equals("pauseMenu"))
            {
                return pauseMenu;
            }
            else if (textID.Equals("settingsMenu"))
            {
                return settingsMenu;
            }
            else if (textID.Equals("gameOver"))
            {
                return gameOver;
            }
            else if (textID.Equals("setName"))
            {
                return setName;
            }
            else if (textID.Equals("highscores"))
            {
                return highscores;
            }
            else if (textID.Equals("player"))
            {
                return player;
            }
            else if (textID.Equals("resume"))
            {
                return resume;
            }
            else if (textID.Equals("restart"))
            {
                return restart;
            }
            else if (textID.Equals("quitGame"))
            {
                return quitGame;
            }
            else if (textID.Equals("next"))
            {
                return next;
            }
            else if (textID.Equals("previous"))
            {
                return back;
            }
            else if (textID.Equals("back"))
            {
                return back;
            }
            else if (textID.Equals("accept"))
            {
                return accept;
            }
            else if (textID.Equals("ok"))
            {
                return ok;
            }
            else if (textID.Equals("cancel"))
            {
                return cancel;
            }
            else if (textID.Equals("yes"))
            {
                return yes;
            }
            else if (textID.Equals("no"))
            {
                return no;
            }
            else if (textID.Equals("apply"))
            {
                return apply;
            }
            else if (textID.Equals("revert"))
            {
                return revert;
            }
            else if (textID.Equals("confirmExitGame"))
            {
                return confirmExitGame;
            }
            else if (textID.Equals("confirmStartGame"))
            {
                return confirmStartGame;
            }
            else if (textID.Equals("confirmReturnToMainMenu"))
            {
                return confirmReturnToMainMenu;
            }
            else if (textID.Equals("confirmSaveSettings"))
            {
                return confirmSaveSettings;
            }
            else if (textID.Equals("confirmEraseHighscores"))
            {
                return confirmEraseHighscores;
            }
            else if (textID.Equals("launchHelp"))
            {
                return launchHelp;
            }
            else if (textID.Equals("flippersHelp"))
            {
                return flippersHelp;
            }
            else if (textID.Equals("nudgeHelp"))
            {
                return nudgeHelp;
            }

            // If the text ID does not correspond
            // with any text, error text is returned
            else
            {
                return error;
            }
        }

        // Error
        protected virtual string error { get { return "--Error--"; } }

        // Menus
        protected virtual string play { get { return "Play"; } }
        protected virtual string mainMenu { get { return "Main Menu"; } }
        protected virtual string pauseMenu { get { return "Paused"; } }
        protected virtual string settingsMenu { get { return "Settings"; } }
        protected virtual string gameOver { get { return "Game Over"; } }
        protected virtual string resume { get { return "Resume"; } }
        protected virtual string restart { get { return "Play Again"; } }
        protected virtual string quitGame { get { return "Quit Game"; } }
        protected virtual string setName { get { return "Set player name"; } }
        protected virtual string highscores { get { return "Highscores"; } }
        protected virtual string player { get { return "Player"; } }
        protected virtual string next { get { return "Next"; } }
        protected virtual string previous { get { return "Previous"; } }
        protected virtual string back { get { return "Back"; } }
        protected virtual string accept { get { return "Accept"; } }
        protected virtual string ok { get { return "OK"; } }
        protected virtual string cancel { get { return "Cancel"; } }
        protected virtual string yes { get { return "Yes"; } }
        protected virtual string no { get { return "No"; } }
        protected virtual string apply { get { return "Apply"; } }
        protected virtual string revert { get { return "Revert"; } }

        // Confirmation
        protected virtual string confirmExitGame
        { get { return "Exit the game?"; } }
        protected virtual string confirmStartGame
        { get { return "Start playing?"; } }
        protected virtual string confirmReturnToMainMenu
        { get { return "Return to the main menu? Your score won't be saved."; } }
        protected virtual string confirmSaveSettings
        { get { return "Apply settings?"; } }
        protected virtual string confirmEraseHighscores
        { get { return "Are you sure you want to erase saved highscores?"; } }

        // Help
        protected virtual string launchHelp
        { get { return "Press and hold the Space bar and then release it to lauch a ball."; } }
        protected virtual string flippersHelp
        { get { return "Press Left Control to use the flipper bat on the left and Numpad Enter to use the ones on the right."; } }
        protected virtual string nudgeHelp
        { get { return "Press Q or O to nudge the ball left or right, respectively."; } }
    }
}
