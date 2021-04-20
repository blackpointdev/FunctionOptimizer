using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionOptimizer.Operations.Crossing.Mapper
{
    class CrossMethodMapper
    {
        public CrossMethod MapToCrossMethodEnum(int index)
        {
            switch (index)
            {
                case (0):
                    return CrossMethod.OnePoint;
                case (1):
                    return CrossMethod.TwoPoints;
                case (2):
                    return CrossMethod.ThreePoints;
                case (3):
                    return CrossMethod.Homogeneous;
                default:
                    throw new ArgumentException("Cross method was not foud.");
            }
        }
    }
}
