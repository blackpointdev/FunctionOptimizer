using System;

namespace FunctionOptimizer.Selection.Mapper
{
    class SelectionMethodMapper
    {
        public SelectionMethod MapToSelectionMethodEnum(int index)
        {
            switch(index)
            {
                case (0):
                    return SelectionMethod.Best;
                case (1):
                    return SelectionMethod.Roulette;
                case (2):
                    return SelectionMethod.Tournament;
                default:
                    throw new ArgumentException("Selection method was not foud.");
            }
        }
    }
}
