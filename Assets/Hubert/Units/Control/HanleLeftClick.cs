using UnityEngine;

namespace Hubert.Units.Control
{
    public class HanleLeftClick : MonoBehaviour
    {
        private Camera _camera;

        public LayerMask selectable;
        public LayerMask structure;

        void Start()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            // click left mouse button
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray        ray = _camera.ScreenPointToRay(Input.mousePosition);

                // left click on structure
                if(Physics.Raycast(ray , out hit, Mathf.Infinity, structure))
                {
                    // deselect all units
                    UnitSelections.Instance.DeselectAll();
                    
                    // update buttons in hud
                    
                }
                // check if click is on selectable object
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectable))
                {
                    // multiselect with shift key pressed
                    if (Input.GetKey(KeyCode.LeftControl))
                    {
                        UnitSelections.Instance.STRGClickSelect(hit.collider.gameObject);
                    }
                    else
                    {
                        UnitSelections.Instance.ClickSelect(hit.collider.gameObject);
                    }
                }
                else
                {
                    // left click on NOT selectable
                    if (!Input.GetKey(KeyCode.LeftControl))
                    {
                        UnitSelections.Instance.DeselectAll();
                    }
                }
            }
        }
    }
}