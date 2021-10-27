using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace coffee.ARFoundation
{
    /// <summary>
    /// 檢測地板點擊管理器
    /// 點擊地板後處理互動行為
    /// 生成物件與控制物件位置
    /// </summary>
    public class DetechGroundManager : MonoBehaviour
    {
        [Header("點擊後要生成的物件")]
        public GameObject goSpawn;
        [Header("AR 射線管理器"), Tooltip("此管理器須置於攝影機 Origin 物件上")]
        public ARRaycastManager arRaycastManager;
        [Header("生成物件要面向的攝影機物件")]
        public Transform traCamera;
        [Header("生成物件面向速度"), Range(0, 100)]
        public float speedLookAt = 3.5f;

        private Transform traSpawn;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        /// <summary>
        /// 滑鼠左鍵與觸控
        /// </summary>
        private bool inputMouseLeft { get => Input.GetKeyDown(KeyCode.Mouse0); }

        private void Update()
        {
            ClickAndDetechGround();
            SpawnLookAtCamera();
        }

        /// <summary>
        /// 點擊與檢測地板
        /// 1. 偵測是否按指定按鍵
        /// 2. 紀錄點擊座標
        /// 3. 射線檢測
        /// 4. 執行互動
        /// </summary>
        private void ClickAndDetechGround()
        {
            if (inputMouseLeft)  // 如果按下指定按鍵
            {
                Vector2 positionMouse = Input.mousePosition;                                          // 取得點擊座標
                // Ray ray = Camera.main.ScreenPointToRay(positionMouse);                             // 將座標轉為射線
                if (arRaycastManager.Raycast(positionMouse, hits, TrackableType.PlaneWithinPolygon))  // 如果 射線打到指定的地板物件
                {
                    Vector3 positionHit = hits[0].pose.position;                                      // 取得點擊座標
                    if (traSpawn == null)                                                             // 如果 物件未生成 則生成一個物件
                    {
                        traSpawn = Instantiate(goSpawn, positionHit, Quaternion.identity).transform;
                        traSpawn.localScale = Vector3.one * 0.5f;
                    }
                    else                                                                              // 否則 有生成過物件 則更新該物件座標
                    {
                        traSpawn.position = positionHit;
                    }
                }
            }
        }

        /// <summary>
        /// 生成物件面向攝影機
        /// </summary>
        private void SpawnLookAtCamera()
        {
            if (traSpawn == null) return;                                                                 // 如果 生成物件為空值(未生成) 跳出Method
            Quaternion angle = Quaternion.LookRotation(traCamera.position - traSpawn.position);           // 取得向量
            traSpawn.rotation = Quaternion.Lerp(traSpawn.rotation, angle, Time.deltaTime * speedLookAt);  // 角度插值
            Vector3 angleOriginal = traSpawn.localEulerAngles;                                            // 取得角度
            angleOriginal.x = 0;                                                                          // 凍結X
            angleOriginal.z = 0;                                                                          // 凍結Z
            traSpawn.localEulerAngles = angleOriginal;                                                    // 更新角度
        }
    }
}