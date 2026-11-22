using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class HypertextBase : Text, IPointerClickHandler, IEventSystemHandler
{
	private struct ClickableEntry
	{
		public string Word;

		public int StartIndex;

		public Color Color;

		public Action<string> OnClick;

		public List<Rect> Rects;

		public ClickableEntry(string word, int startIndex, Color color, Action<string> onClick)
		{
			Word = word;
			StartIndex = startIndex;
			Color = color;
			OnClick = onClick;
			Rects = new List<Rect>();
		}
	}

	private Canvas _rootCanvas;

	private const int CharVertsNum = 6;

	private readonly List<ClickableEntry> _entries = new List<ClickableEntry>();

	private static readonly ObjectPool<List<UIVertex>> _verticesPool = new ObjectPool<List<UIVertex>>(null, (List<UIVertex> l) =>
	{
		l.Clear();
	});

	private Canvas RootCanvas
	{
		get
		{
			return _rootCanvas ?? (_rootCanvas = GetComponentInParent<Canvas>());
		}
	}

	protected void RegisterClickable(int startIndex, int wordLength, Color color, Action<string> onClick)
	{
		if (onClick != null && startIndex >= 0 && wordLength >= 0 && startIndex + wordLength <= text.Length)
		{
			_entries.Add(new ClickableEntry(text.Substring(startIndex, wordLength), startIndex, color, onClick));
		}
	}

	public virtual void RemoveClickable()
	{
		_entries.Clear();
	}

	protected abstract void RegisterClickable();

	protected override void OnPopulateMesh(VertexHelper vertexHelper)
	{
		base.OnPopulateMesh(vertexHelper);
		_entries.Clear();
		RegisterClickable();
		List<UIVertex> vertices = _verticesPool.Get();
		vertexHelper.GetUIVertexStream(vertices);
		Modify(ref vertices);
		vertexHelper.Clear();
		vertexHelper.AddUIVertexTriangleStream(vertices);
		_verticesPool.Release(vertices);
	}

	private void Modify(ref List<UIVertex> vertices)
	{
		int count = vertices.Count;
		int i = 0;
		for (int entryCount = _entries.Count; i < entryCount; i++)
		{
			ClickableEntry entry = _entries[i];
			int j = entry.StartIndex;
			for (int wordEnd = entry.StartIndex + entry.Word.Length; j < wordEnd; j++)
			{
				int vertexIndex = j * 6;
				if (vertexIndex + 6 > count)
				{
					break;
				}
				Vector2 min = Vector2.one * float.MaxValue;
				Vector2 max = Vector2.one * float.MinValue;
				for (int k = 0; k < 6; k++)
				{
					UIVertex vertex = vertices[vertexIndex + k];
					vertex.color = entry.Color;
					vertices[vertexIndex + k] = vertex;
					Vector3 position = vertices[vertexIndex + k].position;
					if (position.y < min.y)
					{
						min.y = position.y;
					}
					if (position.x < min.x)
					{
						min.x = position.x;
					}
					if (position.y > max.y)
					{
						max.y = position.y;
					}
					if (position.x > max.x)
					{
						max.x = position.x;
					}
				}
				entry.Rects.Add(new Rect
				{
					min = min,
					max = max
				});
			}
			List<Rect> rowRects = new List<Rect>();
			foreach (List<Rect> row in SplitRectsByRow(entry.Rects))
			{
				rowRects.Add(CalculateAABB(row));
			}
			entry.Rects = rowRects;
			_entries[i] = entry;
		}
	}

	private List<List<Rect>> SplitRectsByRow(List<Rect> rects)
	{
		List<List<Rect>> rows = new List<List<Rect>>();
		int rowStart = 0;
		int i = 1;
		for (int count = rects.Count; i < count; i++)
		{
			if (rects[i].xMin < rects[i - 1].xMin)
			{
				rows.Add(rects.GetRange(rowStart, i - rowStart));
				rowStart = i;
			}
		}
		if (rowStart < rects.Count)
		{
			rows.Add(rects.GetRange(rowStart, rects.Count - rowStart));
		}
		return rows;
	}

	private Rect CalculateAABB(List<Rect> rects)
	{
		Vector2 min = Vector2.one * float.MaxValue;
		Vector2 max = Vector2.one * float.MinValue;
		int i = 0;
		for (int count = rects.Count; i < count; i++)
		{
			if (rects[i].xMin < min.x)
			{
				min.x = rects[i].xMin;
			}
			if (rects[i].yMin < min.y)
			{
				min.y = rects[i].yMin;
			}
			if (rects[i].xMax > max.x)
			{
				max.x = rects[i].xMax;
			}
			if (rects[i].yMax > max.y)
			{
				max.y = rects[i].yMax;
			}
		}
		return new Rect
		{
			min = min,
			max = max
		};
	}

	private Vector3 ToLocalPosition(Vector3 position, Camera camera)
	{
		if (!RootCanvas)
		{
			return Vector3.zero;
		}
		if (RootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
		{
			return transform.InverseTransformPoint(position);
		}
		Vector2 localPoint = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, camera, out localPoint);
		return localPoint;
	}

	void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
	{
		Vector3 point = ToLocalPosition(eventData.position, eventData.pressEventCamera);
		for (int i = 0; i < _entries.Count; i++)
		{
			for (int j = 0; j < _entries[i].Rects.Count; j++)
			{
				if (_entries[i].Rects[j].Contains(point))
				{
					_entries[i].OnClick(_entries[i].Word);
					break;
				}
			}
		}
	}
}
