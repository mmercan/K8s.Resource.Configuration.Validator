using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using k8s.Models;
using Sentinel.Core.K8s.Models.Entities;

namespace Sentinel.Core.K8s.Models.Istio
{

    [KubernetesEntity(Group = "networking.istio.io", Kind = "EnvoyFilter", ApiVersion = "v1alpha3", PluralName = "envoyfilters")]
    [EntityScope(EntityScope.Namespaced)]
    public class EnvoyFilter : CustomResourceMeta<V1ObjectMeta, EnvoyFilterSpec, EnvoyFilterStatus>
    {

    }
}