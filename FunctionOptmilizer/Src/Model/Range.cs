using System;

namespace FunctionOptimizer.Model
{
    class Range
    {
        public Range(int Start, int End)
        {
            this.Start = Start;
            this.End = End;
        }

        public Range()
        {

        }

        public int Start { get; set; }
        public int End { get; set; }

        public override string ToString()
        {
            return String.Format("StartRange: {0}, EndRange: {1}", Start, End);
        }
    }
}
