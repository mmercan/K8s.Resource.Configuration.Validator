using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s.Models;
using Sentinel.Core.K8s.Models.Entities;
using static Sentinel.Core.K8s.Models.CRDs.DeploymentScalerResource;

namespace Sentinel.Core.K8s.Models.Istio
{
        /// <summary>
    /// Represents the <see href="https://istio.io/latest/docs/reference/config/networking/virtual-service/">virtual service specification</see>, which is a configuration affecting traffic routing.
    /// </summary>

    
    [KubernetesEntity(Group = "networking.istio.io", Kind = "VirtualService", ApiVersion = "v1alpha3", PluralName = "virtualservices")]
    [EntityScope(EntityScope.Namespaced)]
    public class VirtualService  : CustomResourceMeta<V1ObjectMeta,VirtualServiceSpec>
    {
        
    }
}