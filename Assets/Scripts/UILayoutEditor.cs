using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UILayoutEditor : MonoBehaviour {
	public int CurrentPage;
	public PanelLayout[] Panels;

	void Awake(){
		Debug.Log("Editor causes this Awake");
	}

	void Update(){
		Debug.Log("Editor causes this Update");
	}
		
}


[System.Serializable][ExecuteInEditMode]
public class PanelLayout : MonoBehaviour{
	public string PanelName;
	public List<GameObject> Elements = new List<GameObject>();
	public List<Style> ElementStyle = new List<Style>();
	void Awake(){
		foreach (var element in this.Elements) {
			GetAllElementsInHierachy(element.transform);
		}
	}

	PanelLayout(string PanelName, List<GameObject> Elements){
		this.PanelName = PanelName;
		this.Elements = Elements;
		foreach (var element in this.Elements) {
			GetAllElementsInHierachy(element.transform);
		}
	}
	void GetAllElementsInHierachy(Transform parent){
		foreach (Transform child in parent) {
			Style style = new Style (child.gameObject);
			ElementStyle.Add (style);
			GetAllElementsInHierachy (child);
		}
	}
}

[System.Serializable]
public class Style{
	public GameObject Element;
	public string StyleName;
	public Style(GameObject element, string styleName = ""){
		StyleName = styleName;
		Element = element;
	}
}
[System.Serializable]
public class TextStyle:Style{
	public Font Font;
	public Color FontColor;
	public int FontSize;
	public TextStyle(GameObject element, string styleName = ""):base(element, styleName){
		StyleName = styleName;
		Element = element;
	}
}

[System.Serializable]
public class ImageStyle:Style{
	public Sprite Sprite;
	public Color Color;
	public ImageStyle(GameObject element, string styleName = ""):base(element, styleName){
		StyleName = styleName;
		Element = element;
	}
}

[System.Serializable]
public class ButtonStyle:Style{
	public float NormalDepth;
	public float PressedDepth;
	public float HoverDepth;
	public Sprite NormalSprite;
	public Sprite PressedSprite;
	public Sprite HoverSprite;
	public ButtonStyle(GameObject element, string styleName = ""):base(element, styleName){
		StyleName = styleName;
		Element = element;
	}
}