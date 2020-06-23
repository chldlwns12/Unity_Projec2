using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public GameObject fxEffect;
    public GameObject granadeFactory;
    public GameObject PointGranade;
    public float throwPower = 20.0f;

    public float damege = 10.0f;

    EnemeyFSM em;


    private void Start()
    {
        em = GameObject.Find("Enemy").GetComponent<EnemeyFSM>();
    }


    // Update is called once per frame
    void Update()
    {
        Fire();
    }

    private void Fire()
    {
        //마우스왼쪽버튼 클릭시 레이캐스트로 총알발사
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo;

            if(Physics.Raycast(ray, out hitInfo))
            {
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);

                GameObject spark = Instantiate(fxEffect, hitInfo.point + (hitInfo.normal * 0.01f), rot);

                em.hitDamage(damege);
            }

            //레이어 마스크 사용 충돌처리(최적화)
            //유니티 내부적으로 속도향상을 위해 비트연산 처리가 된다
            //총 32비트를 사용하기 때문에 레이어도 32개까지 추가가능함
            //int layer = gameObject.layer;
            //layer = 1 << 8;
            //0000 0000 0000 0001 => 0000 0001 0000 0000
            //layer = 1 << 8 | 1 << 9 | 1 << 12;
            //0000 0001 0000 0000 => Player
            //0000 0010 0000 0000 => Enemey
            //0001 0000 0000 0000 => Boss
            //0001 0011 0000 0000 => P, E, B 모두 다 충돌처리
            //if (Physics.Raycast(ray, out hitInfo, 100, layer))    //layer만 충돌
            //if (Physics.Raycast(ray, out hitInfo, 100, ~layer))     //layer만 충돌제외
            //{
            //    //if문이 많이 들어가면
            //    //성능이 조금이라도 떨어질 수밖에 없다
            //    //비트연산은 성능 최적화에 도움이 된다.
            //}
        }
        //마우스우측버튼 클릭시 폭탄투척 하기
        if(Input.GetMouseButtonDown(1))
        {
            GameObject granade = Instantiate(granadeFactory);
            granade.transform.position = PointGranade.transform.position;
            Rigidbody rb = granade.GetComponent<Rigidbody>();
            //전방으로 물리적인 힘을 가한다
            //rb.AddForce(Camera.main.transform.forward * throwPower, ForceMode.Impulse);
            //ForceMode.Acceleration => 연속적인 힘을 가한다 (질량의 영향을 안받음)
            //ForceMode.Force => 연속적인 힘을 가한다 (질량의 영향을 받음)
            //ForceMode.Impulse => 순간적인 힘을가한다 (질량의 영향을 받음)
            //ForceMode.VelocityChange => 순간적인 힘을 가한다 (질량의 영향을 안받음)

            //45도 각도로 발사
            //각도로 주려면 어떻게 해야 할까? (벡터의 덧셈)
            Vector3 dir = Camera.main.transform.forward + Camera.main.transform.up;
            dir.Normalize();
            rb.AddForce(dir * throwPower, ForceMode.Impulse);
        }
    }
}
