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
    
    public class EasyLevelController : LevelController
    {
        public static class States
        {
            public const int Start = 0;
            public const int Game = 1;
            public const int Paused = 2;
            public const int LevelCompleted = 3;
            public const int LevelFailed = 4;
            public const int SceneLoadingStart = 5;
            public const int SceneLoading = 6;
            public const int SceneLoadingEnd = 7;

            public static readonly int[] All =
            {
                Start, Game, Paused, LevelCompleted, LevelFailed
            };
        }
        
        public override void OnInject()
        {
            LevelSettingsMono levelSettingsMono = FindObjectOfType<LevelSettingsMono>();
            var levelSettings = levelSettingsMono == null ? new LevelSettings() : levelSettingsMono.levelSettings;
            var assetReferencesMono = FindObjectOfType<AssetReferencesMono>();
            var assetReferences = assetReferencesMono == null ? new AssetReferences() : assetReferencesMono.assetReferences;
            manager.Inject(levelSettings);
            manager.Inject(assetReferences);
            manager.Inject(new LevelState());
            manager.Inject(new ConnectionDB());
            manager.Inject(new LineRendererPositionData());
            
            // Set start state to Game and don't instantiate startUI if you don't have a start menu
            manager.ChangeState(States.Start);
        }


        public override void AddSystems()
        {
            manager.AddSystem(new LevelLoadingSystem(), States.All); // PreUpdate - Only Works During Start
            manager.AddSystem(new CallLaterSystem(), States.All); // PreUpdate
            manager.AddSystem(new InputSystem(), States.Game); // PreUpdate - Only Works During Game
            manager.AddSystem(new UISystem(), States.All);
            manager.AddSystem(new StartGameSystem(), States.Start);
            
            // Game
            manager.AddSystem(new NodeLevelGeneratorSystem(), States.Start);
            manager.AddSystem(new NodeInitializeSystem(), States.Game);
            manager.AddSystem(new NodeOccupySystem(), States.Game);
            
            manager.AddSystem(new ResourceGenerateSystem(), States.Game);
            manager.AddSystem(new ResourceDamageSystem(), States.Game);
            
            manager.AddSystem(new NodeShieldSystem(), States.Game);
            manager.AddSystem(new NodeAddShieldSystem(), States.Game);
            manager.AddSystem(new NodeRemoveShieldSystem(), States.Game);
            manager.AddSystem(new UnitNodeSelectionSystem(), States.Game);
            
            manager.AddSystem(new NodeHighlightSystem(), States.Game);
            
            manager.AddSystem(new ResourceTransferSystem(), States.Game);
            manager.AddSystem(new ResourceCollisionSystem(), States.Game);
            
            manager.AddSystem(new NodeTypeChangeSystem(), States.Game);
            
            // Visuals
            manager.AddSystem(new ConnectionLineRenderSystem(), States.Game);
            manager.AddSystem(new ShieldRenderSystem(), States.Game);
            manager.AddSystem(new ResourceTextRenderSystem(), States.Game);
            manager.AddSystem(new ResourceTransferIndicatorSystem(), States.Game);
            
            // AI
            manager.AddSystem(new NodeDecisionSystem(), States.Game);
            manager.AddSystem(new NodeDecisionApplySystem(), States.Game);
            manager.AddSystem(new NodePathFindSystem(), States.Game);
            manager.AddSystem(new NodeCaptureSystem(), States.Game);
            manager.AddSystem(new NodeHelpFrontierSystem(), States.Game);
            
            manager.AddSystem(new DebugSystem(), States.Game);
            // manager.AddSystem(new NodePathSystem(), States.Game);
            
            manager.AddSystem(new TransformSystem(), States.All); // Awake
            manager.AddSystem(new ParentSystem(), States.All); // Update
            manager.AddSystem(new DestroySystem(), States.All); // Late Update
        }
    }
}