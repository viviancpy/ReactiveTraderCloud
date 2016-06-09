#!/bin/bash
set -euo pipefail

nginx_user="nginx"
jenkins_user="jenkins"
jenkins_port="8888"

# Install jenkins
wget -q -O - https://jenkins-ci.org/debian/jenkins-ci.org.key | sudo apt-key add -
sudo sh -c 'echo deb http://pkg.jenkins-ci.org/debian-stable binary/ > /etc/apt/sources.list.d/jenkins.list'
sudo apt-get update
sudo apt-get install jenkins -y
sudo sed -i "s/8080/$jenkins_port/g" /etc/default/jenkins

# Install docker
sudo apt-get purge lxc-docker* || true
sudo apt-get purge docker.io*  || true
sudo apt-get update
sudo apt-get install -y apt-transport-https ca-certificates
sudo apt-key adv --keyserver hkp://p80.pool.sks-keyservers.net:80 --recv-keys 58118E89F3A912897C070ADBF76221572C52609D
echo "deb https://apt.dockerproject.org/repo ubuntu-xenial main" > /tmp/docker.list
sudo cp /tmp/docker.list /etc/apt/sources.list.d/docker.list
rm /tmp/docker.list
sudo apt-get update
sudo apt-get install -y docker-engine --allow-unauthenticated
sudo systemctl start docker
sudo systemctl enable docker
sudo usermod -aG docker jenkins

# install nginx
nginx=stable # use nginx=development for latest development version
sudo add-apt-repository ppa:nginx/$nginx -y
sudo apt-get update
sudo apt-get install nginx -y

cat <<EOF > /tmp/nginx.conf
user $nginx_user;

worker_processes  1;

pid  /var/run/nginx.pid;

events {
    worker_connections  1024;
}

http {
    access_log /var/log/nginx/access.log;
    error_log  /var/log/nginx/error.log;

    include         /etc/nginx/mime.types;
    sendfile        on;
    default_type    application/octet-stream;

    server_names_hash_bucket_size   128;

    server {
        listen      80;
        
        #ssl                    on;
        #ssl_certificate        /etc/ssl/server.pem;
        #ssl_certificate_key    /etc/ssl/server.key;
        
        location / {
            proxy_set_header        Host              \$host;
            proxy_set_header        X-Real-IP         \$remote_addr;
            proxy_set_header        X-Forwarded-Proto \$scheme;
            proxy_set_header        X-Forwarded-For   \$proxy_add_x_forwarded_for;
            
            proxy_pass http://localhost:$jenkins_port;
        }
    }
}
EOF

cat <<EOF > /tmp/nginx.service
[Unit]
Description=nginx

[Service]
Restart=always
ExecStart=/usr/sbin/nginx
User=$nginx_user

[Install]
WantedBy=multi-user.target
EOF

sudo cp /tmp/nginx.conf    /etc/nginx/
sudo cp /tmp/nginx.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo useradd -s /bin/false $nginx_user
sudo chown -R $nginx_user:$nginx_user /var/log/nginx

sudo systemctl enable nginx
sudo systemctl enable jenkins
sudo systemctl stop nginx || true
sudo systemctl stop jenkins || true
sudo systemctl start nginx
sudo systemctl start jenkins
