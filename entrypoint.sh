#!/bin/bash

for module in /app/Modules/*; do \
    dotnet build "$module"
done

for plugin in /app/Plugins/*; do \
    dotnet build "$plugin"
done

exec dotnet watch run --no-launch-profile --project ./Web/Grand.Web/Grand.Web.csproj --urls http://0.0.0.0:8080
