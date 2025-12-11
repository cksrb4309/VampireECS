using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[CreateAssetMenu(fileName = "ShooterConfig", menuName = "Config/Base/ShooterConfig")]
public class ShooterConfig : AbilityComponentConfig<ShooterData>
{
    public override void ApplyTier(Tier tier) { }
    public override string GetDescription()
    {
        return $"이것은 기본공격 추가다";
    }
    public async override void Apply()
    {
        AbilityPrefabLibrary prefabLibrary = await EntityUtility.GetOrWaitForSingletonComponentAsync<AbilityPrefabLibrary>();

        ShooterData shooterData = new ShooterData
        {
            OwnerFaction = Faction.Player,
            ProjectilePrefab = prefabLibrary.ShooterProjectilePrefab,
            MuzzleDistance = 2,
            MuzzleOffset = 0,
            ElapsedTime = 0
        };

        EntityUtility.SetComponentToSingletone(shooterData, typeof(PlayerTag));

        new GameObject("ShooterSetupObj").AddComponent<ShooterSetup>();
    }
}
public class ShooterSetup : MonoBehaviour
{
    public void OnEnable()
    {
        MousePositionEventBus.Subscribe(SetDirection);
        AttackInputEventBus.Subscribe(SetCanAttack);
    }
    public void OnDisable()
    {
        MousePositionEventBus.Unsubscribe(SetDirection);
        AttackInputEventBus.Unsubscribe(SetCanAttack);
    }
    public void SetDirection(MouseWorldPositionMessage message)
    {
        ShooterData shooterData = EntityUtility.GetComponentToSingletone<ShooterData>(typeof(PlayerTag));
        LocalToWorld localToWorld = EntityUtility.GetComponentToSingletone<LocalToWorld>(typeof(PlayerTag));

        shooterData.Direction = math.normalize(message.Position - localToWorld.Position);

        EntityUtility.SetComponentToSingletone(shooterData, typeof(PlayerTag));
    }
    public void SetCanAttack(AttackIsPressedMessage message)
    {
        ShooterCanFireData shooterCanFireData = EntityUtility.GetComponentToSingletone<ShooterCanFireData>(typeof(PlayerTag));

        shooterCanFireData.CanFire = message.IsPressed;

        EntityUtility.SetComponentToSingletone(shooterCanFireData, typeof(PlayerTag));
    }
}
