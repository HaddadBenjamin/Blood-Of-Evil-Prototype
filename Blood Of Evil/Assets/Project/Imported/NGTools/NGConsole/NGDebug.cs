using NGTools;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using UnityEngine;
using DConditional = System.Diagnostics.ConditionalAttribute;

public partial class NGDebug
{
	[DConditional("UNITY_EDITOR")]
	public static void	LogHierarchy(GameObject gameObject)
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.MultiContextsStartChar);

		if (gameObject != null)
		{
			Transform	transform = gameObject.transform;

			while (transform != null)
			{
				buffer.Append(transform.GetInstanceID());
				buffer.Append(InternalNGDebug.MultiContextsSeparator);

				transform = transform.parent;
			}

			// Remove last separator.
			buffer.Length -= 1;
		}

		buffer.Append(InternalNGDebug.MultiContextsEndChar);
		Debug.Log(buffer.ToString());
	}

	[DConditional("UNITY_EDITOR")]
	public static void	LogHierarchy(UnityEngine.Component component)
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.MultiContextsStartChar);

		if (component != null)
		{
			Transform	transform = component.transform;

			while (transform != null)
			{
				buffer.Append(transform.GetInstanceID());
				buffer.Append(InternalNGDebug.MultiContextsSeparator);

				transform = transform.parent;
			}

			// Remove last separator.
			buffer.Length -= 1;
		}

		buffer.Append(InternalNGDebug.MultiContextsEndChar);
		Debug.Log(buffer.ToString());
	}

	[DConditional("UNITY_EDITOR")]
	public static void	LogHierarchy(RaycastHit hit)
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.MultiContextsStartChar);

		if (hit.transform != null)
		{
			Transform	transform = hit.transform;

			while (transform != null)
			{
				buffer.Append(transform.GetInstanceID());
				buffer.Append(InternalNGDebug.MultiContextsSeparator);

				transform = transform.parent;
			}

			// Remove last separator.
			buffer.Length -= 1;
		}

		buffer.Append(InternalNGDebug.MultiContextsEndChar);
		Debug.Log(buffer.ToString());
	}

	[DConditional("UNITY_EDITOR")]
	public static void	Log(params Object[] objects)
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.MultiContextsStartChar);

		if (objects.Length > 0)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				if (objects[i] != null)
					buffer.Append(objects[i].GetInstanceID());
				else
					buffer.Append(0);

				buffer.Append(InternalNGDebug.MultiContextsSeparator);
			}

			// Remove last separator.
			buffer.Length -= 1;
		}

		buffer.Append(InternalNGDebug.MultiContextsEndChar);
		Debug.Log(buffer.ToString());
	}

	[DConditional("UNITY_EDITOR")]
	public static void	Log(params RaycastHit[] hits)
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.MultiContextsStartChar);

		if (hits.Length > 0)
		{
			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].collider != null)
					buffer.Append(hits[i].collider.GetInstanceID());
				else
					buffer.Append(0);

				buffer.Append(InternalNGDebug.MultiContextsSeparator);
			}

			// Remove last separator.
			buffer.Length -= 1;
		}

		buffer.Append(InternalNGDebug.MultiContextsEndChar);
		Debug.Log(buffer.ToString());
	}

	[DConditional("UNITY_EDITOR")]
	public static void	LogCollection(IEnumerable<RaycastHit> hits)
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.MultiContextsStartChar);

		foreach (var hit in hits)
		{
			if (hit.collider != null)
				buffer.Append(hit.collider.GetInstanceID());
			else
				buffer.Append(0);

			buffer.Append(InternalNGDebug.MultiContextsSeparator);
		}

		// Remove last separator.
		if (buffer.Length > 0)
			buffer.Length -= 1;

		buffer.Append(InternalNGDebug.MultiContextsEndChar);
		Debug.Log(buffer.ToString());
	}

	[DConditional("UNITY_EDITOR")]
	public static void	LogCollection<T>(IEnumerable<T> objects) where T : Object
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.MultiContextsStartChar);

		foreach (var obj in objects)
		{
			if (obj != null)
				buffer.Append(obj.GetInstanceID());
			else
				buffer.Append(0);

			buffer.Append(InternalNGDebug.MultiContextsSeparator);
		}

		// Remove last separator.
		if (buffer.Length > 0)
			buffer.Length -= 1;

		buffer.Append(InternalNGDebug.MultiContextsEndChar);
		Debug.Log(buffer.ToString());
	}

	private static Dictionary<int, FieldInfo[]>		cachedFieldInfos = new Dictionary<int, FieldInfo[]>();
	private static Dictionary<int, PropertyInfo[]>	cachedPropertiesInfos = new Dictionary<int, PropertyInfo[]>();

	[DConditional("UNITY_EDITOR")]
	public static void	Snapshot(object o)
	{
		NGDebug.Snapshot(o, BindingFlags.Public | BindingFlags.Instance, null);
	}

	[DConditional("UNITY_EDITOR")]
	public static void	Snapshot(object o, Object context)
	{
		NGDebug.Snapshot(o, BindingFlags.Public | BindingFlags.Instance, context);
	}

	[DConditional("UNITY_EDITOR")]
	public static void	Snapshot(object o, BindingFlags bindingFlags)
	{
		NGDebug.Snapshot(o, bindingFlags, null);
	}

	[DConditional("UNITY_EDITOR")]
	public static void	Snapshot(object o, BindingFlags bindingFlags, Object context)
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.DataStartChar);

		if (o != null)
		{
			FieldInfo[]	fields;
			System.Type type = o.GetType();
			int			hash = type.GetHashCode() + bindingFlags.GetHashCode();

			buffer.Append(type.Name);

			if (NGDebug.cachedFieldInfos.TryGetValue(hash, out fields) == false)
			{
				fields = type.GetFields(bindingFlags);
				NGDebug.cachedFieldInfos.Add(hash, fields);
			}

			for (int i = 0; i < fields.Length; i++)
			{
				buffer.Append(InternalNGDebug.DataSeparator);
				buffer.Append(fields[i].Name);
				buffer.Append("=");

				object	v = fields[i].GetValue(o);

				if (fields[i].FieldType.IsPrimitive == true || fields[i].FieldType.IsValueType == true)
					buffer.Append(v);
				else if (v == null)
					buffer.Append("NULL");
				else if (v is System.Array)
					buffer.Append(fields[i].FieldType.GetElementType().Name + "[" + (v as System.Array).Length + "]");
				else if (typeof(IList).IsAssignableFrom(fields[i].FieldType) == true)
					buffer.Append(fields[i].FieldType.Name + "<" + Utility.GetArraySubType(fields[i].FieldType).Name + ">[" + (v as IList).Count + "]");
				else
					buffer.Append(v.ToString().Replace(InternalNGDebug.DataSeparator, InternalNGDebug.DataSeparatorReplace));
			}

			PropertyInfo[]	properties;

			type = o.GetType();

			if (NGDebug.cachedPropertiesInfos.TryGetValue(hash, out properties) == false)
			{
				properties = type.GetProperties(bindingFlags);

				List<PropertyInfo>	filteredProperties = new List<PropertyInfo>(properties.Length);

				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].CanWrite == false ||
						properties[i].CanRead == false ||
						properties[i].GetGetMethod(false) == null ||
						properties[i].GetSetMethod(false) == null)
					{
						continue;
					}

					filteredProperties.Add(properties[i]);
				}

				properties = filteredProperties.ToArray();
				NGDebug.cachedPropertiesInfos.Add(hash, properties);
			}

			for (int i = 0; i < properties.Length; i++)
			{
				buffer.Append(InternalNGDebug.DataSeparator);
				buffer.Append(properties[i].Name);
				buffer.Append("=");

				object	v = properties[i].GetValue(o, null);

				if (properties[i].PropertyType.IsPrimitive == true || properties[i].PropertyType.IsValueType == true)
					buffer.Append(v);
				else if (v == null)
					buffer.Append("NULL");
				else if (v is System.Array)
					buffer.Append(properties[i].PropertyType.GetElementType().Name + "[" + (v as System.Array).Length + "]");
				else if (typeof(IList).IsAssignableFrom(properties[i].PropertyType) == true)
					buffer.Append(properties[i].PropertyType.Name + "<" + Utility.GetArraySubType(properties[i].PropertyType).Name + ">[" + (v as IList).Count + "]");
				else
					buffer.Append(v.ToString().Replace(InternalNGDebug.DataSeparator, InternalNGDebug.DataSeparatorReplace));
			}
		}
		else
			buffer.Append("NULL");

		buffer.Append(InternalNGDebug.DataEndChar);
		Debug.Log(buffer.ToString(), context);
	}

	/// <summary>
	/// <para>Log generating an exception to output a log with a stack trace to the console.</para>
	/// <para>Works on multi-threads.</para>
	/// </summary>
	/// <param name="message"></param>
	[DConditional("UNITY_EDITOR")]
	public static void	MTLog(string message)
	{
		Debug.LogException(new MTLog(message));
	}

	/// <summary>
	/// <para>Log generating an exception to output a log with a stack trace to the console.</para>
	/// <para>Works on multi-threads.</para>
	/// </summary>
	/// <param name="message"></param>
	/// <param name="context"></param>
	[DConditional("UNITY_EDITOR")]
	public static void	MTLog(string message, Object context)
	{
		Debug.LogException(new MTLog(message), context);
	}

	[DConditional("UNITY_EDITOR")]
	public static void	LogTags(string message, params string[] tags)
	{
		StringBuilder	buffer = Utility.sharedBuffer;

		buffer.Length = 0;
		buffer.Append(InternalNGDebug.MultiTagsStartChar);

		buffer.Append(message);
		buffer.Append(InternalNGDebug.MultiTagsSeparator);

		for (int i = 0; i < tags.Length; i++)
		{
			buffer.Append(tags[i]);
			buffer.Append(InternalNGDebug.MultiTagsSeparator);
		}

		// Remove last separator.
		buffer.Length -= 1;
		buffer.Append(InternalNGDebug.MultiTagsEndChar);
		Debug.Log(buffer.ToString());
	}
}

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class MTLog : System.Exception
{
	public override string	Message { get { return this.message; } }

	private string	message;

	public	MTLog(string message)
	{
		this.message = message;
	}
}

public class DataStore : ScriptableObject
{
	public List<object>	array = new List<object>();
}