﻿using System;
using System.Collections;
using Parkitect.UI;
using UnityEngine;

namespace PeepCam
{
    class PeepCam : MonoBehaviour
    {
        private GameObject _headCam;

        private bool _isWalking;

        private PeepCam _peepCam;

        public static PeepCam Instance;

        private Camera _cam;

        float fps = 0.0f;

        void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

			CullingGroupManager.Instance.setTargetCamera(_cam);
		}
		
		void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab) && !_isWalking)
            {
                if (Utility.getObjectBelowMouse().hitSomething)
                {
                    EnterHeadCam(Utility.getObjectBelowMouse().hitPosition + Vector3.up);
                }
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                LeaveHeadCam();
            }

            if(_isWalking)
                AdaptFarClipPaneToFPS();
        }

        private void AdaptFarClipPaneToFPS()
        {
            fps = 1.0f / Time.deltaTime;

            if (fps < 40)
            {
                _cam.farClipPlane = Math.Max(40, _cam.farClipPlane - 1f);
            }

            if (fps > 55)
            {
                _cam.farClipPlane = Math.Min(120, _cam.farClipPlane + 1f);
            }
        }
        
        public void EnterHeadCam(Vector3 position)
        {
            if (_isWalking)
                return;

            _headCam = new GameObject();
            _headCam.layer = LayerMask.NameToLayer("CoasterCars");

            _cam = _headCam.AddComponent<Camera>();
			CullingGroupManager.Instance.setTargetCamera(_cam);
			_headCam.AddComponent<AudioListener>();
            _headCam.AddComponent<FpsMouse>();
            _headCam.AddComponent<PlayerController>();
            CharacterController cc = _headCam.AddComponent<CharacterController>();

            cc.radius = 0.1f;
            cc.height = 0.3f;
            cc.center = new Vector3(0, -0.3f, 0);

            _headCam.transform.position = position;

            UIWorldOverlayController.Instance.gameObject.SetActive(false);

            Camera.main.GetComponent<CameraController>().enabled = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _isWalking = true;
        }

        public void LeaveHeadCam()
        {
            if (!_isWalking)
                return;

            Destroy(_headCam);

            Camera.main.GetComponent<CameraController>().enabled = true;
			CullingGroupManager.Instance.setTargetCamera(Camera.main.GetComponent<Camera>());

			UIWorldOverlayController.Instance.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _isWalking = false;
        }

        void OnDestroy()
        {
            LeaveHeadCam();
        }
    }
}
