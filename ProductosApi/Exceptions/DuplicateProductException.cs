namespace ProductosApi.Exceptions;


public class DuplicateProductException : Exception
{
    public DuplicateProductException(string name)
        : base($"Ya existe un producto con el nombre '{name}'.")
    {
    }
}
