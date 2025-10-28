using TheGame;

namespace XIV.Ecs
{
    /*
     System And Unity Events Execution Order
        PreAwake (Ignores System State)  -> call SetCustomAssign and SetCustomReset here
        Awake (Ignores System State) -> entity creation should be after this point
        Start (Ignores System State)
        FixedUpdate
        Physics Update (Unity Only)
        PreUpdate (System Only)
            - System Manager State Changes
            - Events
            - PreUpdate 
        Update
        Coroutines
        LateUpdate
     */
    
    public class LevelController : EasyLevelController
    {
        public static class States
        {
            public const int Start = 0;
            public const int Game = 1;
            public const int Paused = 2;
            public const int LevelCompleted = 3;
            public const int LevelFailed = 4;

            public static readonly int[] All =
            {
                Start, Game, Paused, LevelCompleted, LevelFailed
            };
        }
        
        
        public PrefabReferences prefabReferences;
        
        public override void OnInject()
        {
            LevelSettingsMono levelSettingsMono = FindObjectOfType<LevelSettingsMono>();
            var levelSettings = levelSettingsMono == null ? new LevelSettings() : levelSettingsMono.levelSettings;
            manager.Inject(levelSettings);
            manager.Inject(prefabReferences);
            manager.Inject(new LevelState());
            manager.Inject(new ConnectionDB());
            manager.Inject(new LineRendererPositionData());
            
            // Set start state to Game and don't instantiate startUI if you don't have a start menu
            manager.ChangeState(States.Start);
        }


        public override void AddSystems()
        {
            manager.AddSystem(new LevelLoadingSystem(), States.All); // Awake
            manager.AddSystem(new CallLaterSystem(), States.All); // PreUpdate
            manager.AddSystem(new InputSystem(), States.Game); // PreUpdate - Only Works During Game
            manager.AddSystem(new UISystem(), States.All);
            manager.AddSystem(new StartGameSystem(), States.Start);
            
            // Game
            manager.AddSystem(new NodeLevelGeneratorSystem(), States.Start);
            manager.AddSystem(new NodeInitializeSystem(), States.Game);
            
            manager.AddSystem(new NodeResourceGenerateSystem(), States.Game);
            manager.AddSystem(new UnitNodeSelectionSystem(), States.Game);
            
            manager.AddSystem(new NodeHighlightSystem(), States.Game);
            
            manager.AddSystem(new ResourceTransferSystem(), States.Game);
            manager.AddSystem(new ResourceCollisionSystem(), States.Game);
            
            manager.AddSystem(new ConnectionLineRenderSystem(), States.Game);
            manager.AddSystem(new ShieldRenderSystem(), States.Game);
            manager.AddSystem(new NodeDebugSystem(), States.Game);
            
            
            manager.AddSystem(new TransformSystem(), States.All); // Awake
            manager.AddSystem(new ParentSystem(), States.All); // Update
            manager.AddSystem(new DestroySystem(), States.All); // Late Update
        }
    }
}