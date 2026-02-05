using VContainer;
using VContainer.Unity;

public class BattleSceneLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<PlayerMoveInputBridge>(Lifetime.Scoped);
        builder.Register<MouseInputBridge>(Lifetime.Scoped);
        builder.Register<PlayerAttackInputBridge>(Lifetime.Scoped);

        builder.RegisterBuildCallback(container =>
        {
            container.Resolve<PlayerMoveInputBridge>();
            container.Resolve<MouseInputBridge>();
            container.Resolve<PlayerAttackInputBridge>();
        });
    }
}
