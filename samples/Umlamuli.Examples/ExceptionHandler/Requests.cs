//-----------------------------------------------------------------------
// <copyright file="Requests.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
namespace Umlamuli.Examples.ExceptionHandler;

public class PingResource : Ping;

public class PingNewResource : Ping;

public class PingResourceTimeout : PingResource;

public class PingProtectedResource : PingResource;