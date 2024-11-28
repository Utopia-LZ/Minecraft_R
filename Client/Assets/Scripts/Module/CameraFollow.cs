using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ViewType
{
    First,
    Third,
}

public class CameraFollow : MonoBehaviour
{
    public ViewType viewType = ViewType.Third;

    public Camera Camera;
    public static float thirdSpeed = 20f;
    public static float followSpeed = 6f;
    //����仯�ٶ�
    public static float zoomSpeed = 2f;

    public static float radius = 5f;
    public static float maxRadius = 10f;
    public static float minRadius = 2f;
    private float horizontalAngle = 0;
    private float verticalAngle = 0;
    private Vector3 cameraOffset = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        //Ĭ��Ϊ�����
        Camera = Camera.main;
        SwitchView();
        EventHandler.OnLeaveRoom += OnLeaveRoom;
    }

    //��������
    void Zoom()
    {
        if (viewType == ViewType.First) return;

        float axis = Input.GetAxis("Mouse ScrollWheel");
        radius += axis * zoomSpeed;
        radius = Mathf.Clamp(radius, minRadius, maxRadius);
        HorizontalMove(0);
        VerticalMove(0);
    }

    //�����Ƕ�
    void Rotate()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        if (viewType == ViewType.First)
        {
            if(x != 0)
            {
                transform.Rotate(transform.up * x);
            }
            if(y != 0)
            {
                Camera.transform.Rotate(Vector3.left * y);
            }
        }
        else if(viewType == ViewType.Third)
        {
            if (x != 0)
            {
                HorizontalMove(x);
                Vector3 dir = Camera.transform.forward;
                dir.y = 0;
                transform.forward = dir;
            }
            if (y != 0)
            {
                VerticalMove(y);
            }
            CameraSmoothMove();
        }

        /*if (!Input.GetMouseButton(1))
        {//�Ҽ�
            return;
        }
        float axis = Input.GetAxis("Mouse X");
        distance.x += 2 * axis;
        distance.x = Mathf.Clamp(distance.x, -20, 20);*/
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SwitchView();
        }
    }

    //�������update֮����
    void LateUpdate()
    {
        /*//���λ��
        Vector3 pos = transform.cameraOffset;
        //��ҷ���
        Vector3 forward = transform.forward;
        Vector3 rigiht = transform.right;
        //���Ŀ��λ��
        Vector3 targetPos = pos;
        targetPos = pos + forward * radius + rigiht * distance.x;
        targetPos.y += distance.y;
        //���λ��
        Vector3 cameraPos = Camera.transform.cameraOffset;
        cameraPos = Vector3.MoveTowards(cameraPos, targetPos, Time.deltaTime * speed);
        Camera.transform.cameraOffset = cameraPos;
        //��׼���
        Camera.main.transform.LookAt(pos + offset);*/
        Zoom();
        //�����Ƕ�
        Rotate();
    }

    private void SwitchView()
    {
        viewType = (ViewType)(((int)viewType + 1) % 2);
        switch (viewType)
        {
            case ViewType.First:
                Camera.transform.SetParent(transform);
                Camera.transform.position = transform.position + transform.up * 0.5f;
                Camera.transform.rotation = transform.rotation;
                break;
            case ViewType.Third:
                Camera.transform.SetParent(null);
                Camera.transform.position = transform.position - transform.forward * radius;
                Camera.transform.LookAt(transform.position);
                break;
        }
    }

    //�����˳�ˮƽ�ƶ��ӽ�
    private void HorizontalMove(float x)
    {
        horizontalAngle += x * thirdSpeed * Time.deltaTime;
        if (horizontalAngle > 360 || horizontalAngle < 0)
            horizontalAngle = (360 + horizontalAngle) % 360;
        cameraOffset.x = Mathf.Sin(horizontalAngle / 180f * Mathf.PI) * Mathf.Sin(verticalAngle / 180f * Mathf.PI) * radius;
        cameraOffset.z = Mathf.Cos(horizontalAngle / 180f * Mathf.PI) * Mathf.Sin(verticalAngle / 180f * Mathf.PI) * radius;
        //Debug.Log("HorizontalAngle:" + horizontalAngle);
    }
    //��ֱ�ƶ��ӽ�
    private void VerticalMove(float y)
    {
        verticalAngle += y * thirdSpeed * Time.deltaTime;
        if (verticalAngle < 80) verticalAngle = 80f;
        if (verticalAngle > 179) verticalAngle = 179f;
        cameraOffset.y = -Mathf.Cos(verticalAngle / 180f * Mathf.PI) * radius;
        //Debug.Log("VerticalAngle:" + verticalAngle + "PositionY:" + cameraOffset.y);
    }

    //���ƽ������
    private void CameraSmoothMove()
    {
        Vector3 diff = transform.position + cameraOffset - Camera.transform.position;
        Camera.transform.position += followSpeed * Time.deltaTime * diff;
        Camera.transform.LookAt(transform.position);
    }

    private void OnLeaveRoom()
    {
        if(viewType == ViewType.First)
        {
            Camera.transform.SetParent(null);
        }
    }
}