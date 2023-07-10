FROM node:16.20-alpine3.17  as development
WORKDIR /app
RUN npm install -g @angular/cli
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
EXPOSE 4200 49153
ENTRYPOINT ["ng","serve","--host","0.0.0.0","--poll","500"]
CMD ["--configuration","docker"]

FROM development AS build
ARG CONFIGURATION="production"
RUN ng build --configuration=$CONFIGURATION

FROM nginx:alpine3.17 as release
COPY --from=build /app/.nginx/nginx.conf /etc/nginx/conf.d/default.conf
WORKDIR /usr/share/nginx/html
# Remove default nginx static assets
RUN rm -rf ./*
# Copy static assets from builder stage
COPY --from=build /app/dist/eshop-ui-client-portal .
# Containers run nginx with global directives and daemon off
EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]

# For development runtime environment use:
# Build (--no-cache parameter is optional - it could help in some issues)
# docker build -t client-portal-development --target development --no-cache .
# Run with browser debugging
# docker run -p 4201:4200 -p 49153:49153 -d --rm -v D:\VSProjects\eshop-on-microservices\eshop-ui-client-portal\src:/app/src --name client-portal-dev  client-portal-development
# Run without browser debugging
# docker run -p 4201:4200 -d --rm client-portal-development
# Run with other configuration (production in the example)
# docker run -p 4201:4200 -d --rm client-portal-development --configuration=production

# For production runtime environment use:
# Build
# docker build --target release --no-cache  -t client-portal-release .
# Build with other configuration (docker in the example)
# docker build --target release --no-cache  -t client-portal-release --build-arg="CONFIGURATION=docker" .
# Run
# docker run -d --rm -p 4201:80 client-portal-release


