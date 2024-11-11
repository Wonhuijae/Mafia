using Photon.Pun;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public CinemachineCamera cineCam;

    public float moveSpeed = 2f;
    public float rotateSpeed = 180f;
    public float gravity = -9.81f;

    private PhotonView photonView;
    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private Vector3 velocity;

    [SerializeField]
    private GameObject cameraPos;
    [SerializeField]
    private GameObject playerModel;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        
        photonView = GetComponent<PhotonView>();

        // 플레이어가 네트워크에 참여했을 때, 자신만의 카메라를 설정
        if (photonView.IsMine)
        {
            // Cinemachine 카메라 가져오기
            cineCam = FindAnyObjectByType<CinemachineCamera>();

            // 카메라가 따라갈 대상 설정 (자신의 모델을 따라가도록)
            cineCam.Follow = playerModel.transform;

            // 카메라가 바라볼 대상 설정 (자신의 모델을 바라보도록)
            cineCam.LookAt = playerModel.transform;
        }
        else
        {
            // 다른 플레이어의 카메라는 설정하지 않음 (자동으로 다른 플레이어의 카메라를 사용)
            if (cineCam != null) Destroy(cineCam.gameObject); // 다른 플레이어의 카메라는 없애버림
        }
    }

    private void Update()
    {
        if (photonView.IsMine && animator != null)
        {
            // 움직임 애니메이션
            float animMoveParam = moveInput.y;
            animator.SetFloat("Move", animMoveParam);

            float animTurnParam = moveInput.x;
            animator.SetFloat("Turn", animTurnParam);

            if (moveInput != Vector2.zero)
            {
                // 이동
                Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
                move = move.normalized;
                Vector3 moveDirection = transform.forward * moveInput.y;
                controller.Move(moveDirection * moveSpeed * Time.deltaTime);

                // 회전
                float turn = moveInput.x * Time.deltaTime * rotateSpeed;
                transform.Rotate(Vector3.up * turn);
            }

            // 중력
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void SetAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }
}