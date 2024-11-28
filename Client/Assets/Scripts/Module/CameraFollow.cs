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
    //距离变化速度
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
        //默认为主相机
        Camera = Camera.main;
        SwitchView();
        EventHandler.OnLeaveRoom += OnLeaveRoom;
    }

    //调整距离
    void Zoom()
    {
        if (viewType == ViewType.First) return;

        float axis = Input.GetAxis("Mouse ScrollWheel");
        radius += axis * zoomSpeed;
        radius = Mathf.Clamp(radius, minRadius, maxRadius);
        HorizontalMove(0);
        VerticalMove(0);
    }

    //调整角度
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
        {//右键
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

    //所有组件update之后发生
    void LateUpdate()
    {
        /*//玩家位置
        Vector3 pos = transform.cameraOffset;
        //玩家方向
        Vector3 forward = transform.forward;
        Vector3 rigiht = transform.right;
        //相机目标位置
        Vector3 targetPos = pos;
        targetPos = pos + forward * radius + rigiht * distance.x;
        targetPos.y += distance.y;
        //相机位置
        Vector3 cameraPos = Camera.transform.cameraOffset;
        cameraPos = Vector3.MoveTowards(cameraPos, targetPos, Time.deltaTime * speed);
        Camera.transform.cameraOffset = cameraPos;
        //对准玩家
        Camera.main.transform.LookAt(pos + offset);*/
        Zoom();
        //调整角度
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

    //第三人称水平移动视角
    private void HorizontalMove(float x)
    {
        horizontalAngle += x * thirdSpeed * Time.deltaTime;
        if (horizontalAngle > 360 || horizontalAngle < 0)
            horizontalAngle = (360 + horizontalAngle) % 360;
        cameraOffset.x = Mathf.Sin(horizontalAngle / 180f * Mathf.PI) * Mathf.Sin(verticalAngle / 180f * Mathf.PI) * radius;
        cameraOffset.z = Mathf.Cos(horizontalAngle / 180f * Mathf.PI) * Mathf.Sin(verticalAngle / 180f * Mathf.PI) * radius;
        //Debug.Log("HorizontalAngle:" + horizontalAngle);
    }
    //竖直移动视角
    private void VerticalMove(float y)
    {
        verticalAngle += y * thirdSpeed * Time.deltaTime;
        if (verticalAngle < 80) verticalAngle = 80f;
        if (verticalAngle > 179) verticalAngle = 179f;
        cameraOffset.y = -Mathf.Cos(verticalAngle / 180f * Mathf.PI) * radius;
        //Debug.Log("VerticalAngle:" + verticalAngle + "PositionY:" + cameraOffset.y);
    }

    //相机平滑跟踪
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