# Using Docker with the eshop-ui-customer-portal

The following commands should be executed from the `eshop-ui-customer-portal` folder.

## Run the application in the development runtime environment:

Build:

```sh
docker build -t customer-portal-development --target development .
```

Run:

```sh
docker run -p 4201:4201 -d --rm customer-portal-development
```
Run with debugging in a browser:

```sh
docker run -p 4201:4201 -p 49153:49153 -d --rm -v .\src:/app/src --name customer-portal-dev  customer-portal-development
```

Run using a specific build configuration (production in the example):

```sh
docker run -p 4201:4201 -d --rm customer-portal-development --configuration=production
```

## Run the application in the production runtime environment:

Build:

```sh
docker build --target release --no-cache  -t customer-portal-release .
```

Build using a specific build configuration (development in the example):

```sh
docker build --target release --no-cache  -t customer-portal-release --build-arg="CONFIGURATION=development" .
```

Run:

```sh
docker run -d --rm -p 4201:80 customer-portal-release
```