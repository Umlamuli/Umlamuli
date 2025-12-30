//-----------------------------------------------------------------------
// <copyright file="Exceptions.cs" company="Umlamuli">
// Original Copyright (c) 2025 Jimmy Bogard. All rights reserved.
// Licensed under the Apache License, Version 2.0
//
// Modifications Copyright 2025 Umlamuli
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace Umlamuli.Examples.ExceptionHandler;

public class ConnectionException : Exception;

public class ForbiddenException : ConnectionException;

public class ResourceNotFoundException : ConnectionException;

public class ServerException : Exception;