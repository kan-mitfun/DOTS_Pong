using Unity.Entities;

[GenerateAuthoringComponent]
public struct SpeedIncreaseOverTimeData : IComponentData
{
    //如果只有一個參數，變數名稱用Value即可，但為避免混淆也可以取相對應的英文名
    public float increasePerSecond;
}
