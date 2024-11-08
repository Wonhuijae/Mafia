﻿using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotateSpeed = 180f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponentInChildren<CharacterController>();
    }

    private void Update()
    {
        if (moveInput != Vector2.zero)
        {
            // 이동
            Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
            move = move.normalized;
            Vector3 moveDirection = /*transform.right * moveInput.x + */transform.forward * moveInput.y;
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // 회전
            float turn = moveInput.x * Time.deltaTime * rotateSpeed ;
            transform.Rotate(Vector3.up * turn);
        }

        // 중력
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>(); 
    }
}