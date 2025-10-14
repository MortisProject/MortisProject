// Assets/Scripts/Monster/Core/MonsterSpawnPoint.cs
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor; // Handles 클래스를 사용하기 위해 에디터 네임스페이스를 추가합니다.
#endif

namespace Monster
{
    /// <summary>
    /// 몬스터가 스폰될 위치와 종류(태그)를 정의하는 마커입니다.
    /// </summary>
    public class MonsterSpawnPoint : MonoBehaviour
    {
        [Header("스폰 설정")]
        [Tooltip("MonsterPoolManager에 등록된 몬스터 태그를 입력해야 합니다.")]
        public string monsterTag;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 스폰 포인트의 위치를 시각적으로 표시합니다.
            Gizmos.color = Color.red; // 빨간색
            Gizmos.DrawSphere(transform.position, 0.5f);
            
            // 몬스터 태그를 씬 뷰에 텍스트로 표시합니다.
            if (!string.IsNullOrEmpty(monsterTag))
            {
                Handles.color = Color.white;
                Handles.Label(transform.position + Vector3.up * 1f, $"Tag: {monsterTag}");
            }
        }
#endif
    }
}