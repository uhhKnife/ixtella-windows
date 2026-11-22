using System;
using System.Collections.Generic;
using UnityEngine;

public class CircleOutline : ModifiedShadow
{
	[SerializeField]
	private int m_circleCount = 2;

	[SerializeField]
	private int m_firstSample = 4;

	[SerializeField]
	private int m_sampleIncrement = 2;

	public int circleCount
	{
		get
		{
			return m_circleCount;
		}
		set
		{
			m_circleCount = Mathf.Max(value, 1);
			if (graphic != null)
			{
				graphic.SetVerticesDirty();
			}
		}
	}

	public int firstSample
	{
		get
		{
			return m_firstSample;
		}
		set
		{
			m_firstSample = Mathf.Max(value, 2);
			if (graphic != null)
			{
				graphic.SetVerticesDirty();
			}
		}
	}

	public int sampleIncrement
	{
		get
		{
			return m_sampleIncrement;
		}
		set
		{
			m_sampleIncrement = Mathf.Max(value, 1);
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
		int totalSamples = (m_firstSample * 2 + m_sampleIncrement * (m_circleCount - 1)) * m_circleCount / 2;
		int needed = verts.Count * (totalSamples + 1);
		if (verts.Capacity < needed)
		{
			verts.Capacity = needed;
		}
		int count = verts.Count;
		int start = 0;
		int samples = m_firstSample;
		float stepX = effectDistance.x / (float)circleCount;
		float stepY = effectDistance.y / (float)circleCount;
		for (int i = 1; i <= m_circleCount; i++)
		{
			float radiusX = stepX * (float)i;
			float radiusY = stepY * (float)i;
			float angleStep = (float)Math.PI * 2f / (float)samples;
			float angle = (float)(i % 2) * angleStep * 0.5f;
			for (int j = 0; j < samples; j++)
			{
				int end = start + count;
				ApplyShadow(verts, effectColor, start, end, radiusX * Mathf.Cos(angle), radiusY * Mathf.Sin(angle));
				start = end;
				angle += angleStep;
			}
			samples += m_sampleIncrement;
		}
	}
}
