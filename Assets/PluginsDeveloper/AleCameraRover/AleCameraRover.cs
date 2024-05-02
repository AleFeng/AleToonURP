using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class AleCameraRover : MonoBehaviour
{
    [SerializeField]
    [Tooltip("相机脚本是否激活")]
    private bool m_IsActive = true;

    //Transform缓存
    private Transform TransformGet;

    private void Awake()
    {
        TransformGet = transform;

        InitPosRot();
    }

    private void Start()
    {
        //直接设置 初始化位置&旋转
        BtnInitPosRot();
    }

    private void Update()
    {
        //激活开关
        if (!m_IsActive) return;

        //操作状态
        UpdateOperateState();

        //光标 可见锁定状态
        bool isCursorVisible = true;

        //操作状态优先级
        if (m_ListOperateState.Contains(EOperateState.Drag))
        {
            UpdateDrag(); //拖拽
        }
        else if (m_ListOperateState.Contains(EOperateState.RotateAround))
        {
            UpdateRotateAround(); //旋转环绕
        }
        else if (m_ListOperateState.Contains(EOperateState.Rover))
        {
            UpdateMove(); //移动
            UpdateRotate(); //旋转

            isCursorVisible = false;
        }
        else
        {
            UpdateZoom(); //缩放
        }

        //设置 光标可见锁定状态
        SetCursorVisibleUnLock(isCursorVisible);

        //初始化 位置&旋转
        UpdateInitPosRot();
    }

    #region 光标设置
    /// <summary>
    /// 设置 光标 可见锁定状态
    /// </summary>
    /// <param name="isVisible"></param>
    private void SetCursorVisibleUnLock(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
    #endregion

    #region 操作状态
    /// <summary>
    /// 操作状态
    /// </summary>
    private enum EOperateState
    {
        /// <summary>
        /// 漫游
        /// </summary>
        Rover,
        /// <summary>
        /// 拖拽
        /// </summary>
        Drag,
        /// <summary>
        /// 旋转环绕
        /// </summary>
        RotateAround
    }

    //操作状态列表
    private List<EOperateState> m_ListOperateState = new List<EOperateState>();
    //状态共存设置
    private Dictionary<EOperateState, List<EOperateState>> m_DicOperateStateOverlay = new Dictionary<EOperateState, List<EOperateState>>()
    {
        {EOperateState.Rover, new List<EOperateState>(){}},
        {EOperateState.Drag, new List<EOperateState>(){EOperateState.Rover, EOperateState.RotateAround}},
        {EOperateState.RotateAround, new List<EOperateState>(){}}
    };

    /// <summary>
    /// 更新 操作状态
    /// </summary>
    private void UpdateOperateState()
    {
        //鼠标右键 漫游
        if (Input.GetMouseButtonDown(1))
        {
            ResetMoveSpeed();
            AddOperateState(EOperateState.Rover);
        }
        else if (Input.GetMouseButtonUp(1))
            RemoveOperateState(EOperateState.Rover);

        //鼠标中键 拖拽
        if (Input.GetMouseButtonDown(2))
            AddOperateState(EOperateState.Drag);
        else if (Input.GetMouseButtonUp(2))
            RemoveOperateState(EOperateState.Drag);

        //键盘Alt 旋转环绕
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            AddOperateState(EOperateState.RotateAround);
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
            RemoveOperateState(EOperateState.RotateAround);
    }

    //添加 操作状态
    private void AddOperateState(EOperateState state)
    {
        if (m_ListOperateState.Contains(state)) return;

        //检查 共存状态
        var listStateOverlay = m_DicOperateStateOverlay[state];
        for (int i = 0; i < m_ListOperateState.Count; i++)
        {
            var stateHas = m_ListOperateState[i];
            if (!listStateOverlay.Contains(stateHas)) return;
        }

        m_ListOperateState.Add(state);
    }

    //移除 操作状态
    private void RemoveOperateState(EOperateState state)
    {
        m_ListOperateState.Remove(state);
    }
    #endregion

    #region 缩放
    [Header("缩放")]
    [SerializeField]
    [Tooltip("缩放速度")]
    private float m_ZoomSpeed = 20f;

    /// <summary>
    /// 更新 缩放
    /// </summary>
    private void UpdateZoom()
    {
        TransformGet.Translate(Vector3.forward * Input.mouseScrollDelta.y * Time.deltaTime * m_ZoomSpeed);
    }
    #endregion

    #region 移动
    [Header("移动")]
    [SerializeField]
    [Tooltip("移动速度")]
    private float m_MoveSpeed = 0.02f;

    [SerializeField]
    [Tooltip("快速移动速度")]
    private float m_MoveQuickSpeed = 0.04f;

    [SerializeField]
    [Tooltip("加速度"), Range(0f, 1f)]
    private float m_MoveSpeedAcceleration = 0.4f;

    /// <summary>
    /// 更新 移动
    /// </summary>
    private void UpdateMove()
    {
        Vector3 moveDirect = Vector3.zero; //移动方向
        float moveSpeed = m_MoveSpeed; //移动速度

        //快速移动
        if (Input.GetKey(KeyCode.LeftShift))
            moveSpeed = m_MoveQuickSpeed;

        //移动方向
        if (Input.GetKey(KeyCode.W))
            moveDirect += TransformGet.forward;
        if (Input.GetKey(KeyCode.S))
            moveDirect -= TransformGet.forward;
        if (Input.GetKey(KeyCode.A))
            moveDirect -= TransformGet.right;
        if (Input.GetKey(KeyCode.D))
            moveDirect += TransformGet.right;
        //高低移动
        if (Input.GetKey(KeyCode.E))
            moveDirect += TransformGet.up;
        if (Input.GetKey(KeyCode.Q))
            moveDirect -= TransformGet.up;

        //执行移动
        TransformGet.position += CalculateMoveSpeed(moveDirect != Vector3.zero, moveSpeed, moveDirect);

        //调整移动速度 TODO
        float speedChange = Input.mouseScrollDelta.y * Time.deltaTime;
    }

    private Vector3 m_MoveSpeedDirCur; //当前 移动速度方向
    private float m_MoveSpeedFactorCur; //当前 移动速度系数

    /// <summary>
    /// 计算 移动速度
    /// </summary>
    /// <param name="isMove"></param>
    private Vector3 CalculateMoveSpeed(bool isMove, float moveSpeed, Vector3 moveDir)
    {
        //移动速度系数 增加或减少
        if (isMove)
        {
            m_MoveSpeedFactorCur += m_MoveSpeedAcceleration * 4f * Time.deltaTime;
            m_MoveSpeedFactorCur = Mathf.Clamp01(m_MoveSpeedFactorCur);

            m_MoveSpeedDirCur = LerpVector3(m_MoveSpeedDirCur, moveDir * moveSpeed, m_MoveSpeedFactorCur * m_MoveSpeedAcceleration * 0.7f);
        }
        else
        {
            m_MoveSpeedFactorCur -= m_MoveSpeedAcceleration * 4f * Time.deltaTime;
            m_MoveSpeedFactorCur = Mathf.Clamp01(m_MoveSpeedFactorCur);

            m_MoveSpeedDirCur = LerpVector3(m_MoveSpeedDirCur, Vector3.zero, (1 - m_MoveSpeedFactorCur) * m_MoveSpeedAcceleration * 0.3f);
        }

        return m_MoveSpeedDirCur;
    }

    //重设 移动速度
    private void ResetMoveSpeed()
    {
        m_MoveSpeedDirCur = Vector3.zero;
        m_MoveSpeedFactorCur = 0f;
    }

    //向量 插值
    private Vector3 LerpVector3(Vector3 vecA, Vector3 vecB, float value)
    {
        value = Mathf.Clamp01(value);
        Vector3 vecLerp = vecA + value * (vecB - vecA);
        return vecLerp;
    }
    #endregion

    #region 旋转
    [Header("旋转")]
    [SerializeField]
    [Tooltip("旋转速度")]
    private float m_RotateSpeed = 1f;

    /// <summary>
    /// 更新 旋转
    /// </summary>
    private void UpdateRotate()
    {
        //垂直角度
        TransformGet.rotation *= Quaternion.AngleAxis
        (
            -Input.GetAxis("Mouse Y") * m_RotateSpeed,
            Vector3.right
        );
        //水平角度
        TransformGet.rotation = Quaternion.Euler
        (
            TransformGet.eulerAngles.x,
            TransformGet.eulerAngles.y + Input.GetAxis("Mouse X") * m_RotateSpeed,
            TransformGet.eulerAngles.z
        );
    }
    #endregion

    #region 拖拽
    /// <summary>
    /// 更新 拖拽
    /// </summary>
    private void UpdateDrag()
    {
        Vector3 moveDistanc = TransformGet.right * Input.GetAxis("Mouse X") * -0.02f + TransformGet.up * Input.GetAxis("Mouse Y") * -0.02f;
        TransformGet.position += moveDistanc;
    }
    #endregion

    #region 旋转环绕
    /// <summary>
    /// 旋转环绕
    /// </summary>
    private void UpdateRotateAround()
    {
        if (Input.GetMouseButton(0)) //鼠标左键 旋转环绕
        {
            var posTarget = TransformGet.position + TransformGet.forward * 3f;
            //垂直旋转
            TransformGet.RotateAround(posTarget, Vector3.up, Input.GetAxis("Mouse X"));
            //水平旋转
            var AxisX = TransformGet.right; AxisX.y = 0f; //使轴保持水平
            TransformGet.RotateAround(posTarget, AxisX, -Input.GetAxis("Mouse Y"));
        }
        else if (Input.GetMouseButton(1)) //鼠标右键 缩放
        {
            TransformGet.Translate(Vector3.forward * Input.GetAxis("Mouse X") * 0.02f);
        }
    }
    #endregion

    #region 初始化位置&旋转
    private Vector3 m_InitPosition; //初始化位置
    private Vector3 m_InitRotation; //初始化旋转

    /// <summary>
    /// 初始化 位置&旋转
    /// </summary>
    private void InitPosRot()
    {
        m_InitPosition = TransformGet.position;
        m_InitRotation = TransformGet.eulerAngles;
    }

    /// <summary>
    /// 更新 位置&旋转
    /// </summary>
    private void UpdateInitPosRot()
    {
        //按钮 初始化位置&旋转
        if (Input.GetKeyDown(KeyCode.R))
            BtnInitPosRot();
    }

    //按钮 初始化位置&旋转
    private void BtnInitPosRot()
    {
        TransformGet.position = m_InitPosition;
        TransformGet.eulerAngles = m_InitRotation;
    }
    #endregion
}
