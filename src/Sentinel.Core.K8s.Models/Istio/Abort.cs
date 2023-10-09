using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sentinel.Core.K8s.Models.Istio
{
  public class Abort
  {

    /// <summary>
    /// Gets/sets the HTTP status code to use to abort the Http request
    /// </summary>
    [JsonProperty(PropertyName = "httpStatus")]
    public int HttpStatus { get; set; }

    /// <summary>
    /// Gets/sets the percentage of requests to be aborted with the error code provided
    /// </summary>
    [JsonProperty(PropertyName = "percentage")]
    public Percent Percentage { get; set; } = default!;

  }
}