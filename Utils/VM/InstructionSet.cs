using System;
using System.Collections.Generic;

namespace Utils.VM
{
    public class InstructionSet<VmState, Instruction, Operands>
    {
        public Action<VmState, Operands> this[Instruction key]
        {
            set => _Instructions[key] = value;
            get => _Instructions[key];
        }
        private readonly Dictionary<Instruction, Action<VmState, Operands>> _Instructions = new Dictionary<Instruction, Action<VmState, Operands>>();
    }
}
