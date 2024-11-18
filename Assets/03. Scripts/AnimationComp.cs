using UnityEngine;

public class AnimationComp : MonoBehaviour
{
    public void OnDieEnd()
    {
        GetComponent<Animator>().enabled = false;

        Invoke("Foll", 3f);
    }

    void Foll()
    {
        GetComponentInChildren<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;

        if (transform.position.y < 0)
        {
            Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z);
            transform.position = pos;
        }
    }
}