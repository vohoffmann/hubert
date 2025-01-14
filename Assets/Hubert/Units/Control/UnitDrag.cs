using UnityEngine;

namespace Hubert.Units.Control
{
    public class UnitDrag : MonoBehaviour
    {
        private Camera _camera;

        [SerializeField]
        private RectTransform boxVisuals;

        private Rect selectionBox;

        private Vector2 _startPosition;
        private Vector2 _endPosition;

        void Start()
        {
            _camera = Camera.main;
            _startPosition = Vector2.zero;
            _endPosition = Vector2.zero;

            DrawVisual();
        }

        void Update()
        {
            // when clicked
            if (Input.GetMouseButtonDown(0))
            {
                _startPosition = Input.mousePosition;
                selectionBox = new Rect();
            }

            // when dragging
            if (Input.GetMouseButton(0))
            {
                _endPosition = Input.mousePosition;
                DrawVisual();
                DrawSelection();
            }

            // when release click
            if (Input.GetMouseButtonUp(0))
            {
                SelectUnits();
                _startPosition = Vector2.zero;
                _endPosition = Vector2.zero;
                DrawVisual();
            }
        }

        void DrawVisual()
        {
            Vector2 boxStart = _startPosition;
            Vector2 boxEnd   = _endPosition;

            Vector2 boxCenter = (boxStart + boxEnd) / 2;
            boxVisuals.position = boxCenter;

            Vector2 boxSize = new Vector2(Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y));

            boxVisuals.sizeDelta = boxSize;
        }

        void DrawSelection()
        {
            // x calculation
            if (Input.mousePosition.x < _startPosition.x)
            {
                // dragging left
                selectionBox.xMin = Input.mousePosition.x;
                selectionBox.xMax = _startPosition.x;
            }
            else
            {
                // dragging right
                selectionBox.xMin = _startPosition.x;
                selectionBox.xMax = Input.mousePosition.x;
            }

            // y calculation
            if (Input.mousePosition.y < _startPosition.y)
            {
                // dragging down
                selectionBox.yMin = Input.mousePosition.y;
                selectionBox.yMax = _startPosition.y;
            }
            else
            {
                // dragging up
                selectionBox.yMin = _startPosition.y;
                selectionBox.yMax = Input.mousePosition.y;
            }
        }

        void SelectUnits()
        {
            foreach (var unit in UnitSelections.Instance.unitList)
            {
                if (selectionBox.Contains(_camera.WorldToScreenPoint(unit.transform.position)))
                {
                    UnitSelections.Instance.DragSelect(unit);
                }
            }
        }
    }
}