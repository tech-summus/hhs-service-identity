docker network create -d bridge local.infrastructure-network

docker-compose -f compose/docker-compose.local.yml -p hhs-service-identity-compose build

docker-compose -f compose/docker-compose.local.yml -p hhs-service-identity-compose up -d
docker-compose -p hhs-service-identity-compose logs --follow

docker-compose -f compose/docker-compose.local.yml -p hhs-service-identity-compose down
