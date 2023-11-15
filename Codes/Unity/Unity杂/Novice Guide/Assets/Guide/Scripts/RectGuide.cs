using UnityEngine;

public class RectGuide : GuideBase
{
 
    protected float Width;       
    protected float Height;      

    private float _scaleWidth;  
    private float _scaleHeight;
    private static readonly int SliderXid = Shader.PropertyToID("_SliderX");
    private static readonly int SliderYid = Shader.PropertyToID("_SliderY");

    // Guide
    public override void Guide( Canvas canvas , RectTransform target, TranslateType translateType = TranslateType.Direct, float time = 1)
    {
        base.Guide( canvas,target,translateType,time);

        // Calculate width and height 
        Width = (TargetCorners[3].x - TargetCorners[0].x)/2;
        Height = (TargetCorners[1].y - TargetCorners[0].y)/2;

        Material.SetFloat(SliderXid, Width);
        Material.SetFloat(SliderYid, Height);

    }

    public override void Guide(Canvas canvas, RectTransform target, float scale, float time, TranslateType translateType = TranslateType.Direct, float moveTime = 1)
    {
        this.Guide(canvas, target, translateType, moveTime);

        _scaleWidth = Width * scale;
        _scaleHeight = Height * scale;

        IsScaling = true;
        ScaleTimer = 0;
        this.ScaleTime = time;
    }


    protected override void Update()
    {
        base.Update();
        if (IsScaling)
        {
            Material.SetFloat(SliderXid, Mathf.Lerp(_scaleWidth, Width, ScaleTimer));
            Material.SetFloat(SliderYid, Mathf.Lerp(_scaleHeight, Height, ScaleTimer));
        }
    }

}