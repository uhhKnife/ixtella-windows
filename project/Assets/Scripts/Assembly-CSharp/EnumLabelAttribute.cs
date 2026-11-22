using System;
using UnityEngine;

public class EnumLabelAttribute : PropertyAttribute
{
	public Type enumType;

	public EnumLabelAttribute(Type type)
	{
		enumType = type;
	}
}
