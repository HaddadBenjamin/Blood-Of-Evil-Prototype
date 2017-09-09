#pragma strict

function Start () {
}
var x : float = 6.0;
var y : float = 6.0;
var z : float = 6.0;
function Update () {
transform.Rotate(Vector3(x, y, z));
}