set -x

# Login

aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 222779217717.dkr.ecr.us-east-1.amazonaws.com

# Build

docker build . -t websiteinstance -f Fydar.Dev.WebApp/Dockerfile

# Tag

docker tag websiteinstance:latest 222779217717.dkr.ecr.us-east-1.amazonaws.com/websiteinstance:latest

# Deploy

docker push 222779217717.dkr.ecr.us-east-1.amazonaws.com/websiteinstance:latest
