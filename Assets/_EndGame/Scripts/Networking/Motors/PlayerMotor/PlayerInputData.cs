using UnityEngine;

public struct InputData
{
    private bool Block;
    private bool Dodge;
    private bool LightAttack;
    private bool HeavyAttack;
    private bool Ability1;
    private bool Ability2;
    private bool Ability3;

    private float mouseDownTime;
    private float lastMouseDownTime;
    private bool mouseHeldUsed;

    public bool HaveAbilityInput => LightAttack || HeavyAttack || Ability1 || Ability2 || Ability3;

    public void HandleInputs()
    {
        TryRoll();
        TryBlock();
        TryLightAttack();
        TryHeavyAttack();
        TrySpellAbilityAttack();
        
        CalculateMouseDownTime();
    }
    
    private void TryRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dodge = true;
        }
    }

    private void TryLightAttack()
    {
        if (Input.GetMouseButtonUp(0) && mouseDownTime < 0.2f)
        {
            LightAttack = true;
        }
    }

    private void TryBlock()
    {
        if (Input.GetMouseButton(1))
        {
            Block = true;
        }
    }

    private void TryHeavyAttack()
    {
        if (mouseDownTime > 0.2f && !mouseHeldUsed)
        {
            HeavyAttack = true;
            mouseHeldUsed = true;
        }
    }

    private void TrySpellAbilityAttack()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Ability1 = true;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Ability2 = true;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Ability3 = true;
        }
    }

    private void CalculateMouseDownTime()
    {
        if (Input.GetMouseButton(0))
        {
            mouseDownTime += Time.deltaTime;
        }
        else
        {
            mouseDownTime = 0;
            mouseHeldUsed = false;
        }
    }

    public MovementActionCode GetAndResetMovementActionCode()
    {
        MovementActionCode movementActionCode = MovementActionCode.None;
        
        if (Block)
        {
            Block = false;
            movementActionCode = MovementActionCode.Block;
        }

        if (Dodge)
        {
            Dodge = false;
            movementActionCode = MovementActionCode.Dodge;
        }
        
        return movementActionCode;
    }

    public AbilityActionCode GetAndResetAbilityActionCode()
    {
        AbilityActionCode abilityActionCode = AbilityActionCode.None;
        
        if (LightAttack)
        {
            LightAttack = false;
            abilityActionCode = AbilityActionCode.LightAttack;
        }

        if (HeavyAttack)
        {
            HeavyAttack = false;
            abilityActionCode = AbilityActionCode.HeavyAttack;
        }

        if (Ability1)
        {
            Ability1 = false;
            abilityActionCode = AbilityActionCode.Ability1;
        }
        if (Ability2)
        {
            Ability2 = false;
            abilityActionCode = AbilityActionCode.Ability2;
        }
        
        if (Ability3)
        {
            Ability3 = false;
            abilityActionCode = AbilityActionCode.Ability2;
        }

        return abilityActionCode;
    }
}