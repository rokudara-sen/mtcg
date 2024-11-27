namespace MCTG._06_Domain.Entities;

public class OperationResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public object Data { get; set; }
}