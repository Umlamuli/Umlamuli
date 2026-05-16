//-----------------------------------------------------------------------
// <copyright file="SharedHandlerStateCollection.cs" company="Umlamuli">
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using Xunit;

namespace Umlamuli.SourceGenerator.Tests;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class SharedHandlerStateCollection
{
    public const string Name = "SharedHandlerState";
}
