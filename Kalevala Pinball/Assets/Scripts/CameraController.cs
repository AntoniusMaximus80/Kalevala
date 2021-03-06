﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalevala
{
    public class CameraController : MonoBehaviour
    {
        public enum CameraType
        {
            Default = 0,
            Horizontal = 1
        }

        public enum CameraPosition
        {
            Playfield = 0,
            Launcher = 1,
            Kantele = 2
        }

        [SerializeField]
        private Camera _defaultCamera;

        [SerializeField]
        private Camera _horizontalCamera;

        // These two used for showing the scoreboard when in Kantele mode.
        // Not sure if we need "event camera" otherwise.
        [SerializeField]
        private Camera _scoreBoardCamera;
        [SerializeField]
        private GameObject _scoreBoardView;

        [SerializeField]
        private Transform _defaultCamPlayfieldTransform;

        [SerializeField]
        private Transform _defaultCamKanteleTransform;

        [SerializeField]
        private Transform _horizontalCamPlayfieldTransform;

        [SerializeField]
        private Transform _horizontalCamKanteleTransform;

        [SerializeField]
        private Transform _eventCamLaunchTransform;

        [SerializeField, Range(0f, 5f)]
        private float _movementTime = 1f;

        private Coroutine _currentCameraPan;

        private float _elapsedTime;
        private bool _moving;
        private bool _shaking;

        /// <summary>
        /// The currently used camera.
        /// </summary>
        public Camera CurrentCamera { get; private set; }

        /// <summary>
        /// The camera's position.
        /// </summary>
        public CameraPosition CamPosition { get; private set; }

        private void Start()
        {
            if (_defaultCamera != null)
            {
                _defaultCamPlayfieldTransform.position =
                    _defaultCamera.transform.position;
                _defaultCamPlayfieldTransform.rotation =
                    _defaultCamera.transform.rotation;
            }
            else 
            {
                Debug.LogError("Default camera is not set.");
            }

            CurrentCamera = _defaultCamera;
        }

        public void SetCurrentCamera(CameraType camType)
        {
            CurrentCamera.gameObject.SetActive(false);

            bool useDefault = true;

            switch (camType)
            {
                case CameraType.Horizontal:
                {
                    if (_horizontalCamera != null)
                    {
                        CurrentCamera = _horizontalCamera;
                        useDefault = false;
                    }
                    break;
                }
            }

            if (useDefault)
            {
                CurrentCamera = _defaultCamera;
            }

            CurrentCamera.gameObject.SetActive(true);
            MoveCurrentCamTo(CamPosition, true);

            Debug.Log("Camera mode selected: " + CurrentCamera.name);
        }

        /// <summary>
        /// Starts moving the camera to the given position.
        /// </summary>
        /// <param name="camPosition">
        /// A point of interest on which 
        /// the camera should focus</param>
        /// <param name="instantMove">
        /// Does the movement happen instantly</param>
        public void MoveCurrentCamTo(CameraPosition camPosition,
                                     bool instantMove)
        {
            // Returns because the camera is already at the target position
            if (camPosition == CamPosition)
            {
                return;
            }

            CamPosition = camPosition;

            // Show or hide scoreboard extra view based on need.
            ShowScoreBoard(camPosition);
            
            if (_currentCameraPan != null)
            {
                StopCoroutine(_currentCameraPan);
                EndCameraPan();
            }

            switch (camPosition)
            {
                case CameraPosition.Playfield:
                {
                    _currentCameraPan = StartCoroutine(
                        MoveCamera(GetPlayfieldCamTransform(), instantMove));
                    break;
                }
                case CameraPosition.Launcher:
                {
                    _currentCameraPan = StartCoroutine(
                        MoveCamera(GetLauncherCamTransform(), instantMove));
                    break;
                }
                case CameraPosition.Kantele:
                {
                    _currentCameraPan = StartCoroutine(
                        MoveCamera(GetKanteleCamTransform(), instantMove));
                    break;
                }
            }
        }

        /// <summary>
        /// Show or hide scoreboard extra view based on need.
        /// Currently only needed when camera is showing the Kantele.
        /// </summary>
        /// <param name="camPosition">The current (or coming) camera position</param>
        private void ShowScoreBoard(CameraPosition camPosition)
        {
            _scoreBoardCamera.enabled = (camPosition == CameraPosition.Kantele);
            _scoreBoardView.SetActive(camPosition == CameraPosition.Kantele);
        }



        /// <summary>
        /// A coroutine.
        /// Moves and rotates the camera to match the target
        /// transform's position and rotation.
        /// </summary>
        /// <param name="target">A target transform</param>
        /// <param name="instantMove">
        /// Does the movement happen instantly</param>
        /// <returns></returns>
        private IEnumerator MoveCamera(Transform target,
                                       bool instantMove)
        {
            // Breaks if the camera is already moving or the target is null
            if (_moving || target == null)
            {
                yield break;
            }

            // Starts moving
            _moving = true;
            _elapsedTime = 0;
            float ratio = 0;

            Vector3 startPos = CurrentCamera.transform.position;
            Quaternion startRot = CurrentCamera.transform.rotation;

            // If the movement time is zero or less, the movement is instant
            if (_movementTime == 0)
            {
                instantMove = true;
            }

            while (ratio < 1)
            {
                _elapsedTime += Time.deltaTime;

                if (!instantMove)
                {
                    ratio = _elapsedTime / _movementTime;
                }
                else
                {
                    ratio = 1;
                }

                // Sets the new position closer to the target
                CurrentCamera.transform.position =
                    Vector3.Lerp(startPos, target.position, ratio);

                // Sets the new rotation closer to the target
                CurrentCamera.transform.rotation =
                    Quaternion.Lerp(startRot, target.rotation, ratio);

                yield return null;
            }

            EndCameraPan();
        }

        private void EndCameraPan()
        {
            _moving = false;
            _currentCameraPan = null;
        }

        public void Shake(Vector3 direction, float randomDirAngle,
            float force, float duration)
        {
            StartCoroutine(
                ShakeRoutine(direction, randomDirAngle, force, duration));
        }

        private IEnumerator ShakeRoutine(Vector3 direction,
            float randomDirAngle, float force, float duration)
        {
            // Breaks if a new shake can't be started
            if (_moving || _shaking)
            {
                Debug.LogWarning("Can't shake the camera.");
                yield break;
            }

            Vector3 startPosition = CurrentCamera.transform.position;
            Vector3 shakeDir = direction;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                float waitTime = 0.03f;

                elapsedTime += waitTime;

                // TODO: Random direction
                //Vector3 randDir = direction * randomDirAngle * Random.Range(-0.5f, 1);

                float randForce = force * UnityEngine.Random.Range(-0.5f, 1);

                Vector3 newPosition = startPosition + direction * randForce;
                CurrentCamera.transform.position = newPosition;

                yield return new WaitForSeconds(waitTime);
            }

            CurrentCamera.transform.position = startPosition;
        }

        private Transform GetPlayfieldCamTransform()
        {
            if (CurrentCamera == _defaultCamera)
            {
                return _defaultCamPlayfieldTransform;
            }
            else if (CurrentCamera == _horizontalCamera)
            {
                return _horizontalCamPlayfieldTransform;
            }

            return null;
        }

        private Transform GetLauncherCamTransform()
        {
            return _eventCamLaunchTransform;
        }

        private Transform GetKanteleCamTransform()
        {
            if (CurrentCamera == _defaultCamera)
            {
                return _defaultCamKanteleTransform;
            }
            else if (CurrentCamera == _horizontalCamera)
            {
                return _horizontalCamKanteleTransform;
            }

            return null;
        }
    }
}