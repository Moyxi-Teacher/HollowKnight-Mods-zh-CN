using Osmi.ChildRefs;

namespace Osmi.Game;

#pragma warning disable IDE0051

[PublicAPI]
[RequireComponent(typeof(Rigidbody2D))]
public class MoveToPosition2D : MonoBehaviour {
	[ChildRef] private readonly Rigidbody2D rb = null!;

	public virtual Vector2? TargetPos { get; set; }

	public float accelerationForce;
	public float maxVelocity;


	private void Start() => this.InitChildRefs();

	private void FixedUpdate() {
		if (!TargetPos.HasValue || rb.bodyType != RigidbodyType2D.Dynamic) {
			return;
		}

		Vector2 relPos = TargetPos.Value - transform.position.AsVector2();
		if (relPos.magnitude == 0) {
			return;
		}

		rb.AddForce(relPos.normalized * accelerationForce);
		rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxVelocity);
	}
}
