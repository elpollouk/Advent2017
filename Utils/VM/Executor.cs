using System;

namespace Utils.VM
{
    public interface IProgram<VmState, Instruction, Operands>
    {
        (Instruction, Operands) Fetch(VmState vmState);
    }

    public class Halt : System.Exception { }

    public class Executor<VmState, Instruction, Operands>
    {
        public VmState State
        {
            get;
            private set;
        }

        private readonly InstructionSet<VmState, Instruction, Operands> _InstructionSet;
        private readonly IProgram<VmState, Instruction, Operands> _Program;

        public Executor(InstructionSet<VmState, Instruction, Operands> instructionSet, IProgram<VmState, Instruction, Operands> program, VmState vmState)
        {
            _InstructionSet = instructionSet;
            _Program = program;
            State = vmState;
        }

        private void ExecuteCycle()
        {
            var (instruction, operands) = _Program.Fetch(State);
            _InstructionSet[instruction](State, operands);
        }

        public void Execute(Action<Action> runloop)
        {
            try
            {
                runloop(ExecuteCycle);
            }
            catch (Halt) { }
        }

        public void Execute(int numCycles) => Execute(step => { for (var i = 0; i < numCycles; i++) step(); });
        public void Execute() => Execute(step => { while (true) step(); });
    }
}
