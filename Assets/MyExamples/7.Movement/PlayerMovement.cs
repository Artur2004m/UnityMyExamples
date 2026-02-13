using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExamples.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        public bool rotateTowardsCamera;
        public bool rotateTowardsInput;
        public float rotateSlerp;

        public Camera cam;
        public CharacterController cc;
        public Vector3 velocity;
        public float moveSpeed;
        public float gravityForce;
        public bool gravity;
        public float maxVelocity;

        public bool gizmos;
        public bool isGrounded;
        public float sphereCastRadius;
        public float sphereCastRange;
        public Vector3 sphereCastOffset;
        public LayerMask walkableLayer;
        private RaycastHit hitCached;
        private bool isHit;

        public Vector3 inputDirectionCached;
        void Update()
        {
            Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            inputDirectionCached = inputDir;
            Vector3 cameraForward = cam.transform.forward;
            cameraForward.y = 0f;
            cameraForward.Normalize();
            Vector3 cameraRight = cam.transform.right;
            cameraRight.y = 0f;
            cameraRight.Normalize();
            Vector3 moveDir = inputDir.z * cameraForward + inputDir.x * cameraRight;
            moveDir.Normalize();

            RaycastHit hit;
            if (Physics.SphereCast(transform.position + sphereCastOffset, sphereCastRadius, -transform.up, out hit, sphereCastRange, walkableLayer))
            {
                isHit = true;
                isGrounded = true;
                hitCached = hit;
            }
            else
            {
                isHit = false;
                isGrounded = false;
                hitCached = new RaycastHit();
            }

            velocity += moveDir * moveSpeed;

            if (!isGrounded && gravity)
            {
                velocity += Vector3.down * gravityForce * Time.deltaTime;
            }

            Vector3 horizontal = new Vector3(velocity.x, 0, velocity.z);
            horizontal = Vector3.ClampMagnitude(horizontal, maxVelocity);
            velocity = new Vector3(horizontal.x, velocity.y, horizontal.z);

            if (inputDir.magnitude <= 0f)
            {
                velocity = new Vector3(0f, velocity.y, 0f);
            }

            cc.Move(velocity * Time.deltaTime);
            Rotate();
        }

        private void OnDrawGizmos()
        {
            if (!gizmos) return;

            Vector3 origin = transform.position + sphereCastOffset;
            Gizmos.DrawWireSphere(origin, sphereCastRange);

            if (isHit)
            {
                Gizmos.color = Color.green;
                Vector3 sphereCastMidpoint = origin + (-transform.up * hitCached.distance);
                Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
                Gizmos.DrawSphere(hitCached.point, 0.1f);
                Debug.DrawLine(origin, sphereCastMidpoint, Color.green);
            }
            else
            {
                Gizmos.color = Color.red;
                Vector3 sphereCastMidpoint = origin + (-transform.up * (sphereCastRange - sphereCastRadius));
                Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
                Debug.DrawLine(origin, sphereCastMidpoint, Color.red);
            }
        }

        private void Rotate()
        {
            // Если поворот к камере отключён, ничего не делаем
            if (!rotateTowardsCamera) return;

            Quaternion targetRotation;

            // Если нужно поворачиваться к направлению ввода и ввод есть
            if (rotateTowardsInput && inputDirectionCached != Vector3.zero)
            {
                // Преобразуем ввод из локального пространства камеры в мировое
                // Предполагаем, что inputDirectionCached — это вектор (horizontal, 0, vertical)
                Vector3 worldInput = cam.transform.right * inputDirectionCached.x
                                    + cam.transform.forward * inputDirectionCached.z;
                worldInput.y = 0f;

                // Если после обнуления Y вектор не нулевой, задаём поворот
                if (worldInput != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(worldInput.normalized);
                }
                else
                {
                    // Если вектор обнулился (например, камера смотрит строго вверх/вниз),
                    // оставляем текущий поворот
                    targetRotation = cc.transform.rotation;
                }
            }
            else
            {
                // Иначе поворачиваемся в сторону камеры
                Vector3 cameraForward = cam.transform.forward;
                cameraForward.y = 0f;

                if (cameraForward != Vector3.zero)
                {
                    targetRotation = Quaternion.LookRotation(cameraForward);
                }
                else
                {
                    targetRotation = cc.transform.rotation;
                }
            }

            // Плавно поворачиваем персонажа к целевой ориентации
            cc.transform.rotation = Quaternion.Slerp(cc.transform.rotation, targetRotation, rotateSlerp);
        }

    }


}
