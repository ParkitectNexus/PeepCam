using System;
using System.Collections;
using UnityEngine;

namespace PeepCam
{
    class PeepCam : MonoBehaviour
    {
        private GameObject _headCam;

        private bool _isWalking;

        private PeepCam _peepCam;

        public static PeepCam Instance;

        private float _origShadowDist;
        private int _origQualityLevel;
        private int _origResoWidth;
        private int _origResoHeight;

        private Camera _cam;

        float fps = 0.0f;

        void Awake()
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Tab) && !_isWalking)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    EnterHeadCam(hit.transform.position + Vector3.up);
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
        
        private void LowerGraphicSettings()
        {
            Application.targetFrameRate = -1;

            _origShadowDist = QualitySettings.shadowDistance;
            _origQualityLevel = QualitySettings.GetQualityLevel();
            _origResoWidth = Screen.width;
            _origResoHeight = Screen.height;

            QualitySettings.SetQualityLevel(0);
            QualitySettings.shadowDistance = 0f;
            QualitySettings.antiAliasing = 2;

            if (Screen.fullScreen)
                Screen.SetResolution((int)(_origResoWidth / 1.2f), (int)(_origResoHeight / 1.2f), Screen.fullScreen);
        }

        private void RestoreGraphicSettings()
        {
            Application.targetFrameRate = 60;

            QualitySettings.shadowDistance = _origShadowDist;
            QualitySettings.SetQualityLevel(_origQualityLevel);

            if (Screen.fullScreen)
                Screen.SetResolution(_origResoWidth, _origResoHeight, Screen.fullScreen);
        }

        public void EnterHeadCam(Vector3 position)
        {
            if (_isWalking)
                return;

            LowerGraphicSettings();

            _headCam = new GameObject();

            _cam = _headCam.AddComponent<Camera>();
            _headCam.AddComponent<AudioListener>();
            _headCam.AddComponent<FpsMouse>();
            _headCam.AddComponent<PlayerController>();
            CharacterController cc = _headCam.AddComponent<CharacterController>();

            cc.radius = 0.1f;
            cc.height = 0.5f;
            cc.center = new Vector3(0, -0.5f, 0);

            _headCam.transform.position = position;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _isWalking = true;
        }

        public void LeaveHeadCam()
        {
            if (!_isWalking)
                return;

            RestoreGraphicSettings();

            Destroy(_headCam);

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
