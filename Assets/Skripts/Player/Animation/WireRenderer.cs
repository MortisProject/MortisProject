// Assets/Scripts/Player/Animation/WireRenderer.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class WireRenderer : MonoBehaviour
{
    private enum State { Hidden, Launching, Taut }
    private State _currentState = State.Hidden;

    [Header("Settings")]
    [Tooltip("라인 렌더러의 점 개수입니다. 높을수록 부드럽습니다.")]
    [SerializeField] private int _pointCount = 20;
    [Tooltip("발사 시 와이어가 출렁이는 강도입니다.")]
    [SerializeField] private float _wobbleStrength = 4f;
    [Tooltip("출렁임이 원래 위치로 돌아오려는 탄성입니다.")]
    [SerializeField] private float _springiness = 60f;
    [Tooltip("출렁임이 점차 멈추는 정도입니다.")]
    [SerializeField] private float _damping = 4f;

    private LineRenderer _lineRenderer;
    private Transform _startPoint;
    private Transform _endPoint;
    private Transform _hookTransform; 

    // 출렁임을 계산하기 위한 가상의 중간점들
    private List<Vector3> _points;
    private List<Vector3> _pointVelocities;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _pointCount;

        _points = new List<Vector3>(_pointCount);
        _pointVelocities = new List<Vector3>(_pointCount);
        for (int i = 0; i < _pointCount; i++)
        {
            _points.Add(Vector3.zero);
            _pointVelocities.Add(Vector3.zero);
        }
    }

    private void LateUpdate()
    {
        if (_currentState == State.Hidden) return;

        if (_currentState == State.Launching)
        {
            DrawWobble();
            // 훅이 목표에 거의 도달하면 팽팽한 모드로 전환
            if (Vector3.Distance(_hookTransform.position, _endPoint.position) < 0.5f)
            {
                SetTaut();
            }
        }
        else if (_currentState == State.Taut)
        {
            DrawTaut();
        }
    }

    /// <summary>
    /// 와이어를 활성화하고 발사 연출을 시작합니다.
    /// </summary>
    public void Activate(Transform start, Transform end, Transform hook)
    {
        _startPoint = start;
        _endPoint = end;
        _hookTransform = hook;
        _lineRenderer.enabled = true;
        _currentState = State.Launching;

        for (int i = 0; i < _pointCount; i++)
        {
            _points[i] = _startPoint.position;
            _pointVelocities[i] = Vector3.zero;
        }
        _pointVelocities[0] = Random.onUnitSphere * _wobbleStrength;
    }

    /// <summary>
    /// 와이어를 비활성화합니다.
    /// </summary>
    public void Deactivate()
    {
        _currentState = State.Hidden;
        _lineRenderer.enabled = false;
        _startPoint = null;
        _endPoint = null;
        _hookTransform = null;
    }

    private void SetTaut()
    {
        _currentState = State.Taut;
    }

    /// <summary>
    /// 출렁이는 와이어의 각 점 위치를 계산하고 그립니다.
    /// </summary>
    private void DrawWobble()
    {
        Vector3 startPos = _startPoint.position;
        Vector3 endPos = _hookTransform.position;

        _points[0] = startPos; // 시작점은 항상 플레이어 손
        _points[_pointCount - 1] = endPos; // 끝점은 항상 와이어 타겟

        for (int i = 1; i < _pointCount - 1; i++)
        {
            float t = (float)i / (_pointCount - 1);

            // 점이 있어야 할 이상적인 직선 위의 위치
            Vector3 idealPosition = Vector3.Lerp(startPos, endPos, t);

            // 이상적인 위치로 돌아가려는 복원력(탄성) 계산
            Vector3 restoringForce = (idealPosition - _points[i]) * _springiness;

            // 움직임을 멈추게 하려는 감쇠력 계산
            Vector3 dampingForce = -_pointVelocities[i] * _damping;

            // 두 힘을 합쳐 가속도를 구하고, 속도와 위치를 업데이트
            Vector3 acceleration = (restoringForce + dampingForce) / 1f; // 질량=1로 가정
            _pointVelocities[i] += acceleration * Time.deltaTime;
            _points[i] += _pointVelocities[i] * Time.deltaTime;
        }

        // 계산된 모든 점의 위치를 LineRenderer에 설정
        _lineRenderer.SetPositions(_points.ToArray());
    }
    private void DrawTaut()
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, _startPoint.position);
        _lineRenderer.SetPosition(1, _endPoint.position);
    }
}