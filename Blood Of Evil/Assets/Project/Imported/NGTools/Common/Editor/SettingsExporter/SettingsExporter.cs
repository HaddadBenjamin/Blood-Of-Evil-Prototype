using NGTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace NGToolsEditor
{
	using UnityEngine;

	public static class SettingsExporter
	{
		public const BindingFlags	SearchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		public sealed class Node
		{
			public enum Options
			{
				Normal,
				Hidden
			}

			public bool		include = true;
			public Options	options = Options.Normal;

			public string		name;
			public string		value;
			public List<Node>	children = new List<Node>();

			public readonly Node	parent;

			public	Node(Node parent, string name)
			{
				this.parent = parent;
				this.name = name;
			}

			public void	Serialize(BinaryWriter sw)
			{
				if (this.include == true)
				{
					sw.Write(this.name);
					sw.Write(this.value ?? string.Empty);

					int	count = 0;

					for (int i = 0; i < this.children.Count; i++)
					{
						if (this.children[i].include == true)
							++count;
					}

					sw.Write((byte)count);

					for (int i = 0; i < this.children.Count; i++)
					{
						if (this.children[i].include == true)
							this.children[i].Serialize(sw);
					}
				}
			}

			public void	Deserialize(BinaryReader sw)
			{
				this.name = sw.ReadString();
				this.value = sw.ReadString();

				int	count = sw.ReadByte();

				this.children.Capacity = count;

				for (int i = 0; i < count; ++i)
				{
					Node	child = new Node(this, string.Empty);
					child.Deserialize(sw);
					this.children.Add(child);
				}
			}
		}

		[Serializable]
		public sealed class ExportNode
		{
			public string		name;
			public string		value;
			public ExportNode[]	children;

			public	ExportNode(Node node)
			{
				this.name = node.name;
				this.value = node.value;

				int	count = 0;

				for (int i = 0; i < node.children.Count; i++)
				{
					if (node.children[i].include == true)
						++count;
				}

				this.children = new ExportNode[count];

				for (int i = 0, j = 0; i < node.children.Count; i++)
				{
					if (node.children[i].include == true)
					{
						this.children[j] = new ExportNode(node.children[i]);
						++j;
					}
				}
			}
		}

		public static Node	Collect(params object[] instances)
		{
			Node	root = new Node(null, "ROOT");

			for (int i = 0; i < instances.Length; i++)
				root.children.Add(SettingsExporter.Browse(null, instances[i]));

			return root;
		}

		private static Node	Browse(Node parent, object instance)
		{
			Type	t = instance.GetType();
			Node	node = new Node(parent, t.FullName);

			foreach (FieldInfo field in Utility.EachFieldHierarchyOrdered(t, typeof(object), SettingsExporter.SearchFlags))
			{
				if (field.IsDefined(typeof(ExportableAttribute), true) == true)
					node.children.Add(SettingsExporter.Browse(node, field, field.GetValue(instance)));
			}

			foreach (PropertyInfo property in Utility.EachPropertyHierarchyOrdered(t, typeof(object), SettingsExporter.SearchFlags))
			{
				if (property.IsDefined(typeof(ExportableAttribute), true) == true)
					node.children.Add(SettingsExporter.Browse(node, property, property.GetValue(instance, null)));
			}

			if (t.IsDefined(typeof(HideFromExportAttribute), true) == true)
				node.options = Node.Options.Hidden;

			return node;
		}

		private static Node	Browse(Node parent, FieldInfo field, object instance)
		{
			Node	node = new Node(parent, field.Name);

			if (instance == null)
				return node;

			if (field.FieldType.IsPrimitive == true ||
				field.FieldType == typeof(string))
			{
				ExportableAttribute[]	attributes = field.GetCustomAttributes(typeof(ExportableAttribute), true) as ExportableAttribute[];

				if (attributes.Length > 0 && attributes[0].fields != null)
				{
					for (int j = 0; j < attributes[0].fields.Length; j++)
					{
						FieldInfo	fi = field.FieldType.GetField(attributes[0].fields[j]);

						InternalNGDebug.Assert(fi != null, "Field \"" + attributes[0].fields[j] + "\" was not found in type " + field.FieldType + ".");

						node.children.Add(SettingsExporter.Browse(node, fi, fi.GetValue(instance)));
					}
				}

				node.value = instance.ToString();
			}
			else if (typeof(Object).IsAssignableFrom(field.FieldType) == true)
			{
				Object	o = instance as Object;

				if (o != null)
				{
					string	path = AssetDatabase.GetAssetPath(o);
					node.value = AssetDatabase.AssetPathToGUID(path);
				}
			}
			else if (field.FieldType.IsArray == true ||
					 instance is IEnumerable)
			{
				IEnumerable	array = instance as IEnumerable;
				int			count = 0;

				foreach (var item in array)
				{
					if (item.GetType().IsDefined(typeof(ExcludeFromExportAttribute), true) == false)
					{
						node.children.Add(SettingsExporter.Browse(node, item));
						++count;
					}
				}
				node.value = count.ToString();
			}
			else if (field.FieldType.IsClass == true ||
					 field.FieldType.IsStruct() == true)
			{
				ExportableAttribute[]	attributes = field.GetCustomAttributes(typeof(ExportableAttribute), true) as ExportableAttribute[];

				if (attributes.Length > 0 && attributes[0].fields != null)
				{
					for (int j = 0; j < attributes[0].fields.Length; j++)
					{
						FieldInfo	fi = field.FieldType.GetField(attributes[0].fields[j]);

						InternalNGDebug.Assert(fi != null, "Field \"" + attributes[0].fields[j] + "\" was not found in type " + field.FieldType + ".");

						node.children.Add(SettingsExporter.Browse(node, fi, fi.GetValue(instance)));
					}
				}

				foreach (FieldInfo subField in Utility.EachFieldHierarchyOrdered(field.FieldType, typeof(object), SettingsExporter.SearchFlags))
				{
					if (subField.IsDefined(typeof(ExportableAttribute), true) == true)
						node.children.Add(SettingsExporter.Browse(node, subField, subField.GetValue(instance)));
				}

				foreach (PropertyInfo subProperty in Utility.EachPropertyHierarchyOrdered(field.FieldType, typeof(object), SettingsExporter.SearchFlags))
				{
					if (subProperty.IsDefined(typeof(ExportableAttribute), true) == true)
						node.children.Add(SettingsExporter.Browse(node, subProperty, subProperty.GetValue(instance, null)));
				}
			}

			if (field.IsDefined(typeof(HideFromExportAttribute), true) == true)
				node.options = Node.Options.Hidden;

			return node;
		}

		private static Node	Browse(Node parent, PropertyInfo property, object instance)
		{
			Node	node = new Node(parent, property.Name);

			if (instance == null)
				return node;

			if (property.PropertyType.IsPrimitive == true ||
				property.PropertyType == typeof(string))
			{
				node.value = instance.ToString();
			}
			else if (property.PropertyType.IsArray == true ||
					 instance is IEnumerable)
			{
				IEnumerable	array = instance as IEnumerable;
				int			count = 0;

				foreach (var item in array)
				{
					if (item.GetType().IsDefined(typeof(ExcludeFromExportAttribute), true) == false)
					{
						node.children.Add(SettingsExporter.Browse(node, item));
						++count;
					}
				}
				node.value = count.ToString();
			}
			else if (property.PropertyType.IsClass == true ||
					 property.PropertyType.IsStruct() == true)
			{
				ExportableAttribute[]	attributes = property.GetCustomAttributes(typeof(ExportableAttribute), true) as ExportableAttribute[];

				if (attributes.Length > 0 && attributes[0].fields != null)
				{
					for (int j = 0; j < attributes[0].fields.Length; j++)
					{
						FieldInfo	fi = property.PropertyType.GetField(attributes[0].fields[j]);

						InternalNGDebug.Assert(fi != null, "Field \"" + attributes[0].fields[j] + "\" was not found in type " + property.PropertyType + ".");

						node.children.Add(SettingsExporter.Browse(node, fi, fi.GetValue(instance)));
					}
				}

				foreach (FieldInfo subField in Utility.EachFieldHierarchyOrdered(property.PropertyType, typeof(object), SettingsExporter.SearchFlags))
				{
					if (subField.IsDefined(typeof(ExportableAttribute), true) == true)
						node.children.Add(SettingsExporter.Browse(node, subField, subField.GetValue(instance)));
				}

				foreach (PropertyInfo subProperty in Utility.EachPropertyHierarchyOrdered(property.PropertyType, typeof(object), SettingsExporter.SearchFlags))
				{
					if (subProperty.IsDefined(typeof(ExportableAttribute), true) == true)
						node.children.Add(SettingsExporter.Browse(node, subProperty, subProperty.GetValue(instance, null)));
				}
			}

			if (property.IsDefined(typeof(HideFromExportAttribute), true) == true)
				node.options = Node.Options.Hidden;

			return node;
		}

		public static bool	Export(IEnumerable<object> instances, Node node, string file)
		{
			foreach (object instance in instances)
			{
				if (instance is ISettingExportable)
					(instance as ISettingExportable).PreExport();
			}

			try
			{
				using (StreamWriter	sr = new StreamWriter(file))
				using (BinaryWriter	sw = new BinaryWriter(sr.BaseStream))
				{
					node.Serialize(sw);
				}
			}
			catch (Exception ex)
			{
				InternalNGDebug.LogException(ex);
				return false;
			}

			return true;
		}

		public static void	Import(string file)
		{
			Node	root = new Node(null, string.Empty);

			using (StreamReader sr = new StreamReader(file))
			using (BinaryReader br = new BinaryReader(sr.BaseStream))
			{
				root.Deserialize(br);
			}

			ImportSettingsWizard	isw = ScriptableWizard.GetWindow<ImportSettingsWizard>(true, "Import Settings", true);

			isw.Init(root);
			//OutputNode(root);
		}

		private static void	OutputNode(SettingsExporter.ExportNode node, int depth = 0)
		{
			Debug.Log(new string('	', depth) + node.name + "=" + node.value);

			for (int i = 0; i < node.children.Length; i++)
				OutputNode(node.children[i], depth + 1);
		}

		private static void	OutputNode(SettingsExporter.Node node, int depth = 0)
		{
			Debug.Log(new string('	', depth) + node.name + "=" + node.value);

			for (int i = 0; i < node.children.Count; i++)
				OutputNode(node.children[i], depth + 1);
		}

		private static void	StringifyNode(StringBuilder buffer, SettingsExporter.Node node, int depth = 0)
		{
			if (node.include == true)
			{
				buffer.Append('	', depth);
				buffer.Append(node.name);
				buffer.Append('=');
				if (node.value != null)
					buffer.AppendLine(node.value);
				else
					buffer.AppendLine();
			}

			for (int i = 0; i < node.children.Count; i++)
				SettingsExporter.StringifyNode(buffer, node.children[i], depth + 1);
		}
	}
}