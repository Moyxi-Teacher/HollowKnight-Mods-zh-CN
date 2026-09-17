using Hkmp.Math;
using HkmpPouch;
using static Konpanion.Utilities;
namespace Konpanion
{
    internal class KonpanionUpdate : PipeEvent
    {
        public static string Name = "KonpanionUpdate";
        public Vector2 pos { get; set; }
        public State anim { get; set; }
        public Direction dir { get; set; }
        public override string GetName() => KonpanionUpdate.Name;

        public override string ToString()
        {
            return $"{i2s((int)anim)}{Constants.Separator}{i2s((int)dir)}{Constants.Separator}{f2s(pos.X)}{Constants.Separator}{f2s(pos.Y)}";
        }
    }

    internal class KonpanionUpdateFactory : IEventFactory
    {
        public static KonpanionUpdateFactory Instance { get; internal set; } = new KonpanionUpdateFactory();

        public PipeEvent FromSerializedString(string serializedData)
        {
            var pEvent = new KonpanionUpdate();
            var Split = serializedData.Split(Constants.SplitSep);
            pEvent.anim = (State)s2i(Split[0]);
            pEvent.dir = (Direction)s2i(Split[1]);
            pEvent.pos = new Vector2(s2f(Split[2]), s2f(Split[3]));
            return pEvent;
        }

        public string GetName() => KonpanionUpdate.Name;
    }
}
