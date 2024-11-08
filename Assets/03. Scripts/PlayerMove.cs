using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float speed = 2f;
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

        Debug.Log(moveInput);
        if (moveInput != Vector2.zero)
        {
            // 이동
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            controller.Move(move * speed * Time.deltaTime);

            // 회전
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.5f);
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