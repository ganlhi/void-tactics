using UnityEngine;
using Vdev.Messaging;

public static class MessageBus
{
    public sealed class NextTurn : Message<NextTurn>
    { }

    public sealed class RunningTurn : Message<RunningTurn, bool>
    { }

    public sealed class PlayerReady : Message<PlayerReady, bool>
    { }

    public sealed class SelectShip : Message<SelectShip, GameObject>
    { }

    public sealed class Plot : Message<Plot, ManeuverAxis, int>
    { }

    public sealed class PlotThrust : Message<PlotThrust, float>
    { }

    public sealed class RadialMenu : Message<RadialMenu, GameObject>
    { }

    public sealed class AddEnergyWeaponTarget : Message<AddEnergyWeaponTarget, GameObject>
    { }

    public sealed class RemoveEnergyWeaponTarget : Message<RemoveEnergyWeaponTarget, GameObject>
    { }
}