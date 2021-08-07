using Mirror;
using UnityEngine;

public class InputData
{
    private bool Block = false;
    private bool Dodge = false;
    private bool LightAttack = false;
    private bool HeavyAttack = false;

    public void HandleInputs()
    {
        TryRoll();
        TryBlock();
        TryLightAttack();
        
    }
    
    [Client]
    private void TryRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dodge = true;
        }
    }

    [Client]
    private void TryLightAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LightAttack = true;
        }
    }

    [Client]
    private void TryBlock()
    {
        if (Input.GetMouseButton(1))
        {
            Block = true;
        }
    }

    [Client]
    public ActionCodes SetActionCode()
    {
        ActionCodes ac = ActionCodes.None;
        
        if (Block)
        {
            Block = false;
            ac = ActionCodes.Block;
        }

        if (Dodge)
        {
            Dodge = false;
            ac = ActionCodes.Dodge;
        }

        if (LightAttack)
        {
            LightAttack = false;
            ac = ActionCodes.LightAttack;
        }

        return ac;
    }
}