using UnityEngine;
using UnityEngine.AI;

namespace Hubert.Units
{
    public class UnitBrain : MonoBehaviour
    {
        public UnitType unitType;
        public State _state;
        private GameObject _unloadingPoint;
        private ResourceType _resourceType;
        private float _gatheringStartTime;
        private NavMeshAgent _agent;

        [SerializeField] private int _resources;
        [SerializeField] private GameObject target;

        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _state = State.IDLE;
        }

        void Update()
        {
            // animation
            if (!_agent)
            {
                print("ACHTUNG !!!!");
                return;
            }

            // entfernung zum ziel ( agent ) ermitteln
            float distance = Vector3.Distance(transform.position, _agent.destination);

            if (distance > .5f &&
                _state == State.IDLE)
            {
                setState(State.WALKING);
            }

            if (!target)
            {
                if (_state is State.WALKING or State.RUNNING &&
                    distance < 0.1f)
                {
                    setState(State.IDLE);
                }

                return;
            }

            // handle worker
            if (unitType == UnitType.WORKER)
            {
                if (_state == State.GATHERING)
                {
                    // genug resourcen gesammelt ?
                    // dann zum abgabepunkt und abgeben
                    if (Time.time - _gatheringStartTime > 5 ||
                        _resources == 10)
                    {
                        _resources = 10;
                        setState(State.RETURNINGGOODS);
                        updateAgentDestination();
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_state != State.RETURNINGGOODS &&
                target &&
                other.gameObject == target)
            {
                _agent.ResetPath();
                _agent.angularSpeed = 0;
                setState(State.GATHERING);
                _gatheringStartTime = Time.time;
                transform.rotation =
                    Quaternion.LookRotation(
                        other.transform.position - transform.position,
                        new Vector3(0, 1, 0)
                    );

                return;
            }

            if (_state == State.RETURNINGGOODS &&
                other.gameObject == _unloadingPoint)
            {
                // resourcen entladen ... 
                _resources = 0;
                setState(State.IDLE);
                if (target)
                {
                    updateAgentDestination();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            _agent.isStopped = false;
            _agent.angularSpeed = 2500;
        }

        private void setState(State state)
        {
            _state = state;
        }

        public void SetTarget(GameObject pTarget)
        {
            // wenn parameter null, dann taget löschen 
            if (!pTarget)
            {
                target = pTarget;
                setState(State.IDLE);
                return;
            }

            if (target != pTarget)
            {
                target = pTarget;
                setState(State.IDLE);

                if (unitType == UnitType.WORKER)
                {
                    switch (pTarget.tag)
                    {
                        case "wasser":
                            _unloadingPoint = GameObject.Find("Townhall");
                            _resourceType = ResourceType.WATER;
                            break;
                        case "stone":
                            _unloadingPoint = GameObject.Find("Townhall");
                            _resourceType = ResourceType.GOLD;
                            break;
                        case "wood":
                            _unloadingPoint = GameObject.Find("Townhall");
                            _resourceType = ResourceType.WOOD;
                            break;
                    }

                    updateAgentDestination();
                }
                // else force ...
            }
        }

        private void updateAgentDestination()
        {
            // agent auf target setzen
            const float maxDist = 10f;

            Vector3 targetPosition = _state == State.RETURNINGGOODS
                ? _unloadingPoint.transform.position
                : target.transform.position;
            NavMesh.SamplePosition(
                targetPosition,
                out var myNavHit,
                maxDist,
                5 << NavMesh.GetAreaFromName("Walkable")
            );

            _agent.SetDestination(myNavHit.position);
        }
    }

    public enum State
    {
        IDLE = 0,
        WALKING = 1,
        RUNNING = 2,
        GATHERING = 3,
        RETURNINGGOODS = 4,
        FIGHTING = 5
    }

    public enum UnitType
    {
        WORKER = 0,
        INFANTRY = 1
    }

    public enum ResourceType
    {
        WATER = 0,
        WOOD = 1,
        GOLD = 2
    }
}