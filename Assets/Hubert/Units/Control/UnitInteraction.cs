using UnityEngine;
using UnityEngine.AI;

namespace Hubert.Units.Control
{
    public class UnitInteraction : MonoBehaviour
    {
        public LayerMask ground;
        public LayerMask interactable;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Camera _camera;
        [SerializeField] private UnitBrain _unitBrain;

        private void Update()
        {
            // right click
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                // wenn object 'auf' dem layer interactable
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactable))
                {
                    _unitBrain.SetTarget(hit.transform.gameObject);
                }
                else if (Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
                {
                    if (_agent)
                    {
                        _agent.SetDestination(hit.point);
                        _unitBrain.SetTarget(null);
                    }
                    else
                    {
                        print("es ist kein NavMeshAgend gesetzt !!!");
                    }
                }
            }
        }
    }
}