using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class TextInput : MonoBehaviour
    {
        public const char RESERVED_ILLEGAL_CHAR = '§';

        [SerializeField]
        private int _maxTextLength = 20;

        [SerializeField]
        private bool _allowKeyboardInput;

        [SerializeField]
        private float _buttonHoldInterval = 0.2f;

        [SerializeField] // Serialized for debugging
        private bool _upperCase;

        public List<char> _text; // Public for debugging

        private bool _justActivated;

        private bool _buttonPressed;
        private bool _buttonHeldDown;
        private KeyCode _lastPressedKey;
        private float _buttonHoldStartTime = 0f;

        public bool TextChanged { get; set; }

        private void Start()
        {
            _text = new List<char>();
            _lastPressedKey = KeyCode.None;
        }

        public void Activate()
        {
            _justActivated = true;
        }

        public void Deactivate()
        {
            _buttonPressed = false;
            _buttonHeldDown = false;
            _lastPressedKey = KeyCode.None;
            _buttonHoldStartTime = 0f;
        }

        public void CheckKeyboardInput()
        {
            // Prevents deactivating when releasing the
            // submit button which activated the text input.
            // As long as the submit button is held from the
            // start, the CanBeClosed method will return false.
            if (_justActivated)
            {
                _justActivated = Input.GetKey(KeyCode.Return) ||
                                 Input.GetKeyDown("joystick button 0");
            }

            else if (_allowKeyboardInput)
            {
                _buttonPressed = false;

                _upperCase = Input.GetKey(KeyCode.LeftShift);

                if (GetKey(KeyCode.Backspace)) { RemoveLastChar(); }
                else if (GetKey(KeyCode.A)) { AddChar(UpperOrLowerChar('a')); }
                else if (GetKey(KeyCode.B)) { AddChar(UpperOrLowerChar('b')); }
                else if (GetKey(KeyCode.C)) { AddChar(UpperOrLowerChar('c')); }
                else if (GetKey(KeyCode.D)) { AddChar(UpperOrLowerChar('d')); }
                else if (GetKey(KeyCode.E)) { AddChar(UpperOrLowerChar('e')); }
                else if (GetKey(KeyCode.F)) { AddChar(UpperOrLowerChar('f')); }
                else if (GetKey(KeyCode.G)) { AddChar(UpperOrLowerChar('g')); }
                else if (GetKey(KeyCode.H)) { AddChar(UpperOrLowerChar('h')); }
                else if (GetKey(KeyCode.I)) { AddChar(UpperOrLowerChar('i')); }
                else if (GetKey(KeyCode.J)) { AddChar(UpperOrLowerChar('j')); }
                else if (GetKey(KeyCode.K)) { AddChar(UpperOrLowerChar('k')); }
                else if (GetKey(KeyCode.L)) { AddChar(UpperOrLowerChar('l')); }
                else if (GetKey(KeyCode.M)) { AddChar(UpperOrLowerChar('m')); }
                else if (GetKey(KeyCode.N)) { AddChar(UpperOrLowerChar('n')); }
                else if (GetKey(KeyCode.O)) { AddChar(UpperOrLowerChar('o')); }
                else if (GetKey(KeyCode.P)) { AddChar(UpperOrLowerChar('p')); }
                else if (GetKey(KeyCode.Q)) { AddChar(UpperOrLowerChar('q')); }
                else if (GetKey(KeyCode.R)) { AddChar(UpperOrLowerChar('r')); }
                else if (GetKey(KeyCode.S)) { AddChar(UpperOrLowerChar('s')); }
                else if (GetKey(KeyCode.T)) { AddChar(UpperOrLowerChar('t')); }
                else if (GetKey(KeyCode.U)) { AddChar(UpperOrLowerChar('u')); }
                else if (GetKey(KeyCode.V)) { AddChar(UpperOrLowerChar('v')); }
                else if (GetKey(KeyCode.W)) { AddChar(UpperOrLowerChar('w')); }
                else if (GetKey(KeyCode.X)) { AddChar(UpperOrLowerChar('x')); }
                else if (GetKey(KeyCode.Y)) { AddChar(UpperOrLowerChar('y')); }
                else if (GetKey(KeyCode.Z)) { AddChar(UpperOrLowerChar('z')); }
                else if (GetKey(KeyCode.Alpha0)) { AddChar('0'); }
                else if (GetKey(KeyCode.Alpha1)) { AddChar('1'); }
                else if (GetKey(KeyCode.Alpha2)) { AddChar('2'); }
                else if (GetKey(KeyCode.Alpha3)) { AddChar('3'); }
                else if (GetKey(KeyCode.Alpha4)) { AddChar('4'); }
                else if (GetKey(KeyCode.Alpha5)) { AddChar('5'); }
                else if (GetKey(KeyCode.Alpha6)) { AddChar('6'); }
                else if (GetKey(KeyCode.Alpha7)) { AddChar('7'); }
                else if (GetKey(KeyCode.Alpha8)) { AddChar('8'); }
                else if (GetKey(KeyCode.Alpha9)) { AddChar('9'); }
                else if (GetKey(KeyCode.Space)) { AddChar(' '); }
                else if (GetKey(KeyCode.Minus)) { AddChar('-'); }

                UpdateButtonHoldInterval();
            }
        }

        private bool GetKey(KeyCode key)
        {
            bool keyPressed = Input.GetKey(key);
            if (keyPressed)
            {
                _buttonPressed = true;
            }

            return keyPressed && KeyIsCurrentlyValid(key);
        }

        private bool KeyIsCurrentlyValid(KeyCode key)
        {
            bool result = false;

            if (_buttonHeldDown)
            {
                // Pressing different key is accepted
                if (key != _lastPressedKey)
                {
                    result = true;
                    _buttonHoldStartTime = Time.time;
                }
            }
            else
            {
                result = true;
                _buttonHeldDown = true;
                _buttonHoldStartTime = Time.time;
            }

            _lastPressedKey = key;

            return result;
        }

        private void UpdateButtonHoldInterval()
        {
            if (_buttonHeldDown)
            {
                // No button is pressed
                if (!_buttonPressed)
                {
                    _buttonHeldDown = false;
                }

                // Checks if enough time has passed while holding a button
                else if (Time.time - _buttonHoldStartTime > _buttonHoldInterval)
                {
                    _buttonHeldDown = false;
                }
            }
        }

        private char UpperOrLowerChar(char c)
        {
            if (_upperCase)
            {
                return char.ToUpper(c);
            }
            else
            {
                return char.ToLower(c);
            }
        }

        public void AddChar(char c)
        {
            if (_text.Count < _maxTextLength)
            {
                _text.Add(c);
                TextChanged = true;
            }
        }

        public char RemoveLastChar()
        {
            char removedChar = RESERVED_ILLEGAL_CHAR;

            if (_text.Count > 0)
            {
                int index = _text.Count - 1;
                removedChar = RemoveChar(index);
            }

            return removedChar;
        }

        public char RemoveChar(int index)
        {
            char removedChar = RESERVED_ILLEGAL_CHAR;

            if (_text.Count > 0)
            {
                if (index < 0)
                {
                    index = 0;
                }
                else if (index >= _text.Count)
                {
                    index = _text.Count - 1;
                }

                removedChar = _text[index];
                _text.RemoveAt(index);

                TextChanged = true;
            }

            return removedChar;
        }

        public void AddChars(string chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                _text.Add(chars[i]);
            }

            TextChanged = true;
        }

        public void ClearText()
        {
            _text.Clear();
            TextChanged = true;
        }

        public string GetText()
        {
            string textString = "";

            foreach (char c in _text)
            {
                textString += c;
            }

            TextChanged = false;

            return textString;
        }

        /// <summary>
        /// Checks whether the text input can be closed.
        /// If the submit button has been held since
        /// activating the text input, it cannot be
        /// closed.
        /// </summary>
        /// <returns>Can the text input be closed</returns>
        public bool CanBeClosed()
        {
            return ( !_justActivated );
        }
    }
}
