FROM node:16.20-alpine3.17  as development
WORKDIR /app
RUN npm install -g @angular/cli
COPY package.json package-lock.json ./
RUN npm ci
COPY . .
EXPOSE 4200 49153
CMD ["ng", "serve", "--host", "0.0.0.0", "--poll", "500","--configuration=docker"]

FROM development AS build
RUN npm run build --prod

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

# For development use:
# docker build -t employee-portal-development --target development --no-cache .
# docker run -p 4202:4200 -p 49153:49153 -d --rm -v D:\VSProjects\eshop-on-microservices\eshop-ui-employee-portal\src:/app/src --name employee-portal-dev  employee-portal-development
# docker run -d --rm employee-portal-dev

# For production use:
# docker build --target release --no-cache  -t employee-portal-release .
# docker run -d --rm -p 4202:80 employee-portal-release
