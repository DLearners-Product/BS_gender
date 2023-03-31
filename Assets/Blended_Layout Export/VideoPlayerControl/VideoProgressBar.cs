using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoProgressBar : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoLoading, spawnedLoader;
    public Camera Cam;
    public Image progress;
    double lastTimePlayed;
    float waitTime = 3f, 
            time = 0f;
    
    private void Awake()
    {
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        Check_Video_Buffering();
        if (videoPlayer.frameCount > 0)
            progress.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
    }

    private void Check_Video_Buffering(){
        if (videoPlayer.isPlaying && (Time.frameCount % (int)(videoPlayer.frameRate + 1)) == 0)
        {
            if (lastTimePlayed == videoPlayer.time)
            {
                // Debug.Log($"buffering");
                time += Time.deltaTime;
                if(spawnedLoader == null && time > waitTime)
                    spawnedLoader = Instantiate(videoLoading, gameObject.transform);
            }
            else
            {
                // Debug.Log($"not buffering");
                time = 0f;
                if(spawnedLoader != null)
                    Destroy(spawnedLoader);
            }
            lastTimePlayed = videoPlayer.time;
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        TrySkip(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TrySkip(eventData);
    }

    private void TrySkip(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            progress.rectTransform, eventData.position, Cam, out localPoint))
        {
            float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
            SkipToPercent(pct);
        }
    }

    private void SkipToPercent(float pct)
    {
        var frame = videoPlayer.frameCount * pct;
        videoPlayer.frame = (long)frame;
    }
}