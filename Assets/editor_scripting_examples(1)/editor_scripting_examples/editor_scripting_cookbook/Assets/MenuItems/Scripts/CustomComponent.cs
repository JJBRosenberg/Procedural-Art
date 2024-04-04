using UnityEngine;

namespace MenuItemCookbookRecipes
{

	public class CustomComponent : MonoBehaviour
	{

		////////////////////////////////////////////////////////////////////////////////////
		////	Recipe 5 : A variation of context menu's for your custom component
		////////////////////////////////////////////////////////////////////////////////////

		//The nice thing of this approach is that this doesn't require an editor script 
		//(as long as you don't use editor classes)

		[ContextMenu("Custom option 1")]
		private void customComponentMethod()
		{
			Debug.Log("Custom option 1 executed");
			transform.Translate(Vector3.up);
		}

		//We can also add menu items to fields instead of components

		[ContextMenuItem("Reset to default value", "resetTheMagicWord")]
		public string magicWord;

		private void resetTheMagicWord()
		{
			magicWord = "Pleeeeeze";
		}

	}

}
 