using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModifiedShadow : Shadow
{
	public override void ModifyMesh(VertexHelper vh)
	{
		if (IsActive())
		{
			List<UIVertex> vertices = ListPool<UIVertex>.Get();
			vh.GetUIVertexStream(vertices);
			ModifyVertices(vertices);
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertices);
			ListPool<UIVertex>.Release(vertices);
		}
	}

	public virtual void ModifyVertices(List<UIVertex> verts)
	{
	}
}
