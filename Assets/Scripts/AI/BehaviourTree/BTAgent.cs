using UnityEngine;
using UnityEngine.AI;

namespace EventBTree
{
    public class BTAgent : MonoBehaviour
    {
        [SerializeField]
        protected Transform head;
        public NavMeshAgent NavAgent { get; protected set; }
        public bool AgentFree
        {
            get
            {
                if (NavAgent.pathPending)
                {
                    print("Path status:" + NavAgent.pathPending);
                    return false;
                }
                if (NavAgent.velocity.magnitude > 0.1)
                {
                    return false;
                }
                if (Vector3.Distance(NavAgent.destination, transform.position) > NavAgent.stoppingDistance + NavAgent.baseOffset)
                {
                    return false;
                }
                return true;
            }
        }
        protected virtual void Awake()
        {
            NavAgent = GetComponent<NavMeshAgent>();
        }
        public virtual bool MoveToPoint(Vector3 point, bool rotate = true, float stoppingDistance = 0)
        {
            NavAgent.isStopped = false;
            NavAgent.updateRotation = rotate;
            NavAgent.SetDestination(point);
            return NavAgent.pathStatus == NavMeshPathStatus.PathComplete;
        }
        public virtual bool Wander(int nrOfTries = 10, float radius = 10)
        {
            Debug.Log("Wandering");
            while (nrOfTries != 0)
            {
                nrOfTries--;
                Vector3 randomDirection = Random.insideUnitCircle.normalized;
                randomDirection *= radius;
                randomDirection += transform.position;
                Debug.DrawLine(transform.position, randomDirection);
                NavMeshHit hit;
                Vector3 finalPosition = Vector3.zero;
                if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavAgent.areaMask))
                {
                    return MoveToPoint(hit.position);
                }
            }
            return false;
        }
        public virtual bool LookAt(Vector3 point)
        {
            transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
            if (head != null)
            {
                head.LookAt(point);
            }
            return true;
        }
        public virtual void Stop()
        {
            NavAgent.isStopped = true;
        }
        public virtual bool AttackMelee(float range, Transform target)
        {
            if (Vector3.Distance(transform.position, target.position) <= range)
            {
                Debug.Log($"{transform} attacking {target}");
                return true;
            }
            return false;
        }
    }
}
