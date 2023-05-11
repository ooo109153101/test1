using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("參考物件")]
    public Camera PlayerCamera;

    private void Update()
    {
        // 判斷有沒有按下左鍵
        if (Input.GetMouseButtonDown(0) == true)
        {
            Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  // 從攝影機射出一條射線
            RaycastHit hit;  // 宣告一個射擊點
            Vector3 targetPoint;  // 宣告一個位置點變數，到時候如果有打到東西，就存到這個變數

            // 如果射線有打到具備碰撞體的物件
            if (Physics.Raycast(ray, out hit) == true)
                targetPoint = hit.point;         // 將打到物件的位置點存進 targetPoint
            else
                targetPoint = ray.GetPoint(75);  // 如果沒有打到物件，就以長度75的末端點取得一個點，存進 targetPoint

            Debug.DrawRay(ray.origin, targetPoint - ray.origin, Color.red, 10); // 畫出這條射線
        }
    }
}
