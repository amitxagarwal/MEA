#!/bin/bash

Write-Host " a McaApiUri ------  ----------- $env:McaApiUri ";
pwsh ./build.ps1 "$@"
