namespace Osmi.Game;

[PublicAPI]
public enum Layer {
	Default = 0,
	TransparentFX = 1,
	IgnoreRaycast = 2,
	Water = 4,
	UI = 5,
	Terrain = 8,
	Player = 9,
	TransitionGates = 10,
	Enemies = 11,
	Projectiles = 12,
	HeroDetector = 13,
	TerrainDetector = 14,
	EnemyDetector = 15,
	Tinker = 16,
	Attack = 17,
	Particle = 18,
	InteractiveObject = 19,
	HeroBox = 20,
	Grass = 21,
	EnemyAttack = 22,
	DamageAll = 23,
	Bouncer = 24,
	SoftTerrain = 25,
	Corpse = 26,
	uGUI = 27,
	HeroOnly = 28,
	ActiveRegion = 29,
	MapPin = 30,
	OrbitShield = 31
}
