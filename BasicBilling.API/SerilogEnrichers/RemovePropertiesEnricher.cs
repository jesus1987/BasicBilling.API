using Serilog.Core;
using Serilog.Events;

namespace BasicBilling.API.SerilogEnrichers
{
    internal class RemovePropertiesEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory lepf)
        {
            logEvent.RemovePropertyIfPresent("RequestPath");
        }
    }
}
