# О архитектуре и особенностях
Краткое описание архитектуры проекта и разбор основных модулей.


## Основная архитектура
### 1. Единая точка входа - GameEntryPoint
Скрипт [GameEntryPoint.cs](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Root/GameEntryPoint.cs).

Здесь инициализируются все глобальные системы (конкретно UIRoot) и сервисы (например, сервис загрузки, обеспечивающий переключение сцен и отображения экрана загрузки). Их хранение обеспечивается в [DI-контейнере](https://github.com/Krusnik777/CombatArena/tree/master/Assets/_Project/Scripts/DI). Этот класс также обеспечивает связку точек входа сцен и передачу/обработку входных/выходных данных на игровые сцены.

### 2. Точка входа на сцену - EntryPoint : MonoBehaviour
[EntryPoint](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/EntryPoints/EntryPoint.cs) - это абстрактный класс, обеспечивающий вход на игровые сцены, созданные в редакторе Unity (например, MainMenuScene, Level1Scene, Level2Scene и т.п.), методом Run(), который на вход получает параметры сцены и выдает выходные параметры для передачи в другие сцены, перезагрузку, сервисы и т.д.

Так как этот класс является MonoBehaviour, то именно он дает обеспечить связку игры с другими MonoBehaviour-объектами на сцене (когда требуется).

В проекте есть только одна сцена с геймплеем и следовательно одна точка входа - [GameplayEntryPoint](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/EntryPoints/GameplayEntryPoint.cs).
В ней инициализируется игрок, локальные сервисы/фабрики, UI сцены и запускается машина состояний игрового процесса.

### 3. Машина состояний игрового процесса - GameplayStateMachine : AbstractStateMachine
Абстракция стандартной машины состояний описана в [Assets/_Project/Scripts/StateMachine](https://github.com/Krusnik777/CombatArena/tree/master/Assets/_Project/Scripts/StateMachine).

В игре используется [GameplayStateMachine](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/StateMachines/GameplayStateMachine.cs), которая управляет порядком всего игрового процесса и состоит из трех состояний:
- Состояние перед битвой ([BeforeBattleState](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/StateMachines/States/BeforeBattleState.cs)) - игрок получает под управление аватара и должен продвинуться дальше, чтобы запустилась битва.
- Состояние битвы ([ActiveBattleState](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/StateMachines/States/ActiveBattleState.cs)) - включаются основной игровой процесс, включается игровой HUD и т.п.; происходит контроль игрового процесса и определяется результат - победа или поражение.
- Состояние после битвы ([BattleResultState](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/StateMachines/States/BattleResultState.cs)) - выключает все игровые процессы и в зависимости от полученного результата из предыдущего состояния включает экран победы или экран поражения: выбор в этих окнах дает перезапустить игру заново (в случае победы перезапуск обеспечивает большее количество врагов на сцене в следующий раз) или выйти из игры.

## Геймплей и Билды
### - [Билд v.0.1.0](https://github.com/Krusnik777/CombatArena/releases/tag/0.1.0). Видео геймплея v.0.1.0 - [Google-диск ](https://drive.google.com/file/d/19D5ZIrqsWVNi9xmpWXCd1RLcg3HOGnMD/view) или [Youtube](https://youtu.be/5uJq18Qlsg8).
### - [Билд v.0.2.0](https://github.com/Krusnik777/CombatArena/releases/tag/0.2.0). Видео геймплея v.0.2.0 - [Google-диск ](https://drive.google.com/file/d/14KUnD-Ec8y6pX7PAHpPS_tTBIHlkA4Ce/view) или [Youtube](https://youtu.be/sE4LDuVU9UQ).

## Модули
### 1. Combat - Система здоровья и урона - Health, Damage, IDamageProcessor и IDamageModifier
[Health](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Health/Health.cs) - стандартный класс, в котором происходит обработка значений очков здоровья игрока/врагов.

[Damage](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Health/Damage.cs) - структура для хранения данных урона.

[IDamageModifier](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Health/DamageModifiers/IDamageModifier.cs) - модификатор урона, который как-то обрабатывает урон на основе заданной логики. В проекте есть три реализации этого интерфейса:
- [CriticalDamageModifier](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Health/DamageModifiers/CriticalDamageModifier.cs) - модификатор критического урона;
- [ArmorDefenceModifier](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Health/DamageModifiers/ArmorDefenceModifier.cs) - модификатор защиты от урона;
- [ArmorBreakDamageModifier](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Health/DamageModifiers/ArmorBreakDamageModifier.cs) - модификатор обхода защиты.

[IDamageProcessor](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Health/DamageProcessor/IDamageProcessor.cs) - пайплайн обработки урона с помощью модификаторов.
Конкретная реализация-пример - [DamageProcessor](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Health/DamageProcessor/DamageProcessor.cs) - помимо обработки входящих модификторов еще может иметь пассивные модификаторы, которые будут действовать на весь входящий урон, пока модификатор не снимут. Но в проекте эта особенность не используется.

В Health создается экземпляр конкретной реализации IDamageProcessor, который и производит обработку входящих Damage-контейнеров.

### 2. Combat - Контроллер игрока и способности - Player, PlayerAvatarMovement, IAbility, IDamageDealer и др.
[Player](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Entities/Player/Player.cs) - класс игрока, в котором осуществляется управление модулями аватара (вьюшки) игрока на игровой сцене ([PlayerView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Entities/Player/PlayerView.cs)).

Конкретно модуль [PlayerAvatarMovement](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Entities/Player/PlayerAvatarMovement.cs) отвечает за перемещение аватара игрока.

Интерфейс [IAbility](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Abilities/_Base/IAbility.cs) дает набор функций взаимодействия со способностями. [Ability](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Abilities/_Base/Ability.cs) - это абстрактный класс любых способностей и реализация этого интерфейса.

Унаследованные от Ability [AttackAbility](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Abilities/AttackAbility.cs) (атакаующая способность) и [DashAbility](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Abilities/DashAbility.cs) (способность рывка) содержат уже разную логику работы данных способностей. [IAbilityAttacker](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Entities/IAbilityAttacker.cs) и [IAbilityMover](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Entities/IAbilityMover.cs) - это обработчики вызванных способностей.

[AbilityConfig](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Configs/Abilities/AbilityConfig.cs) на основе ScriptableObject и наследники [AttackAbilityConfig](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Configs/Abilities/AttackAbilityConfig.cs)/[DashAbilityConfig](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Configs/Abilities/DashAbilityConfig.cs) содержат параметры работы способностей (например, Cooldown).

IDamageDealer - пустышка-интерфейс для разграничения разных типов нанесения урона: [AOEDamageDealer](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Entities/DamageDealer/AOEDamageDealer.cs) (урон по площади) и [SwordDamageDealer](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Entities/DamageDealer/SwordDamageDealer.cs) (урон примерно по движению меча).

Так, у игрока в виде примера реализованы три способности - базовая атака (под капотом способность с нулевым Cooldown), атака по площади и рывок.

Стоит заметить, что система способностей применяется и для врагов ([Enemy](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/Entities/Enemy/Enemy.cs)) - в конфигах врагов ([EnemyConfig](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Configs/EnemyConfig.cs)) используются конфиги атакующих способностей (AttackAbilityConfig) для настройки атак у врагов, как и внутри класса предусматриваются свои реализации IAbilityAttacker и IDamageDealer (тут используется у всех AOEDamageDealer, но можно расширять далее и создать третий тип наносителя урона). 

### 3. UI - HUD и Экраны состояний - IWindowView, Window, Screen и др.
В основе управления UI-системами стоит MVP (с некоторыми нарушениями - [UIHealth](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/UIHealth.cs) и [UIAbility](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/UIAbility.cs), например, только инициализируются из презентера/контроллера окна, а внутри есть логика). Абстракция системы работы окон расположена в [Assets/_Project/Scripts/UI/Windows](https://github.com/Krusnik777/CombatArena/tree/master/Assets/_Project/Scripts/UI/Windows), где [IWindowView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Windows/IWindowView.cs) и конкретно [WindowView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Windows/WindowView.cs) - MonoBehaviour-отображения окна ([Window](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Windows/Window.cs)). В проекте используется наследники экрана ([Screen](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Windows/Screen.cs)):
- [BattleScreen](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/BattleScreen.cs) ([BattleScreenView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/BattleScreenView.cs)) - HUD боевого состояния игры - отображет способности (и их кулдаун/доступность), здоровье игрока и здоровье ближайшего врага (выделяется на игровой сцене особым эффектом). У самих врагов также отображается свои шкалы здоровья над головами, но это не относится к работе экрана.
- [BeforeBattleScreen](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/BeforeBattleScreen.cs) ([BeforeBattleScreenView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/BeforeBattleScreenView.cs)) - экран в состоянии перед боем. ~~Содержит ноль по логике и во вьюшке только отображается одна фраза, но можно расширять как угодно.~~ v.0.2.0 - теперь выполняет больше визуальную роль: фейд-ин после загрузки и показ сообщения игроку.
- [VictoryScreen](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/VictoryScreen.cs) ([VictoryScreenView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/VictoryScreenView.cs))/[DefeatScreen](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/DefeatScreen.cs) ([DefeatScreenView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/DefeatScreenView.cs)) - экраны победы и поражения. По логике идентичные (только названия и пара ссылок различаются), так что ~~правильнее было было бы сделать сначала абстрактный результативный экран с общей логикой и уже от него наследовать эти экраны...~~ с v.0.2.0 являются пустышками, которые наследуются от единого окна результата - [ResultScreen](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/ResultScreen.cs) ([ResultScreenView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Gameplay/UI/Screens/ResultScreenView.cs)) и которые в дальнейшем можно расширять и дополнять различающейся логикой.

### 4. UI - Тултипы - TooltipView, TooltipData, ITooltipTrigger и ITooltipHandler
Абстракция тултипов содержится в [Assets/_Project/Scripts/UI/Tooltips](https://github.com/Krusnik777/CombatArena/tree/master/Assets/_Project/Scripts/UI/Tooltips).

[TooltipView](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Tooltips/TooltipView.cs) - отображение тултипа, на которое поступают данные показа ([TooltipData](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Tooltips/TooltipData.cs)).

[ITooltipTrigger](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Tooltips/ITooltipTrigger.cs) и [TooltipTrigger](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Tooltips/TooltipTrigger.cs) - обработчики входа/выхода курсора для отображения тултипов. TooltipTrigger наследуют UIHealth и UIAbility. Конкретно взаимодействие курсора с UIAbility подтягивает данные из конфига привязанной к отображению способности.

[ITooltipHandler](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/UI/Tooltips/ITooltipHandler.cs) - обработчик показа тултипов. [TooltipsService](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Game/Services/TooltipsProvider/TooltipsService.cs) - реализация этого интерфейса.

### 5. UI - Прогресс загрузки - LoadingStep, LoadingManager, LoadingScreen и LoadingStepFailureData
[LoadingManager](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Loading/LoadingManager.cs) управляет загрузкой игровых сцен и показом/обновлением загрузочного экрана.

Загрузочный экран ([LoadingScreen](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Loading/LoadingScreen.cs)) содержит прогресс-бар и текстовую строку, в которой отображается статус загрузки (описание шага загрузки).

Загрузка осуществляется поэтапно - по шагам. [LoadingStep](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Loading/LoadingStep.cs) - это контейнер конкретного этапа загрузки - в него можно прокинуть ассинхронный метод из любого сервиса (например, какой-нибудь IAPService, который выгружает продукты).

[LoadingStepFailureData](https://github.com/Krusnik777/CombatArena/blob/master/Assets/_Project/Scripts/Loading/LoadingStepFailureData.cs) - это контейнер данных для обработки ошибок загрузок разных игровых этапов. В LoadingManager обеспечен механизм обработки таких ошибок и таких данных.

Для имитации пошаговой загрузки и ошибок в GameEntryPoint создаются фейковые LoadingStep'ы и передаются в LoadingManager при загрузке/перезагрузке сцены игрового процесса.
