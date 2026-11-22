using System.Collections.Generic;
using UnityEngine;

public class BoxOutline : ModifiedShadow
{
	private const int maxHalfSampleCount = 20;

	[SerializeField]
	[Range(1f, 20f)]
	private int m_halfSampleCountX = 1;

	[SerializeField]
	[Range(1f, 20f)]
	private int m_halfSampleCountY = 1;

	public int halfSampleCountX
	{
		get
		{
			return m_halfSampleCountX;
		}
		set
		{
			m_halfSampleCountX = Mathf.Clamp(value, 1, 20);
			if (graphic != null)
			{
				graphic.SetVerticesDirty();
			}
		}
	}

	public int halfSampleCountY
	{
		get
		{
			return m_halfSampleCountY;
		}
		set
		{
			m_halfSampleCountY = Mathf.Clamp(value, 1, 20);
			if (graphic != null)
			{
				graphic.SetVerticesDirty();
			}
		}
	}

	public override void ModifyVertices(List<UIVertex> verts)
	{
		if (!IsActive())
		{
			return;
		}
		int needed = verts.Count * (m_halfSampleCountX * 2 + 1) * (m_halfSampleCountY * 2 + 1);
		if (verts.Capacity < needed)
		{
			verts.Capacity = needed;
		}
		int count = verts.Count;
		int start = 0;
		float stepX = effectDistance.x / (float)m_halfSampleCountX;
		float stepY = effectDistance.y / (float)m_halfSampleCountY;
		for (int i = -m_halfSampleCountX; i <= m_halfSampleCountX; i++)
		{
			for (int j = -m_halfSampleCountY; j <= m_halfSampleCountY; j++)
			{
				if (i != 0 || j != 0)
				{
					int end = start + count;
					ApplyShadow(verts, effectColor, start, end, stepX * (float)i, stepY * (float)j);
					start = end;
				}
			}
		}
	}
}
