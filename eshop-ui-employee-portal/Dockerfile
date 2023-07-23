FROM node:16.20-alpine3.17  as development
WORKDIR /app
RUN npm install -g @angular/cli
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
EXPOSE 4200 49153
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
COPY --from=build /app/dist/eshop-ui-employee-portal .
# Containers run nginx with global directives and daemon off
EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]

# For development runtime environment use:
# Build
# docker build -t employee-portal-development --target development.
# Run with browser debugging
# docker run -p 4202:4200 -p 49153:49153 -d --rm -v .\src:/app/src --name employee-portal-dev  employee-portal-development
# Run without browser debugging
# docker run -p 4202:4200 -d --rm employee-portal-development
# Run with other configuration (production in the example)
# docker run -p 4202:4200 -d --rm employee-portal-development --configuration=production

# For production runtime environment use:
# Build
# docker build --target release --no-cache  -t employee-portal-release .
# Build with other configuration (development in the example)
# docker build --target release --no-cache  -t employee-portal-release --build-arg="CONFIGURATION=development" .
# Run
# docker run -d --rm -p 4202:80 employee-portal-release