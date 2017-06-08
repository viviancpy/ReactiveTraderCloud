#! /bin/bash

build=$1
if [[ $build = "" ]];then
  echo "populate-eventstore-build: build number required as first parameter"
  exit 1
fi

# fail fast
set -euo pipefail

. ../../../config

# run eventstore
docker run -d --net=host $eventstoreContainer > eventstore_id

# populate it
populateCommand=`cat "../../../../src/server/Populate Event Store.bat"`
docker run -t --net=host      \
     $serversContainer.$build \
     $populateCommand > populate_id

body=$(cat << EOF
fromStreams(["\$ce-trade", "\$ce-creditAccount"])
    .when({
        TradeCreatedEvent: function (s, e) {
            linkTo("trade_execution", e);
        },
        CreditReservedEvent: function (s, e) {
            linkTo("trade_execution", e);
        },
        CreditLimitBreachedEvent: function (s, e) {
            linkTo("trade_execution", e);
        },
        TradeCompletedEvent: function (s, e) {
            linkTo("trade_execution", e);
        },
        TradeRejectedEvent: function (s, e) {
            linkTo("trade_execution", e);
        }
    });
EOF
)

curl -X POST -i "http://localhost:2113/projections/continuous?name=trade_execution_proj&type=JS&enabled=true&emit=true&trackemittedstreams=true" -u admin:changeit -H "Content-Type: application/json" -d "$body"

curl -X PUT -i "http://localhost:2113/subscriptions/trade_execution/trade_execution_group" -u admin:changeit -H "Content-Type: application/json" -d '{ "resolveLinktos": true }'

# commit container
docker commit `cat eventstore_id` $populatedEventstoreContainer
docker tag $populatedEventstoreContainer $populatedEventstoreContainer.$build

docker kill `cat eventstore_id`
