using UnityEngine;

public class GrenadePickupTrigger : MonoBehaviour
{
    [HideInInspector] public Grenade grenade;

    private void Awake()
    {
        grenade = GetComponentInParent<Grenade>();
    }

    public bool TryPickUp(PlayerController2D player)
    {
        if (grenade == null) return false;
        return grenade.TryPickUp(player);
    }
}