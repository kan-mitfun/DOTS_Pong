using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

[AlwaysSynchronizeSystem]
public class BallGoalCheckSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        #region 第一寫法
        //EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        //Entities
        ////只需要球，所以用了 WithAll<BallTag>()
        //.WithAll<BallTag>()
        ////WithStructuralChanges : executes the lambda function on the main thread and disables Burst 
        ////so that you can make structural changes to your entity data within the function. 
        ////For better performance, use a concurrent EntityCommandBuffer instead.
        //.WithStructuralChanges()
        ////WithoutBurst()：因為使用到GameManager.main，所以必須取消BurstCompile，否則會有Burst Error(non-readonly static field)。
        ////註：新版本使用BurstCompile，不用寫出WithBurst()，因預設已是Burst。除非要拿掉BurstCompile才需要加入WithoutBurst()
        //.WithoutBurst()
        ////Entity：需要本體、以便應該要Destory時使用
        //.ForEach((Entity entity, in Translation trans) =>
        //{
        //    float3 pos = trans.Value;
        //    float bound = GameManager.main.xBound;

        //    if (pos.x >= bound)
        //    {
        //        GameManager.main.PlayerScored(0);
        //        EnitityManager.DestroyEntity(entity);
        //    }
        //    else if (pos.x <= -bound)
        //    {
        //        GameManager.main.PlayerScored(1);
        //        EnitityManager.DestroyEntity(entity);
        //    }
        //}).Run();//因為使用了EnitityManager.DestroyEntity()，所以需要在主線程執行
        #endregion

        //WithStructuralChanges()，在簡單的內容中使用上沒有問題，但如果在比較複雜的內容中，可能會影響到效能。例如頻繁的Create及Destroy Entity

        //The EntityCommandBuffer(ECB) class solves two important problems:

        //When you're in a job, you can't access the EntityManager.
        //When you perform a structural change (like creating an entity), you create a sync point and must wait for all jobs to complete.

        //The EntityCommandBuffer abstraction allows you to queue up changes (from either a job or from the main thread) 
        //so that they can take effect later on the main thread.


    #region 第二寫法
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithAll<BallTag>()
            .WithoutBurst()
            .ForEach((Entity entity, in Translation trans) =>
            {
                float3 pos = trans.Value;
                float bound = GameManager.main.xBound;

                if (pos.x >= bound)
                {
                    GameManager.main.PlayerScored(0);
                    ecb.DestroyEntity(entity);
                }
                else if (pos.x <= -bound)
                {
                    GameManager.main.PlayerScored(1);
                    ecb.DestroyEntity(entity);
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
        #endregion


        return default;
	}
}
