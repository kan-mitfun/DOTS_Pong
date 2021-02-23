using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

//原本的
public class PlayerInputSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        //讓Unity知道哪些資料read-only，可以同時讀取更多的資料，進而有更高的處理效率
        //PaddleInputData，只須讀取，加上 in
        //PaddleMovementData，需寫入加上 ref。
        //ref 放在前方，in 放在後方
        Entities.ForEach((ref PaddleMovementData moveData, in PaddleInputData inputData) =>
        {
            moveData.direction = 0;

            moveData.direction += Input.GetKey(inputData.upKey) ? 1 : 0;
            moveData.direction -= Input.GetKey(inputData.downKey) ? 1 : 0;
        }).Run();
        //多線程執行：Schedule(inputDeps)
        //主線程執行：Run()
        //Input 只能在主線程

        return inputDeps;
    }
}

//直接回傳 inputDeps 會造成系統的困惑，為什麼前面沒有做事，是不是在等待其他工作需要這個 inputDeps，此狀況會造成些許延遲，來做一些線程檢查之類的事。
//所以有以下兩種方式，讓 Unity scheduler 可以優化此情境。
//1.加入inputDeps.Complete()。回傳改為 new JobHandle()
//2.加入[AlwaysSynchonizeSystem]。回傳改為 default


//第一種方式
//public class PlayerInputSystem : JobComponentSystem
//{
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        inputDeps.Complete()
//        Entities.ForEach((ref PaddleMovementData moveData, in PaddleInputData inputData) =>
//        {
//            moveData.direction = 0;

//            moveData.direction += Input.GetKey(inputData.upKey) ? 1 : 0;
//            moveData.direction -= Input.GetKey(inputData.downKey) ? 1 : 0;
//        }).Run();

//        return new JobHandle();
//    }
//}

//第二種方式
//AlwaysSynchronizeSystem can be applied to a JobComponentSystem to force it to synchronize on all of its dependencies before every update.
//完成所有主線程工作，確保其他相依工作完成，再來執行我。
//[AlwaysSynchronizeSystem]
//public class PlayerInputSystem : JobComponentSystem
//{
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {

//        Entities.ForEach((ref PaddleMovementData moveData, in PaddleInputData inputData) =>
//        {
//            moveData.direction = 0;

//            moveData.direction += Input.GetKey(inputData.upKey) ? 1 : 0;
//            moveData.direction -= Input.GetKey(inputData.downKey) ? 1 : 0;
//        }).Run();

//        return default;
//    }
//}