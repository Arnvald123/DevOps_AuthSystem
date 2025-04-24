#!/bin/bash

cd ~/myapp || exit
git pull origin main

# Побудова проекту
dotnet build --configuration Release

# Запуск проекту
dotnet run

