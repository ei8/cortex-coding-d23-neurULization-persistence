﻿using ei8.Cortex.Coding.d23.Grannies;
using System;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    public class GrannyResult
    {
        public GrannyResult(
            bool success,
            IGranny granny
        )
        {
            this.Success = success;
            this.Granny = granny;
        }

        public bool Success { get; private set; }

        public IGranny Granny { get; private set; }
    }
}
