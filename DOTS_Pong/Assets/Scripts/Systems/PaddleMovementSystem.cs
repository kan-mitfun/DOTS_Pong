using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

//[AlwaysSynchronizeSystem]
public class PaddleMovementSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float deltaTime = Time.DeltaTime;
		float yBound = GameManager.main.yBound;

        #region 第一寫法
        JobHandle myJob = Entities.ForEach((ref Translation trans, in PaddleMovementData data) =>
        {
          trans.Value.y = math.clamp(trans.Value.y + (data.speed * data.direction * deltaTime), -yBound, yBound);
        }).Schedule(inputDeps);

        return myJob;
        #endregion

        #region 第二寫法
        // 這邊只有兩個控制板(玩家)，執行內容也是消耗效率無限小的功能，丟到多線程的消耗反而更大。所以調整為主線程執行
        //Entities.ForEach((ref Translation trans, in PaddleMovementData data) =>
        //{
        //	trans.Value.y = math.clamp(trans.Value.y + (data.speed * data.direction * deltaTime), -yBound, yBound);
        //}).Run();

        //return default;
        #endregion
    }
}



//PlayerInputSystem/PaddleMovementSyatem 分成兩部份原因：
//是在做兩件不同的事
//PlayerInputSystem內的 Input.GetKey 一定要在主線程，PaddleMovementSyatem不限制在主線程執行。
