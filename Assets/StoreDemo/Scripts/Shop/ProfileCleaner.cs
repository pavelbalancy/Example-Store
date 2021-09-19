using UnityEngine;

[RequireComponent(typeof(MyButton))]
public class ProfileCleaner : MonoBehaviour
{
    private MyButton _button;

    private void Awake()
    {
        _button = GetComponent<MyButton>();
        _button.onClick.AddListener(ResetProfileClicked);
    }

    private void ResetProfileClicked()
    {
        GameProgress.ResetProgress();
    }
}
