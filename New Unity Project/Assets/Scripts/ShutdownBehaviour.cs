using UnityEngine;

/// <summary>
/// Behaviour class that shuts down the application if the escape key is
/// pressed. Add this to any game object to make it work.
/// </summary>
public class ShutdownBehaviour : MonoBehaviour
{
    /// <summary>
    /// Shuts down the application if the escape key is pressed.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
