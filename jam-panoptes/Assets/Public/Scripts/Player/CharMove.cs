using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMove : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;
    private CharacterController cc;
    private Vector3 velocity = new Vector3();
    private Vector2 input;
    private Transform cam; 
    private Vector3 orientation;
    public Transform player;
    private Animator animator;

    private void Awake() {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveChar();

        Debug.Log(velocity);
    }

    private void MoveChar(){
        float dt = Time.deltaTime;
        velocity = new Vector3(0, cc.isGrounded ? 0 : velocity.y, 0);
        velocity += Physics.gravity * dt;

        input = InputManager.Instance.RawMovementDirection;

        velocity += input.y * cam.forward * dt * moveSpeed;
        velocity += input.x * cam.right * dt * moveSpeed;

        cc.Move(velocity);
        bool pressInput = input.x != 0 || input.y != 0;
        animator.SetBool("isRunning", pressInput);
        if(pressInput){
            player.rotation = Quaternion.RotateTowards(player.rotation, Quaternion.Euler(new Vector3(0 , Vector3.SignedAngle(transform.forward, cc.velocity, Vector3.up), 0)), rotateSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos() {
        if(cc)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + cc.velocity);
        }
    }

    public void SetInTurret(bool next)
    {
        animator.SetBool("inTurret", next);
    }
}
