using System;

namespace FunctionOptimizer.Operations.Mutation.Mapper
{
    class MutationMethodMapper
    {
        public MutationMethod MapToMutationMethodEnum(int index)
        {
            switch(index)
            {
                case (0):
                    return MutationMethod.Edge;
                case (1):
                    return MutationMethod.OnePoint;
                case (2):
                    return MutationMethod.TwoPoints;
                default:
                    throw new ArgumentException("Mutation method was not foud.");
            }
        }
    }
}
