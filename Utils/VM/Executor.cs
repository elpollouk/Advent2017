namespace Utils.VM
{
    public interface IProgram<Instruction, VmState>
    {
        Instruction Fetch(VmState vmState);
    }

    public class Executor<Instruction, VmState>
    {
        public VmState State
        {
            get;
            private set;
        }

        private readonly InstructionSet<Instruction, VmState> _InstructionSet;
        private readonly IProgram<Instruction, VmState> _Program;

        public Executor(InstructionSet<Instruction, VmState> instructionSet, IProgram<Instruction, VmState> program, VmState vmState)
        {
            _InstructionSet = instructionSet;
            _Program = program;
            State = vmState;
        }

        public void Execute(int numCycles)
        {
            for (var i = 0; i < numCycles; i++)
            {
                var instruction = _Program.Fetch(State);
                _InstructionSet[instruction](State);
            }
        }
    }
}
