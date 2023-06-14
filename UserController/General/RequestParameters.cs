namespace UserController.General;

public class RequestParameters
{
    public int Page { get; set; }

    public int Offset { get; set; }

    public string? Text { get; set; }

    public string? Code { get; set; }
}