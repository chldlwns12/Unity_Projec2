using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float PlayerSpeed = 5.0f;

    CharacterController cc;

    //중력적용
    public float gravity = -20;
    float velocityY;    //낙하속도(벨로시티는 방향과 힘을 들고 있다)
    float jumpPower = 10.0f;
    int jumpCount = 0;

    //public float PlayerHp = 100.0f;
    //public float cerruntHp = 100.0f;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = Vector3.right * h + Vector3.forward * v;

        //transform.Translate(dir.normalized * PlayerSpeed * Time.deltaTime);

        //카메라가 보는 방향으로 이동해야 한다
        dir = Camera.main.transform.TransformDirection(dir);
        transform.Translate(dir.normalized * PlayerSpeed * Time.deltaTime);

        //심각한 문제 : 하늘 날라다님, 땅 뚫음, 충돌처리 안됨
        //캐릭터컨트롤러 컴포넌트를 사용한다!!
        //캐릭터컨트롤러는 충돌감지만 하고 물리가 적용안된다
        //따라서 충돌감지를 하기 위해서는 반드시
        //캐릭터컨트롤러 컴포넌트가 제공해주는 함수로 이동처리해야 한다
        //cc.Move(dir * PlayerSpeed * Time.deltaTime);

        //중력적용하기
        

        //캐릭터 점프
        //점프버튼을 누르면 수직속도에 점프파워를 넣는다
        //땅에 닿으면 0으로 초기화
        //CollisionFlags.Above;
        //CollisionFlags.Below;
        //CollisionFlags.Sides;
        //if(cc.isGrounded)//땅에 닿았냐?
        //{
        //    velocityY = 0;
        //    jumpCount = 0;
        //}
        if (cc.collisionFlags == CollisionFlags.Below)
        {
            velocityY = 0;
            jumpCount = 0;
        }
        else
        {
            velocityY += gravity * Time.deltaTime;
            dir.y = velocityY;
        }
        if(Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            velocityY = jumpPower;
            jumpCount++;
        }
        cc.Move(dir * PlayerSpeed * Time.deltaTime);
    }
}
