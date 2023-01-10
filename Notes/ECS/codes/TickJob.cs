// 按帧倒计时

public TickJob
{
    public int Frames;
    public Action Callback;
    public int TapStep;
 
    public TickJob(int delayFrames, int tapStep, Action callback)
    {
        Frames = delayFrames;
        Callback = callback;
        TapStep = tapStep;
    }
 
 
    // 返回true意味着结束
    public bool Tick()
    {
        Frames--;
        if(Frames <= 0)
        {
            Callback.Invoke();
            return true;
        }
        return false;
    }
}
 
 
public void AddTickJob(int frames,int tapStep, Action callback)
{
    _tickJobs.Add(new TickJob(frames, tapStep, callback));
}
 
 
world.AddTickJob(affectDelay, tapAffectStep, () =>
{
    AffectCell(cell, tapAffectStep, world, ai);
});