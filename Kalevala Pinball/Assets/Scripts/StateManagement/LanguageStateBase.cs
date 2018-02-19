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

        public string GetText(string textTitle)
        {
            if (textTitle.Equals("play"))
            {
                return play;
            }
            else if (textTitle.Equals("mainMenu"))
            {
                return mainMenu;
            }
            else if (textTitle.Equals("settingsMenu"))
            {
                return settingsMenu;
            }
            else if (textTitle.Equals("launchHelp"))
            {
                return launchHelp;
            }
            else if (textTitle.Equals("flippersHelp"))
            {
                return flippersHelp;
            }
            else if (textTitle.Equals("nudgeHelp"))
            {
                return nudgeHelp;
            }

            // If the text title does not correspond
            // with any text, error text is returned
            else
            {
                return error;
            }
        }

        protected virtual string error { get { return "--Error--"; } }

        protected virtual string play { get { return "Play"; } }
        protected virtual string mainMenu { get { return "Main menu"; } }
        protected virtual string settingsMenu { get { return "Settings menu"; } }

        protected virtual string launchHelp
        { get { return "Press and hold the Space bar and then release it to lauch a ball."; } }
        protected virtual string flippersHelp
        { get { return "Press A to use the flipper bats on the left and L to use the ones on the right."; } }
        protected virtual string nudgeHelp
        { get { return "Press Q or O to nudge the ball left or right, respectively."; } }
    }
}
