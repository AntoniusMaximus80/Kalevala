using UnityEngine;
using Kalevala.Persistence;

namespace Kalevala
{
    public class StatusPanelManager : MonoBehaviour
    {
        public enum PanelType
        {
            All = 0,
            ShootAgain = 1,
            Autosave = 2,
            GameMode_Sampo = 3
        }

        [SerializeField]
        private int _workshopKOHEnteredRequirement = 3;

        [SerializeField]
        private IlmarinenKickoutHole _workshopKOH;

        [SerializeField]
        private LiveAgainLight _shootAgainLight;

        private int _workshopKOHEnteredNum;

        private void Start()
        {
            if (_workshopKOH != null)
            {
                // Listens to an event which is fired when a ball enters Ilmarinen's workshop
                _workshopKOH.BallEntered += WorkshopKOHEntered;
            }
            else
            {
                Debug.LogError("Ilmarinen's workshop KOH is not set.");
            }

            if (_shootAgainLight == null)
            {
                Debug.LogError("Shoot Again light is not set.");
            }
        }

        private GameModeStateType GameMode
        {
            get
            {
                return GameManager.Instance.GameMode.State;
            }
        }

        public void ResetStatus()
        {
            _workshopKOHEnteredNum = 0;

            SwitchLight(PanelType.All, false);
        }

        // All logic that makes the game mode change
        #region Game Mode Logic

        public void WorkshopKOHEntered()
        {
            if (GameMode != GameModeStateType.Sampo)
            {
                _workshopKOHEnteredNum++;

                // Starts the Sampo mode
                if (_workshopKOHEnteredNum >= _workshopKOHEnteredRequirement)
                {
                    StartGameMode(GameModeStateType.Sampo);
                   
                }
                else
                {
                    Viewscreen.WorkShopEntered(_workshopKOHEnteredRequirement - _workshopKOHEnteredNum);
                }
            }
        }

        public void EndSampoMode()
        {
            StartGameMode(GameModeStateType.Normal);

            //TODO add endSampoMode to viewScreen
            //Viewscreen.EndSampoMode();
        }    

        private void StartGameMode(GameModeStateType gameMode)
        {
            GameManager.Instance.StateManager.
                PerformTransition(gameMode);

            Debug.Log("Starting game mode: " + gameMode.ToString());
        }

        #endregion Game Mode Logic

        // Controlling the status panel lights
        #region Lights

        public void SwitchLight(PanelType panel, bool onoff)
        {
            switch (panel)
            {
                case PanelType.ShootAgain:
                {
                    _shootAgainLight.Switch(onoff);
                    break;
                }
                case PanelType.Autosave:
                {
                    //_light.Switch(onoff);
                    break;
                }
                case PanelType.GameMode_Sampo:
                {
                    //_light.Switch(onoff);
                    break;
                }

                // Turns all panel lights on or off
                case PanelType.All:
                {
                    _shootAgainLight.Switch(onoff);
                    //_light1.Switch(onoff);
                    //_light2.Switch(onoff);
                    //_light3.Switch(onoff);
                    break;
                }
            }
        }

        #endregion Lights
    }
}
