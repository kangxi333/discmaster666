using UnityEngine;

public class LoseHUD : PegboardHUDBase
{
    public override void UpdateHUD()
    {
        GameMaster.Instance.ResetPlayButton();
    }
}
