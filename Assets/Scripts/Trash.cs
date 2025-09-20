using UnityEngine;

public class Trash : MonoBehaviour
{
    public void PickUp()
    {
        Destroy(gameObject);
    }
}
