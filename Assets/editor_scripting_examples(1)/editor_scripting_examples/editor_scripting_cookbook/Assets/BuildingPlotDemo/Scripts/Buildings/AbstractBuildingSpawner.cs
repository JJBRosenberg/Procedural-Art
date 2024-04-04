using UnityEngine;

/**
 * Override this class to initialize your own building, see SimpleBuildingSpawner
 */
public abstract class AbstractBuildingSpawner : MonoBehaviour
{
	public abstract GameObject Initialize(Vector3 position, Vector2 plotSize);
}
