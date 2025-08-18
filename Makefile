.PHONY: build run clean

build:
	docker-compose build

run:
	docker-compose up

down:
	docker-compose down --volumes --remove-orphans

clean:
	sudo dotnet clean
	sudo find ./src -type d -name "bin" -exec rm -rf {} +
	sudo find ./src -type d -name "obj" -exec rm -rf {} +
	sudo find ./src -type d -name "node_modules" -exec rm -rf {} +