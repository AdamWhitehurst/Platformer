using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayButtonController : MonoBehaviour
{
	void TextMove()
	{
		GetComponent<Text> ().rectTransform.position = new Vector3 (GetComponent<Text> ().rectTransform.position.x, GetComponent<Text> ().rectTransform.position.y - 5, GetComponent<Text> ().rectTransform.position.z);
		Debug.Log ("Click");
	}
}
