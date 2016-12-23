using System;

public class NGLoggerAttribute : Attribute
{
	public readonly string	tag;

	public	NGLoggerAttribute(string tag)
	{
		this.tag = tag;
	}
}