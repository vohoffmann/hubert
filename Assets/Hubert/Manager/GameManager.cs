using UnityEngine;

namespace Src.Manager
{
    public class GameManager : MonoBehaviour
    {
        // instance (init in (Awake)
        public static GameManager Instance;

        public int Gold { get; private set; }
        public int Stones { get; private set; }
        public int Wood { get; private set; }
        public int Iron { get; private set; }
        public int Workers { get; private set; }

        private void Awake()
        {
            /*
             * make GameManager singleton
             */
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// set resolution independent to project settings to avoid do whole setup horror :)
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void OnRuntimeMethodLoad()
        {
            Screen.SetResolution(1280, 720, false);
        }

        public void TestButton()
        {
            var random = new System.Random();
            
            EventManager.UpdateGold.Invoke(random .Next(0, 100));
            EventManager.UpdateStones.Invoke(random .Next(0, 100));
            EventManager.UpdateWood.Invoke(random .Next(0, 100));
            EventManager.UpdateIron.Invoke(random .Next(0, 100));
            EventManager.UpdateWorkers.Invoke(random .Next(0, 100));
        }

        /// <summary>
        /// bind the actions to methods
        /// </summary>
        private void OnEnable()
        {
            EventManager.UpdateGold += HandleUpdateGold;
            EventManager.UpdateStones += HandleUpdateStones;
            EventManager.UpdateWood += HandleUpdateWood;
            EventManager.UpdateIron += HandleUpdateIron;
            EventManager.UpdateWorkers += HandleUpdateWorkers;

#if UNITY_EDITOR
            Application.logMessageReceived += HandleException;
#endif
        }

        private void HandleUpdateGold(int value)
        {
            Gold += value;
            EventManager.GoldUpdated?.Invoke();
        }

        private void HandleUpdateStones(int value)
        {
            Stones += value;
            EventManager.StonesUpdated?.Invoke();
        }

        private void HandleUpdateWood(int value)
        {
            Wood += value;
            EventManager.WoodUpdated?.Invoke();
        }

        private void HandleUpdateIron(int value)
        {
            Iron += value;
            EventManager.IronUpdated?.Invoke();
        }

        private void HandleUpdateWorkers(int value)
        {
            Workers += value;
            EventManager.WorkersUpdated?.Invoke();
        }

        private void OnDisable()
        {
            EventManager.UpdateGold -= HandleUpdateGold;
            EventManager.UpdateStones -= HandleUpdateStones;
            EventManager.UpdateWood -= HandleUpdateWood;
            EventManager.UpdateIron -= HandleUpdateIron;
            EventManager.UpdateWorkers -= HandleUpdateWorkers;

#if UNITY_EDITOR
            Application.logMessageReceived -= HandleException;
#endif
        }

        /// <summary>
        /// log exception ... for now it is only for running the app in the editor
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void HandleException(string condition, string stackTrace,
            LogType type)
        {
            if (type == LogType.Exception)
                LogError("Exception: " + condition + "\n" + stackTrace);
        }

        private static void LogError(string message)
        {
#if UNITY_EDITOR
            Debug.LogError("-----------------------------------");
            Debug.LogError(message);
            Debug.LogError("-----------------------------------");
#endif
        }


        /// <summary>
        /// helper function to log only in editor
        /// </summary>
        /// <param name="text">text to log</param>
        public static void Log(string message)
        {
#if UNITY_EDITOR
            Debug.Log("-----------------------------------");
            Debug.Log(message);
            Debug.Log("-----------------------------------");
#endif
        }
    }
}