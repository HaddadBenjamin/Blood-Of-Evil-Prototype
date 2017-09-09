#pragma strict

static function		GetChildrenGameObject(parentGameObject : GameObject, child : String)
{
	for(var i : int = 0; i < parentGameObject.transform.childCount; i++)
	{
    	if (parentGameObject.transform.GetChild(i).name == child)
    	         return parentGameObject.transform.GetChild(i).gameObject;
   	}	
   	return null;
}

//return le transform d'un fils si ca a marche et null si ce n'est pas le cas
static function		FindChildTransform(parent : Transform, childName : String) : Transform
{
	if (parent.name.Equals(childName)) return parent;
    for (var child : Transform in parent)
    {
    	var result : Transform = FindChildTransform(child, childName);
    	if (result != null) return result;
    }
    return null;
}