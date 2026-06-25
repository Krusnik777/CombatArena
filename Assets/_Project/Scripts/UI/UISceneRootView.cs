using UnityEngine;

namespace UI
{
    public class UISceneRootView : MonoBehaviour
    {
        [field: SerializeField] public Transform ScreensTransform { get; private set; }
        [field: SerializeField] public Transform PopupsTransform { get; private set; }
        [field: Header("Tooltips")]
        [field: SerializeField] public Canvas TooltipsCanvas { get; private set; }
        [field: SerializeField] public Vector2 TooltipOffset  { get; private set; } = new Vector2(10, -10);
    }
}
