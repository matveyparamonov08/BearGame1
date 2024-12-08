using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras;
    private int currentCameraIndex = 0;
    public MonoBehaviour[] scriptsToDisable;
    public GameObject uiElement;
    
    void Start()
    {
        SwitchCamera(currentCameraIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentCameraIndex++;
            if (currentCameraIndex >= cameras.Length)
            {
                currentCameraIndex = 0;
            }
            SwitchCamera(currentCameraIndex);
        }
    }

    void SwitchCamera(int index)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = (i == index);
        }

        if (index == 0)
        {
            EnableScripts();
            LockCursor();
            HideUIElement();
        }
        else
        {
            DisableScripts();
            UnlockCursor();
            ShowUIElement();
        }
    }

    void DisableScripts()
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }
    }

    void EnableScripts()
    {
        foreach (var script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }
        
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void HideUIElement()
    {
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }

    void ShowUIElement()
    {
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }
}