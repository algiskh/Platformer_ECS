using System.Collections.Generic;
using UnityEngine;

#region Singleton_entities
/// <summary>
/// Singleton component for holding main configuration data
/// </summary>
public struct  MainConfigComponent
{
	public MainConfigHolder Value;
}

public struct SoundHolderComponent
{
	public SoundHolder Value;
}

public struct EffectsHolderComponent
{
	public EffectsHolder Value;
}

/// <summary>
/// Singleton for collection of spawnpoints for mobs
/// </summary>
public struct SpawnPointsComponent
{
	public Transform[] Value;
}

/// <summary>
/// Singleton for holding a pool of mobs
/// </summary>
public struct MobPoolComponent
{
	public List<Mob> Value;
}

public struct BulletPoolComponent
{
	public Stack<Bullet> Value;
}

public struct EffectPoolComponent
{
	public List<SceneEffect> Value;
	public Transform Parent;
}
#endregion

public struct  MobComponent
{
	public Mob Value;
	public MobConfig Config;
}

public struct MoveComponent
{
	public Transform Transform;
	public Vector2 Direction;
	public float Speed;
}

public struct HealthComponent
{
	public float CurrentHealth;
	public float MaxHealth;
}

public struct RequestDamageComponent
{
	public float Damage;
	public int TargetEntity;
}

/// <summary>
/// Component for holding a simple collision handling (try to avoid using physics for this)
/// </summary>
public struct CollisionComponent
{
	public CollisionType CollisionType;
	public float Radius; // simple collision radius for collision detection
}

/// <summary>
/// Singleton component for managing spawn requests
/// </summary>
public struct SpawnRequestComponent
{
	// Presets
	public float MinCoolDown;
	public float MaxCoolDown;
	// Runtime values
	public float LastSpawnTime;
	public float CurrentCoolDown;
	public bool IsBlocked;
}

#region Player

public struct PlayerComponent
{
	public Player Value;
	public PlayerState State;
}

public struct PlayerInputComponent
{
	public Vector2 Move;
	public Vector2 PreviousMove;
	public bool IsFiring;
	public bool IsRight;
}

public struct MuzzleComponent
{
	public GunConfig GunConfig;
	public Transform Transform;
	public float PrevFireTime;
	public bool IsFiring;
	public int Count;
	public AudioSource AudioSource;
}

public struct RequestFireComponent
{
	public bool IsRight;
}

public struct BulletComponent
{
	public Bullet Bullet;
	public float Damage;
	public float LifeTime;
	public bool IsDisposed;
}
#endregion

public struct ParallaxComponent
{
	public ParallaxObject Value;
}

public struct RequestLootSpawn
{
	public MobConfig.PossibleLoot[] PossibleLoots;
	public Vector2 Position;
}

public struct LootComponent
{
	public Loot Loot;
	public LootType LootType;
	public bool IsDisposed;
	public int Count;
}

public struct LootPoolComponent
{
	public Stack<Loot> Value;
}

public struct LootParentComponent
{
	public Transform Value;
}

public struct AmmoCounterComponent
{
	public AmmoCounter Value;
}

public struct RequestOpenWindowComponent
{
	public WindowType WindowType;
}

public struct FailWindowComponent
{
	public FailWindow Value;
}

public struct BorderComponent
{
	public Transform Transform;
	public bool IsPlayerNearBy;
}

public struct EffectComponent
{
	public SceneEffect Effect;
	public float LifeTime;
}

public struct RequestEffectComponent
{
	public string EffectId;
	public Vector2 Position;
}