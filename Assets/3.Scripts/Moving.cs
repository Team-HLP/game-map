using UnityEngine;
using UnityEngine.InputSystem;

public class Moving : MonoBehaviour
{
    public float speed;     // ���� �ӵ�
    public float turnSpeed = 25f; // ȸ�� �ӵ�
    public InputActionProperty leftMoveAction;   // �޼� ��Ʈ�ѷ� �Է�
    public InputActionProperty rightMoveAction;  // ������ ��Ʈ�ѷ� �Է�
    void Start()
    {
        speed = 20f;
    }
    void Update()
    {
        // ����
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // �¿� �Է� �� ��������
        Vector2 leftInput = leftMoveAction.action.ReadValue<Vector2>();
        Vector2 rightInput = rightMoveAction.action.ReadValue<Vector2>();

        // �� �� ���� �� ū �� ��� (�Էµ� �ո� �����)
        float horizontal = Mathf.Abs(rightInput.x) > Mathf.Abs(leftInput.x) ? rightInput.x : leftInput.x;

        // �¿� �̵�
        transform.Translate(Vector3.right * horizontal * turnSpeed * Time.deltaTime);
    }
}