namespace Catalog.API.Exceptions
{
    public class GetProductByIdException : Exception
    {
        public GetProductByIdException() : base("GetProductByIdHandler exception")
        {
        }
    }
}
