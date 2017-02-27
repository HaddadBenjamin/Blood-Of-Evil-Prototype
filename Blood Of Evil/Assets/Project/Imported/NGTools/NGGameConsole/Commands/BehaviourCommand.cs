using NGTools.NGRemoteScene;
using System;
using System.Reflection;

namespace NGTools.NGGameConsole
{
	public class BehaviourCommand : CommandNode
	{
		public	BehaviourCommand(string name, string description, object instance) : base(instance, name, description)
		{
			Type			t = this.instance.GetType();
			FieldInfo[]		fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			PropertyInfo[]	properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
			MethodInfo[]	methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

			for (int i = 0; i < fields.Length; i++)
			{
				CommandAttribute[]	attr = fields[i].GetCustomAttributes(typeof(CommandAttribute), false) as CommandAttribute[];
				if (attr.Length > 0)
					InternalNGDebug.LogError(Errors.CLI_ForbiddenCommandOnField, "Attribute Command can not be assigned on field \"" + fields[i].Name + "\" from \"" + instance.GetType().Name + "\" alias \"" + name + "\".");
			}

			for (int i = 0; i < properties.Length; i++)
			{
				CommandAttribute[]	attr = properties[i].GetCustomAttributes(typeof(CommandAttribute), false) as CommandAttribute[];
				if (attr.Length > 0)
				{
					if (attr[0].name.Contains(NGCLI.CommandsArgumentsSeparator.ToString()) == true)	
						InternalNGDebug.LogError(Errors.CLI_ForbiddenCharInName, "Attribute Command has invalid char '" + NGCLI.CommandsArgumentsSeparator + "' in its name \"" + attr[0].name + "\" on field \"" + properties[i].Name + "\" from \"" + instance.GetType().Name + "\".");

					try
					{
						this.AddChild(new PropertyCommand(attr[0], properties[i], this.instance));
					}
					catch (NotSupportedPropertyTypeException)
					{
						InternalNGDebug.LogError(Errors.CLI_UnsupportedPropertyType, "Property \"" + properties[i].Name + "\" of type \"" + properties[i].PropertyType.Name + "\" from \"" + instance.GetType().Name + "\" alias \"" + name + "\" is unsupported.");
					}
				}
			}

			for (int i = 0; i < methods.Length; i++)
			{
				CommandAttribute[]	attr = methods[i].GetCustomAttributes(typeof(CommandAttribute), false) as CommandAttribute[];
				if (attr.Length > 0)
				{
					if (methods[i].ReturnType != typeof(string))
						InternalNGDebug.Log(Errors.CLI_MethodDoesNotReturnString, "Command \"" + attr[0].name + "\" from \"" + instance.GetType().Name + "\" alias \"" + name + "\" is a method and should return a string to be exposed.");
					else
						this.AddChild(new MethodCommand(attr[0], methods[i], this.instance));
				}
			}
		}
	}
}