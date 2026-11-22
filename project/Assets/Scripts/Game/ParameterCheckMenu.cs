public class ParameterCheckMenu : MenuBase
{
	protected override void LoadPrefab()
	{
		base.LoadPrefab();
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.PARAMETER_CONTROLLER);
		AddPrefabLoadRequest(MenuCommonData.ChildObjectType.GENDER_OBJECTS_CONTROLLER);
	}

	protected override void Initialize()
	{
		base.Initialize();
		GenderSwitchObjectsController genderObjects = GetChildObject(MenuCommonData.ChildObjectType.GENDER_OBJECTS_CONTROLLER).GetComponent<GenderSwitchObjectsController>();
		genderObjects.SetObjectActive(GenderSwitchObjectsController.SwitchObjectType.COSTUME_COUNT, false);
		genderObjects.SetObjectActive(GenderSwitchObjectsController.SwitchObjectType.CURRENT_GENDER, true);
		genderObjects.SetObjectActive(GenderSwitchObjectsController.SwitchObjectType.GENDER_SELECT_MESSAGE, false);
		genderObjects.SetCurrentGender(GetCurrentGender());
	}

	protected override void OnButtonClick()
	{
		base.OnButtonClick();
		GoToNextMenu(MenuCommonData.MenuType.MENU_NONE);
	}

	private void SetParameterValue(PlayerStatusManager.PlayerStatusType status)
	{
		ParameterAreaController parameters = GetChildObject(MenuCommonData.ChildObjectType.PARAMETER_CONTROLLER).GetComponent<ParameterAreaController>();
		ParameterAreaController.ParameterType parameterType = GetParemterType(status);
		if (parameterType != ParameterAreaController.ParameterType.NONE)
		{
			parameters.SetParameterValue(parameterType);
		}
	}

	private ParameterAreaController.ParameterType GetParemterType(PlayerStatusManager.PlayerStatusType status)
	{
		switch (status)
		{
		case PlayerStatusManager.PlayerStatusType.TYPE_WEIGHT:
			return ParameterAreaController.ParameterType.WEIGHT;
		case PlayerStatusManager.PlayerStatusType.TYPE_STRENGTH:
			return ParameterAreaController.ParameterType.STRENGTH;
		case PlayerStatusManager.PlayerStatusType.TYPE_INTELLIGENCE:
			return ParameterAreaController.ParameterType.INTELLIGENCE;
		case PlayerStatusManager.PlayerStatusType.TYPE_MAGIC:
			return ParameterAreaController.ParameterType.MAGIC;
		default:
			return ParameterAreaController.ParameterType.NONE;
		}
	}
}
