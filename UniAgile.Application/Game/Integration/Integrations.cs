using System;
using System.Collections.Generic;
using UniAgile.Dependency;

namespace UniAgile.Game.Integration
{
    public class Integrations<T>
        where T : class
    {
        public Integrations(IReadOnlyList<KeyValuePair<string, T>> integrations)
        {
            foreach (var integration in integrations)
            {
                try
                {
                    IntegrationMappings.Add(integration.Key, integration.Value);
                }
                catch (Exception e)
                {
                    throw new
                        Exception($"Unable to register integration {integration.Key} with implementation {integration.Value.GetType()}. Message: {e.Message}");
                }
            }
        }

        public Dictionary<string, T> IntegrationMappings { get; private set; } = new Dictionary<string, T>();
    }
}