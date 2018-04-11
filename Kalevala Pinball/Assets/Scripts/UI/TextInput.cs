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
        private bool _allowKeyboardInput;

        [SerializeField] // Serialized for debugging
        private bool _upperCase;

        public List<char> _text; // Public for debugging; a property would be preferred

        public bool _textChanged; // Public for debugging; a property would be preferred

        private void Start()
        {
            _text = new List<char>();
        }

        public void CheckKeyboardInput()
        {
            // TODO: Yikes

            if (_allowKeyboardInput)
            {
                _upperCase = Input.GetKey(KeyCode.LeftShift);

                if (Input.GetKeyDown(KeyCode.Backspace)) { RemoveLastChar(); }
                else if (Input.GetKeyDown(KeyCode.A)) { AddChar(UpperOrLowerChar('a')); }
                else if (Input.GetKeyDown(KeyCode.B)) { AddChar(UpperOrLowerChar('b')); }
                else if (Input.GetKeyDown(KeyCode.C)) { AddChar(UpperOrLowerChar('c')); }
                else if (Input.GetKeyDown(KeyCode.D)) { AddChar(UpperOrLowerChar('d')); }
                else if (Input.GetKeyDown(KeyCode.E)) { AddChar(UpperOrLowerChar('e')); }
                else if (Input.GetKeyDown(KeyCode.F)) { AddChar(UpperOrLowerChar('f')); }
                else if (Input.GetKeyDown(KeyCode.G)) { AddChar(UpperOrLowerChar('g')); }
                else if (Input.GetKeyDown(KeyCode.H)) { AddChar(UpperOrLowerChar('h')); }
                else if (Input.GetKeyDown(KeyCode.I)) { AddChar(UpperOrLowerChar('i')); }
                else if (Input.GetKeyDown(KeyCode.J)) { AddChar(UpperOrLowerChar('j')); }
                else if (Input.GetKeyDown(KeyCode.K)) { AddChar(UpperOrLowerChar('k')); }
                else if (Input.GetKeyDown(KeyCode.L)) { AddChar(UpperOrLowerChar('l')); }
                else if (Input.GetKeyDown(KeyCode.M)) { AddChar(UpperOrLowerChar('m')); }
                else if (Input.GetKeyDown(KeyCode.N)) { AddChar(UpperOrLowerChar('n')); }
                else if (Input.GetKeyDown(KeyCode.O)) { AddChar(UpperOrLowerChar('o')); }
                else if (Input.GetKeyDown(KeyCode.P)) { AddChar(UpperOrLowerChar('p')); }
                else if (Input.GetKeyDown(KeyCode.Q)) { AddChar(UpperOrLowerChar('q')); }
                else if (Input.GetKeyDown(KeyCode.R)) { AddChar(UpperOrLowerChar('r')); }
                else if (Input.GetKeyDown(KeyCode.S)) { AddChar(UpperOrLowerChar('s')); }
                else if (Input.GetKeyDown(KeyCode.T)) { AddChar(UpperOrLowerChar('t')); }
                else if (Input.GetKeyDown(KeyCode.U)) { AddChar(UpperOrLowerChar('u')); }
                else if (Input.GetKeyDown(KeyCode.V)) { AddChar(UpperOrLowerChar('v')); }
                else if (Input.GetKeyDown(KeyCode.W)) { AddChar(UpperOrLowerChar('w')); }
                else if (Input.GetKeyDown(KeyCode.X)) { AddChar(UpperOrLowerChar('x')); }
                else if (Input.GetKeyDown(KeyCode.Y)) { AddChar(UpperOrLowerChar('y')); }
                else if (Input.GetKeyDown(KeyCode.Z)) { AddChar(UpperOrLowerChar('z')); }
                else if (Input.GetKeyDown(KeyCode.Alpha0)) { AddChar('0'); }
                else if (Input.GetKeyDown(KeyCode.Alpha1)) { AddChar('1'); }
                else if (Input.GetKeyDown(KeyCode.Alpha2)) { AddChar('2'); }
                else if (Input.GetKeyDown(KeyCode.Alpha3)) { AddChar('3'); }
                else if (Input.GetKeyDown(KeyCode.Alpha4)) { AddChar('4'); }
                else if (Input.GetKeyDown(KeyCode.Alpha5)) { AddChar('5'); }
                else if (Input.GetKeyDown(KeyCode.Alpha6)) { AddChar('6'); }
                else if (Input.GetKeyDown(KeyCode.Alpha7)) { AddChar('7'); }
                else if (Input.GetKeyDown(KeyCode.Alpha8)) { AddChar('8'); }
                else if (Input.GetKeyDown(KeyCode.Alpha9)) { AddChar('9'); }
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
            _text.Add(c);
            _textChanged = true;
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

                _textChanged = true;
            }

            return removedChar;
        }

        public void AddChars(string chars)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                _text.Add(chars[i]);
            }

            _textChanged = true;
        }

        public void ClearText()
        {
            _text.Clear();
            _textChanged = true;
        }

        public string GetText()
        {
            string textString = "";

            foreach (char c in _text)
            {
                textString += c;
            }

            _textChanged = false;

            return textString;
        }
    }
}
