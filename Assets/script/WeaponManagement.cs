using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // �n���o�Ӥ~�౱���r��

public class WeaponManagement : MonoBehaviour
{
    [Header("�ѦҪ���")]
    public Camera PlayerCamera;
    public Transform attackPoint;

    [Header("�l�u�w�m����")]
    public GameObject bullet;

    [Header("�j�K�]�w")]
    public int magazineSize;        // �]�w�u���i�H��h�����l�u�H
    public int bulletsLeft;         // �l�u�٦��h�����H(�p�G�S���n���աA�A�i�H�]�w�� Private)
    public float reloadTime;        // �]�w���u���һݭn���ɶ�
    public float recoilForce;       // �ϧ@�ΤO

    bool reloading;                 // ���L�ܼơG�x�s�O���O���b���u�������A�HTrue�G���b���u���BFalse�G���u�����ʧ@�w����

    [Header("UI����")]
    public TextMeshProUGUI ammunitionDisplay; // �u�q���
    public TextMeshProUGUI reloadingDisplay;  // ��ܬO���O���b���u���H

    [Header("�Z��")]
    public GameObject[] weaponObjects;        // �Z���M��

    int weaponNumber = 0;                     // �ثe��ܪZ�������ǽs��
    GameObject weaponInUse;                   // �ثe��ܪZ��

    private void Start()
    {
        bulletsLeft = magazineSize;        // �C���@�}�l�u���]�w���������A
        reloadingDisplay.enabled = false;  // �N��ܥ��b���u�����r�����ð_��

        ShowAmmoDisplay();                 // ��s�u�q���
        weaponInUse = weaponObjects[0];    // �C���@�}�l�]�w�Z������0�ӪZ��
    }

    private void Update()
    {
        MyInput();
    }

    // ��k�G�������a�ާ@���A
    private void MyInput()
    {
        // �P�_�G���S�����U����H
        if (Input.GetMouseButtonDown(0) == true)
        {
            // �p�G�٦��l�u�A�åB�S�����b���ˤl�u�A�N�i�H�g��
            if (bulletsLeft > 0 && !reloading)
            {
                Shoot();
            }
        }

        // �P�_�G1.�����UR��B2.�l�u�ƶq�C��u�������u�q�B3.���O���u�������A�A�T�ӱ��󳣺����A�N�i�H���u��
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        // �P�_�G���U�Ʀr��1�A�������Z��0
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0, 0);

        // �P�_�G���U�Ʀr��2�A�������Z��1
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(0, 1);

        // �P�_�G�u�ʷƹ��u��
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)      // ���e�u��
            SwitchWeapon(1);
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // ����u��
            SwitchWeapon(-1);
    }

    // ��k�G�g���Z��
    private void Shoot()
    {
        Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  // �q��v���g�X�@���g�u
        RaycastHit hit;  // �ŧi�@�Ӯg���I
        Vector3 targetPoint;  // �ŧi�@�Ӧ�m�I�ܼơA��ɭԦp�G������F��A�N�s��o���ܼ�

        // �p�G�g�u�������ƸI���骺����
        if (Physics.Raycast(ray, out hit) == true)
            targetPoint = hit.point;         // �N���쪫�󪺦�m�I�s�i targetPoint
        else
            targetPoint = ray.GetPoint(75);  // �p�G�S�����쪫��A�N�H����75�������I���o�@���I�A�s�i targetPoint

        Debug.DrawRay(ray.origin, targetPoint - ray.origin, Color.red, 10); // �e�X�o���g�u

        Vector3 shootingDirection = targetPoint - attackPoint.position; // �H�_�I�P���I�������I��m�A�p��X�g�u����V
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); // �b�����I�W�����ͤ@�Ӥl�u
        currentBullet.transform.forward = shootingDirection.normalized; // �N�l�u�����V�P�g�u��V�@�P

        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * 20, ForceMode.Impulse); // �̾ڭ����V���e�l�u
        //currentBullet.GetComponent<Rigidbody>().AddForce(PlayerCamera.transform.up * , ForceMode.Impulse);

        bulletsLeft--;    // �N�u�������l�u��@�A�H�U���g�k���O�@�˪��N��
                          //bulletsLeft -= 1;               
                          //bulletsLeft = bulletsLeft - 1;  // ����o�۪��g�k

        // ��y�O����
        this.GetComponent<Rigidbody>().AddForce(-shootingDirection.normalized * recoilForce, ForceMode.Impulse);

        ShowAmmoDisplay();                 // ��s�u�q���

        weaponInUse.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Fire");  // Ĳ�o�uFire�v��Ĳ�o�ܼ�
    }

    // ��k�G���u��������ɶ��]�w
    private void Reload()
    {
        reloading = true;                      // �����N���u�����A�]�w���G���b���u��
        reloadingDisplay.enabled = true;       // �N���b���u�����r����ܥX��
        Invoke("ReloadFinished", reloadTime);  // �̷�reloadTime�ҳ]�w�����u���ɶ��˼ơA�ɶ���0�ɰ���ReloadFinished��k
    }

    // ��k�G���u��
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;            // �N�l�u��
        reloading = false;                     // �N���u�����A�]�w���G�󴫼u������
        reloadingDisplay.enabled = false;      // �N���b���u�����r�����áA�������u�����ʧ@
        ShowAmmoDisplay();
    }

    // ��k�G��s�u�q���
    private void ShowAmmoDisplay()
    {
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText($"Ammo {bulletsLeft} / {magazineSize}");
    }

    // ��k�G�Z�������A�Ѽ�_addNumber�B_weaponNumber
    private void SwitchWeapon(int _addNumber, int _weaponNumber = 0)
    {
        // �N�Z���M��������áA���@���������áA�A��ܻݭn���Z��
        foreach (GameObject item in weaponObjects)
        {
            item.SetActive(false);
        }

        // switch �P�_���G�H�Ѽ�_addNumber�P�_�n�������Z��
        switch (_addNumber)
        {
            case 0:                                                   // _addNumber == 0�A�N��Ϋ��䪽�����w�Z���}�C��}
                weaponNumber = _weaponNumber;
                break;
            case 1:                                                   // _addNumber == 1�A�N���W�u�ƹ��u��
                if (weaponNumber == weaponObjects.Length - 1)         // ��{�`���Ʀr�A���w�쥻���Z���}�C��}�w�g�O�̫�@�ӪZ���A�h�N�Z���}�C��}�]�w��0
                    weaponNumber = 0;
                else
                    weaponNumber += 1;
                //weaponNumber = (weaponNumber == weaponObjects.Length - 1) ? 0 : weaponNumber += 1; // �]�i�H��H�W���P�_���g���o��
                break;
            case -1:                                                   // _addNumber == -1�A�N���U�u�ƹ��u��
                if (weaponNumber == 0)                                 // ��{�`���Ʀr�A���w�쥻���Z���}�C��}�O�Ĥ@�ӪZ���A�h�N�Z���}�C��}���M�檺�̫�@�Ӧ�}
                    weaponNumber = weaponObjects.Length - 1;
                else
                    weaponNumber -= 1;
                //weaponNumber = (weaponNumber == 0) ? weaponObjects.Length - 1 : weaponNumber -= 1; // �]�i�H��H�W���P�_���g���o��
                break;
        }
        weaponObjects[weaponNumber].SetActive(true);    // ��ܩҫ��w���Z��
        weaponInUse = weaponObjects[weaponNumber];      // �]�w�ثe�ҿ�ܪ��Z������(���ɥi�H�ΨӰ���Z���үS�w����k�A�U�@���`�|����)
    }
}