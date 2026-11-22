using UnityEngine;

public abstract class ScrollTextMenuBase : MenuBase
{
	private ResourceRequest textRequest_;

	private TextAsset textAsset_;

	protected override void LoadPrefab()
	{
		base.LoadPrefab();
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.SCROLL_TEXT_CONTROLLER);
		textRequest_ = Resources.LoadAsync(GetTextFilename());
	}

	protected override bool IsCompletedLoading()
	{
		if (!base.IsCompletedLoading())
		{
			return false;
		}
		return textRequest_.isDone;
	}

	protected abstract string GetTextFilename();

	protected TextAsset GetTextAsset()
	{
		return textRequest_.asset as TextAsset;
	}

	protected void UnloadTextAsset()
	{
		Resources.UnloadAsset(textRequest_.asset);
	}
}
