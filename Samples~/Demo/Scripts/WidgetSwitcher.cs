using UnityEngine;

public class WidgetSwitcher : MonoBehaviour
{
    private int activeWidgetIndex;

    public int ActiveWidgetIndex { get { return activeWidgetIndex; } }

    public void OnEnable()
    {
        SetActiveWidgetIndex(0);
    }

    public bool HideAllWidgets()
    {
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if(!transform.GetChild(i).GetComponent<WidgetSwitcher>())
                transform.GetChild(i).gameObject.SetActive(false);
            }
            return true;
        }
        return false;
    }

    public void SetActiveWidgetIndex(int index)
    {
        activeWidgetIndex = index;
        if (index < 0) index = 0;
        if (index >= transform.childCount) index = transform.childCount - 1;
        
        if(HideAllWidgets())
        {
            transform.GetChild(index).gameObject.SetActive(true);
        }
    }
}