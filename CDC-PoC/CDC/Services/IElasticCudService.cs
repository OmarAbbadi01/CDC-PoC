using CDC_PoC.CDC.Models;

namespace CDC_PoC.CDC.Services;

public interface IElasticCudService
{
    Task HandleChanges(Payload payload);
}