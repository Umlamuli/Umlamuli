param($target = "K:\dev\Umlamuli\src\Umlamuli.SourceGenerator")

$header = @"
//-----------------------------------------------------------------------
// <copyright file="{0}" company="Umlamuli">
// Copyright 2025 Umlamuli. All rights reserved.
// Licensed under the Apache License, Version 2.0
// </copyright>
//-----------------------------------------------------------------------

"@

Get-ChildItem -Path $target -Recurse -Include "*.cs" | ForEach-Object {
    $filePath = $_.FullName
    $fileName = $_.Name

    # Read the file as a single string to preserve content exactly
    $fileText = [System.IO.File]::ReadAllText($filePath)

    # Check if a copyright header already exists at the top
    $hasHeader = $fileText -match "<copyright" -or $fileText -match "Copyright \(c\)"

    if (-not $hasHeader) {
        $prepended = ([string]::Format($header, $fileName)) + $fileText
        [System.IO.File]::WriteAllText($filePath, $prepended)
        Write-Host "Prepended header to $fileName"
    } else {
        Write-Host "Header already present in $fileName"
    }
}