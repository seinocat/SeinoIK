using System;
using Cinemachine;
using UnityEngine;

namespace Tests.Scripts
{
    public class CharacterController : MonoBehaviour
    {
        public Animator Animator;
        public CinemachineVirtualCamera Camera;
        public float Speed;
        public Vector3 Dir;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                this.Dir = Vector3.forward;
                this.Speed = 5f;
                Animator.SetFloat(Animator.StringToHash("Speed"), 5f);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                this.Dir = Vector3.back;
                this.Speed = -5f;
                Animator.SetFloat(Animator.StringToHash("Speed"), 5f);
            }
            
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
            {
                
                this.Speed = 0f;
                Animator.SetFloat(Animator.StringToHash("Speed"), 0f);
            }

            if (Input.GetKey(KeyCode.A))
            {
                this.Dir = Vector3.left;
            }
            
            if (Input.GetKey(KeyCode.D))
            {
                this.Dir = Vector3.right;
            }
            
            if (this.Speed > 0)
            {
                this.transform.position += this.Dir * Speed * Time.deltaTime * 0.2f;
                this.transform.rotation *= Quaternion.LookRotation(this.Dir, this.transform.up);
            }

        }
    }
}