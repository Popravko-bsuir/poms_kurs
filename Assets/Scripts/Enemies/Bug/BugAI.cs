using Pathfinding;
using UnityEngine;

namespace Enemies.Bug
{
    public class BugAI : MonoBehaviour
    {
        public Rigidbody2D target;
        public float speed = 200f;
        public float nextWayPoyntDistance = 1f;

        private Path _path;
        private int _currentWaypoynt;
        private bool _isReachedEndOfPath;

        private Seeker seeker;
        [SerializeField] private Rigidbody2D rb;

        void Start()
        {
            seeker = FindObjectOfType<Seeker>();
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }

        void FixedUpdate()
        {
            if (_path == null)
            {
                return;
            }

            if (_currentWaypoynt >= _path.vectorPath.Count)
            {
                _isReachedEndOfPath = true;
                return;
            }

            _isReachedEndOfPath = false;

            Vector2 direction = ((Vector2) _path.vectorPath[_currentWaypoynt] - rb.position).normalized;
            Vector2 force = direction * (speed * Time.deltaTime);
            rb.AddForce(force);

            float distance = Vector2.Distance(rb.position, _path.vectorPath[_currentWaypoynt]);

            if (distance < nextWayPoyntDistance)
            {
                _currentWaypoynt++;
            }
        }

        void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                _path = p;
                _currentWaypoynt = 0;
            }
        }
    }
}