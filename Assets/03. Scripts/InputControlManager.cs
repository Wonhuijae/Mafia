using UnityEngine;
using UnityEngine.InputSystem;

public class InputControlManager : MonoBehaviour
{
    // �ؽ�Ʈ �Է� �� �÷��̾� �̵� Ȱ��ȭ/��Ȱ��ȭ ����
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
