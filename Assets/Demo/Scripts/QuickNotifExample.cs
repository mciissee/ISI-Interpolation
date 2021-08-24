using UnityEngine;
using InfinityEngine.Interpolations;
using InfinityEngine;

public class QuickNotifExample : MonoBehaviour {

    private NotificationOptions options;
    public float entryDuration = 1.0f;
    public float closeDelay = 1.5f;
    public float scaleFactor = 1f;
    private Camera mainCamera;
    QuickNotification notif;

    private void Start () {
        options = new NotificationOptions();
        mainCamera = Camera.main;
	}
	
	void Update () {
        options.EntryDuration = entryDuration;
        options.CloseDelay = closeDelay;

        if (Input.GetMouseButtonDown(0))
        {
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            notif = QuickNotificationManager.CreateScalableNotication("Notification", options, scaleFactor);
            notif.TextColor = Infinity.RandomColor();
            notif.ShowNotification("Click", pos);
        }
    }
}
