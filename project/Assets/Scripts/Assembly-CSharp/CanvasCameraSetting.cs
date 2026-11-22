using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSetting : MonoBehaviour
{
	public enum SortingLayer
	{
		BACK = 0,
		DEFAULT = 1,
		FRONT = 2
	}

	public SortingLayer layer_ = SortingLayer.DEFAULT;

	private readonly Dictionary<SortingLayer, string> SORTING_LAYER_NAME_LIST = new Dictionary<SortingLayer, string>
	{
		{
			SortingLayer.BACK,
			"Back"
		},
		{
			SortingLayer.DEFAULT,
			"Default"
		},
		{
			SortingLayer.FRONT,
			"Front"
		}
	};

	private void Start()
	{
		InitializeCanvas();
	}

	private void InitializeCanvas()
	{
		Canvas component = GetComponent<Canvas>();
		component.renderMode = RenderMode.ScreenSpaceCamera;
		component.worldCamera = Camera.main;
		component.sortingLayerName = SORTING_LAYER_NAME_LIST[layer_];
	}
}
