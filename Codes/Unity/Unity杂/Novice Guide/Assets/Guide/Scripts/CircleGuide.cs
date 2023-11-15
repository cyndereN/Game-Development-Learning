using UnityEngine;

public class CircleGuide : GuideBase
{

    private float _r;        // radius of cut-out area

    private float _scaleR;
    private static readonly int SliderID = Shader.PropertyToID("_Slider");


    public override void Guide(Canvas canvas, RectTransform target, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        base.Guide(canvas, target,translateType,moveTime);
        float width = (TargetCorners[3].x - TargetCorners[0].x) / 2;
        float height = (TargetCorners[1].y - TargetCorners[0].y) / 2;
       
        _r  = Mathf.Sqrt( width * width + height * height);
        
        this.Material.SetFloat(SliderID, _r);
    }

    public override void Guide(Canvas canvas, RectTransform target, float scale, float time, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        // base.Guide(canvas, target, scale, time);
        this.Guide(canvas, target,translateType, moveTime);

        _scaleR = _r * scale;
        this.Material.SetFloat(SliderID, _scaleR);

        this.ScaleTime = time;
        IsScaling = true;
        ScaleTimer = 0;
    }

    protected override void Update()
    {
        base.Update();

        if ( IsScaling )
        {
            this.Material.SetFloat(SliderID, Mathf.Lerp(_scaleR, _r, ScaleTimer));
        }

    }


}