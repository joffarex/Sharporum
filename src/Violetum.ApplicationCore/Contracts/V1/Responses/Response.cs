namespace Violetum.ApplicationCore.Contracts.V1.Responses
{
    public class Response<TViewModel>
    {
        public TViewModel Data { get; set; }
    }
}