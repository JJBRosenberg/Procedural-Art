using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookBomb : MonoBehaviour {

	private void Update()
	{
		if (!Input.GetMouseButtonDown(0)) return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit info;

		if (Physics.Raycast(ray, out info))
		{
			if (info.rigidbody != null)
			{
				Debug.Log("Adding explosion force");
				info.rigidbody.AddExplosionForce(15, info.point, 4,0.1f, ForceMode.VelocityChange);
			}
		}
	}
}
