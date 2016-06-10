#! /bin/bash

id="<% cli.id %>"
image="<% base.dotnet.image %>:<% base.dotnet.major %>.<% base.dotnet.minor %>"

# fail fast
set -euo pipefail

docker push ${image}
docker push ${image}.${id}
