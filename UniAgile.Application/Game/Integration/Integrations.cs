using System;
using System.Collections.Generic;
using UniAgile.Dependency;

namespace UniAgile.Game.Integration
{
    public class Integrations<T>
        where T : class
    {
        public Integrations(IDependencyService dependencyService,
                            IReadOnlyList<(string Id, Type ImplementationType)>
                                integrationTypes)
        {
            DependencyService = dependencyService;
            IntegrationTypes = integrationTypes;
        }

        private readonly IDependencyService DependencyService;

        private readonly IReadOnlyList<(string Id, Type ImplementationType)>
            IntegrationTypes;

        private Dictionary<string, T> IntegrationMappingsField;

        public Dictionary<string, T> IntegrationMappings
        {
            get
            {
                if (IntegrationMappingsField == null)
                {
                    IntegrationMappingsField = new Dictionary<string, T>();

                    foreach (var integrationType in IntegrationTypes)
                    {
                        IntegrationMappingsField.Add(integrationType.Id,
                                                     (T) DependencyService.Resolve(integrationType.ImplementationType));
                    }
                }

                return IntegrationMappingsField;
            }
        }
    }
}