// RefreshService.cs
using System;

public class RefreshService
{
    public event Action? OnRefreshRequested;

    public void RequestRefresh() => OnRefreshRequested?.Invoke();
}
