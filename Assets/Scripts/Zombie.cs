﻿using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드 가져오기

// 좀비 AI 구현
public class Zombie : LivingEntity
{
    public LayerMask whatIsTarget;          // 추적 대상 레이어

    private LivingEntity targetEntity;      // 추적 대상
    private NavMeshAgent navMeshAgent;      // 경로 계산 AI 에이전트

    public ParticleSystem hitEffect;        // 피격 시 재생할 파티클 효과
    public AudioClip deathSound;            // 사망 시 재생할 소리
    public AudioClip hitSound;              // 피격 시 재생할 소리

    private Animator zombieAnimator;        // 애니메이터 컴포넌트
    private AudioSource zombieAudioPlayer;  // 오디오 소스 컴포넌트
    private Renderer zombieRenderer;        // 렌더러 컴포넌트

    public float damage = 20f;              // 공격력
    public float timeBetAttack = 0.5f;      // 공격 간격
    private float lastAttackTime;           // 마지막 공격 시점

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.isDead)
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    // 초기화
    private void Awake() {
        
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();

        zombieRenderer = GetComponentInChildren<Renderer>();
    }

    // 좀비 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(ZombieData zombieData) {

        startingHealth = zombieData.health;
        health = zombieData.health;

        damage = zombieData.damage;

        //navMeshAgent.speed = zombieData.speed;
        navMeshAgent.speed = 0.5f;

        zombieRenderer.material.color = zombieData.skinColor;
    }

    private void Start() {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    private void Update() {
        // 추적 대상의 존재 여부에 따라 다른 애니메이션 재생
        zombieAnimator.SetBool("HasTarget", hasTarget);
    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로 갱신
    private IEnumerator UpdatePath() {
        // 살아 있는 동안 무한 루프
        while (!isDead)
        {
            if (hasTarget)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else
            {
                navMeshAgent.isStopped = true;

                // 20유닛의 반지름을 가진 가상의 구를 그렸을 때 구와 겹치는 모든 콜라이더를 가져옴
                // whatIsTarget 레이어를 가진 콜라이더만 가져오도록 필터링
                Collider[] colliders = Physics.OverlapSphere(transform.position, 20f, whatIsTarget);
                for(int i = 0; i < colliders.Length; ++i)
                {
                    LivingEntity livingEntity = colliders[i].GetComponent<LivingEntity>();
                    if(null != livingEntity && !livingEntity.isDead)
                    {
                        targetEntity = livingEntity;
                        break;
                    }
                }
            }

            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    // 데미지를 입었을 때 실행할 처리
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal) {

        if (!isDead) 
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            zombieAudioPlayer.PlayOneShot(hitSound);
        }
        
        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        Collider[] zombieColliders = GetComponents<Collider>();
        for(int i = 0; i < zombieColliders.Length; ++i)
            zombieColliders[i].enabled = false;

        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;

        zombieAnimator.SetTrigger("Die");
        zombieAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other) {
        // 트리거 충돌한 상대방 게임 오브젝트가 추적 대상이라면 공격 실행
        if(!isDead && lastAttackTime + timeBetAttack <= Time.time)
        {
            LivingEntity attackTarget = other.GetComponent<LivingEntity>();
            if(null != attackTarget && targetEntity == attackTarget)
            {
                lastAttackTime = Time.time;

                Vector3 hitPoint = other.ClosestPoint(transform.position);
                Vector3 hitNormal = transform.position - other.transform.position;

                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}