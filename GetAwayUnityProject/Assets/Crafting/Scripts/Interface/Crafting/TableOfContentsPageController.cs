using UnityEngine;
using System.Collections;

public class TableOfContentsPageController : MonoBehaviour {

	public MeshRenderer bookPageMesh;

	// Use this for initialization
	void Start () {
		string pageTexturePath = "Crafting/page-toc";
		bookPageMesh.material.mainTexture = (Texture2D) Resources.Load(pageTexturePath);
	}
}
