﻿using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using static Photon.Pun.PhotonAnimatorView;

public class PlayerMove : MonoBehaviourPunCallbacks
{
    public CinemachineCamera cineCam;

    public float moveSpeed = 2f;
    public float rotateSpeed = 180f;
    public float gravity = -9.81f;

    private PhotonView myPhotonView;
    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput;
    private Vector3 velocity;

    [SerializeField]
    private GameObject cameraPos;

    private GameObject playerModel;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        myPhotonView = GetComponent<PhotonView>();

        animator = GetComponentInChildren<Animator>();
        playerModel = animator.gameObject;

        SetCameraTarget(playerModel);

        GetComponent<PlayerInput>().enabled = false;

        NetworkManager.Instance.AddPlayer(gameObject);
    }

    public override void OnLeftRoom()
    {
        NetworkManager.Instance.RemovePlayer(gameObject);
        base.OnLeftRoom();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if(targetPlayer == PhotonNetwork.LocalPlayer && PhotonNetwork.LocalPlayer.NickName != "")
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    private void Update()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (myPhotonView.IsMine && animator != null)
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

    public void SetCameraTarget(GameObject _target)
    {
        playerModel = _target;

        // 플레이어 카메라 설정
        if (myPhotonView.IsMine)
        {
            // Cinemachine 카메라 가져오기
            cineCam = FindAnyObjectByType<CinemachineCamera>();

            // 카메라가 추적 대상 설정
            cineCam.Follow = playerModel.transform;
            cineCam.LookAt = playerModel.transform;
        }
        else
        {
            // 다른 플레이어의 카메라는 설정하지 않음 (자동으로 다른 플레이어의 카메라를 사용)
            if (cineCam != null) Destroy(cineCam.gameObject); // 다른 플레이어의 카메라는 없애버림
        }
    }
}