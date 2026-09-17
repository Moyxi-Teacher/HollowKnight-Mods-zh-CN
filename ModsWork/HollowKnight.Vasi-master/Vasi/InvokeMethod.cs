using System;
using HutongGames.PlayMaker;

namespace Vasi
{
    public class InvokeMethod : FsmStateAction
    {
        private readonly Action? _action;

        // PlayMaker 通过反射实例化 FsmStateAction，要求存在公开的无参构造函数。
        // 参见 Task #70 对 FiveKnights.InvokeMethod 修复的相同问题。
        public InvokeMethod()
        {
            _action = null;
        }

        public InvokeMethod(Action a)
        {
            _action = a;
        }

        public override void OnEnter()
        {
            // 空值安全调用：当通过无参构造函数创建后未赋值时，不抛出空引用异常。
            _action?.Invoke();

            Finish();
        }
    }
}