using CDC_PoC.Models;

namespace CDC_PoC.Services;

public interface IDocumentTenantIdInjector
{
    Task<Payload> InjectTenantId(Payload payload);
}