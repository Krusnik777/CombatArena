using DI;
using CombatArena.Game.Root;
using CombatArena.Game.Services;
using UI;
using R3;
using UnityEngine;
using CombatArena.Game.Gameplay.Entities.Player;
using CombatArena.Game.Gameplay;
using CombatArena.Game.Gameplay.UI;
using CombatArena.Game.Gameplay.Entities.Enemy;


namespace CombatArena.Game.EntryPoints
{
    public class GameplayEntryPoint : EntryPoint
    {
        [SerializeField] private UISceneRootView m_sceneUIRootPrefab;
        [SerializeField] private PlayerView m_playerView;
        [SerializeField] private EnemyView[] m_enemyViews;

        private Subject<string> _onEnd;

        private System.IDisposable _disposable;

        public override Observable<string> Run(DIContainer sceneContainer)
        {
            Debug.Log("ENTRY POINT: Started Gameplay");

            _onEnd = new();

            var abilitiesProvider = sceneContainer.Resolve<AbilityConfigsProvider>();
            var gameInputService = sceneContainer.Resolve<GameInputService>();
            var player = new Player(m_playerView, sceneContainer.Resolve<PlayerConfigsProvider>(), gameInputService);
            sceneContainer.RegisterInstance(player);
            
            var playerAbilitiesFactory = new PlayerAbilitiesFactory(abilitiesProvider.AbilitiesCollection);
            var playerAbilities = playerAbilitiesFactory.Create(player);
            player.AssignAbilities(playerAbilities);
            
            SetupUI(sceneContainer);

            var enemyConfigsProvider = sceneContainer.Resolve<EnemyConfigsProvider>();

            for (int i = 0; i < m_enemyViews.Length; i++)
            {
                var enemy = new Enemy(m_enemyViews[i].Config, m_enemyViews[i]);
            }

            _disposable = gameInputService.OnTestPressed.Subscribe(_ =>
            {
                /*int value = Random.Range(1, 50);
                int type = Random.Range(0, 2);

                if (type == 0) player.Health.TakeDamage(new Damage()
                {
                    BaseValue = value,
                    ResultValue = value,
                    Modifiers = new()
                });

                if (type == 1) player.Health.Heal(value);*/
            });

            return _onEnd;
        }

        private void OnDestroy()
        {
            DisposeOfListeners();
        }

        private void FinishGame()
        {
            DisposeOfListeners();

            _onEnd.OnNext("FINISH");
        }

        private void DisposeOfListeners()
        {
            _disposable?.Dispose();
            //_stateMachine?.Dispose();
        }

        private void SetupUI(DIContainer sceneContainer)
        {
            var uiRoot = sceneContainer.Resolve<UIRootView>();
            var uiSceneRoot = Instantiate(m_sceneUIRootPrefab);
            uiRoot.AttachSceneUI(uiSceneRoot.gameObject);

            var windowsFactory = new GameplayWindowsFactory(uiSceneRoot.ScreensTransform, uiSceneRoot.PopupsTransform);
            sceneContainer.RegisterInstance(new UIWindowsProvider(windowsFactory));
            //sceneContainer.RegisterFactory(_ => new UIWindowsProvider(windowsFactory)).AsSingle();

            var screen = sceneContainer.Resolve<UIWindowsProvider>().ShowScreen<BattleScreen>();
            screen.Initialize(sceneContainer.Resolve<Player>());
        }
    }
}
