using UnityEngine;
using UnityEngine.InputSystem;

public class InputControlManager : MonoBehaviour
{
    // 텍스트 입력 시 플레이어 이동 활성화/비활성화 관리
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        UISetting.OnInputTextStart += OnInputStart;
        UISetting.OnInputTextEnd += OnInputEnd;
    }

    private void OnDisable()
    {
        UISetting.OnInputTextStart -= OnInputStart;
        UISetting.OnInputTextEnd -= OnInputEnd;
    }

    private void OnInputStart()
    {
        playerInput.enabled = false;
    }

    private void OnInputEnd()
    {
        playerInput.enabled = true;
    }
}
