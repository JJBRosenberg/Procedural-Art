using UnityEngine;

[System.Serializable]
public class HighScore
{
	public string name;
	public int score;
}

public class MoreAdvancedDemo : MonoBehaviour
{
	public HighScore[] highscores;

	private void Reset()
	{
		highscores = null;	
	}

	private void OnValidate()
	{
		Debug.Log("High scores changed");
	}
}

