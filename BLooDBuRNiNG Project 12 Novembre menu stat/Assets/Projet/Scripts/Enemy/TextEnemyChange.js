#pragma strict

var textColor : Color;

function Start() 
{
	var myTextMesh = (GetComponent(TextMesh) as TextMesh);
    GetComponent(TextMesh).text = gameObject.name;
	renderer.material.SetColor("_Color", textColor);
}