using CDC_PoC.Models;

namespace CDC_PoC.Services;

public interface IElasticCudService
{
    Task HandleChanges(Payload payload);
}