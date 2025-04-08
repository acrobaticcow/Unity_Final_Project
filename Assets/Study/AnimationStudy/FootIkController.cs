using UnityEngine;

public class FootIkController : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public Transform body;
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		transform.position = body.position;
	}
}
