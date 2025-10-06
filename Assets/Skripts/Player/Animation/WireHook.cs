// Assets/Scripts/Player/Animation/WireHook.cs
using UnityEngine;

/// <summary>
/// 와이어 발사 시 생성되는 '훅' 오브젝트의 움직임과 방향을 제어합니다.
/// </summary>
public class WireHook : MonoBehaviour
{
    [Header("Hook Settings")]
    [Tooltip("훅이 날아갈 목표 Transform입니다. PlayerWireLaunchState에서 설정됩니다.")]
    public Transform target;

    [Tooltip("훅이 날아가는 속도입니다.")]
    public float speed = 50f;

    [Tooltip("사용할 3D 모델의 '앞쪽'이 향하는 축을 설정합니다. \n" +
         "예: 모델의 위쪽(Y축)이 앞이라면 (0, 1, 0)으로 설정")]
    public Vector3 modelForwardDirection = Vector3.up;

    /// <summary>
    /// 매 프레임마다 목표를 향해 이동하고, 목표를 바라보도록 방향을 회전시킵니다.
    /// </summary>
    void Update()
    {
        // 목표(target)가 설정되어 있는지 확인합니다.
        if (target != null)
        {
            // 1. 이동: 현재 위치에서 target의 위치까지 일정한 속도(speed)로 이동합니다.
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // 2. 회전: Quaternion을 사용하여 정교하게 방향을 제어합니다.
            // 목표 지점까지의 거리가 매우 가깝지 않을 때만 회전을 계산합니다. (오류 방지)
            if (Vector3.SqrMagnitude(target.position - transform.position) > 0.01f)
            {
                // a. 훅에서 타겟을 향하는 방향 벡터를 계산합니다.
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                // b. 'modelForwardDirection'이 'directionToTarget'을 바라보도록 하는 회전값을 계산합니다.
                // Quaternion.LookRotation은 기본적으로 Z축이 앞을 보도록 만들므로,
                // FromToRotation을 사용해 우리가 지정한 축(modelForwardDirection)이 타겟을 향하도록 보정합니다.
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget) * Quaternion.FromToRotation(Vector3.forward, modelForwardDirection);

                // c. 계산된 최종 회전값을 훅의 rotation에 적용합니다.
                transform.rotation = targetRotation;
            }
        }
    }
}