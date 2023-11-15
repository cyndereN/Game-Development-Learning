using UnityEngine;


public class GuidePanel : MonoBehaviour
{

    GuideController _guideController;

    Canvas _canvas;

    private void Awake()
    {
        _canvas = transform.GetComponentInParent<Canvas>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _guideController = transform.GetComponent<GuideController>();
        _guideController.Guide(_canvas, GameObject.Find("Button").GetComponent<RectTransform>(), GuideType.Rect);

        Invoke("Test",2);
    }

    void Test() {
        _guideController.Guide(_canvas, GameObject.Find("jb").GetComponent<RectTransform>(), GuideType.Rect, 2,2, TranslateType.Slow,2f);
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
