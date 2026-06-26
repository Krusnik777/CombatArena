using CombatArena.Game.Gameplay.Entities.Levels;
using CombatArena.Game.Gameplay.Entities.Player;
using R3;
using Screen = UI.Windows.Screen;
using ITooltipHandler = UI.Tooltips.ITooltipHandler;

namespace CombatArena.Game.Gameplay.UI
{
    public class BattleScreen : Screen
    {
        private BattleScreenView _concreteView => _view as BattleScreenView;

        private System.IDisposable _enemiesRemainedListenerDisposable;
        private System.IDisposable _detectedEnemyListenerDisposable;

        public BattleScreen(BattleScreenView view) : base(view) { }

        public override void Dispose()
        {
            base.Dispose();

            _concreteView.UIPlayerHealth.Dispose();
            _concreteView.UIEnemyHealth.Dispose();

            _concreteView.UIAbilityA.Dispose();
            _concreteView.UIAbilityX.Dispose();
            _concreteView.UIAbilityY.Dispose();

            for (int i = 0; i < _concreteView.ControlsTips.Length; i++) _concreteView.ControlsTips[i].Dispose();

            _enemiesRemainedListenerDisposable?.Dispose();
            _detectedEnemyListenerDisposable?.Dispose();
        }

        public void Initialize(Player player, GameplayLevelController levelController)
        {
            _concreteView.UIPlayerHealth.Bind(player.Health);

            _concreteView.UIAbilityA.Bind(player.Abilities.AbilityA);
            _concreteView.UIAbilityX.Bind(player.Abilities.AbilityX);
            _concreteView.UIAbilityY.Bind(player.Abilities.AbilityY);

            for (int i = 0; i < _concreteView.ControlsTips.Length; i++) _concreteView.ControlsTips[i].Initialize();

            _enemiesRemainedListenerDisposable = levelController.OnEnemyDied.Subscribe(remainingEnemiesCount =>
            {
                var count = remainingEnemiesCount < 0 ? 0 : remainingEnemiesCount;
                _concreteView.EnemiesRemainingText.text = $"Remaining Enemies: {count}";
            });

            _detectedEnemyListenerDisposable = levelController.EnemyDetectedByPlayer.Subscribe(enemy =>
            {
                _concreteView.UIEnemyHealth?.Dispose();

                if (enemy == null)
                {
                    _concreteView.EnemyInfo.SetActive(false);
                    return;
                }

                _concreteView.EnemyName.text = enemy.Name;
                _concreteView.UIEnemyHealth.Bind(enemy.Health);

                _concreteView.EnemyInfo.SetActive(true);
            });
        }

        public void BindTooltipHandler(ITooltipHandler tooltipHandler)
        {
            _concreteView.UIPlayerHealth.BindTooltipHandler(tooltipHandler);
            _concreteView.UIEnemyHealth.BindTooltipHandler(tooltipHandler);

            _concreteView.UIAbilityA.BindTooltipHandler(tooltipHandler);
            _concreteView.UIAbilityX.BindTooltipHandler(tooltipHandler);
            _concreteView.UIAbilityY.BindTooltipHandler(tooltipHandler);
        }
    }
}
