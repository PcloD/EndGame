using Mirror;
namespace FirstGearGames.Mirrors.InitializingLocalPlayers
{
    public class FindAndSetupObjects : NetworkBehaviour
    {
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            /* DO NOT USE FIND METHODS!
            * Find methods are extremely expensive, and often
            * unreliable. */

            //Tell canvas abodut players health script.
            //HealthPercent hp = GetComponent<HealthPercent>();
            //HealthBarUI meter = FindObjectOfType<HealthBarUI>();
            //if (meter != null)
            //    meter.SetPlayerHealthPercent(hp);

            ////Tell camera about player.
            //CameraController controller = FindObjectOfType<CameraController>();
            //if (controller != null)
            //    controller.SetPlayerTransform(transform);

            ////Tell enemies player exist so they know when to follow.
            //EnemyFollowPlayer[] enemies = FindObjectsOfType<EnemyFollowPlayer>();
            //foreach (EnemyFollowPlayer enemy in enemies)
            //    enemy.SetPlayerHealthPercent(hp);
        }

    }

}