#!/bin/bash

major="<% reactivetrader.servers.major %>"
minor="<% reactivetrader.servers.minor %>"
servers_image="<% reactivetrader.servers.image %>:${major}.${minor}"

# fail fast
set -euo pipefail

# smoke tests
echo " "
echo "Starting integration tests ..."
test_command="dotnet test --configuration Release Adaptive.ReactiveTrader.Server.IntegrationTests"
docker run         \
  --net=host       \
  ${servers_image} \
  bash -c "${test_command}"
