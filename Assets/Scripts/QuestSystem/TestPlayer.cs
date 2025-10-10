using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f; // �÷��̾��� �̵� �ӵ��� �����մϴ�.

    private Rigidbody rb; // Rigidbody ������Ʈ�� ������ �����Դϴ�.

    void Start()
    {
        // ������ ���۵� �� Rigidbody ������Ʈ�� ã�Ƽ� rb ������ �Ҵ��մϴ�.
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. Ű���� �Է� �ޱ�
        float moveX = Input.GetAxis("Horizontal"); // A, D �Ǵ� �¿� ȭ��ǥ Ű �Է� (-1.0f ~ 1.0f)
        float moveZ = Input.GetAxis("Vertical");   // W, S �Ǵ� ���Ʒ� ȭ��ǥ Ű �Է� (-1.0f ~ 1.0f)

        // 2. �̵� ���� ���� ����
        Vector3 movement = new Vector3(moveX, 0f, moveZ);

        // 3. Rigidbody�� �̿��� �÷��̾� �̵���Ű��
        // Time.deltaTime�� ���� ������ �ӵ��� ������� ������ �ӵ��� �����̰� �մϴ�.
        rb.velocity = new Vector3(movement.x * moveSpeed, rb.velocity.y, movement.z * moveSpeed);
    }
}