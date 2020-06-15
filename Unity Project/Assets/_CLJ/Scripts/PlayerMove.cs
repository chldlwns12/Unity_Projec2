using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float PlayerSpeed = 5.0f;

    CharacterController cc;

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
        cc.Move(dir * PlayerSpeed * Time.deltaTime);
    }
}
