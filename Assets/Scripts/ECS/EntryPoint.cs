using ECS;
using Leopotam.EcsLite;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
	[Header("Configs")]
	[SerializeField] private MainConfigHolder _mainConfigHolder;
	[SerializeField] private SoundHolder _soundHolder;
	[SerializeField] private EffectsHolder _effectsHolder;

	[Header("Game Objects")]
	[SerializeField] private Transform[] _mobSpawnPoints;
	[SerializeField] private Player _player;
	[SerializeField] private Transform _lootParent;
	[SerializeField] private Transform _effectsParent;
	[SerializeField] private AmmoCounter _ammoCounter;
	[SerializeField] private FailWindow _failWindow;
	[SerializeField] private Transform[] _borders;

	private EcsWorld _world;
    private EcsSystems _systems;

	void Start()
    {
        _world = new EcsWorld();
		_systems = new EcsSystems(_world);

		AddInitialEntities(_world);

		_systems.Add(new MobSpawnSystem())
			.Add(new InputSystem())
			.Add(new PlayerSystem())
			.Add(new MoveSystem())
			.Add(new BulletSystem())
			.Add(new CollisionSystem())
			.Add(new DamageSystem())
			.Add(new ParallaxSystem())
			.Add(new LootSystem())
			.Add(new CheckFailSystem())
			.Add(new EffectsSystem())
			.Add(new UISystem())
			.Init();
	}

	private void AddInitialEntities(EcsWorld world)
	{
		#region CreatingSingletons
		ref var mainConfig = ref world.CreateSimpleEntity<MainConfigComponent>();
		mainConfig.Value = _mainConfigHolder;

		ref var spawnRequest = ref world.CreateSimpleEntity<SpawnRequestComponent>();
		spawnRequest.MinCoolDown = _mainConfigHolder.MinSpawnCoolDown;
		spawnRequest.MaxCoolDown = _mainConfigHolder.MaxSpawnCoolDown;

		ref var spawnPoints = ref world.CreateSimpleEntity<SpawnPointsComponent>();
		spawnPoints.Value = _mobSpawnPoints;

		ref var effectsHolder = ref world.CreateSimpleEntity<EffectsHolderComponent>();
		effectsHolder.Value = _effectsHolder;

		ref var effectsPool = ref world.CreateSimpleEntity<EffectPoolComponent>();
		effectsPool.Value = new List<SceneEffect>();
		effectsPool.Parent = _effectsParent;

		ref var mobPool = ref world.CreateSimpleEntity<MobPoolComponent>();
		mobPool.Value = new();
		ref var bulletPool = ref world.CreateSimpleEntity<BulletPoolComponent>();
		bulletPool.Value = new();

		ref var lootParentComponent = ref world.CreateSimpleEntity<LootParentComponent>();
		lootParentComponent.Value = _lootParent;

		ref var lootPoolComponent = ref world.CreateSimpleEntity<LootPoolComponent>();
		lootPoolComponent.Value = new Stack<Loot>();


		ref var ammoCounterComponent = ref world.CreateSimpleEntity<AmmoCounterComponent>();
		ammoCounterComponent.Value = _ammoCounter;

		ref var failWindowComponent = ref world.CreateSimpleEntity<FailWindowComponent>();
		failWindowComponent.Value = _failWindow;

		ref var soundHolderComponent = ref world.CreateSimpleEntity<SoundHolderComponent>();
		soundHolderComponent.Value = _soundHolder;
		#endregion

		#region GettingPools
		var playerPool = world.GetPool<PlayerComponent>();
		var moveComponentPool = world.GetPool<MoveComponent>();
		var healthComponentPool = world.GetPool<HealthComponent>();
		var muzzleComponentPool = world.GetPool<MuzzleComponent>();
		var playerInputPool = world.GetPool<PlayerInputComponent>();
		var collisionPool = world.GetPool<CollisionComponent>();
		var borderPool = world.GetPool<BorderComponent>();
		#endregion

		var playerEntity = world.NewEntity();
		ref var playerComponent = ref playerPool.Add(playerEntity);
		playerComponent.Value = _player;

		ref var playerHealth = ref healthComponentPool.Add(playerEntity);
		playerHealth.CurrentHealth = _mainConfigHolder.PlayerConfig.Health;
		playerHealth.MaxHealth = _mainConfigHolder.PlayerConfig.Health;

		#region SettingMuzzle
		ref var muzzleComponent = ref muzzleComponentPool.Add(playerEntity);
		muzzleComponent.Transform = _player.Muzzle;
		muzzleComponent.GunConfig = _mainConfigHolder.GunConfig;
		muzzleComponent.PrevFireTime = 0f;
		muzzleComponent.IsFiring = false;
		muzzleComponent.Count = _mainConfigHolder.InitialAmmo;
		muzzleComponent.AudioSource = _player.AudioSource;
		#endregion

		ref var playerInputComponent = ref playerInputPool.Add(playerEntity);
		ref var playerMoveComponent = ref moveComponentPool.Add(playerEntity);
		playerMoveComponent.Transform = _player.transform;
		playerMoveComponent.Speed = _mainConfigHolder.PlayerConfig.Speed;
		playerMoveComponent.Direction = Vector2.zero;

		#region PresettingScene
		AddParalaxObjects(world);

		foreach (var p in _borders)
		{
			var borderEntity = world.NewEntity();
			ref var collision = ref collisionPool.Add(borderEntity);
			ref var borderComponent = ref borderPool.Add(borderEntity);

			collision.Radius = _mainConfigHolder.DefaultCollisionRadius;
			collision.CollisionType = CollisionType.Border;
			borderComponent.Transform = p;
		}
		#endregion
	}

	private void AddParalaxObjects(EcsWorld world)
	{
		var parallaxObjects = FindObjectsByType<ParallaxObject>(FindObjectsSortMode.None);

		foreach (var p in parallaxObjects)
		{
			ref var parallaxComponent = ref world.CreateSimpleEntity<ParallaxComponent>();
			parallaxComponent.Value = p;
		}
	}

	private void OnDestroy()
	{
		if (_systems != null)
		{
			_systems.Destroy();
			_systems = null;
		}
		if (_world != null)
		{
			_world.Destroy();
			_world = null;
		}
	}

    void Update()
    {
        _systems?.Run();
    }
}