using UnityEngine;
using UnityEditor;
using System;

/**
 * The TexureProcessor is a utility class which automagically tries to map imported textures to a material
 * slot on a material which a matching name. If no such material is found, one is created. Note that this
 * only works for Standard materials from the default non scriptable rendering pipeline.
 * (But feel free to adapt it for the SRP, upload it to the asset store and score ;)).
 * 
 * How to use it:
 *	- select IDS/Turn texture processing on from the main menu
 *	- import a bunch of textures matching the postfixes below into your project
 *	- Watch your materials update or appear in the root of your Asset folder
 *	- select IDS/Turn texture processing off from the main menu
 * 
 * @author J.C. Wichman, Inner Drive Studios.
 */
public class TextureProcessor : AssetPostprocessor
{
	//When a texture is imported we look for a match with any of the substrings in the first array,
	//eg _nrm or _normal. If a texturename ends in one of these strings, we try to locate a material
	//(see below) and then store the texture in the material/shader slot denoted by the second string (eg _BumpMap)
    private static readonly (string[], string)[] _mappings =
    {
        (new string[] {"_nrm", "_normal"},		"_BumpMap"),
        (new string[] {"_alb", "_albedo"},		"_MainTex"),
        (new string[] {"_m", "_metallic"},		"_MetallicGlossMap"),
        (new string[] {"_h", "_height"},		"_ParallaxMap"),
        (new string[] {"_ao", "_occlusion"},	"_OcclusionMap"),
    };

	//unfortunately normap maps require special treatment, since we also want to automagically switch their texture types on import
    private static readonly int normalSlotIndex = 0;

	//importing a 1000 textures might wreak havoc on your project if you are aware of this automated import process, so 
	//by default we are off. Might be even better to make this a right-click-asset-folder action, but haven't looked into
	//that yet.
    private static bool textureProcessingOn = false;

    [MenuItem("IDS/Turn texture processing on")]
    private static void turnTextureProcessingOn()
    {
        textureProcessingOn = true;
    }

    [MenuItem("IDS/Turn texture processing off")]
    private static void turnTextureProcessingOff()
    {
        textureProcessingOn = false;
    }

    void OnPreprocessTexture()
    {
        if (!textureProcessingOn) return; 

		//texture type can only be changed in the preprocess phase, so check whether the imported texture's name
		//matches with one of the normal map extensions and if so, switch it's texture type.
        string assetPathLower = assetPath.ToLower();

		foreach (string postFix in _mappings[normalSlotIndex].Item1)
        {
            if (assetPathLower.Contains(postFix))
            {
                Debug.Log("Auto changed texture type to normal for " + assetPath);
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.textureType = TextureImporterType.NormalMap;
                break;
            }
        }
    }

    void OnPostprocessTexture(Texture2D pTexture)
    {
        if (!textureProcessingOn) return;

		//after importing a material, see if it matches with any of the specified postfixes in _mappings
		//and if so get the material name and material slot for it
        string assetPathLower = assetPath.ToLower();
        string materialName = null;
        string slot = null;

        foreach ((string[], string) mapping in _mappings)
        {
            foreach (string postFix in mapping.Item1)
            {
                if (assetPathLower.Contains(postFix))
                {
                    materialName = getMaterialName(assetPath, postFix);
                    slot = mapping.Item2;
                    break;
                }
            }
            if (materialName != null) break;
        }

        if (materialName == null) return;

		//if we were able to establish a material name, try to find this material in the asset database
        Debug.Log("Looking for material " + materialName);
        string[] guids = AssetDatabase.FindAssets("t:material " + materialName);

		//if there was no such material, create it on the fly
        Material material = null;
        if (guids.Length == 0)
        {
            Debug.Log("Creating new material... :" + materialName);
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, "Assets/"+materialName+".mat");
        } 
		//if there was 1 material with this name, load it
        else if (guids.Length == 1)
        {
            Debug.Log("Loading material... :"+materialName);
            material = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guids[0]));
        } else
		{
			//let's play safe since we have no idea what to do here
			return;
		}

		//why? Yes good question. Unity that is why. 
		//Setting a material texture slot should simply work, except that it doesn't
        UnityEditor.EditorApplication.delayCall += () =>
            {
                Debug.Log("Updating material");
                material.SetTexture(slot, AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath));
            };
    }

    private string getMaterialName(string pAssetPath, string pPostfix)
    {
        string materialName = pAssetPath.Substring(pAssetPath.LastIndexOf("/") + 1);
        materialName = materialName.Substring(0, materialName.ToLower().IndexOf(pPostfix));
        return materialName;
    }
}
