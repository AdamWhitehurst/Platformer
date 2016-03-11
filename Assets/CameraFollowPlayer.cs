using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour {

	public  GameObject playerObject;

	void Update () {
		Vector3 pos = transform.position;
		pos.y = Mathf.Lerp (transform.position.y, playerObject.transform.position.y, 0.1f);
		pos.x = Mathf.Lerp (transform.position.x, playerObject.transform.position.x, 0.1f);
		transform.position = pos;
	}
}
