using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("�ѦҪ���")]
    public Camera PlayerCamera;

    private void Update()
    {
        // �P�_���S�����U����
        if (Input.GetMouseButtonDown(0) == true)
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
        }
    }
}
