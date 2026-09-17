namespace Osmi.FsmActions;

[PublicAPI]
public sealed class NoOp : FsmStateAction {
	public override void OnEnter() => Finish();
}
