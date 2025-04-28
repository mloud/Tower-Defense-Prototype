using Unity.Entities;
using UnityEngine;

namespace TowerDefense.Scripts.Managers
{
    public class ManualWorldRunner : MonoBehaviour
    {
        [SerializeField] private int simulationStepsPerFrame = 100;
        [SerializeField] private float simulationDeltaTime = 1f / 60f;

        private World world;
        private SimulationSystemGroup simulationGroup;
        private InitializationSystemGroup initializationGroup;
        private PresentationSystemGroup presentationGroup;

        private bool isManualUpdatingActive;
        private float accumulatedTime = 0f;

        private void Awake()
        {
            world = World.DefaultGameObjectInjectionWorld;
            initializationGroup = world.GetExistingSystemManaged<InitializationSystemGroup>();
            simulationGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
            presentationGroup = world.GetExistingSystemManaged<PresentationSystemGroup>();
        }

        public void SetManualWorldUpdaterActive(bool isActive)
        {
            if (isActive)
            {
                if (!ScriptBehaviourUpdateOrder.IsWorldInCurrentPlayerLoop(world))
                {
                    ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(world);
                }
            }
            else
            {
                if (ScriptBehaviourUpdateOrder.IsWorldInCurrentPlayerLoop(world))
                {
                    ScriptBehaviourUpdateOrder.RemoveWorldFromCurrentPlayerLoop(world);
                }
            }
            isManualUpdatingActive = isActive;
        }
        
        private void Update()
        {
            if (!isManualUpdatingActive)
                return;
            
            UpdateWorld(simulationStepsPerFrame, simulationDeltaTime);
        }

        private void UpdateWorld(int steps, float deltaTimePerStep)
        {
            float startTime = Time.realtimeSinceStartup;
    
            for (int i = 0; i < steps; i++)
            {
                accumulatedTime += deltaTimePerStep;
        
                var time = new Unity.Core.TimeData(
                    startTime + accumulatedTime,
                    deltaTimePerStep
               
                );

                world.Unmanaged.Time = time;

                initializationGroup.Update();
                simulationGroup.Update();
                presentationGroup.Update();
            }
        }
    }
}