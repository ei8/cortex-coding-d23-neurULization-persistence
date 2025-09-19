using ei8.Cortex.Coding.d23.Grannies;
using System;

namespace ei8.Cortex.Coding.d23.neurULization.Persistence
{
    /// <summary>
    /// Represents a retrieval Granny Result.
    /// </summary>
    public class GrannyResult
    {
        /// <summary>
        /// Constructs a Granny Result with a specified IParamaterSet.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="success"></param>
        /// <param name="granny"></param>
        public GrannyResult(
            IParameterSet parameters,
            bool success,
            IGranny granny
        ) : this(
            success,
            granny
        )
        {
            this.Parameters = parameters;
        }

        /// <summary>
        /// Constructs a Granny Result with a specified parameter Id.
        /// </summary>
        /// <param name="parameterId"></param>
        /// <param name="success"></param>
        /// <param name="granny"></param>
        public GrannyResult(
            Guid parameterId,
            bool success,
            IGranny granny
        ) : this(
            success,
            granny
        )
        {
            this.ParameterId = parameterId;
        }

        private GrannyResult(bool success, IGranny granny)
        {
            this.Success = success;
            this.Granny = granny;
        }

        /// <summary>
        /// Gets the Parameter set associated with the result.
        /// </summary>
        public IParameterSet Parameters { get; private set; }

        /// <summary>
        /// Gets the Parameter Id set associated with the result.
        /// </summary>
        public Guid? ParameterId { get; private set; }

        /// <summary>
        /// Gets the status of the result.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Gets the resulting retrieved Granny.
        /// </summary>
        public IGranny Granny { get; private set; }
    }
}
