using System;
using System.Collections.Generic;

namespace Utils.VM
{
    public class InstructionSet<Instruction, VmState>
    {
        public Action<VmState> this[Instruction key]
        {
            set => _Instructions[key] = value;
            get => _Instructions[key];
        }
        private readonly Dictionary<Instruction, Action<VmState>> _Instructions = new Dictionary<Instruction, Action<VmState>>();
    }
}
