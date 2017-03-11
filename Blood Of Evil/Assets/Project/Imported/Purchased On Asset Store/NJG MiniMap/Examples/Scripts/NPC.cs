using NJG;
using UnityEngine;
using System.Collections;

namespace NJG
{

    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class NPC : MonoBehaviour
    {
        public float wanderRadius = 25.0f;
        public float walkSpeed = 10f;
        public float idleTime = 1f;

        /// <summary>
        /// Cache transform for speed.
        /// </summary>
        public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

        float mLastSpeed;
        Transform mTrans;
        UnityEngine.AI.NavMeshAgent mNav;
        Vector3 wanderDestination;
        float mIdleTime;

        void Awake() 
        { 
            mNav = GetComponent<UnityEngine.AI.NavMeshAgent>();
            
        }

        void Update()
        {
            if (mLastSpeed != walkSpeed)
            {
                mNav.speed = mLastSpeed = walkSpeed;
            }

            if (!mNav.hasPath && Time.time > mIdleTime && mNav.remainingDistance <= mNav.stoppingDistance || mNav.remainingDistance > wanderRadius)
            {
                Wander();
                mIdleTime = Time.time + idleTime;
            }
        }

        void Wander()
        {
            if (mNav.enabled)
            {
                Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
                randomDirection += cachedTransform.position;
                UnityEngine.AI.NavMeshHit hit;
                UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
                wanderDestination = hit.position;
                mNav.SetDestination(wanderDestination);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(cachedTransform.position, wanderRadius);

            Color c = Color.cyan;
            c.a = 0.5f;
            Gizmos.color = c;
            Gizmos.DrawSphere(cachedTransform.position, wanderRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(cachedTransform.position, wanderDestination);
            Gizmos.DrawSphere(wanderDestination, 2);
        }
    }
}
