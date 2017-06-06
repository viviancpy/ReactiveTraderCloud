#! /bin/bash

apt-get update && apt-get install -y curl
curl http://download.geteventstore.com/binaries/EventStore-OSS-Ubuntu-14.04-v__VEVENTSTORE__.tar.gz | tar xz -C /opt    
  