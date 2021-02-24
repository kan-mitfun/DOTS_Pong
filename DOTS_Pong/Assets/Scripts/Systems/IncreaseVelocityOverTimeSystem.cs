using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;

[AlwaysSynchronizeSystem]
public class IncreaseVelocityOverTimeSystem : JobComponentSystem
{
	protected override JobHandle OnUpdate(JobHandle inputDeps)
	{
		float deltaTime = Time.DeltaTime;

        //PhysicsVelocity : The velocity of a rigid body.
        JobHandle myJob = Entities.ForEach((ref PhysicsVelocity vel, in SpeedIncreaseOverTimeData data) =>
		{
			float2 modifier = new float2(data.increasePerSecond * deltaTime);

            //Linear = LinearVelocity : The body's world-space linear velocity in units per second.
            float2 newVel = vel.Linear.xy;
			newVel += math.lerp(-modifier, modifier, math.sign(newVel));
			vel.Linear.xy = newVel;
		}).Schedule(inputDeps);

		return myJob;
	}
}
