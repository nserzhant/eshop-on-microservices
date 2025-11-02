# Using Docker with the eshop-ui-employee-portal

The following commands should be executed from the `eshop-ui-employee-portal` folder.

## Run the application in the development runtime environment:

Build:

```sh
docker build -t employee-portal-development --target development .
```

Run:

```sh
docker run -p 4202:4202 -d --rm employee-portal-development
```
Run with debugging in a browser:

```sh
docker run -p 4202:4202 -p 49153:49153 -d --rm -v .\src:/app/src --name employee-portal-dev  employee-portal-development
```

Run using a specific build configuration (production in the example):

```sh
docker run -p 4202:4202 -d --rm employee-portal-development --configuration=production
```

## Run the application in the production runtime environment:

Build:

```sh
docker build --target release --no-cache  -t employee-portal-release .
```

Build using a specific build configuration (development in the example):

```sh
docker build --target release --no-cache  -t employee-portal-release --build-arg="CONFIGURATION=development" .
```

Run:

```sh
docker run -d --rm -p 4202:80 employee-portal-release
```