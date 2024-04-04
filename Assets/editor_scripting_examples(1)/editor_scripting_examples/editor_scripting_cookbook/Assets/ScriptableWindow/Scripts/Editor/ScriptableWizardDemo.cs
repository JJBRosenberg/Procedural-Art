using UnityEditor;
using UnityEngine;

public class ScriptableWizardDemo : ScriptableWizard
{
	public string myValue1;
	public bool myValue2;
	public int myValue3;

	[MenuItem("Custom menu/Scriptable wizard demo")]
	private static void init()
	{
		//Use this GetWindow<ScriptableWizardDemo>();
		//Or this if you want to add more options:
		ScriptableWizard.DisplayWizard<ScriptableWizardDemo>("Wanna see some magic?", "DO IT DO IT DO IT!", "NOOOOOO!");
	}

	private void OnWizardCreate()
	{
		Debug.Log("ABACADABRA, SHAZAAAM!");
	}

	private void OnWizardOtherButton()
	{
		Debug.Log("Pooooooock poooooock!");
	}



}
