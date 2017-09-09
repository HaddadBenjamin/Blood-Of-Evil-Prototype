#pragma strict

function Start () {
}
var x : float = 6.0;
var y : float = 0;
var z : float = 0;
function Update () {
transform.Translate(Vector3(x, y, z));
}