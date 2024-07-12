FROM node:22.4.0-alpine3.20  as development
WORKDIR /app
RUN npm install -g @angular/cli@18.1.0
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
EXPOSE 4201 49153
ENTRYPOINT ["ng","serve","--host","0.0.0.0","--poll","500"]
CMD ["--configuration","development"]

FROM development AS build
ARG CONFIGURATION="production"
RUN ng build --configuration=$CONFIGURATION

FROM nginx:alpine3.17 as release
COPY --from=build /app/.nginx/nginx.conf /etc/nginx/conf.d/default.conf
WORKDIR /usr/share/nginx/html
# Remove default nginx static assets
RUN rm -rf ./*
# Copy static assets from builder stage
COPY --from=build /app/dist/eshop-ui-client-portal/browser .
# Containers run nginx with global directives and daemon off
EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]

# For development runtime environment use:
# Build
# docker build -t client-portal-development --target development .
# Run with browser debugging
# docker run -p 4201:4201 -p 49153:49153 -d --rm -v .\src:/app/src --name client-portal-dev  client-portal-development
# Run without browser debugging
# docker run -p 4201:4201 -d --rm client-portal-development
# Run with other configuration (production in the example)
# docker run -p 4201:4201 -d --rm client-portal-development --configuration=production

# For production runtime environment use:
# Build
# docker build --target release --no-cache  -t client-portal-release .
# Build with other configuration (development in the example)
# docker build --target release --no-cache  -t client-portal-release --build-arg="CONFIGURATION=development" .
# Run
# docker run -d --rm -p 4201:80 client-portal-release


