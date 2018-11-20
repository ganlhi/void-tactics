using Vdev.Messaging;

public static class MessageBus
{
    public sealed class NextTurn : Message<NextTurn>
    { }

    public sealed class RunningTurn : Message<RunningTurn, bool>
    { }

    public sealed class PlayerReady : Message<PlayerReady, bool>
    { }

    public sealed class SelectShip : Message<SelectShip>
    { }

    public sealed class Plot : Message<Plot, ManeuverAxis, int>
    { }

    public sealed class PlotThrust : Message<PlotThrust, float>
    { }
}