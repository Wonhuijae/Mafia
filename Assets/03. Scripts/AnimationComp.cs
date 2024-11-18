using UnityEngine;

public class AnimationComp : MonoBehaviour
{
    public void OnDieEnd()
    {
        GetComponent<Animator>().enabled = false;

        Invoke("Foll", 5f);
    }

    void Foll()
    {
        GetComponentInChildren<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}