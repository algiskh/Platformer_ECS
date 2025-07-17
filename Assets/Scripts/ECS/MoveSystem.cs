using Leopotam.EcsLite;
using UnityEngine;

public class MoveSystem : IEcsRunSystem
{
	public void Run(IEcsSystems systems)
	{
		var world = systems.GetWorld();
		var movePool = world.GetPool<MoveComponent>();

		foreach (var entity in world.Filter<MoveComponent>().End())
		{
			ref var moveComponent = ref movePool.Get(entity);

			if (moveComponent.Direction == Vector2.zero)
				continue;

			moveComponent.Transform.position += (Vector3)moveComponent.Direction * moveComponent.Speed * Time.deltaTime;
		}
	}
}