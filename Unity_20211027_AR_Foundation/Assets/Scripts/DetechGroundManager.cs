using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace coffee.ARFoundation
{
    /// <summary>
    /// �˴��a�O�I���޲z��
    /// �I���a�O��B�z���ʦ欰
    /// �ͦ�����P������m
    /// </summary>
    public class DetechGroundManager : MonoBehaviour
    {
        [Header("�I����n�ͦ�������")]
        public GameObject goSpawn;
        [Header("AR �g�u�޲z��"), Tooltip("���޲z�����m����v�� Origin ����W")]
        public ARRaycastManager arRaycastManager;
        [Header("�ͦ�����n���V����v������")]
        public Transform traCamera;
        [Header("�ͦ����󭱦V�t��"), Range(0, 100)]
        public float speedLookAt = 3.5f;

        private Transform traSpawn;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        /// <summary>
        /// �ƹ�����PĲ��
        /// </summary>
        private bool inputMouseLeft { get => Input.GetKeyDown(KeyCode.Mouse0); }

        private void Update()
        {
            ClickAndDetechGround();
            SpawnLookAtCamera();
        }

        /// <summary>
        /// �I���P�˴��a�O
        /// 1. �����O�_�����w����
        /// 2. �����I���y��
        /// 3. �g�u�˴�
        /// 4. ���椬��
        /// </summary>
        private void ClickAndDetechGround()
        {
            if (inputMouseLeft)  // �p�G���U���w����
            {
                Vector2 positionMouse = Input.mousePosition;                                          // ���o�I���y��
                // Ray ray = Camera.main.ScreenPointToRay(positionMouse);                             // �N�y���ର�g�u
                if (arRaycastManager.Raycast(positionMouse, hits, TrackableType.PlaneWithinPolygon))  // �p�G �g�u������w���a�O����
                {
                    Vector3 positionHit = hits[0].pose.position;                                      // ���o�I���y��
                    if (traSpawn == null)                                                             // �p�G ���󥼥ͦ� �h�ͦ��@�Ӫ���
                    {
                        traSpawn = Instantiate(goSpawn, positionHit, Quaternion.identity).transform;
                        traSpawn.localScale = Vector3.one * 0.5f;
                    }
                    else                                                                              // �_�h ���ͦ��L���� �h��s�Ӫ���y��
                    {
                        traSpawn.position = positionHit;
                    }
                }
            }
        }

        /// <summary>
        /// �ͦ����󭱦V��v��
        /// </summary>
        private void SpawnLookAtCamera()
        {
            if (traSpawn == null) return;                                                                 // �p�G �ͦ����󬰪ŭ�(���ͦ�) ���XMethod
            Quaternion angle = Quaternion.LookRotation(traCamera.position - traSpawn.position);           // ���o�V�q
            traSpawn.rotation = Quaternion.Lerp(traSpawn.rotation, angle, Time.deltaTime * speedLookAt);  // ���״���
            Vector3 angleOriginal = traSpawn.localEulerAngles;                                            // ���o����
            angleOriginal.x = 0;                                                                          // �ᵲX
            angleOriginal.z = 0;                                                                          // �ᵲZ
            traSpawn.localEulerAngles = angleOriginal;                                                    // ��s����
        }
    }
}