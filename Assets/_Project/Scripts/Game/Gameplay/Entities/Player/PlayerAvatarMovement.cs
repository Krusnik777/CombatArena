using System.Collections;
using CombatArena.Game.Configs;
using CombatArena.Game.Services;
using UnityEngine;
using Utils;

namespace CombatArena.Game.Gameplay.Entities.Player
{
    public class PlayerAvatarMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController m_characterController;
        [SerializeField] private Transform m_viewTransform;

        private GameInputService _gameInputService;
        private PlayerAvatarConfig _config;

        private Vector3 _directionControl;

        public void Bind(PlayerAvatarConfig config, GameInputService gameInputService)
        {
            _config = config;
            _gameInputService = gameInputService;
        }
        public void Stop() => m_characterController.Move(Vector3.zero);

        public void Teleport(Transform targetPlace)
        {
            m_characterController.Move(Vector3.zero); // just to be safe
            m_characterController.enabled = false;

            transform.position = targetPlace.position;
            m_viewTransform.rotation = targetPlace.rotation;

            m_characterController.enabled = true;
        }

        private void Update()
        {
            if (_isDashing) return;

            GetMoveDirection();

            if (_directionControl.magnitude > 0)
            {
                m_characterController.Move(_directionControl * _config.MovementSpeed * Time.deltaTime);
                var targetRotation = Quaternion.LookRotation(_directionControl);
                m_viewTransform.rotation = Quaternion.Lerp(m_viewTransform.rotation, targetRotation, _config.RotationSpeed * Time.deltaTime);
            }
            else
            {
                m_characterController.Move(Vector3.zero);
            }
        }

        private void GetMoveDirection()
        {
            if (_gameInputService == null || _config == null) return;

            var moveDirection = _gameInputService.GetMovementInput(_config.IsInputReverseMovement);
            _directionControl = moveDirection;
            if (_config.IsMovementIsometric) _directionControl = _directionControl.ToIsometric();
            _directionControl.Normalize();
        }

        #region DASH

        private bool _isDashing = false;

        public void PerformDash(DashAbilityConfig config)
        {
            if (_isDashing || _config == null) return;

            var targetDirection = _directionControl.magnitude < 0.1f ? m_viewTransform.forward.normalized : _directionControl;

            StartCoroutine(DashCoroutine(config, targetDirection));
        }

        private IEnumerator DashCoroutine(DashAbilityConfig config, Vector3 dir)
        {
            _isDashing = true;

            float stepDistance = config.DashDistance / config.DashSteps;

            for (int i = 0; i < config.DashSteps; i++)
            {
                m_characterController.Move(dir * stepDistance);
                yield return null;
            }

            _isDashing = false;
        }

        #endregion
    }
}
