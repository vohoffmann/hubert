using System;
using UnityEngine;

namespace Hubert.Units.Control
{
    public class RegisterAsSelectable : MonoBehaviour
    {
        void Start()
        {
            UnitSelections.Instance.unitList.Add(gameObject);
        }

        private void OnDestroy()
        {
	        try
	        {
		        UnitSelections.Instance.unitList.Remove(gameObject);
	        }
	        catch (Exception e)
	        {
		        Console.WriteLine(e);
	        }
        }
    }
}