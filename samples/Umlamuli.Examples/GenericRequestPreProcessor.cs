//-----------------------------------------------------------------------
// <copyright file="GenericRequestPreProcessor.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Umlamuli.Pipeline;

namespace Umlamuli.Examples;

public class GenericRequestPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
{
    private readonly TextWriter _writer;

    public GenericRequestPreProcessor(TextWriter writer)
    {
        _writer = writer;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        return _writer.WriteLineAsync("- Starting Up");
    }
}