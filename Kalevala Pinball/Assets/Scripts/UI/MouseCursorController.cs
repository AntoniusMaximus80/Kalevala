using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Kalevala
{
    public class MouseCursorController : MonoBehaviour
    {
        public static bool cursorActive;

        /// <summary>
        /// The cursor's main point (in pixels)
        /// </summary>
        private Vector2 hotSpot;

        /// <summary>
        /// The cursor mode (Auto or ForceSoftware)
        /// </summary>
        private CursorMode cursorMode = CursorMode.Auto;

        /// <summary>
        /// Is playing with the mouse cursor not hidden
        /// </summary>
        private bool playingUsingMouse;

        /// <summary>
        /// Was the mouse moved
        /// </summary>
        private bool mouseMoved;

        /// <summary>
        /// Where was the cursor in the last frame (system coordinates)
        /// </summary>
        //private Vector3 oldMousePosition;

        /// <summary>
        /// Where was the cursor in the last frame
        /// </summary>
        private Vector3 oldScreenPosition;

        public Action SelectMenuButtonAction { get; set; }

        /// <summary>
        /// Initializes the game object.
        /// </summary>
        private void Start()
        {
            PlayingUsingMouse = true;
            //InitSystemCursor();
        }

        /// <summary>
        /// Updates the mouse cursor.
        /// </summary>
        private void Update()
        {
            UpdatePosition();
            CheckIfPlayingUsingMouse();
        }

        /// <summary>
        /// Initializes the cursor.
        /// The system cursor is used but with a custom texture.
        /// </summary>
        //private void InitSystemCursor()
        //{
        //    // Sets the cursor's main point
        //    hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);

        //    // Gives the game object's sprite to the system cursor
        //    Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        //}

        /// <summary>
        /// Shows or hides the operating system's cursor.
        /// </summary>
        /// <param name="show">will the cursor be shown</param>
        private void ShowSystemCursor(bool show)
        {
            Cursor.visible = show;
        }

        /// <summary>
        /// Resets the system cursor to default.
        /// </summary>
        private void ResetSystemCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }

        /// <summary>
        /// Gets the mouse cursor's world position.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        /// <summary>
        /// Gets the mouse cursor's screen position.
        /// </summary>
        public Vector3 ScreenPosition { get; private set; }

        /// <summary>
        /// Gets or sets whether the game is played with the mouse.
        /// </summary>
        public bool PlayingUsingMouse
        {
            get
            {
                return playingUsingMouse;
            }
            set
            {
                if (playingUsingMouse != value)
                {
                    playingUsingMouse = value;
                    cursorActive = value;

                    ShowSystemCursor(value);

                    if (value)
                    {
                        //Debug.Log("cursor shown");

                        EventSystem.current.SetSelectedGameObject(null);
                    }
                    else
                    {
                        //Debug.Log("cursor hidden");

                        ClearCursorHighlight();

                        // Selects the current menu's default selected button
                        SelectDefaultSelectedMenuButton();

                        //EventSystem.current.SetSelectedGameObject
                        //    (EventSystem.current.firstSelectedGameObject);

                        // Prevents the cursor being immediately shown again
                        oldScreenPosition = ScreenPosition;
                    }
                }
            }
        }

        private void SelectDefaultSelectedMenuButton()
        {
            if (SelectMenuButtonAction != null)
            {
                SelectMenuButtonAction.Invoke();
            }
        }

        private void UpdatePosition()
        {
            // Sets the cursor's old screen position for checking if it changed
            oldScreenPosition = ScreenPosition;

            // Gets the system mouse cursor's position in screen coordinates
            ScreenPosition = Input.mousePosition;
        }

        /// <summary>
        /// Updates the mouse cursor's position.
        /// </summary>
        //private void UpdatePosition()
        //{
        //    // Sets the cursor's old screen
        //    // position for checking if it changed
        //    oldScreenPosition = ScreenPosition;

        //    // Gets the system mouse cursor's position in screen coordinates
        //    ScreenPosition = Input.mousePosition;

        //    // Sets whether the cursor was moved
        //    //mouseMoved = (ScreenPosition != oldScreenPosition);

        //    // Translates the screen coordinates to world coordinates
        //    Vector3 worldPosition =
        //        Camera.main.ScreenToWorldPoint(ScreenPosition);

        //    // Sets the game mouse cursor's position
        //    transform.position = new Vector3(worldPosition.x,
        //                                     worldPosition.y,
        //                                     transform.position.z);
        //}

        public void TogglePlayingUsingMouse()
        {
            PlayingUsingMouse = !PlayingUsingMouse;
        }

        private void CheckIfPlayingUsingMouse()
        {
            if ( !PlayingUsingMouse )
            {
                // Moving the mouse or using its buttons shows the mouse cursor
                if (ScreenPosition != oldScreenPosition ||
                    Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    PlayingUsingMouse = true;
                }
            }

            // InputManager checks if the cursor should be hidden
        }

        public void ClearCursorHighlight()
        {
            PointerEventData pointer =
                new PointerEventData(EventSystem.current);
            pointer.position = ScreenPosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            foreach (RaycastResult raycastResult in raycastResults)
            {
                //Debug.Log(raycastResult.gameObject.name);
                GameObject hoveredObj = raycastResult.gameObject;

                bool success = ClearHighlight(hoveredObj, pointer);

                // Sometimes, the child image may be interactable;
                // in this case, a highlight from parent object is cleared
                if (!success)
                {
                    ClearHighlight(
                        hoveredObj.transform.parent.gameObject, pointer);
                }
            }
        }

        private bool ClearHighlight(GameObject uiElement,
                                    PointerEventData pointer)
        {
            // Highlighted selectable UI element
            Selectable selectable = uiElement.GetComponent<Selectable>();

            if (selectable != null)
            {
                selectable.OnPointerExit(pointer);
                //Debug.Log("Highlight cleared");

                return true;
            }

            //// Highlighted button
            //Button button = uiElement.GetComponent<Button>();

            //// Highlighted toggle
            //Toggle toggle = uiElement.GetComponent<Toggle>();

            //// Highlighted slider
            //Slider slider = uiElement.GetComponent<Slider>();

            //if (button != null)
            //{
            //    button.OnPointerExit(pointer);
            //    //Debug.Log("Button highlight cleared");

            //    return true;
            //}
            //if (toggle != null)
            //{
            //    toggle.OnPointerExit(pointer);
            //    //Debug.Log("Toggle highlight cleared");

            //    return true;
            //}
            //if (slider != null)
            //{
            //    slider.OnPointerExit(pointer);
            //    //Debug.Log("Slider highlight cleared");

            //    return true;
            //}

            return false;
        }
    }
}
