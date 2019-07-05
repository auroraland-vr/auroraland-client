using UnityEngine;
using UnityEngine.Events;
using Fingo;

[System.Serializable]
public class ButtonEvent
{
    public UnityEvent OnButtonDown;
    public UnityEvent OnButtonUp;
    public UnityEvent OnButtonClick;

    public void Initialize()
    {
        if (OnButtonDown == null) OnButtonDown = new UnityEvent();
        if (OnButtonUp == null) OnButtonUp = new UnityEvent();
        if (OnButtonClick == null) OnButtonClick = new UnityEvent();
    }
}

public class FingoController : MonoBehaviour
{
    public ButtonEvent[] buttonEvents = new ButtonEvent[3];
    private bool[] buttonPressed = new bool[3];

    void Start()
    {
        for(int i = 0; i < 3; ++i)
        {
            buttonEvents[i].Initialize();
            buttonPressed[i] = false;
        }
    }


    void Update()
    {
        Controller c = FingoManager.Instance.GetController(HandType.Right);
        if (c != null)
        {
            transform.rotation = c.GetRotation();
            for(int i = 0; i < 3; ++i)
            {
                Button button = c.GetButton(i);
                if (button.Pressed)
                {
                    if (buttonPressed[i]) buttonEvents[i].OnButtonDown.Invoke();
                    else buttonEvents[i].OnButtonClick.Invoke();
                    buttonPressed[i] = true;
                }
                else
                {
                    if (buttonPressed[i]) buttonEvents[i].OnButtonUp.Invoke();
                    buttonPressed[i] = false;
                }
            }
        }
    }
}
