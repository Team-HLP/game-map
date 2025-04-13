using UnityEngine;
using UnityEngine.InputSystem;

public class Moving : MonoBehaviour
{
    public float speed;     // 전진 속도
    public float turnSpeed = 25f; // 회전 속도
    public InputActionProperty leftMoveAction;   // 왼손 컨트롤러 입력
    public InputActionProperty rightMoveAction;  // 오른손 컨트롤러 입력
    void Start()
    {
        speed = 8f;
    }
    void Update()
    {
        // 전진
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // 좌우 입력 값 가져오기
        Vector2 leftInput = leftMoveAction.action.ReadValue<Vector2>();
        Vector2 rightInput = rightMoveAction.action.ReadValue<Vector2>();

        // 둘 중 값이 더 큰 쪽 사용 (입력된 손만 적용됨)
        float horizontal = Mathf.Abs(rightInput.x) > Mathf.Abs(leftInput.x) ? rightInput.x : leftInput.x;

        // 좌우 이동
        transform.Translate(Vector3.right * horizontal * turnSpeed * Time.deltaTime);
    }
}