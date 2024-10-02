using CDC_PoC.CDC;

namespace CDC_PoC.Elastic;

public interface IElasticCudService
{
    Task HandleChanges(Payload payload);
}