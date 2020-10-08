using UnityEngine;

public class CameraTargetFollower : MonoBehaviour {

    public Transform Target;

	private void Update () {
		if(Target!=null)
        {
            transform.position = new Vector3(Target.position.x, Target.position.y,transform.position.z);
        }
	}
}
