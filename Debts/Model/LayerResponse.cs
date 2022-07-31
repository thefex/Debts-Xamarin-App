using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Debts.Model
{
    public class LayerResponse
    {
        private bool _forceFailedResponse;

        public LayerResponse()
        {
            this.ErrorMessages = (IList<string>) new List<string>();
        }

        [JsonIgnore]
        public bool IsSuccess
        {
            get
            {
                if (!this.ErrorMessages.Any<string>())
                    return !this._forceFailedResponse;
                return false;
            }
        }

        [JsonProperty("errorMessages")]
        public IList<string> ErrorMessages { get; set; }

        [JsonIgnore]
        public string FormattedErrorMessages
        {
            get
            {
                if (!this.ErrorMessages.Any<string>())
                    return string.Empty;
                return this.ErrorMessages.Aggregate<string>((Func<string, string, string>) ((prev, current) => prev + Environment.NewLine + current));
            }
        }

        public LayerResponse AddErrorMessage(string errorMsg)
        {
            this.ErrorMessages.Add(errorMsg);
            return this;
        }

        public LayerResponse SetAsFailureResponse()
        {
            this._forceFailedResponse = true;
            return this;
        }
    }
    
    public sealed class LayerResponse<TResult> : LayerResponse
    {
        public LayerResponse(TResult results)
        {
            this.Results = results;
        }

        public LayerResponse()
        {
        }

        public TResult Results { get; }

        public LayerResponse<TResult> AddErrorMessage(string errorMsg)
        {
            base.AddErrorMessage(errorMsg);
            return this;
        }

        public LayerResponse<TResult> SetAsFailureResponse()
        {
            base.SetAsFailureResponse();
            return this;
        }
    }
}