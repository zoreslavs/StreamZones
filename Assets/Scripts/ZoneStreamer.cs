using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ZoneStreamer : MonoBehaviour
{
    [System.Serializable]
    public class ZoneData
    {
        public string zoneName;
        public string address;
        public Vector3 center = Vector3.zero;
        public float enterRadius = 6f;
        public float exitRadius = 8f;

        [HideInInspector] public bool isLoaded;
        [HideInInspector] public bool isBusy;
        [HideInInspector] public AsyncOperationHandle<SceneInstance> handle;
    }

    [SerializeField] private Transform _target;
    [SerializeField] private PuzzleController _puzzleController;
    [SerializeField] private LoadingIndicator _loadingIndicator;
    [SerializeField] private List<ZoneData> _zones = new();
    [SerializeField] private bool _simulateSlowLoad;

    private void Start()
    {
        ShowLoadingIndicatorMessage("Use WASD or arrows to move.");
    }

    private void OnEnable()
    {
        _puzzleController.OnMoveEvent += OnPlayerMove;
    }

    private void OnDisable()
    {
        _puzzleController.OnMoveEvent -= OnPlayerMove;
    }

    private void OnPlayerMove(Vector2Int dir)
    {
        CheckZones();
    }

    private void CheckZones()
    {
        if (!_target || _zones == null || _zones.Count == 0)
            return;

        var targetPos = _target.position;

        foreach (var zone in _zones)
        {
            if (zone.isBusy)
                continue;

            var distSqr = (targetPos - zone.center).sqrMagnitude;
            var enterSqr = zone.enterRadius * zone.enterRadius;
            var exitSqr = zone.exitRadius * zone.exitRadius;

            if (!zone.isLoaded && distSqr <= enterSqr)
                _ = LoadZone(zone);
            else if (zone.isLoaded && distSqr >= exitSqr)
                _ = UnloadZone(zone);
        }
    }

    private async Task LoadZone(ZoneData zone)
    {
        zone.isBusy = true;

        try
        {
            ShowLoadingIndicatorMessage($"Loading {zone.zoneName}...");

            if (_simulateSlowLoad)
                await Task.Delay(500);

            Debug.Log($"[ZoneStreamer] Load start: {zone.zoneName}");
            var loadOperation = Addressables.LoadSceneAsync(zone.address, LoadSceneMode.Additive);
            zone.handle = loadOperation;
            await loadOperation.Task;

            if (loadOperation.Status == AsyncOperationStatus.Succeeded)
            {
                zone.isLoaded = true;
                Debug.Log($"[ZoneStreamer] Load done: {zone.zoneName}");
            }
            else
            {
                Debug.LogWarning($"[ZoneStreamer] Load failed: {zone.zoneName}");
            }
        }
        finally
        {
            HideLoadingIndicatorMessage();
            zone.isBusy = false;
        }
    }

    private async Task UnloadZone(ZoneData zone)
    {
        zone.isBusy = true;

        try
        {
            ShowLoadingIndicatorMessage($"Unloading {zone.zoneName}...");

            if (_simulateSlowLoad)
                await Task.Delay(200);

            Debug.Log($"[ZoneStreamer] Unload start: {zone.zoneName}");

            if (zone.isLoaded && zone.handle.IsValid())
            {
                var unloadOperation = Addressables.UnloadSceneAsync(zone.handle, true);
                await unloadOperation.Task;
            }
            else
            {
                var scene = zone.handle.IsValid() ? zone.handle.Result.Scene : SceneManager.GetSceneByName(zone.zoneName);
                if (scene.IsValid() && scene.isLoaded)
                {
                    var unloadOperation = SceneManager.UnloadSceneAsync(scene);
                    if (unloadOperation != null)
                    {
                        while (!unloadOperation.isDone)
                        {
                            await Task.Yield();
                        }
                    }
                }
            }

            zone.isLoaded = false;
            Debug.Log($"[ZoneStreamer] Unload done: {zone.zoneName}");
        }
        finally
        {
            HideLoadingIndicatorMessage();
            zone.isBusy = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (_zones == null)
            return;

        for (int i = 0; i < _zones.Count; i++)
        {
            var zone = _zones[i];
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(zone.center, zone.enterRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(zone.center, zone.exitRadius);
        }
    }

    private void ShowLoadingIndicatorMessage(string message)
    {
        if (_loadingIndicator != null)
            _loadingIndicator.Show(message);
    }

    private void HideLoadingIndicatorMessage()
    {
        if (_loadingIndicator != null)
            _loadingIndicator.Hide();
    }
}