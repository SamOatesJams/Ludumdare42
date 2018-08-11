using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
	void FixedUpdate ()
	{
	    if (Camera.main == null)
	    {
            return;
	    }
	    transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
