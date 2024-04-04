using UnityEngine;

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(Rigidbody))]
public class ExistingAttributesDemo : MonoBehaviour
{
    [Header("I am a header")]

    [SerializeField]                    private float iAmAPrivateField;
    [HideInInspector]                   public float iAmAPublicField;

    [Space(30)]

    [Tooltip("Booh")] [Range(1, 100)]   public float speed;
    [Multiline(3)]                      public string multiLine;
    [TextArea(2, 5)]                    public string textArea;
}

