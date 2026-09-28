
## DotNet setup

	dotnet add package Microsoft.EntityFrameworkCore.Design

## Build 'my-first-service' image

	docker build -t my-first-service .

	docker run --name postgres-db -e "POSTGRES_PASSWORD=andromeda" -p 5432:5432 -v "pgdata:/var/lib/postgresql" -d postgres
	docker exec -it "postgres-db" "createdb" -U "postgres" "dotnet01"
	docker logs postgres-db
	
	docker exec -it "postgres-db" psql -U postgres -c "CREATE USER dotnet WITH PASSWORD 'andromeda'"
	docker exec -it "postgres-db" psql -U postgres -c "GRANT ALL PRIVILEGES ON DATABASE dotnet01 TO dotnet"
	// docker exec -it "postgres-db" psql -U dotnet01 -d dotnet01 -c "GRANT ALL ON SCHEMA public TO dotnet"
	docker exec -it "postgres-db" psql -U postgres -d dotnet01 -c "CREATE SCHEMA dotnet AUTHORIZATION dotnet"

## Create Shared Network

	docker network create app-net
	docker network connect app-net postgres-db
	docker network connect app-net my-first-app
	
	
	dotnet clean
	dotnet build
	
	dotnet tool run dotnet-ef database update
	
	docker run --name my-first-service -p 8000:8080 --network app-net my-first-service

## Docker Compose

	docker-compose build

	docker-compose up -d

curl -vl "http://[::1]:8000/greetings" \
-H "Content-Type: application/json" \
-d '{"Id":1,"Name":"Sonia","Message":"Benvenuta, Sonia"}'

curl -vl "http://[::1]:8000/greetings" \
-H "Content-Type: application/json" \
-d '{
  "Id": 2,
  "Name": "Paolo",
  "Message": "Welcome, Paolo"
}'

curl -vl "http://[::1]:8000/greetings" \
-H "Content-Type: application/json" \
-d '{"Name":"Karl","Message":"Ciao, Karl"}'

curl -vl "http://[::1]:8000/greetings" \
-H "Content-Type: application/json" \
-d '{"Name":"Steven","Message":"Hi, Steven"}'

curl -vl "http://[::1]:8000/greetingsWithNotify" \
-H "Content-Type: application/json" \
-d '{"Name":"Mia","Message":"Hi, Mia"}'

## RabbitMQ

	dotnet add package RabbitMQ.Client

Console: http://localhost:15672/ (admin/and......)

## Dashboard

	curl http://localhost:8000/greetings
	[{"id":1,"name":"Sonia","message":"Benvenuta, Sonia"},{"id":2,"name":"Paolo","message":"Welcome, Paolo"},{"id":3,"name":"Leah","message":"Willkommen, Leah"},{"id":4,"name":"Karl","message":"Ciao, Karl"},{"id":5,"name":"Steven","message":"Hi, Steven"},{"id":6,"name":"Megan","message":"Hi, Megan"},{"id":7,"name":"Mia","message":"Hi, Mia"},{"id":8,"name":"Mia","message":"Hi, Mia"},{"id":9,"name":"Mia","message":"Hi, Mia"}]

