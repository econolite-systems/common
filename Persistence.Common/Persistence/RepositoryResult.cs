// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
namespace Econolite.Ode.Persistence.Common.Persistence;

public class RepositoryResult
{
    public bool Success { get; }
    public string Errors { get; }

    private RepositoryResult(bool success, string errors)
    {
        Success = success;
        Errors = errors;
    }

    public static RepositoryResult SuccessResult()
    {
        return new RepositoryResult(true, null!);
    }

    public static RepositoryResult FailureResult(string errors)
    {
        return new RepositoryResult (false, errors);
    }
}
