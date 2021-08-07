using UnityEngine;

/// <summary>
/// Motor state send from Server to Client
/// </summary>
public struct ServerPlayerMotorState
{
    public uint FixedFrame;
    public Vector3 Position;
    public Vector3 DodgeVelocity;
    public Vector3 AttackVelocity;
    public Vector3 RecoilVelocity;
    public sbyte TimingStepChange;
}
