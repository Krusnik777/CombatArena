using Loading;
using UnityEngine;

namespace CombatArena.Game.Root
{
    public class UIRootView : MonoBehaviour
    {
        [field: SerializeField] public LoadingScreen LoadingScreen { get; private set; }
        [SerializeField] private Transform _uiSceneContainer;

        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();

            sceneUI.transform.SetParent(_uiSceneContainer, false);
        }

        private void ClearSceneUI()
        {
            var childCount = _uiSceneContainer.childCount;

            for (int i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.GetChild(i).gameObject);
            }
        }

    }
}
