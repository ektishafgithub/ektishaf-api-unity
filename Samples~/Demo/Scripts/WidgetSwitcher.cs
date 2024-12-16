using UnityEngine;

public class WidgetSwitcher : MonoBehaviour
{
    private int activeWidgetIndex;

    public int ActiveWidgetIndex { get { return activeWidgetIndex; } }

    private void OnEnable()
    {
        SetActiveWidgetIndex(0);    
    }

    public void SetActiveWidgetIndex(int index)
    {
        activeWidgetIndex = index;
        int children = transform.childCount;
        if (children > 0)
        {
            if (index < 0) index = 0;
            if (index >= children) index = children - 1;

            for (int i = 0; i < children; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                child.SetActive(false);
            }

            transform.GetChild(index).gameObject.SetActive(true);
        }
    }
}
