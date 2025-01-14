using System.Collections.Generic;
using UnityEngine;

namespace Hubert.Units.Control
{
    public class UnitSelections : MonoBehaviour
    {
        public List<GameObject> unitList = new();
        public List<GameObject> unitsSelected = new();
        private static UnitSelections _instance;
        public static UnitSelections Instance => _instance;

        private void Awake()
        {
            if (_instance != null &&
                _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        public void ClickSelect(GameObject unitToAdd)
        {
            DeselectAll();
            unitsSelected.Add(unitToAdd);
            unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
            unitToAdd.GetComponent<UnitInteraction>().enabled = true;
        }

        public void STRGClickSelect(GameObject unitToAdd)
        {
            if (!unitsSelected.Contains(unitToAdd))
            {
                unitsSelected.Add(unitToAdd);
                unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
                unitToAdd.GetComponent<UnitInteraction>().enabled = true;
            }
            else
            {
                unitsSelected.Remove(unitToAdd);
                unitToAdd.transform.GetChild(0).gameObject.SetActive(false);
                unitToAdd.GetComponent<UnitInteraction>().enabled = false;
            }
        }

        public void DragSelect(GameObject unitToAdd)
        {
            if (!unitsSelected.Contains(unitToAdd))
            {
                unitsSelected.Add(unitToAdd);
                unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
                unitToAdd.GetComponent<UnitInteraction>().enabled = true;
            }
        }

        public void DeselectAll()
        {
            foreach (var unit in unitsSelected)
            {
                unit.GetComponent<UnitInteraction>().enabled = false;
                unit.transform.GetChild(0).gameObject.SetActive(false);
            }

            unitsSelected.Clear();
        }
    }
}