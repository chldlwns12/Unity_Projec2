﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//몬스터 유한상태머신
public class EnemeyFSM : MonoBehaviour
{
    //몬스터 상태 이넘문
    enum EnemyState
    {
        Idle, Move, Attack, Return, Dameged, Die
    }

    EnemyState state;   //몬스터 상태변수
    EnemyState saveState;

    Transform player;
    Transform spawnPoint;

    //애니메이션 제어하기 위한 애니메이터 컴포넌트
    Animator anim;
    //Quaternion startRotation;   //몬스터 시작회전값

    //네비게이션
    NavMeshAgent nav;

    //유용한 기능
    #region "Idle 상태에 필요한 변수들"
    public float searchRange = 20.0f;
    #endregion

    #region "Move 상태에 필요한 변수들"
    public float attackRange = 4.0f;
    public float EnemySpeed = 5.0f;
    #endregion

    #region "Attack 상태에 필요한 변수들"
    //PlayerMove pm = GameObject.Find("Player").GetComponent<PlayerMove>();
    public float attackCount = 2.0f;
    float currentCount = 0.0f;
    #endregion

    #region "Return 상태에 필요한 변수들"
    public float returnRange = 0.5f;
    #endregion

    #region "Dameged 상태에 필요한 변수들"
    public float enemyHp = 100.0f;
    float currentHp = 100.0f;
    #endregion

    #region "Die 상태에 필요한 변수들"
    #endregion

    void Start()
    {
        //몬스터 상태 초기화
        state = EnemyState.Idle;
        //startRotation = transform.rotation;
        player = GameObject.Find("Player").transform;
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //상태에 따른 행동처리
        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Dameged:
                Dameged();
                break;
            case EnemyState.Die:
                Die();
                break;

        }
    }

    private void Idle()
    {
        //1. 플레이어와 일정범위가 되면 이동상태로 변경 (탐지범위)
        //- 플레이어 찾기 (GameObject.Find("Player"))
        //- 일정거리 20미터 (Distance 등)
        //- 상태변경 (state = EnemyState.Move)
        //- 상태전환 출력
        if(Vector3.Distance(player.position, transform.position) < searchRange)
        {
            state = EnemyState.Move;
            print("State : Move");

            //애니메이션
            anim.SetTrigger("Move");
        }
    }

    private void Move()
    {
        //1. 플레이어를 향해 이동 후 공격범위 안에 들어오면 공격상태로 변경
        //2. 플레이어를 추격하더라도 처음위치에서 일정범위를 넘어가면 리턴상태로 돌아오기
        //- 플레이어 처럼 캐릭터컨트롤러를 이용하기
        //- 공격범위 2미터
        //- 상태변경
        //- 상태전환 출력
        //transform.LookAt(player);
        //Vector3 dir = new Vector3(0, 0, 1);
        //transform.Translate(dir * EnemySpeed * Time.deltaTime);
        nav.SetDestination(player.position);

        if (Vector3.Distance(player.position, transform.position) < attackRange)
        {
            state = EnemyState.Attack;
            print("State : Attack");
            anim.SetTrigger("Attack");
        }

        if (Vector3.Distance(player.position, transform.position) > searchRange || 
            Vector3.Distance(spawnPoint.position, transform.position) > 30.0f)
        {
            state = EnemyState.Return;
            print("State : Return");
            anim.SetTrigger("Return");
        }
    }

    private void Attack()
    {
        //1. 플레이어가 공격범위 안에 있다면 일정한 시간 간격으로 플레이어 공격
        //2. 플레이어가 공격범위를 벗어나면 이동상태(재추격)로 변경
        //- 공격범위 1미터
        //- 상태변경
        //- 상태전환 출력
        currentCount += Time.deltaTime;

        if(currentCount > attackCount)
        {
            anim.SetTrigger("Attack");
            print("공격!");
            currentCount = 0f;
        }
        if (Vector3.Distance(player.position, transform.position) > attackRange)
        {
            state = EnemyState.Move;
            currentCount = 0f;
            print("State : Move");
            anim.SetTrigger("Move");
        }
    }

    private void Return()
    {
        //1. 몬스터가 플레이어를 추격하더라도 처음 위치에서 일정범위를 벗어나면 다시 돌아옴
        //- 처음위치에서 일정범위 30미터
        //- 상태변경
        //- 상태전환 출력
        //transform.LookAt(spawnPoint.transform);
        //Vector3 dir = new Vector3(0, 0, 1);
        //transform.Translate(dir * EnemySpeed * Time.deltaTime);
        nav.SetDestination(spawnPoint.position);

        //float distace = Vector3.Distance(player.position, transform.position);
        if (Vector3.Distance(spawnPoint.position, transform.position) < returnRange)
        {
            state = EnemyState.Idle;
            print("State : Idle");
            anim.SetTrigger("Idle");
            transform.rotation = Quaternion.identity;
            //Quaternion.identity => 회전값을 0으로 바꿔준다.
            transform.position = spawnPoint.position;
        }
        //if (distace < searchRange)
        //{
        //    state = EnemyState.Move;
        //    print("State : Move");
        //}
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentHp > 0)
        {
            print("State : Dameged");
            saveState = state;
            state = EnemyState.Dameged;
        }
        else if(currentHp <= 0)
        {
            print("State : Die");
            state = EnemyState.Die;
            StopAllCoroutines();
        }
    }

    //피격상태 (Any State)
    private void Dameged()
    {
        //코루틴을 사용하자
        //1. 몬스터 체력이 1이상
        //2. 다시 이전상태로 변경
        //- 상태변경
        //- 상태전환 출력
        StartCoroutine(EnemyDameged());
    }

    IEnumerator EnemyDameged()
    {
        //currentHp -= 6.0f;

        print("State : Dameged");
        print("Hp 100 / " + currentHp);
        yield return new WaitForSeconds(1.5f);
        state = saveState;
        print("State : " + state.ToString());
        switch (saveState)
        {
            case EnemyState.Idle:
                anim.SetTrigger("Idle");
                break;
            case EnemyState.Move:
                anim.SetTrigger("Move");
                break;
            case EnemyState.Attack:
                anim.SetTrigger("Attack");
                break;
            case EnemyState.Return:
                anim.SetTrigger("Return");
                break;
        }

    }

    //죽음상태 (Any State)
    private void Die()
    {
        //코루틴을 사용하자
        //1. 체력이 0이하
        //2. 몬스터 오브젝트 삭제
        //- 상태변경
        //- 상태전환 출력 (죽었다)
        
        StartCoroutine(EnemyDie());
    }

    IEnumerator EnemyDie()
    {
        //Destroy(gameObject);
        yield return new WaitForSeconds(0.3f);
    }


    public void hitDamage(float value)
    {
        //적체력 -= value;

        if (state == EnemyState.Dameged || state == EnemyState.Die) return;

        currentHp -= value;
        
        if (currentHp > 0)
        {
            print("State : Dameged");
            saveState = state;
            state = EnemyState.Dameged;
            anim.SetTrigger("Dameged");
        }
        else if (currentHp <= 0)
        {
            print("State : Die");
            state = EnemyState.Die;
            anim.SetTrigger("Die");
            StopAllCoroutines();
        }
    }
}
