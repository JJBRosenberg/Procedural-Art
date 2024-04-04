using UnityEngine;

public class CreateStacksFromCode : MonoBehaviour {

	public GameObject[] prefabs;

	// Use this for initialization
	void Start () {

		for (int i = 0; i < 5; i++)
		{
			StackUtility.CreateStack(
					"Stack" + i,					//name
					new Vector3(-7 + 3.5f*i, 0, 0),	//position
					Random.Range(0,360),            //rotation
					Random.Range(5, 15),			//count
					prefabs,
					Random.Range(2, 3),             //start scale
					Random.Range(1,2),				//end scale
					Random.value/4,                 //scale variation
					Random.value/4,                 //random position offset
					Random.Range(0, 40),            //start rotation variance
					Random.Range(0, 10),			//object rotation variance 
					10,                             //object rotation offset
					0,								//pYOffset
					false							//Create compound collider
					);
		}
	}
	
}
