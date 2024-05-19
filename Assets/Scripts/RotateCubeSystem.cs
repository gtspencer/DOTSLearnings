using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct RotateCubeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        // update will only run if an entity has RotateSpeed component
        state.RequireForUpdate<RotateSpeed>();
    }
    
    // CANNOT use burst on managed types (classes) -- this is why structs are used everywhere
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        /*foreach ((RefRW<LocalTransform> localTransform, RefRO<RotateSpeed> rotateSpeed) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotateSpeed>>().WithNone<Player>())
        {
            // .rotateY rotates on a copy, so must 
            localTransform.ValueRW = localTransform.ValueRW.RotateY(rotateSpeed.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);
        }*/

        var rotateCubeJob = new RotatingCubeJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        
        // runs on main thread
        // rotateCubeJob.Run();

        // can assume job is done, though this should be avoided
        // if need second job after first job, then shedule dependency
        // rotateCubeJob.Schedule(state.Dependency).Complete();
        
        rotateCubeJob.Schedule();

        // splits job to multiple chunks
        // rotateCubeJob.ScheduleParallel()
    }

    public void OnDestroy(ref SystemState state)
    {
        // throw new System.NotImplementedException();
    }

    [BurstCompile]
    [WithNone(typeof(Player))]
    public partial struct RotatingCubeJob : IJobEntity
    {
        public float deltaTime;
        // in makes it readonly, ref makes it readwrite
        public void Execute(ref LocalTransform localTransform, in RotateSpeed rotateSpeed)
        {
            localTransform = localTransform.RotateY(rotateSpeed.rotationSpeed * deltaTime);
        }
    }
}
