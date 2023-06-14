namespace UserController.General;

public class Response<T>
{
    public int Status { get; set; }

    public int Cached { get; set; }

    public int Count { get; set; }

    public string? Message { get; set; }

    public T? Data { get; set; }

    public ErrorResponse[]? Errors { get; set; }
}

public class ErrorResponse
{
    public int Status { get; set; }

    public string RequestUrl { get; set; }

    public string Title { get; set; }

    public string? Message { get; set; }
}

public class PaginationResponse<T>
{
    public int Page { get; set; }

    public int Offset { get; set; }

    public int TotalPage { get; set; }

    public int TotalCount { get; set; }

    public string NextPageUrl { get; set; } = "http://api.akilliphone.com/users?page=1&offset=50";

    public string PreviousPageUrl { get; set; } = "http://api.akilliphone.com/users?page=1&offset=50";

    public string FirstPageUrl { get; set; } = "http://api.akilliphone.com/users?page=1&offset=50";

    public string LastPageUrl { get; set; } = "http://api.akilliphone.com/users?page=1&offset=50";

    public T Items { get; set; }
}