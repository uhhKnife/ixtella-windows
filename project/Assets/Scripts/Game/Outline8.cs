using System.Collections.Generic;
using UnityEngine;

public class Outline8 : ModifiedShadow
{
	public override void ModifyVertices(List<UIVertex> verts)
	{
		if (!IsActive())
		{
			return;
		}
		int needed = verts.Count * 9;
		if (verts.Capacity < needed)
		{
			verts.Capacity = needed;
		}
		int count = verts.Count;
		int start = 0;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					int end = start + count;
					ApplyShadow(verts, effectColor, start, end, effectDistance.x * (float)i, effectDistance.y * (float)j);
					start = end;
				}
			}
		}
	}
}
