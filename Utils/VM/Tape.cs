using System.Collections;
using System.Collections.Generic;

namespace Utils.VM
{
    public class Tape<DataType> : IEnumerable<DataType>
    {
        public int Position
        {
            get;
            private set;
        }
        private readonly DataType[] _Data;

        public int Length => _Data.Length;

        public DataType this[int index]
        {
            get => _Data[index];
            set => _Data[index] = value;
        }

        public Tape(int size)
        {
            _Data = new DataType[size];
            Position = size / 2;
        }

        public DataType Read() => _Data[Position];
        public void Write(DataType value) => _Data[Position] = value;
        public void Left(int count = 1) => Position -= count;
        public void Right(int count = 1) => Position += count;

        public IEnumerator<DataType> GetEnumerator()
        {
            for (var i = 0; i < _Data.Length; i++)
                yield return _Data[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => _Data.GetEnumerator();
    }
}
