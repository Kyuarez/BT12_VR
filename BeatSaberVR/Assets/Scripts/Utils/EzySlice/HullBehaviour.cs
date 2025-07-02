using System.Collections;
using UnityEngine;

namespace BeatSabor
{
    public class HullBehaviour : MonoBehaviour
    {
        public Vector3 velocity;
        public Vector3 normal;
        public float deceleration = 1f;
        public float rotationSpeed = 1f;
        private Transform _cam;

        IEnumerator Start()
        {
            _cam = Camera.main.transform;

            while (true)
            {
                float dt = Time.deltaTime;
                transform.position += velocity * dt;
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * dt);

                if (velocity.magnitude <= 0.1f)
                {
                    Destroy(gameObject);
                }

                Vector3 toCam = (_cam.position - transform.position).normalized;
                Quaternion target = Quaternion.FromToRotation(normal, toCam) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * dt);
                yield return null;
            }
        }
    }
}