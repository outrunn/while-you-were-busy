using UnityEngine;

public class UpgradeModalController : MonoBehaviour
{
    private UpgradeModalUI _view;
    private System.Action _onClose;

    public void Setup(int completedDay, System.Action onClose)
    {
        _onClose = onClose;
        _view = GetComponent<UpgradeModalUI>();
        if (_view == null)
            _view = gameObject.AddComponent<UpgradeModalUI>();

        _view.Initialize(completedDay);
    }

    public void Close()
    {
        if (_view != null)
            _view.CloseModal();
        _onClose?.Invoke();
    }
}
