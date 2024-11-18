using UnityEngine;

public class AnimationComp : MonoBehaviour
{
    public void OnDieEnd()
    {
        GetComponent<Animator>().enabled = false;
    }
}