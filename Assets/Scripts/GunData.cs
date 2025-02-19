using UnityEngine;

// 특성 : 어떤 클래스나 매서드, 변수 등에 대한 추가 정보를 제공하는 C# 문법
// 특성 이름을 대괄호([])로 묶고, 필요에 따라 추가 값을 전달
[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "Gun Data")]
public class GunData : ScriptableObject
{
    public AudioClip shotClip;          // 발사 소리
    public AudioClip reloadClip;        // 재장전 소리

    public float damage = 25;           // 공격력

    public int startAmmoRemain = 100;   // 처음에 주어질 전체 탄약
    public int magCapacity = 25;        // 탄창 용량

    public float timeBetFire = 0.12f;   // 총알 발사 간격
    public float reloadTime = 1.8f;     // 재장전 소요 시간
}