#!/bin/bash

for module in /app/Modules/*; do \
    dotnet build "$module" -c Release -p:SourceRevisionId=$GIT_COMMIT -p:GitBranch=$GIT_BRANCH; \
done

for plugin in /app/Plugins/*; do \
    dotnet build "$plugin" -c Release -p:SourceRevisionId=$GIT_COMMIT -p:GitBranch=$GIT_BRANCH; \
done

dotnet restore ./Web/Grand.Web/Grand.Web.csproj
exec dotnet watch run --no-restore --no-launch-profile --project ./Web/Grand.Web/Grand.Web.csproj --urls http://0.0.0.0:8080