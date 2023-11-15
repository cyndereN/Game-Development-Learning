using UnityEngine;
using UnityEngine.UI;


// Type of center moving
public enum TranslateType {
    Direct,
    Slow
}

[RequireComponent(typeof(Image))]
public class GuideBase : MonoBehaviour
{
    protected Material Material; 

    protected Vector3 Center;    // center of cut-out area

    protected RectTransform Target; // target to highlight(cut-out)

    protected readonly Vector3[] TargetCorners = new Vector3[4]; // boundary of target

    #region ScaleChange

    protected float ScaleTimer;
    protected float ScaleTime;
    protected bool IsScaling;
    #endregion

    #region CenterMovement

    private Vector3 _startCenter;

    private float _centerTimer;
    private float _centerTime;
    private bool _isMoving;
    private static readonly int CenterID = Shader.PropertyToID("_Center");

    #endregion

    public Vector3 Center_ {
        get {
            if (Material == null) { return Vector3.zero; }
            return Material.GetVector(CenterID);
        }
    }

    protected virtual void Start()
    {
        //material = transform.GetComponent<Image>().material;
        //if (material == null)
        //{
        //    throw new System.Exception(" 未获取到材质! ");
        //}
    }

    protected virtual void Update()
    {
        if (IsScaling)
        {
            ScaleTimer += Time.deltaTime * 1 / ScaleTime;
            if (ScaleTimer >= 1)
            {
                ScaleTimer = 0;
                IsScaling = false;
            }
        }

        if (_isMoving)
        {
            _centerTimer += Time.deltaTime * 1 / _centerTime;

            // set center
            Material.SetVector(CenterID, Vector3.Lerp(_startCenter, Center, _centerTimer));

            if (_centerTimer >= 1 )
            {
                _centerTimer = 0;
                _isMoving = false;
            }
        }

    }

    // Guide
    public virtual void Guide(Canvas canvas, RectTransform target,TranslateType translateType = TranslateType.Direct, float time = 1)
    {
        // Init material
        Material = transform.GetComponent<Image>().material;

        this.Target = target;


        if (target != null)
        {
            // Get center 
            target.GetWorldCorners(TargetCorners);

            // Change world coord to screen coord
            for (int i = 0; i < TargetCorners.Length; i++)
            {
                TargetCorners[i] = WorldToScreenPoint(canvas, TargetCorners[i]);
            }
            // Calculate center point
            Center.x = TargetCorners[0].x + (TargetCorners[3].x - TargetCorners[0].x) / 2;
            Center.y = TargetCorners[0].y + (TargetCorners[1].y - TargetCorners[0].y) / 2;

            

            switch (translateType)
            {
                case TranslateType.Direct:
                    // Set center point
                    Material.SetVector(CenterID, Center);
                    break;
                case TranslateType.Slow:

                    _startCenter = Material.GetVector(CenterID);

                    _isMoving = true;
                    _centerTimer = 0;
                    _centerTime = time;
                    break;
            }
        }
        else {
            Center = Vector3.zero;
            TargetCorners[0] = new Vector3(-2000,-2000,0);
            TargetCorners[1] = new Vector3(-2000, 2000, 0);
            TargetCorners[2] = new Vector3(2000, 2000, 0);
            TargetCorners[3] = new Vector3(2000, -2000, 0);
        }
         
    }

    public virtual void Guide(Canvas canvas, RectTransform target, float scale, float time, TranslateType translateType = TranslateType.Direct, float moveTime = 1) {

    }

    public Vector2 WorldToScreenPoint(Canvas canvas, Vector3 world)
    {
        
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, world);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, canvas.worldCamera, out var localPoint);
        
        return localPoint;
    }

}
