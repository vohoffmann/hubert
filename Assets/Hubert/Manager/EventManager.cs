using System;
using UnityEngine;

namespace Hubert.Manager
{
    public class EventManager : MonoBehaviour
    {
        public static Action<int> UpdateStones;
        public static Action<int> UpdateGold;
        public static Action<int> UpdateIron;
        public static Action<int> UpdateWood;
        public static Action<int> UpdateWorkers;
        public static Action GoldUpdated;
        public static Action StonesUpdated;
        public static Action IronUpdated;
        public static Action WoodUpdated;
        public static Action WorkersUpdated;
    }
}