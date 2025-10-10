using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f; // 플레이어의 이동 속도를 조절합니다.

    private Rigidbody rb; // Rigidbody 컴포넌트를 참조할 변수입니다.

    void Start()
    {
        // 게임이 시작될 때 Rigidbody 컴포넌트를 찾아서 rb 변수에 할당합니다.
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. 키보드 입력 받기
        float moveX = Input.GetAxis("Horizontal"); // A, D 또는 좌우 화살표 키 입력 (-1.0f ~ 1.0f)
        float moveZ = Input.GetAxis("Vertical");   // W, S 또는 위아래 화살표 키 입력 (-1.0f ~ 1.0f)

        // 2. 이동 방향 벡터 생성
        Vector3 movement = new Vector3(moveX, 0f, moveZ);

        // 3. Rigidbody를 이용해 플레이어 이동시키기
        // Time.deltaTime을 곱해 프레임 속도와 관계없이 일정한 속도로 움직이게 합니다.
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
    }
}