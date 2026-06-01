using UnityEngine;

/// <summary>
/// Skill Frank Leboeuf : joue son SFX à plein volume en coupant la musique.
/// La musique reprend automatiquement à la fin du SFX.
/// </summary>
public class FrankLeboeufSkill : Skill
{
    private const float COOLDOWN = 30f;

    public FrankLeboeufSkill(Sprite image = null, Sprite miniImage = null, AudioClip sfx = null)
        : base("Frank Leboeuf", COOLDOWN, image, miniImage, sfx: sfx, cutMusic: true)
    {
    }

    protected override float sfxVolume => 3f;

    public override void Effect(GameObject target)
    {
        if (!CanUse()) return;

        // base.Effect joue déjà le SFX à volume 1 avec cutMusic:true
        base.Effect(target);

        Debug.Log("Frank Leboeuf !");
    }
}
