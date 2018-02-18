﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class LanguageStateBase
    {
        public enum Language
        {
            English = 0,
            Finnish = 1,
            Russian = 2,
            German = 3
        }

        public virtual Language SelectedLanguage
        {
            get
            {
                return Language.English;
            }
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
