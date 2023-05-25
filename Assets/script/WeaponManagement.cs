using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // 要有這個才能控制文字框

public class WeaponManagement : MonoBehaviour
{
    [Header("參考物件")]
    public Camera PlayerCamera;
    public Transform attackPoint;

    [Header("子彈預置物件")]
    public GameObject bullet;

    [Header("槍枝設定")]
    public int magazineSize;        // 設定彈夾可以放多少顆子彈？
    public int bulletsLeft;         // 子彈還有多少顆？(如果沒有要測試，你可以設定成 Private)
    public float reloadTime;        // 設定換彈夾所需要的時間
    public float recoilForce;       // 反作用力

    bool reloading;                 // 布林變數：儲存是不是正在換彈夾的狀態？True：正在換彈夾、False：換彈夾的動作已結束

    [Header("UI物件")]
    public TextMeshProUGUI ammunitionDisplay; // 彈量顯示
    public TextMeshProUGUI reloadingDisplay;  // 顯示是不是正在換彈夾？

    [Header("武器")]
    public GameObject[] weaponObjects;        // 武器清單

    int weaponNumber = 0;                     // 目前選擇武器的順序編號
    GameObject weaponInUse;                   // 目前選擇武器

    private void Start()
    {
        bulletsLeft = magazineSize;        // 遊戲一開始彈夾設定為全滿狀態
        reloadingDisplay.enabled = false;  // 將顯示正在換彈夾的字幕隱藏起來

        ShowAmmoDisplay();                 // 更新彈量顯示
        weaponInUse = weaponObjects[0];    // 遊戲一開始設定武器為第0個武器
    }

    private void Update()
    {
        MyInput();
    }

    // 方法：偵測玩家操作狀態
    private void MyInput()
    {
        // 判斷：有沒有按下左鍵？
        if (Input.GetMouseButtonDown(0) == true)
        {
            // 如果還有子彈，並且沒有正在重裝子彈，就可以射擊
            if (bulletsLeft > 0 && !reloading)
            {
                Shoot();
            }
        }

        // 判斷：1.有按下R鍵、2.子彈數量低於彈夾內的彈量、3.不是換彈夾的狀態，三個條件都滿足，就可以換彈夾
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        // 判斷：按下數字鍵1，切換為武器0
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0, 0);

        // 判斷：按下數字鍵2，切換為武器1
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(0, 1);

        // 判斷：滾動滑鼠滾輪
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)      // 往前滾動
            SwitchWeapon(1);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // 往後滾動
            SwitchWeapon(-1);
    }

    // 方法：射擊武器
    private void Shoot()
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

        Vector3 shootingDirection = targetPoint - attackPoint.position; // 以起點與終點之間兩點位置，計算出射線的方向
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); // 在攻擊點上面產生一個子彈
        currentBullet.transform.forward = shootingDirection.normalized; // 將子彈飛行方向與射線方向一致

        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * 20, ForceMode.Impulse); // 依據飛行方向推送子彈
        //currentBullet.GetComponent<Rigidbody>().AddForce(PlayerCamera.transform.up * , ForceMode.Impulse);

        bulletsLeft--;    // 將彈夾中的子彈減一，以下的寫法都是一樣的意思
                          //bulletsLeft -= 1;               
                          //bulletsLeft = bulletsLeft - 1;  // 比較囉嗦的寫法

        // 後座力模擬
        this.GetComponent<Rigidbody>().AddForce(-shootingDirection.normalized * recoilForce, ForceMode.Impulse);

        ShowAmmoDisplay();                 // 更新彈量顯示

        weaponInUse.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Fire");  // 觸發「Fire」的觸發變數
    }

    // 方法：換彈夾的延遲時間設定
    private void Reload()
    {
        reloading = true;                      // 首先將換彈夾狀態設定為：正在換彈夾
        reloadingDisplay.enabled = true;       // 將正在換彈夾的字幕顯示出來
        Invoke("ReloadFinished", reloadTime);  // 依照reloadTime所設定的換彈夾時間倒數，時間為0時執行ReloadFinished方法
    }

    // 方法：換彈夾
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;            // 將子彈填滿
        reloading = false;                     // 將換彈夾狀態設定為：更換彈夾結束
        reloadingDisplay.enabled = false;      // 將正在換彈夾的字幕隱藏，結束換彈夾的動作
        ShowAmmoDisplay();
    }

    // 方法：更新彈量顯示
    private void ShowAmmoDisplay()
    {
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText($"Ammo {bulletsLeft} / {magazineSize}");
    }

    // 方法：武器切換，參數_addNumber、_weaponNumber
    private void SwitchWeapon(int _addNumber, int _weaponNumber = 0)
    {
        // 將武器清單全部隱藏，先一次全部隱藏，再顯示需要的武器
        foreach (GameObject item in weaponObjects)
        {
            item.SetActive(false);
        }

        // switch 判斷式：以參數_addNumber判斷要怎麼切換武器
        switch (_addNumber)
        {
            case 0:                                                   // _addNumber == 0，代表用按鍵直接指定武器陣列位址
                weaponNumber = _weaponNumber;
                break;
            case 1:                                                   // _addNumber == 1，代表往上滾滑鼠滾輪
                if (weaponNumber == weaponObjects.Length - 1)         // 實現循環數字，假定原本的武器陣列位址已經是最後一個武器，則將武器陣列位址設定為0
                    weaponNumber = 0;
                else
                    weaponNumber += 1;
                //weaponNumber = (weaponNumber == weaponObjects.Length - 1) ? 0 : weaponNumber += 1; // 也可以把以上的判斷式寫成這樣
                break;
            case -1:                                                   // _addNumber == -1，代表往下滾滑鼠滾輪
                if (weaponNumber == 0)                                 // 實現循環數字，假定原本的武器陣列位址是第一個武器，則將武器陣列位址為清單的最後一個位址
                    weaponNumber = weaponObjects.Length - 1;
                else
                    weaponNumber -= 1;
                //weaponNumber = (weaponNumber == 0) ? weaponObjects.Length - 1 : weaponNumber -= 1; // 也可以把以上的判斷式寫成這樣
                break;
        }
        weaponObjects[weaponNumber].SetActive(true);    // 顯示所指定的武器
        weaponInUse = weaponObjects[weaponNumber];      // 設定目前所選擇的武器物件(屆時可以用來執行武器所特定的方法，下一章節會介紹)
    }
}