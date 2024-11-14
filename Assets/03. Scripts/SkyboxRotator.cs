using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    public float rotateSpeed = 0.5f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed); // 스카이박스 회전
    }
}
