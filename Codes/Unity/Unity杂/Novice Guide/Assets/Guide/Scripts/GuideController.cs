using UnityEngine;
using UnityEngine.UI;

public enum GuideType {
    Rect,
    Circle
}


[RequireComponent(typeof(CircleGuide))]
[RequireComponent(typeof(RectGuide))]
public class GuideController : MonoBehaviour , ICanvasRaycastFilter
{

    private CircleGuide _circleGuide;
    private RectGuide _rectGuide;

    public Material rectMat;
    public Material circleMat;

    private Image _mask;

    private RectTransform _target;

    private GuideType _guideType;

    #region Property
    public Vector3 Center {
        get {

            switch (this._guideType)
            {
                case GuideType.Rect:
                    return _rectGuide.Center_;
                case GuideType.Circle:
                    return _circleGuide.Center_;
            }

            return _rectGuide.Center_;
        }
    }
    #endregion

    private void Awake()
    {
        _mask = transform.GetComponent<Image>();

        if ( _mask == null ) { throw new System.Exception("Mask initialization failed!"); }

        if (rectMat == null || circleMat == null) { throw new System.Exception("Material not assigned!"); }

        _circleGuide = transform.GetComponent<CircleGuide>();
        _rectGuide = transform.GetComponent<RectGuide>();

    }

    private void Guide(RectTransform target, GuideType guideType) {
        this._target = target;
        this._guideType = guideType;

        switch (guideType)
        {
            case GuideType.Rect:
                _mask.material = rectMat;
                break;
            case GuideType.Circle:
                _mask.material = circleMat;
                break;
        }
    }

    public void Guide(Canvas canvas, RectTransform target, GuideType guideType,TranslateType translateType = TranslateType.Direct,float time = 1) {

        Guide(target, guideType);

        switch (guideType)
        {
            case GuideType.Rect:
                _rectGuide.Guide(canvas, target,translateType,time);
                break;
            case GuideType.Circle:
                _circleGuide.Guide(canvas, target,translateType, time);
                break;
        }
    }

    public void Guide(Canvas canvas, RectTransform target, GuideType guideType,float scale,float time, TranslateType translateType = TranslateType.Direct, float moveTime = 1) {

        Guide(target, guideType);

        switch (guideType)
        {
            case GuideType.Rect:
                _rectGuide.Guide(canvas, target,scale,time,translateType,moveTime);
                break;
            case GuideType.Circle:
                _circleGuide.Guide(canvas, target,scale,time, translateType, moveTime);
                break;
        }
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (_target == null) { return false; } 

        return !RectTransformUtility.RectangleContainsScreenPoint(_target,sp);
    }
}
