#! /bin/bash

id="<% cli.id %>"
image="<% base.dotnet.image %>:<% base.dotnet.major %>.<% base.dotnet.minor %>"

# fail fast
set -euo pipefail

docker build --no-cache -t ${image} .
docker tag ${image} ${image}.${id}
