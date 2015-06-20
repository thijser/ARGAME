using Meta;
using UnityEngine;

/// <summary>
/// An example to use camera feed via code.
/// </summary>
/// <seealso cref="T:UnityEngine.MonoBehaviour"/>
public class CameraFeedExample : MonoBehaviour
{
    public int SourceDevice = 1;  // for color feed texture set value = 0, for depth set value = 1, for ir set value = 2;

    /* WARNING: the depthdata is converted to rgb space for display purposes. The values in the depth texture do not represent the actual depth value*/
    public MeshRenderer RenderTarget;

    public Texture2D CameraTexture;

    public void Start()
    {
        // sanity check. espcially if intended to use in Awake() or before that 
        if (DeviceTextureSource.Instance != null && MetaCore.Instance != null)
        {
            DeviceTextureSource.Instance.registerTextureDevice(SourceDevice);
        }

        // get the texture
        if (DeviceTextureSource.Instance.IsDeviceTextureRegistered(SourceDevice))
        {
            CameraTexture = DeviceTextureSource.Instance.GetDeviceTexture(SourceDevice);

            // if a rendering Target is set. Display it
            if (RenderTarget != null && RenderTarget.material != null)
            {
                if (DeviceTextureSource.Instance != null && DeviceTextureSource.Instance.enabled)
                {
                    RenderTarget.material.mainTexture = CameraTexture;
                }
            }
        }
        else
        {
            Debug.LogError("trying to access unregistered device texture");
        }
    }

    public void OnDestroy()
    {
        // Sanity check. Espcially if intended to use in Awake() or before that 
        if (DeviceTextureSource.Instance != null && MetaCore.Instance != null)
        {
            DeviceTextureSource.Instance.unregisterTextureDevice(SourceDevice);
        }
    }
}
